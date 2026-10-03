using System;

namespace MarketTycoon.Gameplay.Checkout
{
    public class CheckoutModel
    {
        private readonly CheckoutConfigSO _config;
        private float _currentServiceDuration;

        public string CashierId => _config.CashierId;
        public string CashierName => _config.CashierName;
        public float ServiceDuration => _currentServiceDuration;

        public event Action<float> OnServiceDurationChanged;

        public CheckoutModel(CheckoutConfigSO config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _currentServiceDuration = _config.BaseServiceDuration;
        }

        public void SetServiceDuration(float duration)
        {
            _currentServiceDuration = MathF.Max(0.1f, duration);
            OnServiceDurationChanged?.Invoke(_currentServiceDuration);
        }

        public void ReduceServiceDuration(float percent)
        {
            float reduction = _currentServiceDuration * (percent / 100f);
            SetServiceDuration(_currentServiceDuration - reduction);
        }
    }
}
