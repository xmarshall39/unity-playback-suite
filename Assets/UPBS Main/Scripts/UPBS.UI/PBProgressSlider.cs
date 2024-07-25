using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UPBS.UI
{
    public class PBProgressSlider : Player.PBFrameControllerUpdateListener
    {
        [SerializeField]
        private Slider frameProgressBar;
        public override void Refresh()
        {
            frameProgressBar.SetValueWithoutNotify(Player.PBFrameController.Instance.CurrentFrameIndex);
        }

        protected override void OnPlaybackControllerInit()
        {
            var frameController = Player.PBFrameController.Instance;
            frameProgressBar.wholeNumbers = true;
            frameProgressBar.minValue = 1;
            frameProgressBar.maxValue = frameController.FrameCount;
            frameProgressBar.SetValueWithoutNotify(1);
        }

        //Auto pause when the slider is grabbed

        //Manually update the frame
        public void ManualFrameUpdate()
        {
            Player.PBFrameController.Instance.SetFrame((int)frameProgressBar.value);
        }

    }
}

