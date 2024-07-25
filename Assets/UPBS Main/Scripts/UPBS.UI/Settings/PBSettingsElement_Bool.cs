using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UPBS.Utility;

namespace UPBS.UI
{
    public class PBSettingsElement_Bool : PBSettingsElement
    {
        [SerializeField]
        private Toggle _toggle;

        public override Selectable DataField => _toggle;

        public override void Construct(SettingsData data)
        {
            base.Construct(data);
            _toggle.onValueChanged.AddListener(SetBoundValue);
            _toggle.SetIsOnWithoutNotify((bool)_data.property.GetValue(null));
        }

        public override void Deconstruct()
        {
            _toggle.onValueChanged.RemoveListener(SetBoundValue);
        }

        private void SetBoundValue(bool val)
        {
            _data.property.SetValue(null, val);
        }
    }
}

