using System;
using System.Collections;
using MarketTycoon.Core.Currency;
using MarketTycoon.Gameplay.AI;
using UnityEngine;

namespace MarketTycoon.Gameplay.Checkout
{
    public class CheckoutController
    {
        private readonly CheckoutModel _model;
        private readonly CheckoutView _view;
        private readonly CheckoutQueueModel _queueModel;
        private readonly ICurrencyService _currencyService;
        private bool _isBusy;

        public CheckoutView View => _view;
        public CheckoutQueueModel QueueModel => _queueModel;
        public bool IsBusy => _isBusy;

        public event Action OnQueueShifted;

        public CheckoutController(CheckoutModel model, CheckoutView view, ICurrencyService currencyService)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _currencyService = currencyService ?? throw new ArgumentNullException(nameof(currencyService));

            _queueModel = new CheckoutQueueModel(maxCapacity: 6);
            _queueModel.OnQueueUpdated += () => OnQueueShifted?.Invoke();
        }

        public bool TryJoinQueue(CustomerController customer, out Vector3 targetPosition)
        {
            if (_queueModel.TryEnqueue(customer, out int slotIndex))
            {
                targetPosition = _view.GetQueueSlotPosition(slotIndex);
                return true;
            }

            targetPosition = Vector3.zero;
            return false;
        }

        public Vector3 GetSlotPositionForCustomer(CustomerController customer)
        {
            int index = _queueModel.GetCustomerIndex(customer);
            if (index < 0) return _view.PaymentStandPoint.position;
            return _view.GetQueueSlotPosition(index);
        }

        public void LeaveQueue(CustomerController customer)
        {
            _queueModel.Dequeue(customer);
        }

        public IEnumerator ProcessPaymentRoutine(int itemsCount, int itemPrice, Action onPaymentCompleted)
        {
            _isBusy = true;

            yield return new WaitForSeconds(_model.ServiceDuration);

            int totalProfit = itemsCount * itemPrice;
            _currencyService.Add(totalProfit);

            _isBusy = false;
            onPaymentCompleted?.Invoke();
        }
    }
}