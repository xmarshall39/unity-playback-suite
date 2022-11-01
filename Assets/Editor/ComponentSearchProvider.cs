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
        public Assembly assembly;

        public override string ToString()
        {
            return $"Type: {type.AssemblyQualifiedName} | Assembly: {assembly.FullName}";
        }
    }

    private List<ComponentData> components = new List<ComponentData>();

    private void OnEnable()
    {
        foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach(Type type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(Component)))
                {
                    components.Add(new ComponentData() { type = type, assembly = assembly });
                }
            }
        }


    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> searchList = new List<SearchTreeEntry>();



        return searchList;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        throw new System.NotImplementedException();
    }


}
