using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace UPBS.Temporary
{
    public class SimpleAutoMovement : MonoBehaviour
    {
        public enum MovementPattern
        {
            Circle, Line, Triangle, Scale, Camera
        }
        private Vector3 _center;
        public Transform lookTarget;
        public Vector3 mov = new Vector3(0, 0, 5);

        public Vector3 T1 = new Vector3(0, 0, 5);
        public Vector3 T2 = new Vector3(0, 0, 5);
        public Vector3 T3 = new Vector3(0, 0, 5);
        private int vertex = 1;

        public float growthfactor = 2.5f;
        public float rotateSpeed = 5f;
        public float radius = 1f;
        public float speed = 5f;
        public MovementPattern pattern;

        private float _angle;
        private bool movementEnabled = false;

        private void Start()
        {
            _center = transform.position;
            StartCoroutine(Adjust());
        }

        private IEnumerator Adjust()
        {
            movementEnabled = true;
            yield return null;
            movementEnabled = false;
        }

        public void Begin()
        {
            if (pattern == MovementPattern.Triangle) transform.position = T1;
            movementEnabled = true;
        }

        public void Stop()
        {
            movementEnabled = false;
        }

        void Update()
        {
            if (movementEnabled)
            {
                _angle += speed * Time.deltaTime;

                switch (pattern)
                {
                    case MovementPattern.Circle:
                        var offset = new Vector3(Mathf.Sin(_angle), Mathf.Cos(_angle), _center.z) * radius;
                        transform.position = _center + offset;
                        break;
                    case MovementPattern.Line:
                        transform.position = Vector3.Lerp(_center, _center + mov, Mathf.PingPong(Time.time * speed, 1));
                        break;
                    case MovementPattern.Triangle:
                        switch (vertex)
                        {
                            case 1:
                                transform.position = Vector3.Lerp(T1, T2, _angle);
                                if (transform.position == T2)
                                {
                                    vertex = 2;
                                    _angle = 0;
                                }
                                break;
                            case 2:
                                transform.position = Vector3.Lerp(T2, T3, _angle);
                                if (transform.position == T3)
                                {
                                    vertex = 3;
                                    _angle = 0;
                                }
                                break;
                            case 3:
                                transform.position = Vector3.Lerp(T3, T1, _angle);
                                if (transform.position == T1)
                                {
                                    vertex = 1;
                                    _angle = 0;
                                    Stop();
                                }
                                break;
                        }
                        break;
                    case MovementPattern.Scale:
                        transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * growthfactor, Mathf.PingPong(Time.time * speed, 1));
                        break;
                    case MovementPattern.Camera:
                        transform.position = Vector3.Lerp(transform.position, mov, _angle);
                        transform.LookAt(lookTarget);
                        if(_angle >= 1)
                        {
                            Stop();
                        }
                        break;
                }
                
            }
            
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(mov, Vector3.one / 2);
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(T1, 1/3f);
            Gizmos.DrawWireSphere(T2, 1/3f);
            Gizmos.DrawWireSphere(T3, 1/3f);
        }
    }
}

