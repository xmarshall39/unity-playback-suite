using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPBS.Utility;
using System.Linq;

namespace UPBS.Data
{
    public class PBSkinnedMeshFrameData : PBFrameDataBase
    {
        public class SkeletalData
        {
            public Vector3 position;
            public Vector3 rotation;
        }

        public Dictionary<string, SkeletalData> SkeletonMapping { get; protected set; } = null;

        private List<GameObject> _skeleton = null;
        private SkinnedMeshRenderer _skinnedMesh = null;

        public PBSkinnedMeshFrameData()
        {

        }

        public PBSkinnedMeshFrameData(SkinnedMeshRenderer skinnedMesh, List<GameObject> skeleton) : base()
        {
            _skeleton = skeleton;
            _skinnedMesh = skinnedMesh;

            if (skinnedMesh == null || skeleton == null) return;

            SkeletonMapping = new Dictionary<string, SkeletalData>();

            //Initialize dictionary with zeroed out values
            foreach (var bone in skeleton)
            {
                SkeletonMapping.Add(bone.name, new SkeletalData());
            }
        }

        public PBSkinnedMeshFrameData(PBSkinnedMeshFrameData other) : base(other)
        {
            //Copy the other skeletonMapping

        }


        public override string[] GetClassHeader()
        {
            if (_skeleton == null || SkeletonMapping == null)
            {
                return base.GetClassHeader();
            }

            string[] boneHeaders = new string[SkeletonMapping.Count * 6];
            int i = 0;
            foreach(var bone in SkeletonMapping)
            {
                foreach( string bonePosAxis in bone.Value.position.Header($"{bone.Key}_pos"))
                {
                    boneHeaders[i] = bonePosAxis;
                    ++i;
                }
                foreach (string boneRotAxis in bone.Value.rotation.Header($"{bone.Key}_rot"))
                {
                    boneHeaders[i] = boneRotAxis;
                    ++i;
                }
            }

            return base.GetClassHeader().Concat(boneHeaders);
        }

        protected override bool ParseRowInternal(PBFrameParser parser, string[] row, int rowNumber)
        {
            bool allClear = base.ParseRowInternal(parser, row, rowNumber);

            if (SkeletonMapping == null)
            {
                UnityEngine.Profiling.Profiler.BeginSample("UPBS_SkinnedMeshFrameData_ParseRowInternal() - Skeleton Creation");
                SkeletonMapping = new Dictionary<string, SkeletalData>();
                //Start at 3 and get right to the bone columns
                string[] columnNames = parser.Columns.Keys.OrderBy(x => parser.Columns[x]).ToArray();
                for (int i = 3; i < columnNames.Length; ++i)
                {
                    SkeletonMapping.Add(columnNames[i], new SkeletalData());
                }
                // When loading in the playback data, we don't have access to skeletal gameobjects yet,
                // so instead we decipher and cache the bone names and heirarchy order once per tracker file.
                UnityEngine.Profiling.Profiler.EndSample();
            }

            UnityEngine.Profiling.Profiler.BeginSample("UPBS_SkinnedMeshFrameData_ParseRowInternal() - Column Parsing");
            foreach (var boneData in SkeletonMapping)
            {
                if (parser.GetColumnValuesAsFloats($"{boneData.Key}_pos", row, rowNumber, out float[] vals, (new Vector3()).HeaderAppends()))
                {
                    boneData.Value.position = new Vector3(vals[0], vals[1], vals[2]);
                }
                else
                {
                    allClear = false;
                    Debug.LogWarning($"Bone [{boneData.Key}]: WorldPosition value in row {rowNumber} could not be parsed!");
                    break;                
                }

                if (parser.GetColumnValuesAsFloats($"{boneData.Key}_rot", row, rowNumber, out float[] rotVals, (new Vector3()).HeaderAppends()))
                {
                    boneData.Value.rotation = new Vector3(rotVals[0], rotVals[1], rotVals[2]);
                }
                else
                {
                    allClear = false;
                    Debug.LogWarning($"Bone [{boneData.Key}]: WorldPosition value in row {rowNumber} could not be parsed!");
                    break;
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();

            return allClear;

        }
    }
}
