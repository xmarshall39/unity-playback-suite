using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UPBS.UI
{
    public class PBProgressSlider : Execution.PBFrameControllerUpdateListener
    {
        [SerializeField]
        private Slider frameProgressBar;
        public override void Refresh()
        {
            frameProgressBar.SetValueWithoutNotify(Execution.PBFrameController.Instance.CurrentFrameIndex);
        }

        protected override void Init()
        {
            var frameController = Execution.PBFrameController.Instance;
            frameProgressBar.wholeNumbers = true;
            frameProgressBar.minValue = 1;
            frameProgressBar.maxValue = frameController.FrameCount;
            frameProgressBar.SetValueWithoutNotify(1);
        }

        //Auto pause when the slider is grabbed

        //Manually update the frame
        public void ManualFrameUpdate()
        {
            Execution.PBFrameController.Instance.SetFrame((int)frameProgressBar.value);
        }

    }
}

