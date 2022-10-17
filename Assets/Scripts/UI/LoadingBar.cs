using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UPBS.UI
{
    public static class RectTransformExtensions
    {
        public static void SetLeft(this RectTransform rt, float left)
        {
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);
        }

        public static void SetRight(this RectTransform rt, float right)
        {
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
        }

        public static void SetTop(this RectTransform rt, float top)
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }

        public static void SetBottom(this RectTransform rt, float bottom)
        {
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
        }
    }

    public class LoadingBar : MonoBehaviour
    {
        
        #region Singleton
        private static LoadingBar _instance;
        public static LoadingBar Instance
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
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        [SerializeField]
        private Canvas loadingBarCanvas;
        [SerializeField]
        private Image _barBackround, _barFillArea;
        [SerializeField]
        private TextMeshProUGUI _progressText;
        [SerializeField]
        private float _fillSpeed = 50f;

        private int _currentProgress;
        private int _maxProgress;

        public bool LoadingBarActive { get; private set; }

        private IEnumerator FillLoadingBar()
        {
            while (LoadingBarActive)
            {
                float fillRatio = (_currentProgress + (int)(_maxProgress * 0.05f)) / (float)_maxProgress;
                float targetFillX = (1 - fillRatio) * _barBackround.rectTransform.sizeDelta.x;
                float smoothFillX = Mathf.SmoothStep(-_barFillArea.rectTransform.offsetMax.x, targetFillX, _fillSpeed * Time.deltaTime);
                _barFillArea.rectTransform.SetRight(smoothFillX);
                yield return null;
            }
        }

        public void BeginLoading(int maxElements)
        {
            
            _currentProgress = 0;
            _maxProgress = maxElements;
            LoadingBarActive = true;

            float fillRatio = (_currentProgress + (int)(_maxProgress * 0.05f)) / (float)_maxProgress;
            float targetFillX = (1 - fillRatio) * _barBackround.rectTransform.sizeDelta.x;
            _barFillArea.rectTransform.SetRight(targetFillX);
            loadingBarCanvas.gameObject.SetActive(true);
            StartCoroutine(nameof(FillLoadingBar));
        }

        public void UpdateProgress(int incriment = 1)
        {
            _currentProgress += incriment;
            _progressText.text = $"{_currentProgress}/{_maxProgress}";
        }

        public void FinishLoading()
        {
            loadingBarCanvas.gameObject.SetActive(false);
            LoadingBarActive = false;
        }

        [EasyButtons.Button]
        public void Test()
        {
            BeginLoading(155);
            StartCoroutine(nameof(TestRoutine));
        }

        private IEnumerator TestRoutine()
        {
            while (LoadingBarActive)
            {
                int inc = Random.Range(1, 5);
                UpdateProgress(inc);
                if (_currentProgress >= _maxProgress) FinishLoading();
                yield return new WaitForSeconds(Random.Range(.5f, 3) / 20);
            }
        }
    }
}

