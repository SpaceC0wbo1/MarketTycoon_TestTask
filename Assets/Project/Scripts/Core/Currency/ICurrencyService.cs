using System;

namespace MarketTycoon.Core.Currency 
{ 
    public interface ICurrencyService
    {
        int CurrentAmount { get; }
        event Action<int> OnAmountChanged;

        void Add(int amount);
        bool TrySpend(int amount);
        bool CanAfford(int amount);
        void SetAmount(int amount);
    }
}

