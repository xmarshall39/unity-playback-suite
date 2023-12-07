using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPBS.Execution
{
    public class PBPostProcessingController : MonoBehaviour
    {
        public void StartImageRendering_OnClick()
        {

        }

        public void StopImageRendering_OnClick()
        {

        }

        public void StartPostProcess_OnClick()
        {

        }

        public void StopPostProcess_OnClick()
        {

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R) && PBPostProcessManager.Instance)
            {
                PBPostProcessManager.Instance.TryBeginPostProcess();
            }
        }
    }

}