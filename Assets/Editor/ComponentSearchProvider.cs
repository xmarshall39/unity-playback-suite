using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System.Reflection;
using System;

public class ComponentSearchProvider : ScriptableObject, ISearchWindowProvider
{
    private struct ComponentData
    {
        public Type type;
        public string assemblyDirectory;

        public override string ToString()
        {
            return $"Type: {type.AssemblyQualifiedName} | Assembly: {assemblyDirectory}";
        }
    }

    private List<ComponentData> components = null;
    private static readonly Dictionary<string, string> AssemblyDirectories = new Dictionary<string, string>()
    {
        { "AIModule", "Navigation" },
        { "AnimationModule", "Animation" }, //Animation would be better, but misc is more accurate
        { "AudioModule", "Audio" },
        { "ClothModule", "Physics" },
        { "CoreModule", "CoreModuleSortLater" },
        { "DirectorModule", "Playables" },
        { "GridModule", "Layout" },
        { "ParticleSystemModule", "Effects" },
        { "PhysicsModule", "Physics" },
        { "Physics2DModule", "Physics 2D" },
        { "Purchasing", "Unity IAP" },
        { "SpatialTracking", "XR" },
        { "SpriteMaskModule", "Rendering" },
        { "TerrainModule", "Miscellaneous" },
        { "TerrainPhysicsModule", "Physics" },
        { "TextMeshPro", "UI" },
        { "TilemapModule", "Tilemap" },
        { "VFXModule", "Effects" },
        { "VehiclesModule", "Physics" },
        { "VideoModule", "Video" },
        { "WindModule", "Misc" },
        { "XR", "XR" }
    };
    private void OnEnable()
    {

    }
    private void CreateComponentTree()
    {
        //Creates an auto-cache
        if (components == null)
        {
            components = new List<ComponentData>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (!type.IsAbstract)
                    {
                        if (type.IsSubclassOf(typeof(Component)))
                        {
                            string treeDir;
                            string[] assemblySplit = assembly.GetName().Name.Split('.');
                            if (assemblySplit.Length >= 2 && assemblySplit[0] == "UnityEngine")
                            {
                                if (!AssemblyDirectories.TryGetValue(assemblySplit[1], out treeDir))
                                {
                                    treeDir = "Miscellaneous/";
                                }

                                else
                                {
                                    treeDir += "/";
                                }

                                treeDir += type.Name;
                            }

                            else
                            {
                                treeDir = "Scripts/";
                                if (!string.IsNullOrEmpty(type.Namespace))
                                    treeDir += type.Namespace + "/";
                                treeDir += type.Name;
                            }
                            components.Add(new ComponentData() { type = type, assemblyDirectory = treeDir });
                        }
                    }

                }
            }
        }
    }
    private void OnDisable()
    {
        components = null;
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        CreateComponentTree();
        List<SearchTreeEntry> searchList = new List<SearchTreeEntry>();
        searchList.Add(new SearchTreeGroupEntry(new GUIContent("Components"), 0));
        List<string> groups = new List<string>();

        foreach(ComponentData comp in components)
        {
            string[] entrySplit = comp.assemblyDirectory.Split('/');
            string groupName = "";
            for(int i = 0; i < entrySplit.Length - 1; ++i)
            {
                groupName += entrySplit[i];
                if (!groups.Contains(groupName))
                {
                    searchList.Add(new SearchTreeGroupEntry(new GUIContent(entrySplit[i]), i + 1));
                    groups.Add(groupName);
                }
                groupName += "/";
            }
            SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(entrySplit[entrySplit.Length - 1]));
            entry.level = entrySplit.Length;
            entry.userData = entrySplit[entrySplit.Length - 1];
            searchList.Add(entry);
        }

        return searchList;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        return false;
    }


}
