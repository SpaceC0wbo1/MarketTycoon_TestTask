using MarketTycoon.Core.Currency;
using MarketTycoon.Gameplay.AI;
using MarketTycoon.Gameplay.Checkout;
using MarketTycoon.Gameplay.Shelf;
using MarketTycoon.UI;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

namespace MarketTycoon.Core.Bootstrap
{
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Views")]
        [SerializeField] private CurrencyView _currencyView;
        [SerializeField] private ShelfView _initialShelfView;
        [SerializeField] private CheckoutView _initialCheckoutView;

        [Header("Systems")]
        [SerializeField] private CustomerSpawner _customerSpawner;

        [Header("Configuration")]
        [SerializeField] private int _initialBalance = 100;

        private CurrencyController _currencyController;
        private ShelfController _shelfController;
        private CheckoutController _checkoutController;

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

            InitializeGame();
        }

        private void InitializeGame() 
        {
            // Currency MVC
            var currencyModel = new CurrencyModel(_initialBalance);
            _currencyController = new CurrencyController(currencyModel, _currencyView);
            Debug.Log($"[GameBootstrap] Currency system initialized. Starting balance: {_initialBalance} ");

            //Shelf MVC
            if(_initialShelfView != null && _initialShelfView.Config != null)
            {
                var shelfModel = new ShelfModel(_initialShelfView.Config);
                _shelfController = new ShelfController(shelfModel, _initialShelfView);
            }
            else
            {
                Debug.LogError("[GameBootstrap] ShelfView or ShelfConfigSO is missing!");
            }

            //Checkout MVC
            if (_initialCheckoutView != null && _initialCheckoutView.Config != null)
            {
                var checkoutModel = new CheckoutModel(_initialCheckoutView.Config);
                _checkoutController = new CheckoutController(checkoutModel, _initialCheckoutView, _currencyController);
            }
            else
            {
                Debug.LogError("[GameBootstrap] CheckoutView or CheckoutConfigSO is missing!");
            }

            //AI Spawner
            if(_customerSpawner != null && _shelfController != null && _checkoutController != null)
            {
                _customerSpawner.Initialize(_shelfController, _checkoutController);
            }

            Debug.Log("[GameBootstrap] Systems have been successfully initialised via ScriptableObjects.");
        }

        private void OnDestroy()
        {
            _currencyController?.Dispose();
            _shelfController?.Dispose();
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




