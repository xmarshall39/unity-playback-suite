using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPBS.Data;
namespace UPBS.Execution
{
    [RequireComponent(typeof(PBTrackerID))]
    public abstract class PBReflection : PBFrameControllerUpdateListener
    {
        protected PBTrackerID trackerID;

        protected virtual void Start()
        {
            if(!TryGetComponent(out trackerID))
            {
                Debug.LogError($"Tracker ID not found on representation {gameObject.name}");
            }
        }
    }
}

