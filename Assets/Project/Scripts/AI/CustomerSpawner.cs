using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarketTycoon.Gameplay.AI
{
    public class CustomerSpawner : MonoBehaviour
    {
        [Header("Prefab & Waypoints")]
        [SerializeField] private GameObject _customerPrefab;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _exitPoint;

        [Header("Pacing")]
        [Tooltip("Time between customer spawn waves.")]
        [SerializeField] private float _waveInterval = 5.0f;
        [Tooltip("Delay between individual customers within the same wave.")]
        [SerializeField] private float _staggerDelay = 0.6f;

        [Header("Capacity Limits")]
        [Tooltip("Maximum total customers permitted in market at once.")]
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

        private IEnumerator SpawnLoop()
        {
            yield return new WaitForSeconds(1.5f);

            while (true)
            {
                _activeCustomers.RemoveAll(c => c == null);

                if (_marketRegistry != null && _marketRegistry.ActiveShelvesCount > 0)
                {
                    // Target count matches active shelves: 1 shelf = 1 customer, 2 shelves = 2 customers, etc.
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
            if (_customerPrefab == null || _spawnPoint == null || _marketRegistry == null) return;

            var targetShelf = _marketRegistry.GetOptimalShelf();
            var targetCheckout = _marketRegistry.GetLessBusyCheckout();

            if (targetShelf == null || targetCheckout == null) return;

            GameObject customerInstance = Instantiate(_customerPrefab, _spawnPoint.position, _spawnPoint.rotation);
            CustomerView view = customerInstance.GetComponent<CustomerView>();
            CustomerController controller = customerInstance.AddComponent<CustomerController>();

            controller.Initialize(view, targetShelf, targetCheckout, _exitPoint);
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