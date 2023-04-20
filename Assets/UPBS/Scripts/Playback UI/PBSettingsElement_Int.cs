using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UPBS.Utility;

namespace UPBS.UI
{
    public class PBSettingsElement_Int : PBSettingsElement
    {
        [SerializeField]
        private TMP_InputField _intInput;

        public override Selectable DataField { get => _intInput; }

        public override void Construct(SettingsData data)
        {
            base.Construct(data);
            _intInput.text = ((int)data.property.GetValue(null)).ToString();

            _intInput.onValueChanged.AddListener(SetBoundValue);
        }

        public override void Deconstruct()
        {
            _intInput.onValueChanged.RemoveListener(SetBoundValue);
        }

        private void SetBoundValue(string input)
        {
            if(int.TryParse(input, out int num))
            {
                _data.property.SetValue(null, num);
            }
        }
    }
}

