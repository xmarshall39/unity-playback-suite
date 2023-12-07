using System;
using System.IO;
using System.Collections;
using Unity.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UPBS.Utility;

namespace UPBS.Execution
{
    public class PBRenderCamera : MonoBehaviour
    {
        private Camera _camera;
        private bool initialized = false;

        private Texture2D screenshot;
        private RenderTexture renderTexture;
        private Rect captureRect;
        private string directory;

        public class ImageData
        {
            public Color32[] imgBytes;
            public uint width;
            public uint height;

            public ImageData(NativeArray<Color32> imgBytes, uint width, uint height)
            {
                this.imgBytes = imgBytes.ToArray();
                this.width = width;
                this.height = height;
            }
        }

        public class RenderCaptureFeedback
        {
            public PBRenderCamera src;
            public int frameNumber;
            public byte[] pixels;

            public RenderCaptureFeedback(PBRenderCamera src, byte[] pixels, int frameNumber)
            {
                this.src = src;
                this.frameNumber = frameNumber;
                this.pixels = pixels;
            }
        }

        private void Start()
        {
            _camera = GetComponent<Camera>();
        }

        public void Initialize()
        {

            // renderTexture = RenderTexture.GetTemporary(PBSettingsLibrary.IRWidth, PBSettingsLibrary.IRHeight, PBSettingsLibrary.IRDepthBuffer, PBSettingsLibrary.IRFormat);
            renderTexture = RenderTexture.GetTemporary(Screen.currentResolution.width, Screen.currentResolution.height, PBSettingsLibrary.IRDepthBuffer, PBSettingsLibrary.IRFormat);
            renderTexture.Create();
            if (_camera.targetTexture != null)
            {
                _camera.targetTexture.Release();
            }
            _camera.targetTexture = renderTexture;
            screenshot = new Texture2D(_camera.targetTexture.width, _camera.targetTexture.height, TextureFormat.ARGB32, false, true);
            captureRect = new Rect(0, 0, _camera.targetTexture.width, _camera.targetTexture.height);
            initialized = true;
        }

        public IEnumerator Capture(int frameNumber, Action<RenderCaptureFeedback> callback)
        {
            if (initialized)
            {
                // Render and Apply must be run before moving to the next frame in playback!
                UnityEngine.Profiling.Profiler.BeginSample("PBRenderCamera => camera.Render()");
                _camera.Render();
                UnityEngine.Profiling.Profiler.EndSample();

                RenderTexture.active = renderTexture;
                UnityEngine.Profiling.Profiler.BeginSample("PBRenderCamera => screenshot.Apply()");
                screenshot.Apply();
                UnityEngine.Profiling.Profiler.EndSample();

                if (SystemInfo.supportsAsyncGPUReadback)
                {
                    UnityEngine.Rendering.AsyncGPUReadbackRequest request = UnityEngine.Rendering.AsyncGPUReadback.Request(renderTexture, 0);
                    while (!request.done)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    NativeArray<Color32> rawColorArray = request.GetData<Color32>();

                    ImageData imageData = new ImageData(
                    rawColorArray,
                    (uint)captureRect.width,
                    (uint)captureRect.height
                    );

                    StartCoroutine(PostCapture(frameNumber, imageData, callback));

                }
                else
                {
                    // For platforms such as WebGL which don't support async gpu readback.
                    // This is an uncommon use case and will result in awful framerates while image rendering is running
                    screenshot.ReadPixels(captureRect, 0, 0);
                    UnityEngine.Profiling.Profiler.BeginSample("PBRenderCamera => screenshot.EncodeToPNG()");
                    byte[] screenPNG = screenshot.EncodeToPNG();
                    UnityEngine.Profiling.Profiler.EndSample();

                    callback?.Invoke(new RenderCaptureFeedback(this, screenPNG, frameNumber));
                }

                yield break;
            }

            UnityEngine.Debug.LogWarning("Calling CameraCapture.Capture() without first calling CameraCatpure.Initialize()!");
            yield break;
        }

        public IEnumerator PostCapture(int frameNumber, ImageData imageData, Action<RenderCaptureFeedback> callback)
        {
            byte[] screenPNG = null;
            Thread imageEncoderThread = new Thread(() => EncodeImageThread(imageData, ref screenPNG));
            imageEncoderThread.Start();
            while (imageEncoderThread.IsAlive)
            {
                yield return null;
            }
            RenderTexture.active = null;
            callback?.Invoke(new RenderCaptureFeedback(this, screenPNG, frameNumber));
        }

        private void EncodeImageThread(object obj, ref byte[] result)
        {
            ImageData data = (ImageData)obj;
            try
            {
                //var sw = System.Diagnostics.Stopwatch.StartNew();
                result = ImageConversion.EncodeArrayToPNG(data.imgBytes, GraphicsFormat.R8G8B8A8_SRGB,
                        data.width, data.height);
                //UnityEngine.Debug.Log($"Encoded to PNG in {sw.ElapsedMilliseconds}ms");
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }
}