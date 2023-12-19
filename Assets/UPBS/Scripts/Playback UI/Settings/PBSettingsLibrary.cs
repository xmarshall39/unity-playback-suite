using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPBS.UI;
using System.Reflection;
using System;

namespace UPBS.Utility
{
    public enum SettingsTab
    {
        General,
        Camera,
        Visualization,
        Export
    }

    public enum SettingType
    {
        None,
        Bool,
        Integer,
        String,
        Enum
    }

    /// <summary>
    /// Marks classes which may contain settings attributes among it's public and private members. These should only be applied to monobeahviours
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class PBSettingsMonoAttribute : Attribute
    {
        public SettingsTab Tab { get; private set; }

        public PBSettingsMonoAttribute(SettingsTab tab)
        {
            Tab = tab;
        }

    }



    public abstract class PBSettingsAttribute : System.Attribute
    {
        public string Name { get; private set; }
        public object DefaultValue { get; private set; }
        public SettingType Type { get; private set; } = SettingType.None;
        public SettingsTab Tab { get; protected set; }

        public PBSettingsAttribute(string name, object defaultValue)
        {
            Name = name;
            DefaultValue = defaultValue;

            if (defaultValue is System.Enum)
            {
                Type = SettingType.Enum;
            }

            else if (defaultValue is int)
            {
                Type = SettingType.Integer;
            }

            else if (defaultValue is bool)
            {
                Type = SettingType.Bool;
            }

            else if (defaultValue is string)
            {
                Type = SettingType.String;
            }

            else
            {
                Type = SettingType.None;
                Debug.LogWarning($"Setting \"{name}\": Default setting value not valid!");
            }
        }
    }


    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PBSettingsVariableAttribute : PBSettingsAttribute
    {
        public PBSettingsVariableAttribute(string name, object defaultValue) : base(name, defaultValue)
        {

        }

        public void SetTab(SettingsTab tab)
        {
            Tab = tab;
        }

    }


    /// <summary>
    /// Settings attribute for application on static variables. These settings will have no container and be serialized independently.
    /// </summary>
    public class PBStaticSettingsAttribute : PBSettingsAttribute
    {
        public PBStaticSettingsAttribute(string name, SettingsTab tab, object defaultValue) : base (name, defaultValue)
        {
            Tab = tab;
        }

    }


    public struct SettingsData
    {
        public PBSettingsAttribute attribute;
        public FieldInfo property;
    }

    public class SettingsAttributeContainer
    {
        public string uniqueName;
        public Type instanceType;
        public UnityEngine.Object instanceRef;
        public PBSettingsAttribute attribute;
        public MemberInfo member;
    }

    public static class PBSettingsLibrary
    {
        public static bool IsInitialized { get; private set; }

        private static Dictionary<SettingsTab, List<SettingsData>> _library;

        private static Dictionary<SettingsTab, Dictionary<string, SettingsAttributeContainer>> _lib;

        public static List<SettingsData> GetSettingsOfType(SettingsTab tab)
        {
            if (IsInitialized)
            {
                if (_library.ContainsKey(tab))
                {
                    return new List<SettingsData>(_library[tab]);
                }

                else
                {
                    return new List<SettingsData>();
                }
            }

            return null;
        }

        public static void ResetToDefault()
        {
            if (IsInitialized)
            {
                foreach (var kvp in _library)
                {
                    foreach(var data in kvp.Value)
                    {
                        data.property.SetValue(null, data.attribute.DefaultValue);
                    }
                }
            }
        }

        public static void ResetToDefault(SettingsTab tab)
        {
            if (IsInitialized)
            {
                foreach (var kvp in _library)
                {
                    foreach (var data in kvp.Value)
                    {
                        if(data.attribute.Tab == tab)
                        {
                            data.property.SetValue(null, data.attribute.DefaultValue);
                        }
                    }
                }
            }
        }

        public static void Initialize()
        {
            if (!IsInitialized)
            {
                _library = new Dictionary<SettingsTab, List<SettingsData>>();

                var settingLibProps = typeof(PBSettingsLibrary).GetProperties();
                foreach (var prop in settingLibProps)
                {
                    foreach (var attr in prop.GetCustomAttributes(true))
                    {
                        PBStaticSettingsAttribute pbSettings = attr as PBStaticSettingsAttribute;
                        if (pbSettings != null)
                        {
                            if (!_library.ContainsKey(pbSettings.Tab))
                            {
                                _library[pbSettings.Tab] = new List<SettingsData>();
                            }

                            //_library[pbSettings.Tab].Add(new SettingsData { attribute = pbSettings, property = prop });

                        }
                    }
                }

                foreach(var kvp in _library)
                {
                    kvp.Value.Sort(delegate (SettingsData c1, SettingsData c2) { return c1.attribute.Name.CompareTo(c2.attribute.Name); });
                }
                IsInitialized = true;
            }
        }

        public static void Init()
        {
            if (!IsInitialized)
            {
                _lib = new Dictionary<SettingsTab, Dictionary<string, SettingsAttributeContainer>>();
                var currentAssembly = Assembly.GetExecutingAssembly();
                HashSet<string> usedNames = new HashSet<string>();

                // Check all monobeaviour classes and find those which are valid settings containers
                foreach (Type type in currentAssembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(MonoBehaviour)))
                    {
                        var containerAttribute = type.GetCustomAttribute(typeof(PBSettingsMonoAttribute), true);
                        if (containerAttribute != null)
                        {
                            PropertyInfo[] propInfo = type.GetProperties();
                            FieldInfo[] fieldInfo = type.GetFields();

                            List<PropertyInfo> settingsProps = new List<PropertyInfo>();
                            List<FieldInfo> settingsFields = new List<FieldInfo>();

                            foreach (var field in fieldInfo)
                            {
                                var attr = field.GetCustomAttribute(typeof(PBSettingsVariableAttribute), true) as PBSettingsVariableAttribute;
                                if (attr != null)
                                {
                                    settingsFields.Add(field);
                                }
                            }

                            foreach (var prop in propInfo)
                            {
                                var attr = prop.GetCustomAttribute(typeof(PBSettingsVariableAttribute), true) as PBSettingsVariableAttribute;
                                if (attr != null)
                                {
                                    settingsProps.Add(prop);
                                }
                            }

                            UnityEngine.Object[] activeConfigObjects = GameObject.FindObjectsOfType(type);
                            foreach (var obj in activeConfigObjects)
                            {
                                foreach (var prop in settingsProps)
                                {
                                    PBSettingsVariableAttribute attr = prop.GetCustomAttribute<PBSettingsVariableAttribute>(true);
                                    // Simple Unique Name Generation
                                    int i = 0;
                                    string baseName = $"{type.Name}_{attr.Name}";
                                    string generatedName = $"{type.Name}_{attr.Name}";
                                    while (usedNames.Contains(generatedName))
                                    {
                                        ++i;
                                        generatedName = baseName + "_" + i;
                                    }

                                    new SettingsAttributeContainer()
                                    {
                                        instanceRef = obj,
                                        member = prop,
                                        attribute = attr,
                                        instanceType = type,
                                        uniqueName = generatedName
                                    };

                                    usedNames.Add(generatedName);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void UnInitialize()
        {
            if (IsInitialized)
            {
                _library.Clear();
                IsInitialized = false;
            }
        }

        #region Settings

        [PBStaticSettings("Image Width", SettingsTab.Export, 16)]
        public static int IRWidth { get; set; } = 16;

        [PBStaticSettings("Image Height", SettingsTab.Export, 9)]
        public static int IRHeight { get; set; } = 9;

        [PBStaticSettings("Depth Buffer", SettingsTab.Export, 24)]
        public static int IRDepthBuffer { get; set; } = 24;

        [PBStaticSettings("Image Format", SettingsTab.Export, RenderTextureFormat.ARGB32)]
        public static RenderTextureFormat IRFormat { get; set; }

        /* We'll hardcode a set of visualization booleans for the image rendering (but not actually use it)
         * Figma mock-up of 
         */

        [PBStaticSettings("Main Camera", SettingsTab.Visualization, false)]
        public static bool MainCameraEnabled { get; set; } = false;

        [PBStaticSettings("Back Camera", SettingsTab.Visualization, false)]
        public static bool BackCameraEnabled { get; set; } = false;

        [PBStaticSettings("Side Camera", SettingsTab.Visualization, false)]
        public static bool SideCameraEnabled { get; set; } = false;

        [PBStaticSettings("Free Camera", SettingsTab.Visualization, false)]
        public static bool FreeCamEnabled { get; set; } = false;


        #endregion
    }
}

