using UnityEngine;

[CreateAssetMenu(fileName = "CheckoutConfig_Default", menuName = "Market Tycoon/Configs/Checkout Config")]
public class CheckoutConfigSO : ScriptableObject
{
    [Header("Cashier Identity")]
    [SerializeField] private string _cashierId = "checkout_01";
    [SerializeField] private string _cashierName = "Standard Cashier";

    [Header("Service Timing")]
    [Tooltip("The standard duration of service for a single customer in seconds")]
    [Min(0.1f)]
    [SerializeField] private float _baseServiceDuration = 1.2f;

    public string CashierId => _cashierId;
    public string CashierName => _cashierName;
    public float BaseServiceDuration => _baseServiceDuration;
}
