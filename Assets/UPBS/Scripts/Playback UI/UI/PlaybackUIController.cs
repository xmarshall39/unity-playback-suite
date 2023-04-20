using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPBS.UI
{
    public class PlaybackUIController : MonoBehaviour
    {
        //Represent the UI via some state machine, maybe?. => Main scene, Settings Menu
        public enum UIState
        {
            InTransition,
            Hidden,
            MainOverlay,
            Settings
        }

        #region Singleton
        //Singleton
        private static PlaybackUIController _instance;
        public static PlaybackUIController Instance
        {
            get => _instance;
        }
        private void Awake()
        {
            if (_instance)
            {
                Destroy(this.gameObject);
            }

            else
            {
                _instance = this;
            }
        }
        #endregion

        // CONSIDER MODULARIZING THESE WINDOWS AND KEEPING THEIR FUNCTIONALITY THERE. THIS'LL BE GOOD FOR STUFF LIKE THE GRAPH WINDOWS
        public UIState CurrentState { get; private set; }
        public UIState PreviousState { get; private set; }
        public UIState TargetState { get; private set; }
        private Vector3 lastMousePosition;
        private float timeSinceMouseMoved = 0f;
        private float timeUntilOverlayHides = 1.5f;
        //How to handle UI focus. If an overlay panel is open and I click outside of it, what happens to the active overlay? Maybe nothing.
        //Detect mouse movement and reveal the video player overlay
        private void Update()
        {
            switch (CurrentState)
            {
                case UIState.Hidden:
                    if (Input.GetAxis("Mouse X") > 0 || Input.GetAxis("Mouse Y") > 0)
                    {
                        //Show Overlay
                    }
                    break;

                case UIState.MainOverlay:
                    if(Vector3.Equals(lastMousePosition, Input.mousePosition))
                    {
                        timeSinceMouseMoved += Time.deltaTime;
                    }

                    else
                    {
                        timeSinceMouseMoved = 0f;
                    }

                    if(timeSinceMouseMoved >= timeUntilOverlayHides)
                    {
                        //Hide overlay
                    }

                    
                    lastMousePosition = Input.mousePosition;
                    break;
                case UIState.Settings:

                    break;
            }

            
        }

    }
}

