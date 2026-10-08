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

        public int ActiveShelvesCount => _activeShelves.Count;
        public int ActiveCheckoutsCount => _activeCheckouts.Count;

        public void ClearAll()
        {
            _activeShelves.Clear();
            _activeCheckouts.Clear();
        }

        public void RegisterShelf(ShelfController shelf)
        {
            if (shelf == null || shelf.View == null) return;

            // Check if already registered
            if (!_activeShelves.Exists(s => s.View == shelf.View))
            {
                _activeShelves.Add(shelf);
                Debug.Log($"[MarketRegistry] Registered NEW shelf: '{shelf.View.gameObject.name}'. Total active: {_activeShelves.Count}");
            }
        }

        public void RegisterCheckout(CheckoutController checkout)
        {
            if (checkout == null || checkout.View == null) return;

            if (!_activeCheckouts.Exists(c => c.View == checkout.View))
            {
                _activeCheckouts.Add(checkout);
                Debug.Log($"[MarketRegistry] Registered NEW checkout: '{checkout.View.gameObject.name}'. Total active: {_activeCheckouts.Count}");
            }
        }

        public ShelfController GetOptimalShelf()
        {
            // Filter only shelves whose GameObjects are actively enabled in hierarchy
            var available = _activeShelves.FindAll(s => s.View != null && s.View.gameObject.activeInHierarchy && s.Model.HasStock);

            if (available.Count == 0)
            {
                // Fallback to any active shelf
                available = _activeShelves.FindAll(s => s.View != null && s.View.gameObject.activeInHierarchy);
            }

            if (available.Count == 0) return null;

            // Prioritize unoccupied shelf
            var free = available.FindAll(s => !s.IsOccupied);
            if (free.Count > 0)
            {
                return free[Random.Range(0, free.Count)];
            }

            return available[Random.Range(0, available.Count)];
        }

        public ShelfController GetRandomAvailableShelf()
        {
            return GetOptimalShelf();
        }

        public CheckoutController GetLessBusyCheckout()
        {
            var available = _activeCheckouts.FindAll(c => c.View != null && c.View.gameObject.activeInHierarchy);
            if (available.Count == 0) return null;

            var freeCheckout = available.Find(c => !c.IsBusy);
            if (freeCheckout != null) return freeCheckout;

            return available[Random.Range(0, available.Count)];
        }
    }
}