using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;
using UnityEngine;

namespace UPBS.Execution
{
    public struct ImageData
    {
        public string filename;
        public byte[] bytes;
    }

    public class ImageProcessor
    {
        ConcurrentQueue<ImageData> _imageQueue;
        Thread _encoderThread;

        private bool _isImageSavingActive = false;
        private bool _isCaptureRunning = false;
        private string _baseDirectory;
        private int _sleepDuration = 50;

        public int FrameCounter { get; private set; } = 0;
        public Stopwatch timer { get; private set; }

        public delegate void OnThreadFinished(Stopwatch s);
        public static OnThreadFinished imageRenderingFinished;

        public ImageProcessor(string baseDir, bool deleteExisting)
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
            _imageQueue = new ConcurrentQueue<ImageData>();
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

        //TODO: FINISH
        public void AbortRecording()
        {
            FrameCounter = 0;
            imageRenderingFinished(timer);
        }

        public void EnqueueData(ImageData data)
        {
            if (_imageQueue == null) return;
            _imageQueue.Enqueue(data);
        }

        private void SaveImagesThread()
        {
            while (true)
            {
                if (!_isCaptureRunning && _imageQueue.Count <= 0) break;
                else if (_imageQueue.Count <= 0)
                {
                    Thread.Sleep(_sleepDuration);
                    continue;
                }

                if (_imageQueue.TryDequeue(out ImageData data))
                {
                    File.WriteAllBytes(Path.Combine(_baseDirectory, data.filename), data.bytes);
                }

                Thread.Sleep(_sleepDuration);
            }

            FrameCounter = 0;
            imageRenderingFinished(timer);
            timer.Stop();
            _isImageSavingActive = false;
        }
    }

    public class PBImageRenderingManager : MonoBehaviour
    {
        [SerializeField]
        private Camera _IRCam;
        [SerializeField]
        private Canvas _IRCanvas;
        [SerializeField]
        private TMPro.TextMeshProUGUI _IRProgressText;

        public void BeginImageRendering()
        {
            //Pause Playback
            PBFrameController.Instance.Pause();
            //Set Frame to 0
            PBFrameController.Instance.SetFrame(0);
            //Hide all playback Cameras => Or maybe not
            //Enable the IR cam

            //Check for correct components

            //Start IR Routine
            _IRCanvas.gameObject.SetActive(true);
            _IRCam.gameObject.SetActive(true);
            _IRProgressText.text = $"0/{PBFrameController.Instance.FinalFrame}";
            StartCoroutine(ProcessImageRendering());
        }

        private IEnumerator ProcessImageRendering()
        {
            yield return null;

            //Initialize Cameras
            IPBCameraBase[] cams = PBCameraManager.Instance.GetPlaybackCameras();
            ImageProcessor imageProcessor = new ImageProcessor(PBLoadingManager.LoadedDirectory, true);
            foreach (IPBCameraBase cam in cams)
            {
                cam.GetRenderCamera().Initialize();
            }

            while (PBFrameController.Instance.CurrentFrameIndex != PBFrameController.Instance.FinalFrame)
            {
                //Record render texture from cameras
                foreach (IPBCameraBase cam in cams)
                {
                    imageProcessor.EnqueueData(new ImageData { bytes = cam.GetRenderCamera().Capture(), filename = Path.Combine(cam.GetRenderCamera().name, $"F_{PBFrameController.Instance.CurrentFrameIndex}") });
                }

                PBFrameController.Instance.SkipFramesForward(1);
                _IRProgressText.text = $"{imageProcessor.FrameCounter}/{PBFrameController.Instance.FinalFrame}";
                yield return null;
            }

            //One more round of recording

            //Save everything to disk
            imageProcessor.PrepareStopRecording();
            while (imageProcessor.IsRunning())
            {
                yield return null;
                _IRProgressText.text = $"{imageProcessor.FrameCounter}/{PBFrameController.Instance.FinalFrame}";
            }
            //Wait for complete event

            _IRCanvas.gameObject.SetActive(false);
            _IRCam.gameObject.SetActive(false);
            _IRProgressText.text = $"0/{PBFrameController.Instance.FinalFrame}";
        }
    }
}
