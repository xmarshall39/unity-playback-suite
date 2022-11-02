using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPBS.Execution
{
    public class PBGlobalReflection : UPBS.Execution.PBReflection
    {
        public override void Refresh()
        {
            var frameData = PBFrameLibraryManager.Instance.GetCurrentLibraryEntry(trackerID.ID) as Data.PBGlobalFrameData;

        }
    }
}