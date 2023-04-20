using UnityEngine;
using System.Linq;

//Should be UPBS.Data???
namespace UPBS.Execution
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
            if (_instance)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion
        public int seed = 1234;
        [SerializeField, ReadOnly]
        private PBTrackerID[] _TIDReferences;
        [SerializeField, ReadOnly]
        private int[] _claimedTags;
        
        [EasyButtons.Button]
        private void RegenerateIDTable()
        {
             System.Random random = new System.Random(seed);
            _TIDReferences = FindObjectsOfType<PBTrackerID>();
            _claimedTags = new int[_TIDReferences.Length];
            

            if(_claimedTags.Length >= 999)
            {
                Debug.LogWarning("Exceeded tag limit of 999. Tag request has been denied!");
                return;
            }


            for(int i = 0; i < _claimedTags.Length; ++i)
            {
                int nextTag;
                do
                {
                    nextTag = random.Next(1, 9999);
                }
                while (_claimedTags.Contains(nextTag));

                _claimedTags[i] = nextTag;
                _TIDReferences[i].Init(nextTag);
            }

            foreach(var tracker in FindObjectsOfType<UPBS.Data.UPBSTracker>())
            {
                tracker.RefreshTID();
            }
            
        }

        [EasyButtons.Button]
        private void RefreshTrackers()
        {
            foreach (var tracker in FindObjectsOfType<UPBS.Data.UPBSTracker>())
            {
                tracker.RefreshTID();
            }
        }

        [ContextMenu("Reset ID Values")]
        private void ResetIDValues()
        {
            seed = 1234;
            _TIDReferences = new PBTrackerID[0];
            _claimedTags = new int[0];
        }
    }
}

