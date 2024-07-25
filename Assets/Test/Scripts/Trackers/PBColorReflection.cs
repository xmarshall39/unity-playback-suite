using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPBS.Player
{
    public class PBColorReflection : UPBS.Player.PBReflection
    {
        private MeshRenderer meshRend;
        private MaterialPropertyBlock propBlock;

        protected override void Start()
        {
            base.Start();
            if (!TryGetComponent(out meshRend))
            {
                Debug.LogWarning($"No Mesh Renderer Found on {gameObject.name}!");
            }
            propBlock = new MaterialPropertyBlock();
        }

        public override void Refresh()
        {
            base.Refresh();
            if(PBFrameLibraryManager.Instance.TryGetCurrentLibraryEntry<Data.PBColorFrameData>(trackerID.ID, out var frameData, name))
            {
                transform.position = frameData.WorldPosition;
                transform.eulerAngles = frameData.EulerRotation;
                propBlock.SetColor("_TintColor", frameData.MaterialColor);
                if (meshRend)
                {
                    meshRend.SetPropertyBlock(propBlock);
                }
            }
        }
    }
}
