using UnityEngine;

namespace MarketTycoon.Gameplay.Checkout
{
    public class CheckoutView : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private CheckoutConfigSO _config;

        [Header("Queue Layout")]
        [Tooltip("Directly assigned queue points. If empty, points will be calculated automatically.")]
        [SerializeField] private Transform[] _manualQueuePoints;
        [SerializeField] private Transform _paymentStandPoint;
        [SerializeField] private float _slotSpacing = 0.9f;
        [SerializeField] private int _totalSlots = 5;

        public CheckoutConfigSO Config => _config;
        public Transform PaymentStandPoint => _paymentStandPoint != null ? _paymentStandPoint : transform;

        public Vector3 GetQueueSlotPosition(int slotIndex)
        {
            if (_manualQueuePoints != null && _manualQueuePoints.Length > slotIndex && _manualQueuePoints[slotIndex] != null)
            {
                return _manualQueuePoints[slotIndex].position;
            }

            // Fallback: calculate backwards along the transform's negative forward or back vector
            Vector3 origin = PaymentStandPoint.position;
            Vector3 queueDirection = -PaymentStandPoint.forward;
            return origin + (queueDirection * (slotIndex * _slotSpacing));
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < _totalSlots; i++)
            {
                Vector3 pos = GetQueueSlotPosition(i);
                Gizmos.DrawWireSphere(pos, 0.25f);
                if (i > 0)
                {
                    Gizmos.DrawLine(GetQueueSlotPosition(i - 1), pos);
                }
            }
        }
    }
}