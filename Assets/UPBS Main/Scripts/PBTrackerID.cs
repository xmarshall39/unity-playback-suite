using UnityEngine;
namespace UPBS
{
    /// <summary>
    /// A unique ID for all trackers which is persistent across editor and runtime instances (unlike InstanceID's).
    /// Essential for reconstructed environments since it associates data with gameobjects
    /// </summary>
    public class PBTrackerID : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        private int _ID;
        [SerializeField, ReadOnly]
        private int _positionInPrefabHierarchy = -1;

        public int ID
        {
            get
            {
                return _ID;
            }
        }

        public int PositionInPrefabHierarchy
        {
            get
            {
                return _positionInPrefabHierarchy;
            }
        }

        [SerializeField, ReadOnly]
        private bool _isInitialized = false;
        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        /// <summary>
        /// Request a tracker ID from the manager
        /// </summary>
        public void Init(int newID)
        {
            _ID = newID;
            _isInitialized = true;
        }

        /// <summary>
        /// Used for environment reconstruction to connect shit. Trust me. Might make a var for this
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool TryGetFrameDataType(out System.Type type)
        {
            if(TryGetComponent(out Data.UPBSTracker tracker))
            {
                type = tracker.GetType();
                return true;
            }

            else
            {
                type = null;
                return false;
            }
        }

        public void SetIDManual(int id)
        {
            _ID = id;
        }

        public void SetHierarchyPosition(int pos)
        {
            _positionInPrefabHierarchy = pos;
        }

    }
}

