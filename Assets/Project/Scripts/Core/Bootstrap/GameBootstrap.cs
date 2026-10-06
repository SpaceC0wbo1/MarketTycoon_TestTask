using System.Collections.Generic;
using MarketTycoon.Core.Currency;
using MarketTycoon.Gameplay;
using MarketTycoon.Gameplay.AI;
using MarketTycoon.Gameplay.Building;
using MarketTycoon.Gameplay.Checkout;
using MarketTycoon.Gameplay.Shelf;
using MarketTycoon.UI;
using UnityEngine;

namespace MarketTycoon.Core.Bootstrap
{
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Views")]
        [SerializeField] private CurrencyView _currencyView;
        [SerializeField] private ShelfView _initialShelfView;
        [SerializeField] private CheckoutView _initialCheckoutView;

        [Header("Systems")]
        [SerializeField] private MarketRegistry _marketRegistry;
        [SerializeField] private CustomerSpawner _customerSpawner;

        [Header("Building Spots")]
        [SerializeField] private List<BuildingSpotController> _buildingSpots = new List<BuildingSpotController>();

        [Header("Configuration")]
        [SerializeField] private int _initialBalance = 250;

        private CurrencyController _currencyController;
        private ShelfController _initialShelfController;
        private CheckoutController _initialCheckoutController;

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
            // 1. Currency MVC
            var currencyModel = new CurrencyModel(_initialBalance);
            _currencyController = new CurrencyController(currencyModel, _currencyView);

            // 2. Initial Shelf MVC
            if (_initialShelfView != null && _initialShelfView.Config != null)
            {
                var shelfModel = new ShelfModel(_initialShelfView.Config);
                _initialShelfController = new ShelfController(shelfModel, _initialShelfView);
                _marketRegistry.RegisterShelf(_initialShelfController);
            }

            // 3. Initial Checkout MVC
            if (_initialCheckoutView != null && _initialCheckoutView.Config != null)
            {
                var checkoutModel = new CheckoutModel(_initialCheckoutView.Config);
                _initialCheckoutController = new CheckoutController(checkoutModel, _initialCheckoutView, _currencyController);
                _marketRegistry.RegisterCheckout(_initialCheckoutController);
            }

            // 4. Initialization of building spots
            foreach (var spot in _buildingSpots)
            {
                if (spot != null && spot.View != null && spot.View.Config != null)
                {
                    var spotModel = new BuildingSpotModel(spot.View.Config, false);
                    spot.Initialize(spotModel, _currencyController, HandleSpotBuilt);
                }
            }

            // 5. Customer Spawner
            if (_customerSpawner != null)
            {
                _customerSpawner.Initialize(_marketRegistry);
            }

            Debug.Log("[GameBootstrap] Initialization with Building System completed.");
        }

        private void HandleSpotBuilt(BuildingSpotController spot)
        {
            Debug.Log($"[GameBootstrap] Споруду '{spot.Model.Title}' побудовано!");

            // If a shelf has been assembled, we register it in the customer system
            var shelfView = spot.View.GetComponentInChildren<ShelfView>(true);
            if (shelfView != null && shelfView.Config != null)
            {
                var newShelfModel = new ShelfModel(shelfView.Config);
                var newShelfController = new ShelfController(newShelfModel, shelfView);
                _marketRegistry.RegisterShelf(newShelfController);
            }

            // If a new cash desk has been set up
            var checkoutView = spot.View.GetComponentInChildren<CheckoutView>(true);
            if (checkoutView != null && checkoutView.Config != null)
            {
                var newCheckoutModel = new CheckoutModel(checkoutView.Config);
                var newCheckoutController = new CheckoutController(newCheckoutModel, checkoutView, _currencyController);
                _marketRegistry.RegisterCheckout(newCheckoutController);
            }
        }

        private void OnDestroy()
        {
            _currencyController?.Dispose();
            _initialShelfController?.Dispose();
        }
    }
}