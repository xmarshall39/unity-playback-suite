using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPBS.Data;
using System.Reflection;
using Unity.Collections;
public class ParserTest : MonoBehaviour
{
    public enum E1
    {
        A, B, C
    }

    public enum E2
    {
        A, B, C
    }

    [ContextMenu("Test")]
    public void Test()
    {
        PBFrameParser parser = new PBFrameParser();
        print("Name: " + parser.GetType().Name);
        print("Full Name: " + parser.GetType().FullName);
        print("Assembly Qualified Name: " + parser.GetType().AssemblyQualifiedName);

        //PROOF THAT INFORMATION ISN'T LOST WHEN WE CAST UP THE INHERITANCE TREE!!!
        PBGlobalFrameData FD = new PBGlobalFrameData();

        System.Type objType = System.Type.GetType(FD.GetType().AssemblyQualifiedName);
        print(objType.FullName);
        object obj = System.Activator.CreateInstance(objType);
        PBFrameDataBase b = (PBFrameDataBase)obj;
        parser.Initialize(b.GetClassHeader());
        foreach (var k in parser.Columns.Keys) print(k);
    }

    [EasyButtons.Button]
    public void Test2()
    {
        System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch e = new System.Diagnostics.Stopwatch();
        e.Start();
        s.Start();
        System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        s.Stop();
        print($"GetAssemblies() time is {s.ElapsedMilliseconds}ms on {assemblies.Length} assembiles");
        s.Reset();

        for (int assI = 0; assI < assemblies.Length; ++assI)
        {

            s.Start();
            System.Type[] allTypes = assemblies[assI].GetTypes();
            s.Stop();
            if (allTypes.Length > 1000)
                print($"GetTypest() time for assembly {assI} is {s.ElapsedMilliseconds}ms on {allTypes.Length} types");
            s.Reset();


            HashSet<System.Type> baseTypes = new HashSet<System.Type>(allTypes);
            HashSet<System.Type> nestedTypes = new HashSet<System.Type>();
            s.Start();
            for (int t = 0; t < allTypes.Length; ++t)
            {
                var nested = allTypes[t].GetNestedTypes();
                foreach ( var n in nested)
                {
                    nestedTypes.Add(n);
                }
            }
            s.Stop();
            if(nestedTypes.Count > 100)
                print($"GetNestedTypes() time on {allTypes.Length} types is {s.ElapsedMilliseconds}ms for {nestedTypes.Count} hits ");
            s.Reset();
            
            baseTypes.IntersectWith(nestedTypes);
            
            if(baseTypes.Count != nestedTypes.Count)
            {
                print("laskihdgfp;oaslehgp;oiewqahiogf;ewqajhw;ogfilajep;luoifhawp;eofh;aswoiseehgfpoisarjhg;klasjhgloiewhg;kosedrhgklisudgvkoisdahgoijuzdfhgklijuashgfjklsqdahgoishdapoiugfhaspo;igf[oigaeoeuirg;lasdgh;laskdjhg;oasrdjhfo9");
            }
            
            print(nestedTypes.Count);
        }
        e.Stop();
        print($"Total Time: {e.ElapsedMilliseconds}ms");
    }
    [EasyButtons.Button]
    public void Test3()
    {
        print(Application.dataPath);
        print(Application.persistentDataPath);
    }
    [EasyButtons.Button]
    public void Test4()
    {
        string[] selected = SFB.StandaloneFileBrowser.OpenFolderPanel("Select data directory", "", false);
        if (selected != null && selected.Length > 0)
        {
            for (int i = 0; i < selected.Length; ++i) print(selected[i]);
        }
    }
    [EasyButtons.Button]
    public void Test5()
    {
        PBTrackerInfo info = new PBTrackerInfo() { frameDataAssemblyName = typeof(PBGlobalFrameData).AssemblyQualifiedName, TID = 10 };
        print(JsonUtility.ToJson(info));
    }
}


