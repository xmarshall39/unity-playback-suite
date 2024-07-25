using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPBS.Player
{
    public struct PBCameraInfo
    {
        public string name;
        public Camera cam;
        public PBRenderCamera renderCamera;
    }

    public interface IPBCameraBase
    {
        void EnableCamera();
        void DisableCamera();
        PBRenderCamera GetRenderCamera();
    }
}

