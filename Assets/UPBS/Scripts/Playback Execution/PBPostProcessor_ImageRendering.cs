using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace UPBS.Execution
{
    public class ImageData : GenericProcessorData<byte[]>
    {
        public override void WriteToFile(string path)
        {
            File.WriteAllBytes(path, data);
        }
    }

    public class PBPostProcessor_ImageRendering : PBPostProcessorBase
    {
        [SerializeField] UI.PBUIController _uiController;
        private IPBCameraBase[] cams = null;
        private DataProcessor<ImageData, byte[]> imageProcessor = null;

        public override IEnumerator CaptureFrame(int frameNumber)
        {
            //Record render texture from cameras
            for (int i = 0; i < cams.Length; ++i)
            {
                yield return cams[i].GetRenderCamera().Capture(PBFrameController.Instance.CurrentFrameIndex, PostCaptureFrame);
            }
        }

        private void PostCaptureFrame(PBRenderCamera.RenderCaptureFeedback imageData)
        {
            imageProcessor.EnqueueData(new ImageData
            {
                data = imageData.pixels,
                subDirectory = imageData.src.name,
                filename = $"F_{imageData.frameNumber}.png"
            });
        }

        public override void MarkFrameCaptureComplete()
        {
            imageProcessor.PrepareStopRecording();
            base.MarkFrameCaptureComplete();

        }

        public override IEnumerator Initialize(int framesToProcess)
        {
            if (_uiController == null)
            {
                _uiController = FindObjectOfType<UI.PBUIController>();
            }

            cams = PBCameraManager.Instance.GetPlaybackCameras();
            imageProcessor = new DataProcessor<ImageData, byte[]>(this.DataDirectory, true);
            foreach (IPBCameraBase cam in cams)
            {
                cam.GetRenderCamera().Initialize();
            }
            imageProcessor.StartRecording();

            yield return base.Initialize(framesToProcess);

            yield break;
        }

        public override IEnumerator WaitForCleanUp()
        {
            while (imageProcessor.IsRunning())
            {
                yield return null;
            }

            yield return base.WaitForCleanUp();
        }
    }

}