using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEngine;

namespace UPBS.Execution
{
    public abstract class GenericProcessorData<T>
    {
        public string filename = string.Empty;
        public string subDirectory = string.Empty;
        public T data;

        public abstract void WriteToFile(string path);
    }

    public class DataProcessor<T, U> where T : GenericProcessorData<U>
    {
        ConcurrentQueue<T> _dataQueue;
        Thread _encoderThread;

        private bool _isImageSavingActive = false;
        private bool _isCaptureRunning = false;
        private string _baseDirectory;
        private int _sleepDuration = 50;

        public int FrameCounter { get; private set; } = 0;
        public Stopwatch timer { get; private set; } = null;

        public delegate void OnThreadFinished(Stopwatch s);
        public static OnThreadFinished processorFinished;

        public DataProcessor(string baseDir, bool deleteExisting)
        {
            _baseDirectory = baseDir;

            if (!Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
                _baseDirectory = baseDir;
            }
            else if (deleteExisting)
            {
                foreach (string filePath in Directory.GetFiles(baseDir))
                {
                    File.Delete(filePath);
                }
            }

            timer = new Stopwatch();
        }

        public void StartRecording()
        {
            FrameCounter = 0;
            _dataQueue = new ConcurrentQueue<T>();
            _isCaptureRunning = true;
            _encoderThread = new Thread(SaveImagesThread);
            _encoderThread.Priority = System.Threading.ThreadPriority.BelowNormal;
            timer.Start();
            _encoderThread.Start();
            _isImageSavingActive = true;
        }

        public bool IsRunning() => _isImageSavingActive;

        /// <summary>
        /// Use once all frames have been sent to the queue. This will give an estimation of how long the thread should take to complete from start.
        /// </summary>
        public int EstimatedThreadDuration()
        {
            return _sleepDuration * FrameCounter * 1000;
        }

        public float GetThreadProgress(int totalFrames)
        {
            return FrameCounter / totalFrames;
        }

        public void PrepareStopRecording()
        {
            _isCaptureRunning = false;

        }

        public void AbortRecording()
        {
            FrameCounter = 0;
            processorFinished(timer);
        }

        public void EnqueueData(T data)
        {
            if (_dataQueue == null) return;
            _dataQueue.Enqueue(data);
        }

        private void SaveImagesThread()
        {
            while (true)
            {
                UnityEngine.Debug.Log("Ticked on SaveImagesThread()");
                if (!_isCaptureRunning && _dataQueue.Count <= 0)
                {
                    UnityEngine.Debug.Log($"Leaving SaveImagesThread() | isCaptureRunning: {_isCaptureRunning} | imageQueueCount: {_dataQueue.Count}");

                    break;
                }
                else if (_dataQueue.Count <= 0)
                {
                    Thread.Sleep(_sleepDuration);
                    continue;
                }

                if (_dataQueue.TryDequeue(out T data))
                {
                    try
                    {
                        string subDir = Path.Combine(_baseDirectory, data.subDirectory);
                        string fullDir = Path.Combine(subDir, data.filename);
                        if (!Directory.Exists(subDir))
                        {
                            Directory.CreateDirectory(subDir);
                        }

                        data.WriteToFile(fullDir);
                        ++FrameCounter;
                    }
                    catch (System.Exception e)
                    {
                        UnityEngine.Debug.Log(e.Message);
                    }
                }

                Thread.Sleep(_sleepDuration);
            }

            // imageRenderingFinished(timer);
            timer.Stop();
            _isImageSavingActive = false;
        }
    }

    public enum PostProcessorState
    {
        Inactive,
        Running,
        CleaningUp
    }

    public abstract class PBPostProcessorBase : MonoBehaviour
    {
        private string _dataDirectory = string.Empty;

        public PostProcessorState State { get; private set; } = PostProcessorState.Inactive;

        public string DataDirectory 
        { 
            get
            {
                if (_dataDirectory == string.Empty)
                {
                    _dataDirectory = System.IO.Path.Combine(PBLoadingManager.LoadedDirectory, "PostProcessedData", this.GetType().Name);
                }

                return _dataDirectory;
            } 
        }

        public int CurrentProgress { get; protected set; } = 0;

        public int TargetProgress { get; protected set; } = 0;

        public virtual IEnumerator CaptureFrame(int frameNumber)
        {
            ++CurrentProgress;
            yield break;
        }

        /// <summary>
        /// Signals to the processor that data for all frames has been captured and enqeued.
        /// </summary>
        public virtual void MarkFrameCaptureComplete()
        {
            State = PostProcessorState.CleaningUp;
        }

        /// <summary>
        /// Try to ensure the correct components and references are setup in order to begin rendering.
        /// </summary>
        /// <returns>Returns true if the Processor is ready to begin recording</returns>
        public virtual IEnumerator Initialize(int framesToProcess)
        {
            TargetProgress = framesToProcess;
            State = PostProcessorState.Running;
            yield break;
        }

        public virtual IEnumerator WaitForCleanUp()
        {
            State = PostProcessorState.Inactive;
            TargetProgress = 0;
            CurrentProgress = 0;
            yield break;
        }
    }

}