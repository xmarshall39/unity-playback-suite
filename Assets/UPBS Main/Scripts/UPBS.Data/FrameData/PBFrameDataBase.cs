using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
namespace UPBS.Data
{
    /// <summary>
    /// A utlitly class capable of retrieving data from any type of FrameData once initialized to that FrameData's structure.
    /// It behaves like a generic pipeline from .csv strings into FrameData info usable at runtime.
    /// </summary>
    public class PBFrameParser
    {
        private readonly static char[] TRIM_CHARS = { '\n', ' ', '\t', '\r', '\b', '\f', '\v', '\0' };
        public Dictionary<string, int> Columns { get; private set; }
        
        public bool IsInitialized() => Columns != null && Columns.Keys.Count > 0;
        public bool Initialize(string[] columnNames)
        {
            Columns = new Dictionary<string, int>();
            for (int i = 0; i < columnNames.Length; ++i)
            {
                string s = columnNames[i].Trim(TRIM_CHARS);
                if (Columns.ContainsKey(s))
                {
                    Debug.LogWarning($"Duplicate column name \"{s}\" found at index {i}");
                }
                else
                {
                    Columns.Add(s, i);
                }

            }
            return IsInitialized();
        }
        /// <summary>
        /// Retrieve a string from 'vals' by translating the desired column 'key' into a 'vals' index
        /// </summary>
        public string GetColumnValue(string key, string[] fullRow, int rowNumber)
        {
            key = key.Trim(TRIM_CHARS);
            if (!Columns.ContainsKey(key))
            {
                Debug.LogWarning($"Provided key \"{key}\" is not a key in the column dictionary!");
                return "0";
            }
            else if (Columns[key] >= fullRow.Length)
            {
                Debug.LogWarning($"Found data column {Columns[key]} is not in range of row {rowNumber}'s provided values");
                return "0";
            }
            else
            {
                string value = fullRow[Columns[key]].Trim(TRIM_CHARS);

                if (string.IsNullOrEmpty(value))
                {
                    return "0";
                }
                else
                {
                    return value;
                }
            }
        }

        public string[] GetColumnValues(string keyBase, string[] fullRow, int rowNumber, params string[] appends)
        {
            keyBase = keyBase.Trim(TRIM_CHARS);
            string[] result = new string[appends.Length];
            for(int i = 0; i < appends.Length; ++i)
            {
                string key = string.Format("{0}_{1}", keyBase, appends[i]);
                result[i] = GetColumnValue(key, fullRow, rowNumber);
            }
            return result;
        }

        public bool GetColumnValuesAsFloats(string keyBase, string[] fullRow, int rowNumber, out float[] floatValues, params string[] appends)
        {
            string[] stringValues = GetColumnValues(keyBase, fullRow, rowNumber, appends);
            floatValues = new float[stringValues.Length];

            for(int i = 0; i < floatValues.Length; ++i)
            {
                if (float.TryParse(stringValues[i], out float temp))
                {
                    floatValues[i] = temp;
                }

                else
                {
                    return false;
                }
            }

            return true;
        }
    }

    /// <summary>
    /// A base class for frame data containers which holds common functionality.
    /// This class (and it's children) define the formatting of Tracker (and external) data collected at runtime and it's mapping in playback.
    /// This effectively synchronizes the functionality of data collection with the expectations of playback.
    /// </summary>
    [System.Serializable]
    public abstract class PBFrameDataBase : ICloneable
    {
        public static object staticDataCache = null;
        public ulong Timestamp { get; private set; } = 0;
        public bool IsEnabled { get; private set; } = true;

        public ulong InitialTimeStamp { get; private set; } = 0;
        public ulong FinalTimeStamp { get; private set; } = 0;


        public PBFrameDataBase() { }
        
        public PBFrameDataBase(string[] dataRow)
        {

        }

        public PBFrameDataBase(PBFrameDataBase other)
        {
            Timestamp = other.Timestamp;
            IsEnabled = other.IsEnabled;
            InitialTimeStamp = other.InitialTimeStamp;
            FinalTimeStamp = other.FinalTimeStamp;
        }
        /// <summary>
        /// Row parsing requires that array entries be in an order corresponding with with the column names provided to InitializeColumns()
        /// </summary>
        /// <returns>True if the FrameData is fully populated</returns>
        public bool ParseRow(PBFrameParser parser, string[] row, int rowNumber)
        {
            if (parser.IsInitialized())
            {
                if (row.Length < parser.Columns.Keys.Count)
                {
                    Debug.LogWarning($"Provided Row {rowNumber} has fewer values than can be populated!");
                }

                return ParseRowInternal(parser, row, rowNumber);
            }

            else
            {
                return false;
            }
        }

        protected virtual bool ParseRowInternal(PBFrameParser parser, string[] row, int rowNumber)
        {
            bool allClear = true;
            if(ulong.TryParse(parser.GetColumnValue("Timestamp", row, rowNumber), out ulong temp))
            {
                Timestamp = temp;
            }
            else
            {
                allClear = false;
                Debug.LogWarning($"Timestamp value in row {rowNumber} could not be parsed!");
            }

            if (bool.TryParse(parser.GetColumnValue("IsEnabled", row, rowNumber), out bool enabled))
            {
                IsEnabled = enabled;
            }
            else
            {
                allClear = false;
                Debug.LogWarning($"IsEnabled value in row {rowNumber} could not be parsed!");
            }
            return allClear;
        }
        
        public virtual string[] GetClassHeader()
        {
            return new string[] { "Timestamp", "IsEnabled" };
        }

        public void SetTimestampMetadata(ulong initial, ulong final)
        {
            InitialTimeStamp = initial;
            FinalTimeStamp = final;
        }

        /// <summary>
        /// Executed once for each file loaded into the frame library right before it's loaded.
        /// This method can be used to compute and cache infrormation common across the tracker file.
        /// </summary>
        public static void OnBeginFileParse(System.Type frameDataType)
        {

        }
        public static void OnEndFileParse()
        {
            staticDataCache = null;
        }

        #region Quick Display Methods
        public string[] GetVariableNamesDisplayAuto()
        {
            Type t = this.GetType();
            var publicFields = t.GetFields().Where(x => x.IsPublic);
            foreach (var f in publicFields) Debug.Log("Public Field: " + f.Name);

            return publicFields.Select(x => x.Name).ToArray();
        }

        public string[] GetPropertiesDisplayAuto()
        {
            Type t = this.GetType();
            var readableProperties = t.GetProperties().Where(x => x.CanRead);
            foreach (var p in readableProperties) Debug.Log("Public Field: " + p.Name);

            return readableProperties.Select(x => x.Name).ToArray();
        }

        public virtual string[] GetVariableNameDisplay()
        {
            return new string[]
            {
                "Timestamp"
            };
        }

        /// <summary>Determines the order of frame data variables for display purposes only</summary>
        /// <returns>An array of variable names in some pre-defined order</returns>
        public virtual string[] GetVariableValuesDisplay()
        {
            return new string[]
            {
                Timestamp.ToString()
            };
        }
        
        /// <summary>
        /// Determines the default value to display for frame variables before initialization
        /// </summary>
        /// <returns>A string array ordered in alignment with GetVariableNames()</returns>
        public virtual string[] GetVariableNullValuesDisplay()
        {
            return new string[]
            {
                "0"
            };
        }
       
        /// <summary>
        /// Determines the default value to display for frame variables in the event of unexpected data or parsing failure
        /// </summary>
        /// <returns>A string array ordered in alignment with GetVariableNames()</returns>
        public virtual string[] GetVariableErrorValuesDisplay()
        {
            return new string[]
            {
                "-1"
            };
        }
        #endregion

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}

