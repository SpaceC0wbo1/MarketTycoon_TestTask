using System.Collections;
using MarketTycoon.Gameplay.Checkout;
using MarketTycoon.Gameplay.Shelf;
using UnityEngine;

namespace MarketTycoon.Gameplay.AI
{
    public class CustomerController : MonoBehaviour
    {
        private CustomerModel _model;
        private CustomerView _view;
        private ShelfController _targetShelf;
        private CheckoutController _targetCheckout;
        private Transform _exitPoint;

        public void Initialize(CustomerView view, ShelfController shelf, CheckoutController checkout, Transform exitPoint)
        {
            _model = new CustomerModel();
            _view = view;
            _targetShelf = shelf;
            _targetCheckout = checkout;
            _exitPoint = exitPoint;

            StartCoroutine(CustomerLifecycleRoutine());
        }

        private IEnumerator CustomerLifecycleRoutine()
        {
            // 1. Walk to Shelf
            _model.CurrentState = CustomerState.WalkingToShelf;
            Vector3 shelfPos = _targetShelf.View.CustomerStandPoint.position;
            _view.MoveTo(shelfPos);

            float elapsed = 0f;
            while (!_view.HasReachedDestination() && elapsed < 4.5f)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            // 2. Pick Item
            _model.CurrentState = CustomerState.TakingItem;
            _targetShelf.SetOccupied(true);
            yield return new WaitForSeconds(0.4f);

            if (_targetShelf.TryTakeProduct())
            {
                _model.AddItems(1, _targetShelf.Model.ItemPrice);
            }

            _targetShelf.SetOccupied(false);

            // 3. Join Checkout Queue
            _model.CurrentState = CustomerState.WalkingToCheckout;

            // Wait until slot in queue is available
            Vector3 queuePosition;
            while (!_targetCheckout.TryJoinQueue(this, out queuePosition))
            {
                yield return new WaitForSeconds(0.3f);
            }

            _view.MoveTo(queuePosition);

            // Listen to queue shifts so customer steps forward when the line moves
            System.Action onQueueShift = () =>
            {
                if (this != null && _view != null && _targetCheckout != null)
                {
                    Vector3 nextSlot = _targetCheckout.GetSlotPositionForCustomer(this);
                    _view.MoveTo(nextSlot);
                }
            };
            _targetCheckout.OnQueueShifted += onQueueShift;

            // 4. Wait in Queue Until Becoming 1st in Line
            _model.CurrentState = CustomerState.WaitingForPayment;

            while (!_targetCheckout.QueueModel.IsFirstInLine(this))
            {
                yield return new WaitForSeconds(0.2f);
            }

            // Once 1st in line, move directly to cash register if not there yet
            _view.MoveTo(_targetCheckout.View.PaymentStandPoint.position);
            while (!_view.HasReachedDestination())
            {
                yield return null;
            }

            // Wait until previous payment finishes
            while (_targetCheckout.IsBusy)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // 5. Payment
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

            // Leave the queue and notify waiting customers behind to step forward
            _targetCheckout.OnQueueShifted -= onQueueShift;
            _targetCheckout.LeaveQueue(this);
            _model.ClearBasket();

            // 6. Walk to Exit
            _model.CurrentState = CustomerState.WalkingToExit;
            if (_exitPoint != null)
            {
                _view.MoveTo(_exitPoint.position);
                elapsed = 0f;
                while (!_view.HasReachedDestination() && elapsed < 7.0f)
                {
                    elapsed += Time.deltaTime;
                    yield return null;
                }
            }

            // 7. Cleanup
            _model.CurrentState = CustomerState.Completed;
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
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