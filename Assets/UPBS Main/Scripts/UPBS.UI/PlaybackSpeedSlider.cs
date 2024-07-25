using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UPBS.UI
{
    public class PlaybackSpeedSlider : UPBS.Player.PBFrameControllerUpdateListener //This may not be a permanent fixture
    {
        [SerializeField]
        private Slider _speedSlider;

        protected override void OnPlaybackControllerInit()
        {
            _speedSlider.wholeNumbers = true;
            _speedSlider.minValue = UPBS.Player.PBFrameController.Instance.MinSpeed;
            _speedSlider.maxValue = UPBS.Player.PBFrameController.Instance.MaxSpeed;
            _speedSlider.value = UPBS.Player.PBFrameController.Instance.MaxSpeed / 2;
        }

        public override void Refresh()
        {
            
        }

        public void UpdateSpeed()
        {
            UPBS.Player.PBFrameController.Instance.Speed = (int)_speedSlider.value;
        }
    }
}

