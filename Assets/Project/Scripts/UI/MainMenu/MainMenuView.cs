using System;
using UnityEngine;
using UnityEngine.UI;

namespace MarketTycoon.UI.MainMenu
{
    public class MainMenuView : MonoBehaviour
    {
        [Header("Menu Buttons")]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitButton;

        public event Action OnPlayClicked;
        public event Action OnSettingsClicked;
        public event Action OnExitClicked;

        private void Awake()
        {
            if (_playButton != null)
                _playButton.onClick.AddListener(() => OnPlayClicked?.Invoke());

            if (_settingsButton != null)
                _settingsButton.onClick.AddListener(() => OnSettingsClicked?.Invoke());

            if (_exitButton != null)
                _exitButton.onClick.AddListener(() => OnExitClicked?.Invoke());
        }
    }
}