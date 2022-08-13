using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UPBS.Temporary
{
    public class SimpleAutoMovement : MonoBehaviour
    {
        private Vector3 _center;
        public float rotateSpeed = 5f;
        public float radius = 1f;

        private float _angle;
        private void Start()
        {
            _center = transform.position;
        }
        void Update()
        {
            _angle += rotateSpeed * Time.deltaTime;

            var offset = new Vector3(Mathf.Sin(_angle), Mathf.Cos(_angle), _center.z) * radius;
            transform.position = _center + offset;
        }
    }
}

