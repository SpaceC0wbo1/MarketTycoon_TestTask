using System;
using MarketTycoon.Core.Currency;
using MarketTycoon.Core.Settings;
using MarketTycoon.UI.HUD;
using MarketTycoon.UI.Settings;
using UnityEngine;

namespace MarketTycoon.UI.HUD
{
    public class GameplayHUDController : IDisposable
    {
        private readonly GameplayHUDView _hudView;
        private readonly ICurrencyService _currencyService;
        private readonly SettingsController _settingsController;
        private bool _isPaused;

        public GameplayHUDController(
            GameplayHUDView hudView,
            ICurrencyService currencyService,
            SettingsController settingsController)
        {
            _hudView = hudView ?? throw new ArgumentNullException(nameof(hudView));
            _currencyService = currencyService ?? throw new ArgumentNullException(nameof(currencyService));
            _settingsController = settingsController;

            // Initial view setup
            _hudView.UpdateCurrency(_currencyService.CurrentAmount);

            // Subscriptions
            _currencyService.OnAmountChanged += _hudView.UpdateCurrency;
            _hudView.OnSettingsClicked += HandleSettingsClicked;
            _hudView.OnPauseClicked += HandlePauseClicked;
        }

        private void HandleSettingsClicked()
        {
            _settingsController?.OpenSettings();
        }

        private void HandlePauseClicked()
        {
            _isPaused = !_isPaused;
            Time.timeScale = _isPaused ? 0f : 1f;
        }

        public void Dispose()
        {
            if (_currencyService != null)
            {
                _currencyService.OnAmountChanged -= _hudView.UpdateCurrency;
            }

            if (_hudView != null)
            {
                _hudView.OnSettingsClicked -= HandleSettingsClicked;
                _hudView.OnPauseClicked -= HandlePauseClicked;
            }
        }
    }
}