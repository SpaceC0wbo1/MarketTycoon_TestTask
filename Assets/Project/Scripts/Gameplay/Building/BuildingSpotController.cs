using System;
using MarketTycoon.Core.Currency;
using UnityEngine;

namespace MarketTycoon.Gameplay.Building
{
    public class BuildingSpotController : MonoBehaviour
    {
        [SerializeField] private BuildingSpotView _view;

        private BuildingSpotModel _model;
        private ICurrencyService _currencyService;
        private Action<BuildingSpotController> _onBuiltCallback;

        public BuildingSpotModel Model => _model;
        public BuildingSpotView View => _view;

        public void Initialize(BuildingSpotModel model, ICurrencyService currencyService, Action<BuildingSpotController> onBuilt)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _currencyService = currencyService ?? throw new ArgumentNullException(nameof(currencyService));
            _onBuiltCallback = onBuilt;

            _view.UpdateVisuals(_model.IsUnlocked);
        }

        private void OnMouseDown()
        {
            TryPurchase();
        }

        public bool TryPurchase()
        {
            if (_model == null || _model.IsUnlocked) return false;

            if (_currencyService.TrySpend(_model.Cost))
            {
                _model.TryUnlock();
                _view.UpdateVisuals(true);
                _view.PlayBuildEffect();

                _onBuiltCallback?.Invoke(this);
                return true;
            }

            Debug.Log($"[BuildingSystem] Insufficient funds! Required: ${_model.Cost}, current balance: ${_currencyService.CurrentAmount}");
            return false;
        }
    }
}