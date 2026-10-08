using System.Collections.Generic;
using System.IO;
using MarketTycoon.Core.Currency;
using MarketTycoon.Core.SaveSystem;
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
        public static GameBootstrap Instance { get; private set; }

        [Header("Starting Economy")]
        [SerializeField] private int _startingCurrency = 100;
        [SerializeField] private CurrencyView _currencyView;

        [Header("Scene References")]
        [SerializeField] private MarketRegistry _marketRegistry;
        [SerializeField] private CustomerSpawner _customerSpawner;
        [SerializeField] private CheckoutView _defaultCheckoutView;
        [SerializeField] private List<BuildingSpotController> _buildingSpots = new List<BuildingSpotController>();

        [Header("Persistence / AutoSave")]
        [SerializeField] private bool _enableAutoSave = true;
        [SerializeField] private float _autoSaveInterval = 5f;

        private CurrencyModel _currencyModel;
        private CurrencyController _currencyController;
        private GameSaveData _currentSaveData;
        private float _autoSaveTimer;
        private bool _isInitialized = false;

        public ICurrencyService CurrencyService => _currencyController;
        public MarketRegistry MarketRegistry => _marketRegistry;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (_marketRegistry != null)
            {
                _marketRegistry.ClearAll();
            }

            InitializeCoreServices();
            InitializeDefaultCheckout();
            InitializeBuildingSpots();
            InitializeSaveSystem();

            _isInitialized = true;

            InitializeGameplayFlow();
        }

        private void InitializeCoreServices()
        {
            if (_currencyView == null)
            {
                _currencyView = FindFirstObjectByType<CurrencyView>();
            }

            _currencyModel = new CurrencyModel();
            _currencyController = new CurrencyController(_currencyModel, _currencyView);
        }

        private void InitializeDefaultCheckout()
        {
            if (_defaultCheckoutView == null)
            {
                _defaultCheckoutView = FindFirstObjectByType<CheckoutView>();
            }

            if (_defaultCheckoutView != null && _defaultCheckoutView.Config != null)
            {
                var checkoutModel = new CheckoutModel(_defaultCheckoutView.Config);
                var checkoutController = new CheckoutController(checkoutModel, _defaultCheckoutView, _currencyController);
                _marketRegistry.RegisterCheckout(checkoutController);
            }
        }

        private void InitializeBuildingSpots()
        {
            foreach (var spot in _buildingSpots)
            {
                if (spot == null || spot.View == null || spot.View.Config == null) continue;

                var model = new BuildingSpotModel(spot.View.Config, spot.InitiallyBuilt);
                spot.Initialize(model, _currencyController, HandleSpotBuilt);

                if (spot.InitiallyBuilt)
                {
                    RegisterBuiltSpotServices(spot);
                }
            }
        }

        private void InitializeSaveSystem()
        {
            string savePath = Path.Combine(Application.persistentDataPath, "savegame.json");
            bool hasSaveFile = File.Exists(savePath);

            _currentSaveData = SaveLoadManager.Load();

            if (hasSaveFile && _currentSaveData.Money >= 0)
            {
                _currencyController.SetAmount(_currentSaveData.Money);
            }
            else
            {
                _currencyController.SetAmount(_startingCurrency);
            }

            if (_currencyView != null)
            {
                _currencyView.UpdateDisplay(_currencyController.CurrentAmount);
            }

            if (hasSaveFile && _currentSaveData.BuildingSpots != null)
            {
                foreach (var spot in _buildingSpots)
                {
                    if (spot == null || spot.View == null) continue;
                    if (spot.InitiallyBuilt) continue;

                    string spotId = spot.View.SpotId;
                    var savedEntry = _currentSaveData.BuildingSpots.Find(s => s.SpotId == spotId);

                    if (savedEntry != null && savedEntry.IsUnlocked)
                    {
                        spot.RestoreSavedState(true);
                        RegisterBuiltSpotServices(spot);
                    }
                    else
                    {
                        spot.RestoreSavedState(false);
                    }
                }
            }
        }

        private void InitializeGameplayFlow()
        {
            if (_customerSpawner != null && _marketRegistry != null)
            {
                _customerSpawner.Initialize(_marketRegistry);
            }
        }

        private void HandleSpotBuilt(BuildingSpotController spot)
        {
            RegisterBuiltSpotServices(spot);

            if (_isInitialized)
            {
                SaveGameState();
            }
        }

        private void RegisterBuiltSpotServices(BuildingSpotController spot)
        {
            if (spot == null) return;

            var roomExpansionView = spot.GetComponentInChildren<RoomExpansionView>(true);
            if (roomExpansionView != null)
            {
                roomExpansionView.UnlockRoom();
            }

            if (spot.IsBuilt)
            {
                var shelfView = spot.GetComponentInChildren<ShelfView>(true);
                if (shelfView != null && shelfView.Config != null)
                {
                    var newShelfModel = new ShelfModel(shelfView.Config);
                    var newShelfController = new ShelfController(newShelfModel, shelfView);
                    _marketRegistry.RegisterShelf(newShelfController);
                }

                var checkoutView = spot.GetComponentInChildren<CheckoutView>(true);
                if (checkoutView != null && checkoutView.Config != null)
                {
                    var newCheckoutModel = new CheckoutModel(checkoutView.Config);
                    var newCheckoutController = new CheckoutController(newCheckoutModel, checkoutView, _currencyController);
                    _marketRegistry.RegisterCheckout(newCheckoutController);
                }
            }
        }

        public void SaveGameState()
        {
            if (!_isInitialized) return;

            if (_currentSaveData == null)
            {
                _currentSaveData = new GameSaveData();
            }

            _currentSaveData.Money = _currencyController.CurrentAmount;
            _currentSaveData.BuildingSpots.Clear();

            foreach (var spot in _buildingSpots)
            {
                if (spot != null && spot.View != null)
                {
                    _currentSaveData.BuildingSpots.Add(new BuildingSpotSaveEntry
                    {
                        SpotId = spot.View.SpotId,
                        IsUnlocked = spot.IsBuilt
                    });
                }
            }

            SaveLoadManager.Save(_currentSaveData);
        }

        private void Update()
        {
            if (!_enableAutoSave || !_isInitialized) return;

            _autoSaveTimer += Time.deltaTime;
            if (_autoSaveTimer >= _autoSaveInterval)
            {
                _autoSaveTimer = 0f;
                SaveGameState();
            }
        }

        private void OnApplicationQuit()
        {
            SaveGameState();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveGameState();
            }
        }

        [ContextMenu("Clear Game Save")]
        public void ClearSaveForTesting()
        {
            SaveLoadManager.ClearSave();
            Debug.Log("[GameBootstrap] Save file deleted.");
        }
    }
}