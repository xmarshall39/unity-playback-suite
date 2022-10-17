using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPBS.Data;
namespace UPBS.Execution
{
    public class PBCameraRepresentation : PBRepresentation
    {
        public override void Refresh()
        {
            base.Refresh();
            Debug.Log("Refreshed");
            // PBFrameDataBase curr = PBFrameLibraryManager.
            // transform.position = 
        }

        protected override void Start()
        {
            base.Start();
        }

    }
}

