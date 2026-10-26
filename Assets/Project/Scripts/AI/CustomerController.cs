using System.Collections;
using System.Collections.Generic;
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
            // -------------------------------------------------------------
            // 1. Browsing strictly inside store bounds
            // -------------------------------------------------------------
            if (_browseDuration > 0.1f && _wanderBoundsList != null && _wanderBoundsList.Count > 0)
            {
                _model.CurrentState = CustomerState.Browsing;
                float browseTimer = 0f;

                while (browseTimer < _browseDuration)
                {
                    Bounds chosenBounds = _wanderBoundsList[Random.Range(0, _wanderBoundsList.Count)];
                    Vector3 wanderTarget = GetSafePointInBounds(chosenBounds);
                    _view.MoveTo(wanderTarget);

                    yield return null;

                    float wanderWaitTime = Random.Range(1.5f, 3.0f);
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

            // -------------------------------------------------------------
            // 2. Walk to Shelf
            // -------------------------------------------------------------
            _model.CurrentState = CustomerState.WalkingToShelf;
            Vector3 shelfPos = _targetShelf.View.CustomerStandPoint.position;
            _view.MoveTo(shelfPos);

            yield return null;

            float walkElapsed = 0f;
            while (!_view.HasReachedDestination() && walkElapsed < 8.0f)
            {
                walkElapsed += Time.deltaTime;
                yield return null;
            }

            // -------------------------------------------------------------
            // 3. Pick Item
            // -------------------------------------------------------------
            _model.CurrentState = CustomerState.TakingItem;
            _targetShelf.SetOccupied(true);

            Vector3 shelfDir = _targetShelf.View.transform.position - transform.position;
            shelfDir.y = 0f;
            if (shelfDir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(shelfDir);
            }

            _view.PlayInteract();
            yield return new WaitForSeconds(0.6f);

            if (_targetShelf.TryTakeProduct())
            {
                _model.AddItems(1, _targetShelf.Model.ItemPrice);
            }

            _targetShelf.SetOccupied(false);
            _view.ResetLocomotion();

            // -------------------------------------------------------------
            // 4. Join Checkout Queue
            // -------------------------------------------------------------
            _model.CurrentState = CustomerState.WalkingToCheckout;

            Vector3 queuePosition;
            while (!_targetCheckout.TryJoinQueue(this, out queuePosition))
            {
                yield return new WaitForSeconds(0.3f);
            }

            // Subscribe immediately so queue shifts are not missed while en route
            _onQueueShiftAction = () =>
            {
                if (this != null && _view != null && _targetCheckout != null && _targetCheckout.View != null)
                {
                    // Reposition if walking towards the queue line or currently waiting in line
                    if (_model.CurrentState == CustomerState.WalkingToCheckout ||
                        _model.CurrentState == CustomerState.WaitingForPayment)
                    {
                        Vector3 nextSlot = _targetCheckout.GetSlotPositionForCustomer(this);
                        _view.MoveTo(nextSlot);
                    }
                }
            };
            _targetCheckout.OnQueueShifted += _onQueueShiftAction;

            // Move to the assigned slot in the queue
            _view.MoveTo(queuePosition);
            yield return null;

            walkElapsed = 0f;
            while (!_view.HasReachedDestination() && walkElapsed < 10.0f)
            {
                walkElapsed += Time.deltaTime;
                yield return null;
            }

            // -------------------------------------------------------------
            // 5. Wait in Queue Until 1st in Line AND Cashier is Ready
            // -------------------------------------------------------------
            _model.CurrentState = CustomerState.WaitingForPayment;

            // Wait in the designated line slot until becoming first and cashier is free
            while (!_targetCheckout.QueueModel.IsFirstInLine(this) || _targetCheckout.IsBusy)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Step forward from queue slot to the payment counter
            if (_targetCheckout.View != null)
            {
                _view.MoveTo(_targetCheckout.View.PaymentStandPoint.position);
                yield return null;

                walkElapsed = 0f;
                while (!_view.HasReachedDestination() && walkElapsed < 4.0f)
                {
                    walkElapsed += Time.deltaTime;
                    yield return null;
                }
            }

            // -------------------------------------------------------------
            // 6. Payment & Releasing the Queue Slot
            // -------------------------------------------------------------
            if (_targetCheckout.View != null)
            {
                Vector3 cashierDir = _targetCheckout.View.transform.position - transform.position;
                cashierDir.y = 0f;
                if (cashierDir != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(cashierDir);
                }
            }

            _view.PlayInteract();

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

            _view.ResetLocomotion();

            // Unsubscribe from queue updates before departing
            if (_onQueueShiftAction != null)
            {
                _targetCheckout.OnQueueShifted -= _onQueueShiftAction;
                _onQueueShiftAction = null;
            }

            _targetCheckout.LeaveQueue(this);
            _model.ClearBasket();

            // -------------------------------------------------------------
            // 7. Walk to Exit
            // -------------------------------------------------------------
            _model.CurrentState = CustomerState.WalkingToExit;
            if (_exitPoint != null)
            {
                _view.MoveTo(_exitPoint.position);
                yield return null;

                walkElapsed = 0f;
                while (!_view.HasReachedDestination() && walkElapsed < 9.0f)
                {
                    walkElapsed += Time.deltaTime;
                    yield return null;
                }
            }

            // -------------------------------------------------------------
            // 8. Cleanup
            // -------------------------------------------------------------
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