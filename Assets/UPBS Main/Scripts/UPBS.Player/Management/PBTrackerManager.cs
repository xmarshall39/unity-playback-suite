using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace UPBS.Data
{
    /// <summary>
    /// Monitors all active trackers and maintains info on them
    /// </summary>
    public class PBTrackerManager : MonoBehaviour
    {
        #region Singleton
        private static PBTrackerManager _instance;
        public static PBTrackerManager Instance
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
        public int editorGenerationSeed = 1234;
        public int runtimeGenerationSeed = 4321;
        [SerializeField, ReadOnly]
        private List<PBTrackerID> _TIDReferences;
        [SerializeField, ReadOnly]
        private List<int> _claimedTags;

        private HashSet<int> _tagHistory = new HashSet<int>();

#if UNITY_EDITOR
        [EasyButtons.Button]
        private void RegenerateIDTable()
        {
            List<UnityEngine.Object> editedObjects = new List<Object>() { this };
            UPBSTracker[] allTrackers = FindObjectsOfType<UPBS.Data.UPBSTracker>();

            System.Random random = new System.Random(editorGenerationSeed);
            _TIDReferences = FindObjectsOfType<PBTrackerID>().ToList();
            _claimedTags = Enumerable.Repeat(0, _TIDReferences.Count).ToList();

            editedObjects.AddRange(_TIDReferences);
            editedObjects.AddRange(allTrackers);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.Undo.RecordObjects(editedObjects.ToArray(), "RegenerateIDTable");
            }
#endif

            if (_claimedTags.Count >= 999)
            {
                Debug.LogWarning("Exceeded tag limit of 999. Tag request has been denied!");
                return;
            }


            for(int i = 0; i < _claimedTags.Count; ++i)
            {
                int nextTag;
                do
                {
                    nextTag = random.Next(1, 9999);
                }
                while (_claimedTags.Contains(nextTag));

                _claimedTags[i] = nextTag;
                _TIDReferences[i].Init(nextTag);
                if (UnityEditor.PrefabUtility.IsPartOfPrefabInstance(_TIDReferences[i]))
                {
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(_TIDReferences[i]);
                }
            }


            foreach (var tracker in allTrackers)
            {
                tracker.RefreshTID();

                if (UnityEditor.PrefabUtility.IsPartOfPrefabInstance(tracker))
                {
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(tracker);
                }
            }
        }
#endif

        public void RequestNewTID(UPBSTracker sourceTracker, PBTrackerID idObj)
        {
            if (_TIDReferences.Contains(idObj))
            {
                return;
            }

            System.Random random = new System.Random(runtimeGenerationSeed);

            int nextTag;
            do
            {
                nextTag = random.Next(1, 9999);
            }
            while (_claimedTags.Contains(nextTag));

            idObj.Init(nextTag);
            _TIDReferences.Add(idObj);
            _claimedTags.Add(nextTag);
        }

        [EasyButtons.Button]
        private void RefreshTrackers()
        {
            foreach (var tracker in FindObjectsOfType<UPBSTracker>())
            {
                tracker.RefreshTID();
            }
        }
    }
}

