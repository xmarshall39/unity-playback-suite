using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPBS.Data;
namespace UPBS.Player
{
    /// <summary>
    /// Framework for classes that "reflect" what a Tracker records at runtime
    /// </summary>
    [RequireComponent(typeof(PBTrackerID))]
    public abstract class PBReflection : PBFrameControllerUpdateListener
    {
        protected PBTrackerID trackerID;
        protected PBTrackerInfo trackerInfo;
        protected PBTrackerInfo globalTrackerInfo;

        protected virtual void Start()
        {
            if(!TryGetComponent(out trackerID))
            {
                Debug.LogError($"Tracker ID not found on representation {gameObject.name}");
            }
        }

        protected override void OnPlaybackControllerInit()
        {
            base.OnPlaybackControllerInit();
            trackerInfo = PBFrameLibraryManager.Instance.GetTrackerMetadata(trackerID.ID);
            globalTrackerInfo = PBFrameLibraryManager.Instance.GetTrackerMetadata(PBFrameLibraryManager.Instance.GlobalTID);
        }

        public override void Refresh()
        {
            if (PBFrameLibraryManager.Instance.TryGetCurrentLibraryEntry<Data.PBFrameDataBase>(trackerID.ID, out var frameData, name))
            {
                bool enabled = frameData.IsEnabled
                               && trackerInfo != null
                               && (trackerInfo.firstTimestamp <= 0 || PBFrameController.Instance.CurrentFrameKey >= (ulong)trackerInfo.firstTimestamp)
                               && (trackerInfo.lastTimestamp <= 0 || PBFrameController.Instance.CurrentFrameKey <= (ulong)trackerInfo.lastTimestamp)
                               && trackerInfo.lastTimestamp >= globalTrackerInfo.lastTimestamp
                               ;
                this.gameObject.SetActive(enabled);
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}

