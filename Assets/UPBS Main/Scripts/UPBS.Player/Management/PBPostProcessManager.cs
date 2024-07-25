using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;
using UnityEngine;
using UPBS.UI;

namespace UPBS.Player
{
    public class PBPostProcessManager : MonoBehaviour
    {
        #region Singleton
        private static PBPostProcessManager _instance;
        public static PBPostProcessManager Instance
        {
            get
            {
                return _instance;
            }
        }
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        public static string POST_PROCESS_FOLDER_NAME = "PostProcess";

        [SerializeField]
        private Camera _IRCam;
        [SerializeField]
        private Canvas _IRCanvas;
        [SerializeField]
        private TMPro.TextMeshProUGUI _IRProgressText;
        [SerializeField] UI.PBUIController _uiController;

        private Coroutine _renderRoutine = null;
        private PBPostProcessorBase[] postProcessors = null;

        public bool TryBeginPostProcess()
        {
            if (_renderRoutine != null)
            {
                return false;
            }

            if (_uiController == null)
            {
                _uiController = FindObjectOfType<UI.PBUIController>();
            }

            if (postProcessors == null)
            {
                postProcessors = FindObjectsOfType<PBPostProcessorBase>();
            }

            string PostProcessDirectory = Path.Combine(PBLoadingManager.LoadedDirectory, POST_PROCESS_FOLDER_NAME);
            if (!Directory.Exists(PostProcessDirectory))
            {
                Directory.CreateDirectory(PostProcessDirectory);
            }

            //Pause Playback
            PBFrameController.Instance.Pause();
            //Set Frame to 0
            PBFrameController.Instance.SetFrame(0);
            //Disable Input

            //TODO: Hide all UI
            _uiController?.ChangeUIState(PBUIController.PBUIState.MainManualHidden);
            _uiController.InputEnabled = false;
            //Hide all playback Cameras => Or maybe not
            //Enable the IR cam

            //Check for correct components

            //Start IR Routine
            _IRCanvas.gameObject.SetActive(true);
            _IRCam.gameObject.SetActive(true);
            _IRProgressText.text = $"0/{PBFrameController.Instance.FinalFrame}";
            _renderRoutine = StartCoroutine(PostProcess());
            return true;
        }

        private IEnumerator PostProcess()
        {
            foreach (var processor in postProcessors)
            {
                yield return processor.Initialize(PBFrameController.Instance.FinalFrame - 1);
            }

            while (PBFrameController.Instance.CurrentFrameIndex != PBFrameController.Instance.FinalFrame)
            {
                int currentFrame = PBFrameController.Instance.CurrentFrameIndex;
                foreach (var processor in postProcessors)
                {
                    yield return processor.CaptureFrame(currentFrame);
                }


                PBFrameController.Instance.SkipFramesForward(1);
                int[] progressStates = new int[postProcessors.Length];
                for (int i = 0; i < postProcessors.Length; ++i)
                {
                    progressStates[i] = postProcessors[i].CurrentProgress;
                }
                
                _IRProgressText.text = $"{Mathf.Min(progressStates)}/{PBFrameController.Instance.FinalFrame}";
                yield return null;
            }

            foreach (var processor in postProcessors)
            {
                processor.MarkFrameCaptureComplete();
            }

            foreach (var processor in postProcessors)
            {
                yield return processor.WaitForCleanUp();
            }

            PostProcessCleanup();
        }

        private void PostProcessCleanup()
        {
            _IRCanvas.gameObject.SetActive(false);
            _IRCam.gameObject.SetActive(false);
            _IRProgressText.text = $"0/{PBFrameController.Instance.FinalFrame}";
            _uiController.InputEnabled = true;
            _renderRoutine = null;
            _uiController?.ChangeUIState(PBUIController.PBUIState.MainShown);
        }
    }

}