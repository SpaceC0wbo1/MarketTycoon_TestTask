using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarketTycoon.UI.HUD
{
    public class GameplayHUDView : MonoBehaviour
    {
        [Header("Currency Display")]
        [SerializeField] private TMP_Text _currencyText;
        [SerializeField] private string _currencyFormat = "Money: {0}";

        [Header("HUD Action Buttons")]
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _pauseButton;

        public event Action OnSettingsClicked;
        public event Action OnPauseClicked;

        private void Awake()
        {
            if (_settingsButton != null)
                _settingsButton.onClick.AddListener(() => OnSettingsClicked?.Invoke());

            if (_pauseButton != null)
                _pauseButton.onClick.AddListener(() => OnPauseClicked?.Invoke());
        }

        public void UpdateCurrency(int amount)
        {
            if (_currencyText != null)
            {
                _currencyText.text = string.Format(_currencyFormat, amount);
            }
        }
    }
}