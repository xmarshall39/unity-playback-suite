using UnityEngine;
using UnityEditor;

namespace UPBS
{
    public class UPBSEditorStyles : MonoBehaviour
    {
        private static GUIStyle header1;
        private static GUIStyle header2;
        private static GUIStyle header3;
        private static GUIStyle utilityButton;

        public static GUIStyle Header1
        {
            get
            {
                if(header1 == null)
                {
                    header1 = new UnityEngine.GUIStyle(EditorStyles.label);

                    header1.fontStyle = FontStyle.Bold;
                    header1.fontSize = 16;
                    header1.padding = new RectOffset(2, 2, 7, 5);
                }

                return header1;
            }
        }

        public static GUIStyle Header2
        {
            get
            {
                if (header2 == null)
                {
                    header2 = new UnityEngine.GUIStyle(EditorStyles.label);

                    header1.fontStyle = FontStyle.Bold;
                    header2.fontSize = 14;
                    header2.padding = new RectOffset(2, 2, 6, 4);
                }

                return header2;
            }
        }

        public static GUIStyle Header3
        {
            get
            {
                if (header3 == null)
                {
                    header3 = new UnityEngine.GUIStyle(EditorStyles.label);

                    header3.fontStyle = FontStyle.Bold;
                    header3.fontSize = 12;
                    header3.padding = new RectOffset(2, 2, 5, 3);
                }

                return header3;
            }
        }

        public static GUIStyle UtilityButton
        {
            get
            {
                if(utilityButton == null)
                {
                    utilityButton = new GUIStyle(GUI.skin.button);

                    utilityButton.fontStyle = Header1.fontStyle;
                    utilityButton.fontSize = Header1.fontSize;
                    utilityButton.padding = Header1.padding;
                }

                return utilityButton;
            }
        }

    }
}

