using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPBS.Player
{
    public abstract class PBCameraVisualization : PBVisualization, IPBCameraBase
    {
        [SerializeField]
        private PBCameraInfo _camInfo;

        public void DisableCamera()
        {
            _camInfo.cam.enabled = false;
        }

        public void EnableCamera()
        {
            _camInfo.cam.enabled = true;
        }

        public PBRenderCamera GetRenderCamera()
        {
            return _camInfo.renderCamera;
        }
    }
}
