using System.Collections;
using MarketTycoon.Gameplay.Checkout;
using MarketTycoon.Gameplay.Shelf;
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

        private ShelfController _shelfController;
        private CheckoutController _checkoutController;
        private Coroutine _spawnRoutine;

        public void Initialize(ShelfController shelf, CheckoutController checkout)
        {
            _shelfController = shelf;
            _checkoutController = checkout;

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
            if (_customerPrefab == null || _spawnPoint == null) return;

            GameObject customerInstance = Instantiate(_customerPrefab, _spawnPoint.position, _spawnPoint.rotation);
            CustomerView view = customerInstance.GetComponent<CustomerView>();
            CustomerController controller = customerInstance.AddComponent<CustomerController>();

            controller.Initialize(view, _shelfController, _checkoutController, _exitPoint);
        }

        private void OnDestroy()
        {
            if (_spawnRoutine != null)
            {
                StopCoroutine (_spawnRoutine);
            }
        }
    }
}

