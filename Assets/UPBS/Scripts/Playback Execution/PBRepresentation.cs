using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPBS.Data;
namespace UPBS.Execution
{
    [RequireComponent(typeof(PBTrackerID))]
    public abstract class PBRepresentation : PBFrameControllerUpdateListener
    {
        protected PBTrackerID trackerID;

        protected virtual void Start()
        {
            if(!TryGetComponent(out trackerID))
            {
                Debug.LogError($"Tracker ID not found on representation {gameObject.name}");
            }
        }

        public override void Refresh()
        {
            var frameData = PBFrameLibraryManager.Instance.GetCurrentLibraryEntry(trackerID.ID) as PBCameraFrameData;
            transform.position = frameData.WorldPosition;
            transform.eulerAngles = frameData.EulerRotation;
        }
    }
}

