using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBTrackerSync : MonoBehaviour
{
    public UXF.TrackerUpdateType universalUpdateType = UXF.TrackerUpdateType.FixedUpdate;

    [EasyButtons.Button]
    public void Synchronize()
    {
        foreach( var tracker in GameObject.FindObjectsOfType<UPBS.Data.UPBSTracker>())
        {
            tracker.updateType = universalUpdateType;
        }
    }
}
