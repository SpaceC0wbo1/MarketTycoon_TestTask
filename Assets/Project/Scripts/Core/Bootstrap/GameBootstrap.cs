using MarketTycoon.Core.Currency;
using MarketTycoon.UI;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MarketTycoon.Core.Bootstrap
{
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Views")]
        [SerializeField] private CurrencyView _currencyView;

        [Header("Configuration")]
        [SerializeField] private int _initialBalance = 100;

        private CurrencyController _currencyController;

        public ICurrencyService CurrencyService => _currencyController;
        public static GameBootstrap Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            InitializeCore();
        }

        private void InitializeCore() 
        {
            var currencyModel = new CurrencyModel(_initialBalance);
            _currencyController = new CurrencyController(currencyModel, _currencyView);

            Debug.Log($"[GameBootstrap] Currency system initialized. Starting balance: {_initialBalance} ");
        }

        private void OnDestroy()
        {
            _currencyController.Dispose();
        }

        #region Debug Context Menu

        [ContextMenu("Debug: Add 50 money")]
        public void DebugAddMoney()
        {
            _currencyController?.Add(50);
        }

        [ContextMenu("Debug: Spend 30 money")]
        public void DebugSpendMoney()
        {
            _currencyController?.TrySpend(30);
        }

        #endregion
    }
}




