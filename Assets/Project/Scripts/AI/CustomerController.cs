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
            // 1. Moving towards the shelf containing the goods

            _model.CurrentState = CustomerState.WalkingToShelf;
            _view.MoveTo(_targetShelf.View.CustomerStandPoint.position);

            while (!_view.HasReachedDestination()) 
            { 
                yield return null;
            }

            // 2. Collection of goods

            _model.CurrentState = CustomerState.TakingItem;
            yield return new WaitForSeconds(0.5f);

            if (_targetShelf.TryTakeProduct())
            {
                _model.AddItems(1, _targetShelf.Model.ItemPrice);
            }

            // 3. Head to the cashier

            _model.CurrentState = CustomerState.WalkingToCheckout;
            _view.MoveTo(_targetCheckout.View.PaymentStandPoint.position);

            while (!_view.HasReachedDestination())
            {
                yield return null;
            }

            // 4.Waiting for a free cashier

            _model.CurrentState = CustomerState.WaitingForPayment;
            while (_targetCheckout.IsBusy)
            {
                yield return new WaitForSeconds(0.2f);
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

            _model.ClearBasket();


            // 6. Leaving the market

            _model.CurrentState = CustomerState.WalkingToExit;
            if (_exitPoint != null) 
            { 
                _view.MoveTo(_exitPoint.position);
                while (!_view.HasReachedDestination()) 
                {
                    yield return null;
                }
            }

            // 7. Deleting a customer

            _model.CurrentState = CustomerState.Completed;
            Destroy(gameObject);
        }
    }
}