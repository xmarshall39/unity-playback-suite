using System.IO;
using UnityEngine;
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

        private void Start()
        {
            _camera = GetComponent<Camera>();
        }

        public void Initialize()
        {
            renderTexture = RenderTexture.GetTemporary(PBSettingsLibrary.IRWidth, PBSettingsLibrary.IRHeight, PBSettingsLibrary.IRDepthBuffer, PBSettingsLibrary.IRFormat);
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

        public byte[] Capture()
        {
            if (initialized)
            {
                _camera.Render();
                RenderTexture.active = renderTexture;
                screenshot.Apply();
                screenshot.ReadPixels(captureRect, 0, 0);
                byte[] screenPNG = screenshot.EncodeToPNG();
                RenderTexture.active = null;
                return screenPNG;
            }

            UnityEngine.Debug.LogWarning("Calling CameraCapture.Capture() without first calling CameraCatpure.Initialize()!");
            return null;
        }
    }
}