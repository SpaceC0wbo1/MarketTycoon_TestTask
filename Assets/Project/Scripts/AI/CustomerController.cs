using System.Collections.Generic;
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

        private List<Bounds> _wanderBoundsList;
        private float _browseDuration;

        public void Initialize(
            CustomerView view,
            ShelfController shelf,
            CheckoutController checkout,
            Transform exitPoint,
            List<Bounds> wanderBoundsList,
            float minBrowseTime = 2.0f,
            float maxBrowseTime = 5.0f)
        {
            _model = new CustomerModel();
            _view = view;
            _targetShelf = shelf;
            _targetCheckout = checkout;
            _exitPoint = exitPoint;

            _wanderBoundsList = wanderBoundsList;
            _browseDuration = Random.Range(minBrowseTime, maxBrowseTime);

            StartCoroutine(CustomerLifecycleRoutine());
        }

        private IEnumerator CustomerLifecycleRoutine()
        {
            // 1. Browsing strictly inside any of the unlocked store zones
            if (_browseDuration > 0.1f && _wanderBoundsList != null && _wanderBoundsList.Count > 0)
            {
                _model.CurrentState = CustomerState.Browsing;
                float browseTimer = 0f;

                while (browseTimer < _browseDuration)
                {
                    // Pick a random unlocked room/zone
                    Bounds chosenBounds = _wanderBoundsList[Random.Range(0, _wanderBoundsList.Count)];
                    Vector3 wanderTarget = GetSafePointInBounds(chosenBounds);
                    _view.MoveTo(wanderTarget);

                    float wanderWaitTime = Random.Range(1.4f, 2.8f);
                    float elapsed = 0f;

                    while (!_view.HasReachedDestination() && elapsed < wanderWaitTime)
                    {
                        elapsed += Time.deltaTime;
                        browseTimer += Time.deltaTime;
                        yield return null;
                    }

                    float inspectDelay = Random.Range(0.6f, 1.2f);
                    yield return new WaitForSeconds(inspectDelay);
                    browseTimer += inspectDelay;
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

            if (_targetCheckout.View != null)
            {
                _view.MoveTo(_targetCheckout.View.PaymentStandPoint.position);
            }

            while (!_view.HasReachedDestination())
            {
                yield return null;
            }

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

        private Vector3 GetSafePointInBounds(Bounds bounds)
        {
            float padding = 0.8f;
            float minX = bounds.min.x + padding;
            float maxX = bounds.max.x - padding;
            float minZ = bounds.min.z + padding;
            float maxZ = bounds.max.z - padding;

            if (minX >= maxX) { minX = bounds.min.x; maxX = bounds.max.x; }
            if (minZ >= maxZ) { minZ = bounds.min.z; maxZ = bounds.max.z; }

            Vector3 randomPoint = new Vector3(
                Random.Range(minX, maxX),
                bounds.center.y,
                Random.Range(minZ, maxZ)
            );

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 3.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }

            return bounds.center;
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