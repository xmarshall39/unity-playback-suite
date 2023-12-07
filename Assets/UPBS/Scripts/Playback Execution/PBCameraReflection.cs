using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPBS.Data;

namespace UPBS.Execution
{
    public class PBCameraReflection : PBReflection, IPBCameraBase
    {
        private PBCameraInfo _camInfo;

        public override void Refresh()
        {
            if(PBFrameLibraryManager.Instance.TryGetCurrentLibraryEntry<PBCameraFrameData>(trackerID.ID, out var frameData, name))
            {
                transform.position = frameData.WorldPosition;
                transform.eulerAngles = frameData.EulerRotation;
            }          
        }

        public void EnableCamera()
        {
            _camInfo.cam.enabled = true;
        }

        public void DisableCamera()
        {
            _camInfo.cam.enabled = false;
        }

        public PBRenderCamera GetRenderCamera()
        {
            return _camInfo.renderCamera;
        }

        protected override void Start()
        {
            base.Start();
            if (TryGetComponent(out Camera cam))
            {
                _camInfo = new PBCameraInfo() { cam = cam, name = gameObject.name };
            }
            else
            {
                Debug.LogError($"No Camera component attached to {gameObject.name}");
            }

            if (!TryGetComponent(out PBRenderCamera renderCam))
            {
                renderCam = gameObject.AddComponent<PBRenderCamera>();
            }

            _camInfo.renderCamera = renderCam;
        }
    }
}

