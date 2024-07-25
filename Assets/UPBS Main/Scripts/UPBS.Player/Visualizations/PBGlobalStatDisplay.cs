using TMPro;
using System.Reflection;
using System.Linq;
using UPBS.Utility;
using UPBS.Data;

namespace UPBS.Player
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
            var b = typeof(PBFrameDataBase);
            var subclassTypes = Assembly
            .GetAssembly(typeof(PBFrameDataBase))
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(PBFrameDataBase)));
            foreach(var x in subclassTypes)
            {
                print(x.Name);
            }
        }

    }
}

