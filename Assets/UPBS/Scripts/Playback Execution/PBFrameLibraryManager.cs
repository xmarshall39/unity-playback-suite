using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UPBS.Data;
using UPBS.Utility;

namespace UPBS.Execution
{
    public struct PBLibValue
    {
        public System.Type type;
        public List<PBFrameDataBase> frameData;
    }

    public class PBFrameLibraryManager : MonoBehaviour
    {

        #region Singleton
        private static PBFrameLibraryManager _instance;
        public static PBFrameLibraryManager Instance
        {
            get
            {
                return _instance;
            }
        }
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        //All of this stuff is populated during environment generation
        public List<int> TIDs;
        //public List<PBFrameType> 
        private Dictionary<int, (System.Type, List<PBFrameDataBase>)> library;
        private Dictionary<System.Type, PBFrameParser> parserDictionary;
        private ConcurrentDictionary<System.Type, PBFrameParser> _parserDictionary;

        private void Start()
        {
            library = new Dictionary<int, (System.Type, List<PBFrameDataBase>)>();
            parserDictionary = new Dictionary<System.Type, PBFrameParser>();
        }

        public bool AddLibraryEntry(PBTrackerInfo trackerInfo, string[] fullFile)
        {
            System.Type type = System.Type.GetType(trackerInfo.frameDataAssemblyName);
            if (!parserDictionary.ContainsKey(type))
            {
                //PBFrameDataBase parserTemplate = (PBFrameDataBase)System.Activator.CreateInstance(type);
                parserDictionary[type] = new PBFrameParser();
                parserDictionary[type].Initialize(fullFile[0].Split(',')); //new string[] { "time", "Playback_Info" }.Concat(parserTemplate.GetClassHeader())
            }

            List<PBFrameDataBase> trackerFrames = new List<PBFrameDataBase>();

            for (int i = 1; i < fullFile.Length; i++)
            {
                var frame = (PBFrameDataBase)System.Activator.CreateInstance(type);
                if (frame.ParseRow(parserDictionary[type], fullFile[i].Split(','), i))
                {
                    trackerFrames.Add(frame);
                }

                else
                {
                    Debug.LogWarning($"Unable to parse row {i}");
                }

            }

            library[trackerInfo.TID] = (type, trackerFrames);

            return true;
        }

        /// <summary>
        /// Returns a copy of a tracker's complete frame data list
        /// </summary>
        /// <param name="TID"></param>
        /// <param name="frameData"></param>
        /// <returns></returns>
        public List<PBFrameDataBase> GetFullLibraryEntry(int TID)
        {
            return new List<PBFrameDataBase>(library[TID].Item2);
        }

        public PBFrameDataBase GetCurrentLibraryEntry(int TID)
        {
            return library[TID].Item2[PBFrameControllerManager.Instance.CurrentFrame].Clone() as PBFrameDataBase;
        }

        /// <summary>
        /// Frees up memory we don't need after completing the library
        /// </summary>
        public void CleanUp()
        {
            parserDictionary.Clear();
        }

    }
}

