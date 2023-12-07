using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPBS.Data;
namespace UPBS.Execution
{
    /// <summary>
    /// Framework for classes that "reflect" what a Tracker records at runtime
    /// </summary>
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

