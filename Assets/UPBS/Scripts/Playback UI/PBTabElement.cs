using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UPBS.UI
{
    public class PBTabElement : MonoBehaviour
    {
        [SerializeField]
        private Color _defualtColor, _selectedColor;
        [SerializeField]
        private Image _background;
        [SerializeField]
        private GameObject _UIPage;

        public void Select()
        {
            _background.color = _selectedColor;
            _UIPage.SetActive(true);
        }

        public void Deselect()
        {
            _background.color = _defualtColor;
            _UIPage.SetActive(false);
        }
    }
}
