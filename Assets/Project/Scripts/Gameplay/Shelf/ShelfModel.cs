using System;
using System.Security.Cryptography.X509Certificates;

namespace MarketTycoon.Gameplay.Shelf
{
    public class ShelfModel
    {
        private readonly ShelfConfigSO _config;
        private int _currentStock;

        public string ProductId => _config.ProductId;
        public string ProductName => _config.ProductName;
        public int MaxCapacity => _config.MaxCapacity;
        public int ItemPrice => _config.UnitPrice;
        public int CurrentStock => _currentStock;
        public bool HasStock => _currentStock > 0;

        public event Action<int, int> OnStockChanged; // current, max

        public ShelfModel(ShelfConfigSO config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _currentStock = _config.MaxCapacity;
        }

        public bool TryTakeItem()
        {
            if (_currentStock <= 0) return false;

            _currentStock--;
            OnStockChanged?.Invoke(_currentStock, _config.MaxCapacity);
            return true;
        }

        public void Restock(int amount)
        {
            _currentStock = Math.Min(_config.MaxCapacity, _currentStock + amount);
            OnStockChanged?.Invoke(_currentStock, _config.MaxCapacity);
        }
    }
}
