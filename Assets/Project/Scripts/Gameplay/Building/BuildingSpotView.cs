using System.Collections;
using TMPro;
using UnityEngine;

namespace MarketTycoon.Gameplay.Building
{
    [RequireComponent(typeof(Collider))]
    public class BuildingSpotView : MonoBehaviour
    {
        [Header("Instance Identity")]
        [Tooltip("Unique save identifier for this specific spot on scene.")]
        [SerializeField] private string _spotId;

        [Header("Config")]
        [SerializeField] private BuildingConfigSO _config;

        [Header("UI & Visual Elements")]
        [Tooltip("Floor marker / construction bounds that should be visible only before construction.")]
        [SerializeField] private GameObject _constructionZoneVisual;

        [Tooltip("World-space TMP text displaying construction cost.")]
        [SerializeField] private TMP_Text _costText;

        [Tooltip("The actual shelf / cashier structure activated after construction.")]
        [SerializeField] private GameObject _objectToActivate;

        public string SpotId => !string.IsNullOrEmpty(_spotId) ? _spotId : gameObject.name;
        public BuildingConfigSO Config => _config;

        private Collider _spotCollider;

        private void Awake()
        {
            _spotCollider = GetComponent<Collider>();
            EnsureReferences();
        }

        private void EnsureReferences()
        {
            if (_spotCollider == null)
            {
                _spotCollider = GetComponent<Collider>();
            }

            // Do NOT auto-assign object to activate if this spot is a room expansion
            if (_config != null && _config.Type == BuildingType.RoomExpansion)
            {
                return;
            }

            if (_objectToActivate == null)
            {
                var shelfChild = GetComponentInChildren<MarketTycoon.Gameplay.Shelf.ShelfView>(true);
                if (shelfChild != null && shelfChild.gameObject != gameObject)
                {
                    _objectToActivate = shelfChild.gameObject;
                }
                else
                {
                    var checkoutChild = GetComponentInChildren<MarketTycoon.Gameplay.Checkout.CheckoutView>(true);
                    if (checkoutChild != null && checkoutChild.gameObject != gameObject)
                    {
                        _objectToActivate = checkoutChild.gameObject;
                    }
                }
            }
        }

        public void SetupCostDisplay()
        {
            if (_costText != null && _config != null)
            {
                _costText.text = $"${_config.BuildCost}";
            }
        }

        public void UpdateVisuals(bool isBuilt)
        {
            EnsureReferences();
            SetupCostDisplay();

            if (_constructionZoneVisual != null)
            {
                _constructionZoneVisual.SetActive(!isBuilt);
            }

            if (_costText != null)
            {
                _costText.gameObject.SetActive(!isBuilt);
            }

            // For room expansion, inner objects are handled directly by RoomExpansionView
            if (_config != null && _config.Type == BuildingType.RoomExpansion)
            {
                if (_spotCollider != null)
                {
                    _spotCollider.enabled = !isBuilt;
                }
                return;
            }

            if (_objectToActivate != null)
            {
                _objectToActivate.SetActive(isBuilt);

                if (isBuilt && _objectToActivate.transform.localScale == Vector3.zero)
                {
                    _objectToActivate.transform.localScale = Vector3.one;
                }
            }

            if (_spotCollider != null)
            {
                _spotCollider.enabled = !isBuilt;
            }
        }

        public void PlayBuildEffect()
        {
            EnsureReferences();

            if (_config != null && _config.Type == BuildingType.RoomExpansion)
            {
                return;
            }

            if (_objectToActivate != null)
            {
                StopAllCoroutines();
                StartCoroutine(PopScaleRoutine(_objectToActivate.transform));
            }
        }

        private IEnumerator PopScaleRoutine(Transform targetTransform)
        {
            Vector3 originalScale = Vector3.one;
            targetTransform.localScale = Vector3.zero;

            float elapsed = 0f;
            float duration = 0.25f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                float scaleMultiplier = Mathf.Sin(progress * Mathf.PI * 0.5f);
                targetTransform.localScale = originalScale * scaleMultiplier;
                yield return null;
            }

            targetTransform.localScale = originalScale;
        }
    }
}