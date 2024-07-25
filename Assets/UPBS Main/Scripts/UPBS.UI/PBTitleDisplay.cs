using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UPBS.UI
{
    public class PBTitleDisplay : Player.PBFrameControllerUpdateListener
    {
        private static readonly int variableCharLimit = 8;
        private readonly string elipsis = "...";

        [SerializeField]
        private TextMeshProUGUI titleText;
        public override void Refresh()
        {
            
        }

        protected override void OnPlaybackControllerInit()
        {
            var frameLibrary = Player.PBFrameLibraryManager.Instance;
            string participant, session, trial;
            string loadedDirectory = UPBS.Player.PBLoadingManager.LoadedDirectory;
            
            string[] splitDirectory = loadedDirectory.Split(System.IO.Path.DirectorySeparatorChar);
            int splitLen = splitDirectory.Length;

            trial = splitDirectory[splitLen - 1].Length > variableCharLimit ? splitDirectory[splitLen - 1].Substring(0, variableCharLimit) + elipsis : splitDirectory[splitLen - 1];
            session = splitDirectory[splitLen - 2].Length > variableCharLimit ? splitDirectory[splitLen - 2].Substring(0, variableCharLimit) + elipsis : splitDirectory[splitLen - 2];
            participant = splitDirectory[splitLen - 3].Length > variableCharLimit ? splitDirectory[splitLen - 3].Substring(0, variableCharLimit) + elipsis : splitDirectory[splitLen - 3];

            titleText.text = $"Participant: {participant} | Session: {session} | Trial: {trial}";
        }
    }
}

