using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UPBS.Utility;

namespace UPBS.UI
{
    public abstract class PBSettingsPage : MonoBehaviour
    {
        public abstract SettingsTab TabType { get; }
        /*
         * Common traits:
         * Vertical layout group
         * Text Entry (w varying format)
         * Data entry field
         * Dynamic number of entries
         * Construct() when made visible -> Add any dynamic data
         * ResetToDefault() -> Return all settings to their default values.
*/

        private List<PBSettingsElement> _dynamicChildren;
        protected void ConstructDynamicChildren()
        {
            PBSettingsLibrary.Initialize();
            _dynamicChildren = new List<PBSettingsElement>();

            foreach(SettingsData data in PBSettingsLibrary.GetSettingsOfType(TabType))
            {
                GameObject prefab = PBSettingsManager.Instance.SettingPrefabs[PBSettingsManager.Instance.SettingTypes.IndexOf(data.attribute.Type)];
                GameObject go = Instantiate(prefab, this.transform);
                if(go.TryGetComponent(out PBSettingsElement settingElement))
                {
                    settingElement.Construct(data);
                    _dynamicChildren.Add(settingElement);

                }
            }
        }

        public abstract void Construct();
        public abstract void ResetToDefault();
    }
}
