using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPBS.Utility;

namespace UPBS.UI
{
    public class PBSettingsManager : MonoBehaviour
    {
        #region Singleton
        private static PBSettingsManager _instance;
        public static PBSettingsManager Instance
        {
            get
            {
                return _instance;
            }
        }
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        
        public Dictionary<SettingType, GameObject> prefabDict;

        public List<SettingType> SettingTypes = new List<SettingType>(4);
        public List<GameObject> SettingPrefabs = new List<GameObject>(4);

        private void OnDestroy()
        {
            PBSettingsLibrary.UnInitialize();
        }
    }
}
