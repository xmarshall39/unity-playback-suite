using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;
using System.Linq;

namespace UPBS.Execution
{
    public class PBGlobalStatDisplay : PBVisualization
    {
        public TextMeshProUGUI frameNumber, fpsText;
        public SerializableSystemType type;
        /// <summary>
        /// Get the current global frame data and cast to ensure validity. Display FPS as a widget on the HUD
        /// </summary>
        public override void Refresh()
        {
            //frameNumber.text = $"Timestamp: {}";
            //fpsText.text = $"{}FPS";
            var b = typeof(UPBS.Data.PBFrameDataBase);
            var subclassTypes = Assembly
            .GetAssembly(typeof(UPBS.Data.PBFrameDataBase))
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(UPBS.Data.PBFrameDataBase)));
            foreach(var x in subclassTypes)
            {
                print(x.Name);
            }
        }

    }
}

