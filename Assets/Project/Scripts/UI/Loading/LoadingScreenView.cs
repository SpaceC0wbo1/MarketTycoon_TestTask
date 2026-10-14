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

        [Header("Visual Smoothing")]
        [Tooltip("Speed at which progress bar fills per second")]
        [SerializeField] private float _fillSpeed = 0.8f;
        [SerializeField] private float _fadeDuration = 0.25f;

        private float _targetProgress;
        private float _currentDisplayedProgress;

        private void Awake()
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }
        }

        /// <summary>
        /// Resets bar values immediately before the view is enabled to prevent flashing 100%.
        /// </summary>
        public void ResetView()
        {
            _targetProgress = 0f;
            _currentDisplayedProgress = 0f;
            ApplyDisplayValue(0f);

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
            }
        }

        public void SetTargetProgress(float target)
        {
            _targetProgress = Mathf.Clamp01(target);
        }

        private void Update()
        {
            if (_currentDisplayedProgress < _targetProgress)
            {
                // Smoothly slide towards target progress
                _currentDisplayedProgress = Mathf.MoveTowards(
                    _currentDisplayedProgress,
                    _targetProgress,
                    _fillSpeed * Time.unscaledDeltaTime
                );

                ApplyDisplayValue(_currentDisplayedProgress);
            }
        }

        public bool IsFillComplete()
        {
            return Mathf.Approximately(_currentDisplayedProgress, _targetProgress) && _currentDisplayedProgress >= 1.0f;
        }

        private void ApplyDisplayValue(float value)
        {
            if (_progressBar != null)
            {
                _progressBar.value = value;
            }

            if (_progressText != null)
            {
                _progressText.text = $"{Mathf.RoundToInt(value * 100f)}%";
            }
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