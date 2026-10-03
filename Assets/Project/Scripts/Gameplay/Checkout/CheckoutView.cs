using UnityEngine;

namespace MarketTycoon.Gameplay.Checkout
{
    public class CheckoutView : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private CheckoutConfigSO _config;

        [Header("Queue Setup")]
        [SerializeField] private Transform _paymentStandPoint;

        public CheckoutConfigSO Config => _config;
        public Transform PaymentStandPoint => _paymentStandPoint != null ? _paymentStandPoint : transform;
    }
}

