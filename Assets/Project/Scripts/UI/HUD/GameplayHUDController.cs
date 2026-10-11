using System;
using MarketTycoon.Core.Currency;
using MarketTycoon.UI.HUD;
using MarketTycoon.UI.Pause;

namespace MarketTycoon.UI.HUD
{
    public class GameplayHUDController : IDisposable
    {
        private readonly GameplayHUDView _hudView;
        private readonly ICurrencyService _currencyService;
        private readonly PausePopupController _pauseController;

        public GameplayHUDController(
            GameplayHUDView hudView,
            ICurrencyService currencyService,
            PausePopupController pauseController)
        {
            _hudView = hudView ?? throw new ArgumentNullException(nameof(hudView));
            _currencyService = currencyService ?? throw new ArgumentNullException(nameof(currencyService));
            _pauseController = pauseController;

            _hudView.UpdateCurrency(_currencyService.CurrentAmount);

            _currencyService.OnAmountChanged += _hudView.UpdateCurrency;
            _hudView.OnPauseClicked += HandlePauseClicked;
        }

        private void HandlePauseClicked()
        {
            if (_pauseController != null)
            {
                if (_pauseController.IsPaused)
                {
                    _pauseController.Resume();
                }
                else
                {
                    _pauseController.Pause();
                }
            }
        }

        public void Dispose()
        {
            if (_currencyService != null)
            {
                _currencyService.OnAmountChanged -= _hudView.UpdateCurrency;
            }

            if (_hudView != null)
            {
                _hudView.OnPauseClicked -= HandlePauseClicked;
            }
        }
    }
}