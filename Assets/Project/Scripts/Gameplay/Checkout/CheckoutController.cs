using System;
using System.Collections;
using MarketTycoon.Core.Currency;
using Unity;
using UnityEngine;

namespace MarketTycoon.Gameplay.Checkout
{
    public class CheckoutController
    {
        private readonly CheckoutModel _model;
        private readonly CheckoutView _view;
        private readonly ICurrencyService _currencyService;
        private bool _isBusy;

        public CheckoutView View => _view;
        public bool IsBusy => _isBusy;

        public CheckoutController(CheckoutModel model, CheckoutView view, ICurrencyService currencyService)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _currencyService = currencyService ?? throw new ArgumentNullException(nameof(currencyService));
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