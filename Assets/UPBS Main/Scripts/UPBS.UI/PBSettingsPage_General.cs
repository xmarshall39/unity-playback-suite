using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPBS.Utility;

namespace UPBS.UI
{
    public class PBSettingsPage_General : PBSettingsPage
    {
        public override SettingsTab TabType { get => SettingsTab.General; }

        private void Start()
        {
            Construct();
        }

        public override void Construct()
        {
            //Show some immutable general settings
            ConstructDynamicChildren();
        }

        public override void ResetToDefault()
        {
            throw new System.NotImplementedException();
        }
    }
}

