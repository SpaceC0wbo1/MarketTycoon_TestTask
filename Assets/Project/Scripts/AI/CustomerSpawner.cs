using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarketTycoon.Gameplay.AI
{
    public class CustomerSpawner : MonoBehaviour
    {
        [Header("Customer Variations")]
        [SerializeField] private GameObject[] _customerPrefabs;

        [Header("Waypoints")]
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _exitPoint;

        [Header("Market Wander Areas")]
        [Tooltip("List of all room wander zone colliders (inactive rooms will be included once unlocked)")]
        [SerializeField] private List<Collider> _wanderZones = new List<Collider>();

        [Header("Browsing Duration")]
        [SerializeField] private float _minBrowseTime = 2.0f;
        [SerializeField] private float _maxBrowseTime = 5.0f;

        [Header("Pacing")]
        [SerializeField] private float _waveInterval = 5.0f;
        [SerializeField] private float _staggerDelay = 0.6f;

        [Header("Capacity Limits")]
        [SerializeField] private int _maxGlobalCapacity = 8;

        private readonly List<GameObject> _activeCustomers = new List<GameObject>();
        private MarketRegistry _marketRegistry;
        private Coroutine _spawnRoutine;

        public void Initialize(MarketRegistry registry)
        {
            _marketRegistry = registry;

            if (_spawnRoutine != null)
            {
                StopCoroutine(_spawnRoutine);
            }
            _spawnRoutine = StartCoroutine(SpawnLoop());
        }

        public void RegisterWanderZone(Collider zoneCollider)
        {
            if (zoneCollider != null && !_wanderZones.Contains(zoneCollider))
            {
                _wanderZones.Add(zoneCollider);
            }
        }

        private List<Bounds> GetActiveWanderBounds()
        {
            var activeBounds = new List<Bounds>();
            for (int i = 0; i < _wanderZones.Count; i++)
            {
                if (_wanderZones[i] != null && _wanderZones[i].gameObject.activeInHierarchy && _wanderZones[i].enabled)
                {
                    activeBounds.Add(_wanderZones[i].bounds);
                }
            }
            return activeBounds;
        }

        private IEnumerator SpawnLoop()
        {
            yield return new WaitForSeconds(1.5f);

            while (true)
            {
                _activeCustomers.RemoveAll(c => c == null);

                if (_marketRegistry != null && _marketRegistry.ActiveShelvesCount > 0)
                {
                    int targetWaveCount = _marketRegistry.ActiveShelvesCount;
                    int allowedToSpawn = Mathf.Min(targetWaveCount, _maxGlobalCapacity - _activeCustomers.Count);

                    for (int i = 0; i < allowedToSpawn; i++)
                    {
                        SpawnSingleCustomer();
                        yield return new WaitForSeconds(_staggerDelay);
                    }
                }

                yield return new WaitForSeconds(_waveInterval);
            }
        }

        private void SpawnSingleCustomer()
        {
            if (_customerPrefabs == null || _customerPrefabs.Length == 0 || _spawnPoint == null || _marketRegistry == null)
                return;

            var targetShelf = _marketRegistry.GetOptimalShelf();
            var targetCheckout = _marketRegistry.GetLessBusyCheckout();

            if (targetShelf == null || targetCheckout == null)
                return;

            GameObject selectedPrefab = _customerPrefabs[Random.Range(0, _customerPrefabs.Length)];
            if (selectedPrefab == null) return;

            GameObject customerInstance = Instantiate(selectedPrefab, _spawnPoint.position, _spawnPoint.rotation);
            CustomerView view = customerInstance.GetComponent<CustomerView>();
            CustomerController controller = customerInstance.AddComponent<CustomerController>();

            List<Bounds> currentActiveZones = GetActiveWanderBounds();

            controller.Initialize(
                view,
                targetShelf,
                targetCheckout,
                _exitPoint,
                currentActiveZones,
                _minBrowseTime,
                _maxBrowseTime
            );

            _activeCustomers.Add(customerInstance);
        }

        private void OnDestroy()
        {
            if (_spawnRoutine != null)
            {
                StopCoroutine(_spawnRoutine);
            }
        }
    }
}