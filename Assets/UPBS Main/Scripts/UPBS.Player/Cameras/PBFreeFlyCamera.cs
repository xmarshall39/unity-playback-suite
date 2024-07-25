using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPBS.Player
{
    public class PBFreeFlyCamera : MonoBehaviour, IPBCameraBase
    {
        private PBCameraInfo _camInfo;
        public float speed = 10f;
        public float rotationAngle = 0.1f;

        public void DisableCamera()
        {
            _camInfo.cam.enabled = false;
        }

        public void EnableCamera()
        {
            _camInfo.cam.enabled = true;
        }

        public PBRenderCamera GetRenderCamera()
        {
            return _camInfo.renderCamera;
        }

        void Start()
        {
            _camInfo.cam = GetComponent<Camera>();
            _camInfo.name = "Free Fly Cam";
            _camInfo.renderCamera = null;
        }

        void Update()
        {
            if (Input.GetMouseButton(1))
            {
                if (Input.GetAxis("Horizontal") != 0)
                {
                    transform.Rotate(transform.up, Input.GetAxis("Horizontal") * rotationAngle);
                }
                if (Input.GetAxis("Vertical") != 0)
                {
                    transform.Rotate(transform.up, Input.GetAxis("Vertical") * rotationAngle);
                }
                if (Input.GetKey(KeyCode.W))
                {
                    transform.Translate(transform.forward * speed);
                }
                if (Input.GetKey(KeyCode.A))
                {
                    transform.Translate(-transform.right * speed);

                }
                if (Input.GetKey(KeyCode.S))
                {
                    transform.Translate(-transform.forward * speed);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    transform.Translate(transform.right * speed);
                }
                if (Input.GetKey(KeyCode.E))
                {
                    transform.Translate(transform.up * speed);
                }
                if (Input.GetKey(KeyCode.Q))
                {
                    transform.Translate(-transform.up * speed);
                }

            }
        }
    }

}