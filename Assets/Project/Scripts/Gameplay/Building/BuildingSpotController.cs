using System;
using MarketTycoon.Core.Bootstrap;
using MarketTycoon.Core.Currency;
using UnityEngine;

namespace MarketTycoon.Gameplay.Building
{
    [RequireComponent(typeof(Collider))]
    public class BuildingSpotController : MonoBehaviour
    {
        [Header("Initial State")]
        [Tooltip("If checked, the structure starts fully built, active, and construction visual is hidden.")]
        [SerializeField] private bool _initiallyBuilt = false;

        [Header("View Reference")]
        [SerializeField] private BuildingSpotView _view;

        private BuildingSpotModel _model;
        private ICurrencyService _currencyService;
        private Action<BuildingSpotController> _onBuiltCallback;
        private Collider _spotCollider;
        private Camera _mainCamera;

        public BuildingSpotModel Model => _model;
        public BuildingSpotView View => _view;
        public bool IsBuilt => _model != null ? _model.IsUnlocked : _initiallyBuilt;
        public bool InitiallyBuilt => _initiallyBuilt;

        private void Awake()
        {
            _spotCollider = GetComponent<Collider>();
            _mainCamera = Camera.main;

            if (_view == null)
            {
                _view = GetComponent<BuildingSpotView>();
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_view == null)
            {
                _view = GetComponent<BuildingSpotView>();
            }

            if (_view != null)
            {
                _view.UpdateVisuals(_initiallyBuilt);
            }
        }
#endif

        public void Initialize(BuildingSpotModel model, ICurrencyService currencyService, Action<BuildingSpotController> onBuilt)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _currencyService = currencyService ?? throw new ArgumentNullException(nameof(currencyService));
            _onBuiltCallback = onBuilt;

            if (_view != null)
            {
                _view.UpdateVisuals(_model.IsUnlocked);
            }

            if (_model.IsUnlocked)
            {
                _onBuiltCallback?.Invoke(this);
            }
        }

        private void Update()
        {
            if (IsBuilt) return;

            if (Input.GetMouseButtonDown(0))
            {
                CheckRaycastClick();
            }
        }

        private void CheckRaycastClick()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
                if (_mainCamera == null) return;
            }

            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 200f))
            {
                if (hit.collider == _spotCollider || hit.transform.IsChildOf(transform))
                {
                    TryPurchase();
                }
            }
        }

        public bool TryPurchase()
        {
            if (_model == null && _view != null && _view.Config != null)
            {
                if (GameBootstrap.Instance != null)
                {
                    _model = new BuildingSpotModel(_view.Config, _initiallyBuilt);
                    _currencyService = GameBootstrap.Instance.CurrencyService;
                }
            }

            if (_model == null) return false;
            if (_model.IsUnlocked) return false;

            if (_currencyService == null) return false;

            if (_currencyService.TrySpend(_model.Cost))
            {
                _model.TryUnlock();

                if (_view != null)
                {
                    _view.UpdateVisuals(true);
                    _view.PlayBuildEffect();
                }

                _onBuiltCallback?.Invoke(this);
                return true;
            }

            return false;
        }

        public void RestoreSavedState(bool isUnlocked)
        {
            if (_model != null)
            {
                _model.RestoreUnlockedState(isUnlocked);
            }

            if (_view != null)
            {
                _view.UpdateVisuals(isUnlocked);
            }
        }
    }
}