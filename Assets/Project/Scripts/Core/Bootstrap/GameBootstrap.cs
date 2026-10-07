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
        [Tooltip("The main cashier counter if not using a building spot for it")]
        [SerializeField] private CheckoutView _initialCheckoutView;

        [Header("Systems")]
        [SerializeField] private MarketRegistry _marketRegistry;
        [SerializeField] private CustomerSpawner _customerSpawner;

        [Header("Building Spots")]
        [SerializeField] private List<BuildingSpotController> _buildingSpots = new List<BuildingSpotController>();

        [Header("Configuration")]
        [SerializeField] private int _initialBalance = 150;

        private CurrencyController _currencyController;
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
            // 1. Currency MVC initialization
            var currencyModel = new CurrencyModel(_initialBalance);
            _currencyController = new CurrencyController(currencyModel, _currencyView);

            // 2. Fallback: Register direct checkout counter if assigned
            if (_initialCheckoutView != null && _initialCheckoutView.Config != null)
            {
                var checkoutModel = new CheckoutModel(_initialCheckoutView.Config);
                _initialCheckoutController = new CheckoutController(checkoutModel, _initialCheckoutView, _currencyController);
                _marketRegistry.RegisterCheckout(_initialCheckoutController);
                Debug.Log("[GameBootstrap] Initial checkout successfully registered.");
            }

            // 3. Initialize building spots
            foreach (var spot in _buildingSpots)
            {
                if (spot != null && spot.View != null && spot.View.Config != null)
                {
                    var spotModel = new BuildingSpotModel(spot.View.Config, spot.InitiallyBuilt);
                    spot.Initialize(spotModel, _currencyController, HandleSpotBuilt);
                }
            }

            // 4. Start Customer Spawner loop
            if (_customerSpawner != null)
            {
                _customerSpawner.Initialize(_marketRegistry);
                Debug.Log("[GameBootstrap] Customer Spawner initialized.");
            }
        }

        private void HandleSpotBuilt(BuildingSpotController spot)
        {
            Debug.Log($"[GameBootstrap] Handling built spot: '{spot.Model.Title}'");

            // 1. Room Expansion
            var roomExpansionView = spot.GetComponentInChildren<RoomExpansionView>(true);
            if (roomExpansionView != null)
            {
                roomExpansionView.UnlockRoom();
                Debug.Log("[GameBootstrap] Room Expansion unlocked!");
            }

            // 2. Shelf registration
            var shelfView = spot.GetComponentInChildren<ShelfView>(true);
            if (shelfView != null && shelfView.Config != null)
            {
                var newShelfModel = new ShelfModel(shelfView.Config);
                var newShelfController = new ShelfController(newShelfModel, shelfView);
                _marketRegistry.RegisterShelf(newShelfController);
                Debug.Log($"[GameBootstrap] Registered Shelf: '{spot.Model.Title}'");
            }

            // 3. Checkout registration
            var checkoutView = spot.GetComponentInChildren<CheckoutView>(true);
            if (checkoutView != null && checkoutView.Config != null)
            {
                var newCheckoutModel = new CheckoutModel(checkoutView.Config);
                var newCheckoutController = new CheckoutController(newCheckoutModel, checkoutView, _currencyController);
                _marketRegistry.RegisterCheckout(newCheckoutController);
                Debug.Log($"[GameBootstrap] Registered Checkout: '{spot.Model.Title}'");
            }
        }

        private void OnDestroy()
        {
            _currencyController?.Dispose();
        }
    }
}