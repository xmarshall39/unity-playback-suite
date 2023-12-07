using UnityEngine;

namespace UPBS.UI
{
    public class PBUIController : MonoBehaviour
    {
        [System.Serializable]
        public enum PBUIState
        {
            MainAutoHidden, MainManualHidden, MainShown, SettingsShown
        }

        public PBUIState CurrentState { get; private set; } = PBUIState.MainShown;
        public bool InputEnabled { get; set; } = true;

        [SerializeField]
        private GameObject mainUI, settingsUI;

        private float idleHideTime = 2.5f;
        private float idleTimer = 0f;
        private Vector3 lastMousePosition = Vector3.zero;
        private bool inTransition;

        //Might be able to use a Unity state machine
        private bool ChangeUIState_Internal(PBUIState targetState)
        {
            switch (CurrentState)
            {
                case PBUIState.MainAutoHidden:
                    switch (targetState)
                    {
                        case PBUIState.MainShown:
                            ShowMainUI_Instant();
                            return true;
                    }
                    break;

                case PBUIState.MainShown:
                    switch (targetState)
                    {
                        case PBUIState.MainAutoHidden:
                            HideMainUI();
                            return true;
                        case PBUIState.MainManualHidden:
                            HideMainUI_Instant();
                            return true;
                        case PBUIState.SettingsShown:
                            ShowSettingsUI();
                            return true;
                    }
                    break;

                case PBUIState.MainManualHidden:
                    switch (targetState)
                    {
                        case PBUIState.MainShown:
                            ShowMainUI_Instant();
                            return true;
                    }
                    break;

                case PBUIState.SettingsShown:
                    switch (targetState)
                    {
                        case PBUIState.MainShown:
                            HideSettingsUI();
                            return true;

                        case PBUIState.MainManualHidden:
                            HideSettingsUI();
                            HideMainUI_Instant();
                            return true;
                    }
                    break;
            }

            return false;
        }

        public bool ChangeUIState(PBUIState targetState)
        {
            bool ret = ChangeUIState_Internal(targetState);
            if (ret)
            {
                CurrentState = targetState;
            }

            return ret;
        }

        public void ChangeUIState_Button(int targetState)
        {
            ChangeUIState((PBUIState)targetState);
        }

        private void HideMainUI_Instant()
        {
            mainUI.SetActive(false);
        }

        private void HideMainUI()
        {
            mainUI.SetActive(false);
        }

        private void ShowSettingsUI()
        {
            mainUI.SetActive(false);
            settingsUI.SetActive(true);
        }

        private void ShowMainUI()
        {
            mainUI.SetActive(true);
            settingsUI.SetActive(false);
        }

        private void ShowMainUI_Instant()
        {
            mainUI.SetActive(true);
        }

        private void HideSettingsUI()
        {
            settingsUI.SetActive(false);
            mainUI.SetActive(true);
        }

        void Update()
        {
            if (CurrentState == PBUIState.MainShown)
            {
                if (InputEnabled && Input.GetAxis("Mouse X") == 0 && Input.GetAxis("Mouse Y") == 0)
                {
                    idleTimer += Time.deltaTime;
                    if (idleTimer > idleHideTime)
                    {
                        ChangeUIState(PBUIState.MainAutoHidden);
                        idleTimer = 0f;
                    }
                }

                else
                {
                    idleTimer = 0f;
                }
            }

            else if (CurrentState == PBUIState.MainAutoHidden)
            {
                if (InputEnabled && Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
                {
                    ChangeUIState(PBUIState.MainShown);
                }

            }
        }
    }

}