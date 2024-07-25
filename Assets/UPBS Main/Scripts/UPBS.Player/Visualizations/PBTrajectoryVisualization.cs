using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UPBS.Data;
using System.Linq;

namespace UPBS.Player
{
    public class PBTrajectoryVisualization : PBVisualization
    {
        public PBReflection targetReflection;

        public LineRenderer headRenderer;
        public LineRenderer tailRenderer;

        public int headLength = 5;
        public int tailLength = 15;

        private List<Data.PBFrameDataBase> headList = new List<Data.PBFrameDataBase>();
        private List<Data.PBFrameDataBase> tailList = new List<Data.PBFrameDataBase>();
        private PBPositionRotationFrameData center = null;
        private int targetID;

        public override void Refresh()
        {

            if( PBFrameLibraryManager.Instance.TryGetCurrentLibraryEntry<PBPositionRotationFrameData>(targetID, out center))
            {
                headList = PBFrameLibraryManager.Instance.GetFrameRange(targetID, headLength);
                tailList = PBFrameLibraryManager.Instance.GetFrameRange(targetID, -tailLength);

                Draw();
            }
        }

        private void Start()
        {
            targetID = targetReflection.GetComponent<PBTrackerID>().ID;
        }

        public void Draw()
        {
            Vector3[] tailElements = new Vector3[tailList.Count + 1];
            tailElements[tailList.Count] = center.WorldPosition;
            Vector3[] temp = tailList.Select(x => ((PBPositionRotationFrameData)x).WorldPosition).ToArray();
            Array.Copy(temp, tailElements, temp.Length);
            tailRenderer.positionCount = tailElements.Length;
            tailRenderer.SetPositions(tailElements);

            Vector3[] headElements = headList.Select(x => ((PBPositionRotationFrameData)x).WorldPosition).ToArray();
            headRenderer.positionCount = headElements.Length;
            headRenderer.SetPositions(headElements);
        }

    }
}
