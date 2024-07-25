using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UPBS.Player
{
    /// <summary>
    /// Any object that responds to frame changes in the playback (i.e. visualizations) should inherit this
    /// </summary>
    public abstract class PBFrameControllerUpdateListener : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            PBFrameController.OnFrameUpdate += Refresh;
            PBFrameController.OnInit += OnPlaybackControllerInit;

        }

        protected virtual void OnDisable()
        {
            

        }

        private void OnDestroy()
        {
            PBFrameController.OnFrameUpdate -= Refresh;
            PBFrameController.OnInit -= OnPlaybackControllerInit;
        }

        protected virtual void OnPlaybackControllerInit()
        {

        }

        public abstract void Refresh();

    }
}

