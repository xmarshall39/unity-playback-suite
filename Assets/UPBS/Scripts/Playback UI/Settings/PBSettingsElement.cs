using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UPBS.Utility;
using TMPro;
using System.Reflection;

namespace UPBS.UI
{
    /*
     * All Settings contain the following:
     * * A name indicating either the setting being altered or the information represented
     * * A secondary data container. This may be empty, or contain some kind of arbitrary UI element. Importantly, this field is effectively "read-only" and context dependent
     * * An input field. An int text field, a toggle, a button. Interactions on this field will impact the settings when saved
     * * 
     * 
     */
    public abstract class PBSettingsElement : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _name;

        [SerializeField]
        private SettingType _type;

        protected SettingsData _data;

        public abstract Selectable DataField { get; }

        public virtual void Construct(SettingsData data)
        {
            _data = data;
            _name.text = _data.attribute.Name;
        }

        public abstract void Deconstruct();

    }
}
