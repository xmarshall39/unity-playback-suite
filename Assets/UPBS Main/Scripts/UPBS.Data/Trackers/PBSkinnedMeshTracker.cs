using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;
using UPBS.Utility;

namespace UPBS.Data
{
    public class PBSkinnedMeshTracker : UPBSTracker
    {
        [SerializeField] private SkinnedMeshRenderer skinnedMesh = null;
        [SerializeField] private GameObject rootObj = null;

        private List<GameObject> skeleton = null;

        public override PBFrameDataBase FrameDataType
        {
            get
            {
                if (!ValidateSkeleton() && skinnedMesh != null && rootObj != null)
                {
                    UpdateSkeleton();
                }
                return new PBSkinnedMeshFrameData(skinnedMesh, skeleton);
            }
        }

        public override Type ReflectionType => typeof(UPBS.Player.PBSkinnedMeshReflection);


        protected override void Start()
        {
            base.Start();
            if (Session.instance && !Session.instance.trackedObjects.Contains(this))
            {
                Session.instance.trackedObjects.Add(this);
            }

            UpdateSkeleton();
        }

        protected override UXFDataRow GetCurrentValues()
        {
            var row = base.GetCurrentValues();
            List<(string, object)> additionalRows = new List<(string, object)>(skeleton.Count * 6);
            for (int i = 0; i < skeleton.Count; ++i)
            {
                additionalRows.Add(($"{skeleton[i].name}_pos_x", skeleton[i].transform.position.x));
                additionalRows.Add(($"{skeleton[i].name}_pos_y", skeleton[i].transform.position.y));
                additionalRows.Add(($"{skeleton[i].name}_pos_z", skeleton[i].transform.position.z));

                additionalRows.Add(($"{skeleton[i].name}_rot_x", skeleton[i].transform.eulerAngles.x));
                additionalRows.Add(($"{skeleton[i].name}_rot_y", skeleton[i].transform.eulerAngles.y));
                additionalRows.Add(($"{skeleton[i].name}_rot_z", skeleton[i].transform.eulerAngles.z));
            }
            row.AddRange(additionalRows);

            return row;
        }

        private void UpdateSkeleton()
        {
            skeleton = new List<GameObject>() { rootObj };
            HelperFunctions.GetAllChildGameobjects(rootObj.transform, skeleton);
        }

        private bool ValidateSkeleton()
        {
            bool valid = skeleton != null;

            if (valid)
            {
                foreach(var bone in skeleton)
                {
                    if (bone == null) return false;
                }
            }

            return valid;
        }
    }
}
