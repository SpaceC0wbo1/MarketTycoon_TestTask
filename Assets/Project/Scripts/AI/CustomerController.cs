using System.Collections;
using MarketTycoon.Gameplay.Checkout;
using MarketTycoon.Gameplay.Shelf;
using UnityEngine;
using UnityEngine.AI;

namespace MarketTycoon.Gameplay.AI
{
    public class CustomerController : MonoBehaviour
    {
        private CustomerModel _model;
        private CustomerView _view;
        private ShelfController _targetShelf;
        private CheckoutController _targetCheckout;
        private Transform _exitPoint;
        private System.Action _onQueueShiftAction;

        private float _browseDuration;

        public void Initialize(
            CustomerView view,
            ShelfController shelf,
            CheckoutController checkout,
            Transform exitPoint,
            float minBrowseTime = 2.0f,
            float maxBrowseTime = 6.0f)
        {
            _model = new CustomerModel();
            _view = view;
            _targetShelf = shelf;
            _targetCheckout = checkout;
            _exitPoint = exitPoint;

            _browseDuration = Random.Range(minBrowseTime, maxBrowseTime);

            StartCoroutine(CustomerLifecycleRoutine());
        }

        private IEnumerator CustomerLifecycleRoutine()
        {
            // 1. Browsing / Wandering around the store before shopping
            if (_browseDuration > 0.1f)
            {
                _model.CurrentState = CustomerState.Browsing;
                float browseTimer = 0f;

                while (browseTimer < _browseDuration)
                {
                    // Pick a random destination on NavMesh within wander radius
                    Vector3 wanderTarget = GetRandomNavMeshPoint(transform.position, 6.0f);
                    _view.MoveTo(wanderTarget);

                    float waitTime = Random.Range(1.0f, 2.5f);
                    float elapsed = 0f;

                    while (!_view.HasReachedDestination() && elapsed < waitTime)
                    {
                        elapsed += Time.deltaTime;
                        browseTimer += Time.deltaTime;
                        yield return null;
                    }

                    // Pause briefly to "inspect" items/scenery
                    yield return new WaitForSeconds(Random.Range(0.5f, 1.2f));
                    browseTimer += 1.0f;
                }
            }

            // 2. Walk to Shelf
            _model.CurrentState = CustomerState.WalkingToShelf;
            Vector3 shelfPos = _targetShelf.View.CustomerStandPoint.position;
            _view.MoveTo(shelfPos);

            float walkElapsed = 0f;
            while (!_view.HasReachedDestination() && walkElapsed < 6.0f)
            {
                walkElapsed += Time.deltaTime;
                yield return null;
            }

            // 3. Pick Item
            _model.CurrentState = CustomerState.TakingItem;
            _targetShelf.SetOccupied(true);
            yield return new WaitForSeconds(0.4f);

            if (_targetShelf.TryTakeProduct())
            {
                _model.AddItems(1, _targetShelf.Model.ItemPrice);
            }

            _targetShelf.SetOccupied(false);

            // 4. Join Checkout Queue
            _model.CurrentState = CustomerState.WalkingToCheckout;

            Vector3 queuePosition;
            while (!_targetCheckout.TryJoinQueue(this, out queuePosition))
            {
                yield return new WaitForSeconds(0.3f);
            }

            _view.MoveTo(queuePosition);

            _onQueueShiftAction = () =>
            {
                if (this != null && _view != null && _targetCheckout != null && _targetCheckout.View != null)
                {
                    Vector3 nextSlot = _targetCheckout.GetSlotPositionForCustomer(this);
                    _view.MoveTo(nextSlot);
                }
            };
            _targetCheckout.OnQueueShifted += _onQueueShiftAction;

            // 5. Wait in Queue Until Becoming 1st in Line
            _model.CurrentState = CustomerState.WaitingForPayment;

            while (!_targetCheckout.QueueModel.IsFirstInLine(this))
            {
                yield return new WaitForSeconds(0.2f);
            }

            // Move to counter
            if (_targetCheckout.View != null)
            {
                _view.MoveTo(_targetCheckout.View.PaymentStandPoint.position);
            }

            while (!_view.HasReachedDestination())
            {
                yield return null;
            }

            // Wait until cashier is ready
            while (_targetCheckout.IsBusy)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // 6. Payment
            bool isPaymentFinished = false;
            StartCoroutine(_targetCheckout.ProcessPaymentRoutine(
                _model.ItemsInBasket,
                _model.PricePerItem,
                () => isPaymentFinished = true
            ));

            while (!isPaymentFinished)
            {
                yield return null;
            }

            // Unsubscribe and leave queue
            if (_onQueueShiftAction != null)
            {
                _targetCheckout.OnQueueShifted -= _onQueueShiftAction;
                _onQueueShiftAction = null;
            }
            _targetCheckout.LeaveQueue(this);
            _model.ClearBasket();

            // 7. Walk to Exit
            _model.CurrentState = CustomerState.WalkingToExit;
            if (_exitPoint != null)
            {
                _view.MoveTo(_exitPoint.position);
                walkElapsed = 0f;
                while (!_view.HasReachedDestination() && walkElapsed < 8.0f)
                {
                    walkElapsed += Time.deltaTime;
                    yield return null;
                }
            }

            // 8. Cleanup
            _model.CurrentState = CustomerState.Completed;
            Destroy(gameObject);
        }

        private Vector3 GetRandomNavMeshPoint(Vector3 origin, float radius)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += origin;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, NavMesh.AllAreas))
            {
                return hit.position;
            }

            return origin;
        }

        private void OnDestroy()
        {
            if (_targetCheckout != null && _onQueueShiftAction != null)
            {
                _targetCheckout.OnQueueShifted -= _onQueueShiftAction;
                _onQueueShiftAction = null;
            }

            if (_targetShelf != null && _model != null && _model.CurrentState == CustomerState.TakingItem)
            {
                _targetShelf.SetOccupied(false);
            }

            if (_targetCheckout != null)
            {
                _targetCheckout.LeaveQueue(this);
            }
        }
    }
}