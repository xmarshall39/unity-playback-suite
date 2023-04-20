using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UPBS.UI
{
    public class PlaybackSpeedSlider : UPBS.Execution.PBFrameControllerUpdateListener //This may not be a permanent fixture
    {
        [SerializeField]
        private Slider _speedSlider;

        protected override void Init()
        {
            _speedSlider.wholeNumbers = true;
            _speedSlider.minValue = UPBS.Execution.PBFrameController.Instance.MinSpeed;
            _speedSlider.maxValue = UPBS.Execution.PBFrameController.Instance.MaxSpeed;
            _speedSlider.value = UPBS.Execution.PBFrameController.Instance.MaxSpeed / 2;
        }

        public override void Refresh()
        {
            
        }

        public void UpdateSpeed()
        {
            UPBS.Execution.PBFrameController.Instance.Speed = (int)_speedSlider.value;
        }
    }
}

