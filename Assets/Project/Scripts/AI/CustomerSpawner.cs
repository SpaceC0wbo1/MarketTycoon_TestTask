using System.Collections;
using UnityEngine;

namespace MarketTycoon.Gameplay.AI
{
    public class CustomerSpawner : MonoBehaviour
    {
        [Header("Prefab & Points")]
        [SerializeField] private GameObject _customerPrefab;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _exitPoint;

        [Header("Interval")]
        [SerializeField] private float _spawnInterval = 3.5f;

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
            while (true)
            {
                yield return new WaitForSeconds(_spawnInterval);
                SpawnCustomer();
            }
        }

        private void SpawnCustomer()
        {
            if (_customerPrefab == null || _spawnPoint == null || _marketRegistry == null) return;

            var targetShelf = _marketRegistry.GetRandomAvailableShelf();
            var targetCheckout = _marketRegistry.GetLessBusyCheckout();

            if (targetShelf == null || targetCheckout == null) return;

            GameObject customerInstance = Instantiate(_customerPrefab, _spawnPoint.position, _spawnPoint.rotation);
            CustomerView view = customerInstance.GetComponent<CustomerView>();
            CustomerController controller = customerInstance.AddComponent<CustomerController>();

            controller.Initialize(view, targetShelf, targetCheckout, _exitPoint);
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