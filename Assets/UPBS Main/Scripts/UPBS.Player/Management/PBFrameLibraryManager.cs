using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UPBS.Data;
using UPBS.Utility;
using FrameDataCollection = System.Collections.Generic.SortedDictionary<ulong, UPBS.Data.PBFrameDataBase>;
using ExternalFrameDataCollection = System.Collections.Generic.Dictionary<string, System.Collections.Generic.SortedDictionary<ulong, UPBS.Data.PBFrameDataBase>>;


namespace UPBS.Player
{
    /// <summary>
    /// Manager class for all FrameData in a playback environment
    /// </summary>
    public class PBFrameLibraryManager : MonoBehaviour
    {
        public class LibraryEntry
        {
            public System.Type type;
            public FrameDataCollection frameCollection;
            public PBTrackerInfo metadata;

            public LibraryEntry(System.Type type, FrameDataCollection frames, PBTrackerInfo info)
            {
                this.type = type;
                this.frameCollection = frames;
                this.metadata = info;
            }
        }

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
        public int GlobalTID { get; set; }
        public void SetGlobalTID(int tid) => GlobalTID = tid;

        private Dictionary< int, LibraryEntry> _library;
        private Dictionary<string, LibraryEntry> _externalLibrary;

        private Dictionary<System.Type, PBFrameParser> parserDictionary;
        private ConcurrentDictionary<System.Type, PBFrameParser> _parserDictionary;

        private void Start()
        {
            _library = new Dictionary<int, LibraryEntry>();
            _externalLibrary = new Dictionary<string, LibraryEntry>();
            parserDictionary = new Dictionary<System.Type, PBFrameParser>();
        }

        /// <summary>
        /// Adds a tracker to the global frame library
        /// </summary>
        /// <param name="trackerInfo"></param>
        /// <param name="fullFile"></param>
        /// <returns>True on success</returns>
        public bool AddLibraryEntry(PBTrackerInfo trackerInfo, string[] fullFile)
        {
            System.Type type = System.Type.GetType(trackerInfo.frameDataAssemblyName);
            if (!parserDictionary.ContainsKey(type))
            {
                //PBFrameDataBase parserTemplate = (PBFrameDataBase)System.Activator.CreateInstance(type);
                parserDictionary[type] = new PBFrameParser();
                parserDictionary[type].Initialize(fullFile[0].Split(',')); //new string[] { "time", "Playback_Info" }.Concat(parserTemplate.GetClassHeader())
            }

            FrameDataCollection trackerFrames = new FrameDataCollection();

            for (int i = 1; i < fullFile.Length; i++)
            {
                var frame = (PBFrameDataBase)System.Activator.CreateInstance(type);
                if (frame.ParseRow(parserDictionary[type], fullFile[i].Split(','), i))
                {
                    if (!trackerFrames.ContainsKey(frame.Timestamp))
                    {
                        trackerFrames.Add(frame.Timestamp, frame);
                    }
                }

                else
                {
                    Debug.LogWarning($"Unable to parse row {i}");
                }

            }

            PBFrameDataBase.OnEndFileParse();
            _library[trackerInfo.TID] = new LibraryEntry(type, trackerFrames, trackerInfo);

            return true;
        }

        /// <summary>
        /// Returns a copy of a tracker's complete frame data list
        /// </summary>
        /// <param name="TID">Tracker ID</param>
        /// <param name="frameData"></param>
        /// <returns></returns>
        public FrameDataCollection GetFullLibraryEntry(int TID)
        {
            return new FrameDataCollection(_library[TID].frameCollection);
        }

        public PBFrameDataBase GetCurrentLibraryEntry(int TID, string goName = "")
        {
            if (_library.ContainsKey(TID))
            {
                return _library[TID].frameCollection[PBFrameController.Instance.CurrentFrameKey].Clone() as PBFrameDataBase;
            }

            else
            {
                Debug.LogError($"TID ({TID}) on {goName} not found in the frame library! Make sure your TrackerID's in the original and playback scenes match!");
                return null;
            }
        }

        public List<PBFrameDataBase> GetFrameRange(int TID, int range, string goName = "")
        {
            List<PBFrameDataBase> frames = new List<PBFrameDataBase>(range);
            int count = Mathf.Abs(range);
            int sign = (int)Mathf.Sign(range);
            for (int i = 0; i < count; ++i)
            {
                ulong frameKey = PBFrameController.Instance.GetFrameKey(PBFrameController.Instance.CurrentFrameIndex + (i * sign));
                if (frameKey > 0)
                {
                    frames.Add(_library[TID].frameCollection[frameKey]);
                }
            }

            return frames;
        }

        /// <summary>
        /// Recieves the most recent frame recorded for a given tracker.
        /// </summary>
        /// <param name="TID">Tracker ID</param>
        /// <param name="frameData">Tracker frame data if found. NULL if not found.</param>
        /// <param name="goName">Debug parameter to help identify problematic tracker data</param>
        /// <returns>True if a valid frameData was found</returns>
        public bool TryGetCurrentLibraryEntry<T>(int TID, out T frameData, string goName = "") where T : PBFrameDataBase
        {
            frameData = null;

            if (_library.ContainsKey(TID))
            {
                FrameDataCollection trackedFrames = _library[TID].frameCollection;
                ulong currentTimestamp = PBFrameController.Instance.CurrentFrameKey;
                if (trackedFrames.ContainsKey(currentTimestamp))
                {
                    var temp = trackedFrames[PBFrameController.Instance.CurrentFrameKey];
                    var tempClone = temp.Clone();
                    frameData =  tempClone as T;
                    return frameData != null;
                }

                // Try to find the latest timestamp that's before the current.
                // Return false if no data was recorded prior to the current point in playback.
                else
                {
                    ulong latestTimestamp = ulong.MaxValue;
                    foreach (var kvp in trackedFrames)
                    {
                        if (kvp.Key > currentTimestamp)
                        {
                            break;
                        }
                        latestTimestamp = kvp.Key;
                    }

                    if (latestTimestamp == ulong.MaxValue)
                    {
                        return false;
                    }

                    else
                    {
                        frameData = trackedFrames[latestTimestamp] as T;
                        return frameData != null;
                    }
                }
            }

            else
            {
                return false;
            }
        }

        public PBTrackerInfo GetTrackerMetadata(int TID, string goName = "")
        {
            if (_library.ContainsKey(TID))
            {
                return _library[TID].metadata.Clone() as PBTrackerInfo;
            }
            else
            {
                Debug.LogError($"TID ({TID}) on {goName} not found in the frame library! Make sure your TrackerID's in the original and playback scenes match!");
                return null;
            }
        }

        public FrameDataCollection GetGlobalFrameData()
        {
            return new FrameDataCollection(_library[GlobalTID].frameCollection);
        }

        /// <summary>
        /// Frees up memory we don't need after filling the library
        /// </summary>
        public void CleanUp()
        {
            parserDictionary.Clear();
        }

    }
}

