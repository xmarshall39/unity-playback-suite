using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPBS.UI
{
    public class PBTabGroup : MonoBehaviour
    {
        [SerializeField]
        protected List<PBTabElement> _tabs;

        public int ActiveTabIndex { get; private set; } = 0;

        public PBTabElement GetActiveTab()
        {
            if(_tabs.Count > ActiveTabIndex)
            {
                return _tabs[ActiveTabIndex];
            }

            return null;
        }

        public bool TrySetActiveTab(PBTabElement tab)
        {
            PBTabElement prevTab = GetActiveTab();

            if (prevTab)
            {
                prevTab.Deselect();
            }

            int result = _tabs.FindIndex(x => x == tab);
            if(result >= 0)
            {
                ActiveTabIndex = result;
                tab.Select();
                return true;
            }

            return false;
        }

        public void SetActiveTab(PBTabElement tab) => TrySetActiveTab(tab);

        public virtual void OnEnable()
        {
            if(_tabs.Count > 0)
            {
                TrySetActiveTab(_tabs[0]);
            }
        }
    }
}
