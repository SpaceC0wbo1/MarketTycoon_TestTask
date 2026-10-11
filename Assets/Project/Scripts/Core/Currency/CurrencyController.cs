using System;

namespace MarketTycoon.Core.Currency
{
    public class CurrencyController : ICurrencyService
    {
        private readonly CurrencyModel _model;

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
    }
}