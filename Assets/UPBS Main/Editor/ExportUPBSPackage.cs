#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;

public class ExportUPBSPackage : MonoBehaviour
{
    [MenuItem("UPBS/Export Package")]
    public static void ExportPackage()
    {
        string version;
        if (File.Exists("Assets/UPBS Main/VERSION.txt"))
        {
            version = File.ReadAllText("Assets/UPBS Main/VERSION.txt");
        }
        else
        {
            version = "unknown";
        }

        string outName = string.Format("UPBS.v{0}.unitypackage", version);
        if (EditorUtility.DisplayDialog("Export Package", string.Format("Export package '{0}'?", outName), "Yes", "Cancel"))
        {
            string[] assets = new string[]
            {
                "Assets/UPBS Main",
                "Assets/Editor",
                "Assets/StreamingAssets",
                "Assets/UXF",
                "Assets/WebGLTemplates",
                "Assets/Unity-UI-Rounded-Corners",
                "Assets/EasyButtons",
                "Assets/TextMesh Pro"
            };

            if (!Directory.Exists("Package"))
                Directory.CreateDirectory("Package");

            string path = Path.Combine("Package", outName);

            print(path);

            ExportPackageOptions options = ExportPackageOptions.Recurse;

            AssetDatabase.ExportPackage(assets, path, options);
        }
    }
}
#endif