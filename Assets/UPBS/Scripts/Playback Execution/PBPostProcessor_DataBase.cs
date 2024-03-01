using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UPBS.Data;

namespace UPBS.Execution
{
    public class PostProcessData : GenericProcessorData<string>
    {
        private void FormatData()
        {
            if (!string.IsNullOrEmpty(this.data))
            {
                this.data = this.data.Trim();
                if (!this.data.EndsWith("\n"))
                {
                    this.data += '\n';
                }
            }
        }

        public override void WriteToFile(string path)
        {
            FormatData();
            File.AppendAllText(path, data); //This is an I/O nightmare
        }
    }

    public abstract class PostProcessDataFrame : GenericProcessorData<string>
    {
        protected UXF.UXFDataTable dataTable = null;

        public abstract string[] GetColumnNames();

        public override void WriteToFile(string path)
        {
            string[] data = dataTable.GetCSVLines();
            File.WriteAllLines(path, data);
        }

        public void PushData(UXF.UXFDataRow dataRow)
        {
            if (dataTable == null)
            {
                dataTable = new UXF.UXFDataTable(GetColumnNames());
            }

            dataTable.AddCompleteRow(dataRow);
        }
    }

    public class PostProcessSampleData : PostProcessDataFrame
    {
        public override string[] GetColumnNames()
        {
            return new string[] { "Modified_Position", "Modified_Rotation" };
        }
    }

    /// <summary>
    /// Base class for post processors which makes modified copies of tracker files.
    /// Until I make a pooled processor class, each tracker will have a single, threaded processor.
    /// </summary>
    public abstract class PBPostProcessor_DataBase : PBPostProcessorBase
    {
        // On every frame, transform the data and send it to the corresponding tracker processor
        
        // Takes in an array of frame data and performs some custom transformation on them. In return, we get an array of strings which contain information that gets saved to a file.
        public abstract string GenerateTrackedObjectData(PBTrackerID trackedObject, PBFrameDataBase frameData);

        [SerializeField] private PBTrackerID[] affectedTrackedObjects;
        private DataProcessor<PostProcessData, string>[] dataProcessors;
        // 1.) Fetch frame data for all affected trackers
        // 2.) Perform generic transformation of each frame's information
        // 3.) Send that data over to the data processor(s)
        public override IEnumerator CaptureFrame(int frameNumber)
        {
            for (int i = 0; i < affectedTrackedObjects.Length; ++i)
            {
                PBTrackerID trackerID = affectedTrackedObjects[i];
                DataProcessor<PostProcessData, string> dataProcessor = dataProcessors[i];

                if (PBFrameLibraryManager.Instance.TryGetCurrentLibraryEntry<PBFrameDataBase>(trackerID.ID, out var frameData, gameObject.name))
                {
                    string objectData = GenerateTrackedObjectData(trackerID, frameData);
                    dataProcessor.EnqueueData(new PostProcessData() { data = objectData, filename = $"{trackerID.gameObject.name}_{trackerID.ID}.csv", subDirectory = "" });
                }
            }

            yield break;
        }

        /// <summary>
        /// Create Post Processors for every tracker in use
        /// </summary>
        public override IEnumerator Initialize(int framesToProcess)
        {
            dataProcessors = new DataProcessor<PostProcessData, string>[affectedTrackedObjects.Length];


            for (int i = 0; i < dataProcessors.Length; ++i)
            {
                dataProcessors[i] = new DataProcessor<PostProcessData, string>(this.DataDirectory, true);
                dataProcessors[i].StartRecording();
            }

            return base.Initialize(framesToProcess);
        }

        public override void MarkFrameCaptureComplete()
        {
            foreach (var processor in dataProcessors)
            {
                processor.PrepareStopRecording();
            }

            base.MarkFrameCaptureComplete();
        }

        public override IEnumerator WaitForCleanUp()
        {
            foreach (var processor in dataProcessors)
            {
                while (processor.IsRunning())
                {
                    yield return null;
                    // Update Progress
                }
            }
            
            yield return base.WaitForCleanUp();
        }
    }

}