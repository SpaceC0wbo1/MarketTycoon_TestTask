using System.Collections.Generic;
using MarketTycoon.Gameplay.Checkout;
using MarketTycoon.Gameplay.Shelf;
using UnityEngine;

namespace MarketTycoon.Gameplay
{
    public class MarketRegistry : MonoBehaviour
    {
        private readonly List<ShelfController> _activeShelves = new List<ShelfController>();
        private readonly List<CheckoutController> _activeCheckouts = new List<CheckoutController>();

        public void RegisterShelf(ShelfController shelf)
        {
            if (!_activeShelves.Contains(shelf))
            {
                _activeShelves.Add(shelf);
            }
        }

        public void RegisterCheckout(CheckoutController checkout)
        {
            if (!_activeCheckouts.Contains(checkout))
            {
                _activeCheckouts.Add(checkout);
            }
        }

        public ShelfController GetRandomAvailableShelf()
        {
            if (_activeShelves.Count == 0) return null;

            var available = _activeShelves.FindAll(s => s.Model.HasStock);
            if (available.Count > 0)
            {
                return available[Random.Range(0, available.Count)];
            }

            return _activeShelves[Random.Range(0, _activeShelves.Count)];
        }

        public CheckoutController GetLessBusyCheckout()
        {
            if (_activeCheckouts.Count == 0) return null;

            var freeCheckout = _activeCheckouts.Find(c => !c.IsBusy);
            if (freeCheckout != null) return freeCheckout;

            return _activeCheckouts[Random.Range(0, _activeCheckouts.Count)];
        }
    }
}