using UnityEngine;
using TMPro;

namespace MarketTycoon.UI
{
    public class CurrencyView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text _currencyText;

        [Header("Formatting")]
        [SerializeField] private string _format = "Money: {0}";

        public void UpdateDisplay(int amount)
        {
            if (_currencyText != null)
            {
                _currencyText.text = string.Format(_format, amount);
            }
        }
    }
}

