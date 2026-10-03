using UnityEngine;

namespace MarketTycoon.Gameplay.Shelf
{
    public class ShelfView : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private ShelfConfigSO _config;

        [Header("Interaction Setup")]
        [SerializeField] private Transform _customerStandPoint;

        public ShelfConfigSO Config => _config;
        public Transform CustomerStandPoint => _customerStandPoint != null ? _customerStandPoint : transform;

        public void UpdateStockVisuals(int currentStock, int maxStock)
        {

        }
    }
}

