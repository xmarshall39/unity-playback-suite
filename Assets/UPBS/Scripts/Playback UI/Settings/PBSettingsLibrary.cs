using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPBS.UI;
using System.Reflection;

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

    public class PBSettingsAttribute : System.Attribute
    {
        public string Name { get; private set; }
        public SettingsTab Tab { get; private set; }
        public object DefaultValue { get; private set; }
        public SettingType Type { get; private set; } = SettingType.None;

        public PBSettingsAttribute(string name, SettingsTab tab, object defaultValue)
        {
            Name = name;
            Tab = tab;
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

    public struct SettingsData
    {
        public PBSettingsAttribute attribute;
        public PropertyInfo property;
    }

    public static class PBSettingsLibrary
    {
        public static bool IsInitialized { get; private set; }

        private static Dictionary<SettingsTab, List<SettingsData>> _library;

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
                        PBSettingsAttribute pbSettings = attr as PBSettingsAttribute;
                        if (pbSettings != null)
                        {
                            if (!_library.ContainsKey(pbSettings.Tab))
                            {
                                _library[pbSettings.Tab] = new List<SettingsData>();
                            }

                            _library[pbSettings.Tab].Add(new SettingsData { attribute = pbSettings, property = prop });

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

        public static void UnInitialize()
        {
            if (IsInitialized)
            {
                _library.Clear();
                IsInitialized = false;
            }
        }

        #region Settings

        [PBSettings("Image Width", SettingsTab.Export, 16)]
        public static int IRWidth { get; set; } = 16;

        [PBSettings("Image Height", SettingsTab.Export, 9)]
        public static int IRHeight { get; set; } = 9;

        [PBSettings("Depth Buffer", SettingsTab.Export, 24)]
        public static int IRDepthBuffer { get; set; } = 24;

        [PBSettings("Image Format", SettingsTab.Export, RenderTextureFormat.ARGB32)]
        public static RenderTextureFormat IRFormat { get; set; }

        /* We'll hardcode a set of visualization booleans for the image rendering (but not actually use it)
         * Figma mock-up of 
         */

        [PBSettings("Main Camera", SettingsTab.Visualization, false)]
        public static bool MainCameraEnabled { get; set; } = false;

        [PBSettings("Back Camera", SettingsTab.Visualization, false)]
        public static bool BackCameraEnabled { get; set; } = false;

        [PBSettings("Side Camera", SettingsTab.Visualization, false)]
        public static bool SideCameraEnabled { get; set; } = false;

        [PBSettings("Free Camera", SettingsTab.Visualization, false)]
        public static bool FreeCamEnabled { get; set; } = false;


        #endregion
    }
}