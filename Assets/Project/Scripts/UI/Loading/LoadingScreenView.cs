using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarketTycoon.UI.Loading
{
    public class LoadingScreenView : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Slider _progressBar;
        [SerializeField] private TMP_Text _progressText;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Fade Settings")]
        [SerializeField] private float _fadeDuration = 0.3f;

        private void Awake()
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }
        }

        public void UpdateProgress(float progress)
        {
            if (_progressBar != null)
            {
                _progressBar.value = progress;
            }

            if (_progressText != null)
            {
                _progressText.text = $"{Mathf.RoundToInt(progress * 100f)}%";
            }
        }

        public IEnumerator FadeInRoutine()
        {
            gameObject.SetActive(true);
            if (_canvasGroup == null) yield break;

            float elapsed = 0f;
            while (elapsed < _fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                _canvasGroup.alpha = Mathf.Clamp01(elapsed / _fadeDuration);
                yield return null;
            }
            _canvasGroup.alpha = 1f;
        }

        public IEnumerator FadeOutRoutine()
        {
            if (_canvasGroup != null)
            {
                float elapsed = 0f;
                while (elapsed < _fadeDuration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    _canvasGroup.alpha = Mathf.Clamp01(1f - (elapsed / _fadeDuration));
                    yield return null;
                }
                _canvasGroup.alpha = 0f;
            }

            gameObject.SetActive(false);
        }
    }
}