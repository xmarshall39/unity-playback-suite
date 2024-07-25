using UnityEngine;
using UPBS.Player;

namespace UPBS.Examples
{
    public class PBTopDownFollowCam : PBCameraVisualization
    {
        [SerializeField]
        private Transform _target;

        public float followDistance = 100;

        public override void Refresh()
        {
            if (_target)
            {
                transform.position = new Vector3(_target.transform.position.x, _target.transform.position.y + followDistance, _target.transform.position.z);
                transform.LookAt(_target, Vector3.up);
            }
        }
    }
}

