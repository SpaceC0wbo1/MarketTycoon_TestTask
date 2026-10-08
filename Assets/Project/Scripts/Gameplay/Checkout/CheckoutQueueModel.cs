using System;
using System.Collections.Generic;
using MarketTycoon.Gameplay.AI;

namespace MarketTycoon.Gameplay.Checkout
{

    public class CheckoutQueueModel
    {
        private readonly int _maxCapacity;
        private readonly List<CustomerController> _queuedCustomers = new List<CustomerController>();

        public int Count => _queuedCustomers.Count;
        public int MaxCapacity => _maxCapacity;
        public bool IsFull => _queuedCustomers.Count >= _maxCapacity;

        public event Action OnQueueUpdated;

        public CheckoutQueueModel(int maxCapacity = 5)
        {
            _maxCapacity = maxCapacity;
        }

        public bool TryEnqueue(CustomerController customer, out int assignedIndex)
        {
            if (IsFull || _queuedCustomers.Contains(customer))
            {
                assignedIndex = -1;
                return false;
            }

            _queuedCustomers.Add(customer);
            assignedIndex = _queuedCustomers.Count - 1;
            OnQueueUpdated?.Invoke();
            return true;
        }

        public void Dequeue(CustomerController customer)
        {
            if (_queuedCustomers.Remove(customer))
            {
                OnQueueUpdated?.Invoke();
            }
        }

        public int GetCustomerIndex(CustomerController customer)
        {
            return _queuedCustomers.IndexOf(customer);
        }

        public bool IsFirstInLine(CustomerController customer)
        {
            return _queuedCustomers.Count > 0 && _queuedCustomers[0] == customer;
        }
    }
}