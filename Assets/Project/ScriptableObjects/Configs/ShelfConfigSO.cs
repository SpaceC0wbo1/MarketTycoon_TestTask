using UnityEngine;

namespace MarketTycoon.Gameplay.Shelf
{
    [CreateAssetMenu(fileName = "ShelfConfig_New", menuName = "Market Tycoon/Configs/Shelf Config")]
    public class ShelfConfigSO : ScriptableObject
    {
        [Header("Product Information")]
        [SerializeField] private string _productId = "bread_01";
        [SerializeField] private string _productName = "Fresh Bread";


        [Header("Economy & Capacity")]
        [Min(1)]
        [SerializeField] private int _unitPrice = 25;

        [Min(1)]
        [SerializeField] private int _maxCapacity = 25;

        public string ProductId => _productId;
        public string ProductName => _productName;
        public int MaxCapacity => _maxCapacity;
        public int UnitPrice => _unitPrice;
    }
}

