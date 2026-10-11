using System;
using UnityEngine;
using UnityEngine.UI;

namespace MarketTycoon.UI.Pause
{
    public class PausePopupView : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _mainMenuButton;

        public event Action OnResumeClicked;
        public event Action OnSettingsClicked;
        public event Action OnMainMenuClicked;

        private void Awake()
        {
            if (_resumeButton != null)
                _resumeButton.onClick.AddListener(() => OnResumeClicked?.Invoke());

            if (_settingsButton != null)
                _settingsButton.onClick.AddListener(() => OnSettingsClicked?.Invoke());

            if (_mainMenuButton != null)
                _mainMenuButton.onClick.AddListener(() => OnMainMenuClicked?.Invoke());
        }

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }
    }
}