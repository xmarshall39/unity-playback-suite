using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UPBS.Utility;

namespace UPBS.UI
{
    public class PBSettingsElement_Enum : PBSettingsElement
    {
        [SerializeField]
        private TMP_Dropdown _dropdown;
        private Type enumType;
        private Dictionary<string, int> _enumMapping;
        public override Selectable DataField { get => _dropdown; }

        public override void Construct(SettingsData data)
        {
            base.Construct(data);

            enumType = data.attribute.DefaultValue.GetType();
            _enumMapping = new Dictionary<string, int>();
            //Populate dropdown with the corresponding enum type
            foreach ( string name in System.Enum.GetNames(enumType))
            {
                _enumMapping.Add(name, (int)Enum.Parse(enumType, name));
            }

            string currentEnumName = Enum.GetName(enumType, _data.property.GetValue(null));
            int currentValue = _enumMapping[currentEnumName];

            _dropdown.ClearOptions();
            _dropdown.AddOptions(new List<string>(_enumMapping.Keys));

            _dropdown.SetValueWithoutNotify(currentValue);
            _dropdown.onValueChanged.AddListener(SetBoundValue);
        }

        public override void Deconstruct()
        {
            _dropdown.ClearOptions();
            _dropdown.onValueChanged.RemoveListener(SetBoundValue);
            _enumMapping.Clear();
        }

        private void SetBoundValue(int value)
        {
            _data.property.SetValue(null, _enumMapping[_dropdown.options[value].text]);
        }

    }
}

