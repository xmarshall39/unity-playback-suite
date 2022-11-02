using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPBS.Data;
namespace UPBS.Execution
{
    public class PBCameraReflection : PBReflection
    {
        public override void Refresh()
        {
            var frameData = PBFrameLibraryManager.Instance.GetCurrentLibraryEntry(trackerID.ID) as PBCameraFrameData;
            transform.position = frameData.WorldPosition;
            transform.eulerAngles = frameData.EulerRotation;
        }

        protected override void Start()
        {
            base.Start();
        }

    }
}

