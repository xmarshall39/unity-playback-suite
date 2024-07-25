using System.Collections.Generic;
using UXF;
using UnityEngine;
using UPBS.Player;
namespace UPBS.Data
{
    [RequireComponent(typeof(PBTrackerID))]
    public abstract class UPBSTracker : Tracker
    {
        [Header("Optional field for dynamically spawned trackers which provides a mapping to it's reflection addressable")]
        public string reflectionPrefabAddress = string.Empty;
        public string reflectionPrefabAssetPath = string.Empty;
        private int TID = -1;
        public int SpawnedRootInstanceID { get; protected set; } = 0;

        public long FirstFrame { get; protected set; } = 0;
        public long LastFrame { get; protected set; } = 0;

        public bool DynamicallyInstantiated { get; protected set; } = false;

        public abstract PBFrameDataBase FrameDataType { get; }
        public abstract System.Type ReflectionType { get; }
        public PBTrackerID TrackerID { get; private set; }

        public override IEnumerable<string> CustomHeader => FrameDataType.GetClassHeader();
        public override string MeasurementDescriptor => TID.ToString() + "_" + UPBS.Constants.UPBS_TRACKER_DESC + "_";
        public override string DataName
        {
            get
            {
                Debug.AssertFormat(MeasurementDescriptor.Length > 0, "No measurement descriptor has been specified for this Tracker!");
                return string.Join("_", new string[]{ MeasurementDescriptor, objectName});
            }
            
        }

        protected void OnValidate()
        {
            RefreshTID();
        }

        protected virtual void Start()
        {
            if (Application.isPlaying)
            {
                TrackerID = GetComponent<PBTrackerID>();
                if (TrackerID == null)
                {
                    Debug.LogError($"Tracker attached to {gameObject.name} cannot be found!");
                }

                //Check for singleton validity
                if (PBGlobalTimestamp.Instance == null)
                {
                    Debug.LogError("PBGlobalTimestamp Instance not found!");
                }
                else
                {
                    FirstFrame = PBGlobalTimestamp.Instance.Timestamp;
                }
            } 
        }

        /// <summary>
        /// Save tracker data manually
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (PBGlobalTimestamp.Instance != null)
            {
                LastFrame = PBGlobalTimestamp.Instance.Timestamp;
            }
            if (Session.instance && Recording && Session.instance.InTrial)
            {
                this.StopRecording();
                float recordingRate = 0;
                switch (this.updateType)
                {
                    case TrackerUpdateType.FixedUpdate:
                        recordingRate = Time.fixedDeltaTime;
                        break;
                    case TrackerUpdateType.LateUpdate:
                        recordingRate = 1f / (float)PBGlobalFrameRate.Instance?.TargetFrameRate();
                        break;
                }

                UPBS.Data.PBTrackerInfo trackerInfo = new UPBS.Data.PBTrackerInfo()
                {
                    frameDataAssemblyName = this.FrameDataType.GetType().AssemblyQualifiedName,
                    originalSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
                    TID = this.TrackerID.ID,
                    prefabInstanceID = SpawnedRootInstanceID,
                    prefabHierarchyKey = this.TrackerID.PositionInPrefabHierarchy,
                    recordingRate = recordingRate,
                    firstTimestamp = this.FirstFrame,
                    lastTimestamp = this.LastFrame
                };

                if (DynamicallyInstantiated)
                {
                    trackerInfo.dynamicallyCreated = DynamicallyInstantiated;
                    trackerInfo.assetAddress = reflectionPrefabAddress;
                    trackerInfo.replicationPrefabAssetPath = reflectionPrefabAssetPath;
                }
                Session.instance.CurrentTrial.SaveJSONSerializableObject(trackerInfo, this.DataName, this.UXFDType);
            }
        }

        protected override UXFDataRow GetCurrentValues()
        {
            return new UXFDataRow() 
            {
                ("Timestamp", PBGlobalTimestamp.Instance.Timestamp),
                ("IsEnabled", gameObject.activeSelf)
            };
        }

        public void RefreshTID()
        {
            if (TrackerID == null)
            {
                if (TryGetComponent(out PBTrackerID temp))
                {
                    TrackerID = temp;
                    TID = temp.ID;
                }
            }

            else
            {
                TID = TrackerID.ID;
            }
        }

        /// <summary>
        /// Call immidiately after adding a Tracker component to the scene at runtime. 
        /// </summary>
        /// <param name="autoStart">Should the tracker automatically begin recording data?</param>
        /// <param name="rootInstanceID">Instance ID of the the gameobject created by an Instantiate() call</param>
        public void InitializeDynamicallySpawnedTracker(bool autoStart, int rootInstanceID)
        {
            DynamicallyInstantiated = true;

            if (PBTrackerManager.Instance != null)
            {
                TrackerID = GetComponent<PBTrackerID>();
                PBTrackerManager.Instance.RequestNewTID(this, TrackerID);
                TID = TrackerID.ID;
                gameObject.name += TrackerID.ID.ToString();
                objectName = gameObject.name.Replace(" ", "_").ToLower();
                SpawnedRootInstanceID = rootInstanceID;
            }

            if (Session.instance != null && autoStart)
            {
                if (!Session.instance.trackedObjects.Contains(this))
                {
                    Session.instance.trackedObjects.Add(this);
                }

                if (!Recording && Session.instance.currentTrialNum > 0)
                {
                    StartRecording();
                }
            }
        }
    }
}

