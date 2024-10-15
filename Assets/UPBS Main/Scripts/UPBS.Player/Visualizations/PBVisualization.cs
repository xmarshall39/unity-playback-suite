using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPBS.Player
{
    public abstract class  PBVisualization : PBFrameControllerUpdateListener
    {
        [SerializeField]
        private List<int> trackerIDs;
    }
}

