using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPBS.Utility;

namespace UPBS.Player
{
    public class PBSkinnedMeshReflection : PBReflection
    {
        private List<GameObject> _skeleton = null;
        private SkinnedMeshRenderer _skinnedMesh = null;

        protected override void Start()
        {
            base.Start();

            //Reconstruct the skeleton
            _skinnedMesh = GetComponentInChildren<SkinnedMeshRenderer>();

            if (_skinnedMesh != null)
            {
                _skeleton = new List<GameObject>() { _skinnedMesh.rootBone.gameObject };
                HelperFunctions.GetAllChildGameobjects(_skinnedMesh.rootBone, _skeleton);
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            if (PBFrameLibraryManager.Instance.TryGetCurrentLibraryEntry<Data.PBSkinnedMeshFrameData>(trackerID.ID, out var frameData, name))
            {
                foreach (var bone in _skeleton)
                {
                    if (frameData.SkeletonMapping.ContainsKey(bone.name))
                    {
                        bone.transform.position = frameData.SkeletonMapping[bone.name].position;
                        bone.transform.eulerAngles = frameData.SkeletonMapping[bone.name].rotation;
                    }
                }
            }
        }
    }
}
