using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UPBS.Execution
{
    public abstract class PBFrameControllerUpdateListener : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            PBFrameController.OnFrameUpdate += Refresh;
        }

        protected virtual void OnDisable()
        {
            PBFrameController.OnFrameUpdate -= Refresh;
        }
        public abstract void Refresh();
    }
}

