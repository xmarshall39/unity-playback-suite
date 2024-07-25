using System;
using UnityEngine;

namespace UPBS.Data
{
    public class PBGlobalTimestamp : MonoBehaviour
    {
        #region Singleton
        private static PBGlobalTimestamp _instance;
        public static PBGlobalTimestamp Instance
        {
            get => _instance;
        }

        private void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        #endregion

        public long Timestamp { get; private set; }

        private void Start()
        {
            Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        private void Update()
        {
            Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
    }

}