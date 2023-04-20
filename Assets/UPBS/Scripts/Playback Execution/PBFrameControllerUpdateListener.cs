using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UPBS.Execution
{
    /// <summary>
    /// Any object that responds to frame changes in the playback (i.e. visualizations) should inherit this
    /// </summary>
    public abstract class PBFrameControllerUpdateListener : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            PBFrameController.OnFrameUpdate += Refresh;
            PBFrameController.OnInit += Init;

        }

        protected virtual void OnDisable()
        {
            PBFrameController.OnFrameUpdate -= Refresh;
            PBFrameController.OnInit -= Init;

        }

        protected virtual void Init()
        {

        }

        public abstract void Refresh();

    }
}

