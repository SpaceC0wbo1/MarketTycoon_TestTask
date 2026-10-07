using System.Collections;
using TMPro;
using UnityEngine;

namespace MarketTycoon.Gameplay.Building
{

    [RequireComponent(typeof(Collider))]
    public class BuildingSpotView : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private BuildingConfigSO _config;

        [Header("UI & Visual Elements")]
        [Tooltip("Floor marker / construction bounds that should be visible only before construction.")]
        [SerializeField] private GameObject _constructionZoneVisual;

        [Tooltip("World-space TMP text displaying construction cost.")]
        [SerializeField] private TMP_Text _costText;

        [Tooltip("The actual shelf / cashier structure activated after construction.")]
        [SerializeField] private GameObject _objectToActivate;

        public BuildingConfigSO Config => _config;

        public void SetupCostDisplay()
        {
            if (_costText != null && _config != null)
            {
                _costText.text = $"${_config.BuildCost}";
            }
        }

        public void UpdateVisuals(bool isBuilt)
        {
            SetupCostDisplay();

            // Disable construction visuals and price label if already built
            if (_constructionZoneVisual != null)
            {
                _constructionZoneVisual.SetActive(!isBuilt);
            }

            if (_costText != null)
            {
                _costText.gameObject.SetActive(!isBuilt);
            }

            // Ensure the main structure is active when built, or hidden when pending build
            if (_objectToActivate != null)
            {
                _objectToActivate.SetActive(isBuilt);
            }

            // Disable interaction collider once built so it can no longer be clicked
            var spotCollider = GetComponent<Collider>();
            if (spotCollider != null)
            {
                spotCollider.enabled = !isBuilt;
            }
        }

        public void PlayBuildEffect()
        {
            if (_objectToActivate != null)
            {
                StartCoroutine(PopScaleRoutine(_objectToActivate.transform));
            }
        }

        private IEnumerator PopScaleRoutine(Transform targetTransform)
        {
            Vector3 originalScale = targetTransform.localScale;
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