using System;

namespace MarketTycoon.Core.Currency
{
    public class CurrencyModel
    {
        private int _currentAmount;

        public int CurrentAmount => _currentAmount;

        public event Action<int> OnAmountChanged;

        public CurrencyModel(int initialAmount = 0)
        {
            _currentAmount = Math.Max(0, initialAmount);
        }

        public void Add(int amount)
        {
            if (amount <= 0) return;

            _currentAmount += amount;
            OnAmountChanged?.Invoke(_currentAmount);
        }

        public bool TrySpend(int amount) 
        {
            if (amount <= 0 || _currentAmount < amount)
            {
                return false;
            }

            _currentAmount -= amount;
            OnAmountChanged?.Invoke(_currentAmount);
            return true;
        }

        public bool CanAfford(int amount)
        {
            return amount >= 0 && _currentAmount >= amount;
        }

        public void SetAmount(int amount)
        {
            _currentAmount = Math.Max(0, amount);
            OnAmountChanged?.Invoke(_currentAmount);
        }
    }
}
