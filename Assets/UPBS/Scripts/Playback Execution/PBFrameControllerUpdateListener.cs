using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UPBS.Execution
{
    public abstract class PBFrameControllerUpdateListener : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            if (PBFrameControllerManager.Instance)
            {
                PBFrameControllerManager.Instance.OnFrameUpdate += Refresh;
            }
        }
        public abstract void Refresh();
    }
}

