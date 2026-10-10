using System;
using MarketTycoon.UI;

namespace MarketTycoon.Core.Currency
{
    public class CurrencyController : ICurrencyService, IDisposable
    {
        private readonly CurrencyModel _model;
        private readonly CurrencyView _view;

        public int CurrentAmount => _model.CurrentAmount;

        public event Action<int> OnAmountChanged
        {
            add => _model.OnAmountChanged += value;
            remove => _model.OnAmountChanged -= value;
        }

        public CurrencyController(CurrencyModel model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public void Add(int amount)
        {
            _model.Add(amount);
        }

        public bool TrySpend(int amount)
        {
            return _model.TrySpend(amount);
        }

        public bool CanAfford(int amount)
        {
            return _model.CanAfford(amount);
        }

        public void SetAmount(int amount)
        {
            _model.SetAmount(amount);
        }

        private void HandleAmountChanged(int newAmount) 
        {
            _view.UpdateDisplay(newAmount);
        }

        public void Dispose()
        {
            if (_model != null)
            {
                _model.OnAmountChanged -= HandleAmountChanged;
            }
        }
    }
}
