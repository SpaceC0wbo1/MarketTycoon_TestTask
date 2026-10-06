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
        [SerializeField] private GameObject _constructionZoneVisual;
        [SerializeField] private TMP_Text _costText;
        [SerializeField] private GameObject _objectToActivate;

        public BuildingConfigSO Config => _config;

        private void Awake()
        {
            UpdateVisuals(false);
            if (_costText != null && _config != null)
            {
                _costText.text = $"${_config.BuildCost}";
            }
        }

        public void UpdateVisuals(bool isBuilt)
        {
            if (_constructionZoneVisual != null)
            {
                _constructionZoneVisual.SetActive(!isBuilt);
            }

            if (_costText != null)
            {
                _costText.gameObject.SetActive(!isBuilt);
            }

            if (_objectToActivate != null)
            {
                _objectToActivate.SetActive(isBuilt);
            }


            var col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = !isBuilt;
            }
        }

        public void PlayBuildEffect()
        {
            if (_objectToActivate != null)
            {
                StartCoroutine(PopScaleRoutine(_objectToActivate.transform));
            }
        }

        private System.Collections.IEnumerator PopScaleRoutine(Transform targetTransform)
        {
            Vector3 targetScale = targetTransform.localScale;
            targetTransform.localScale = Vector3.zero;

            float elapsed = 0f;
            float duration = 0.25f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                float scaleMod = Mathf.Sin(progress * Mathf.PI * 0.5f);
                targetTransform.localScale = targetScale * scaleMod;
                yield return null;
            }

            targetTransform.localScale = targetScale;
        }
    }
}