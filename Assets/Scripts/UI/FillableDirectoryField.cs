using UnityEngine;
using TMPro;
namespace UPBS.UI
{
    //TODO: Add UPBS player prefs prefix to globals
    public class FillableDirectoryField : MonoBehaviour
    {
        private TextMeshProUGUI directoryText;

        public delegate void OnDirectoryChanged();
        public static OnDirectoryChanged directoryChanged;

        private void Start()
        {
            if(PlayerPrefs.HasKey("UPBS_ARBITRARY_KEY"))
            {
                SetDirectory(PlayerPrefs.GetString("UPBS_ARBITRARY_KEY"));
            }
        }

        private void SetDirectory(string directory)
        {
            directoryText.text = directory;
            PlayerPrefs.SetString("UPBS_ARBITRARY_KEY", directory);
            directoryChanged.Invoke();
            
        }

        public bool IsDirectoryFilled() => string.IsNullOrWhiteSpace(directoryText.text);
    }
}

