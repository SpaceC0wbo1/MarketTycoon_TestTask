namespace MarketTycoon.Gameplay.AI
{
    public enum CustomerState
    {
        Spawning,
        Browsing,
        WalkingToShelf,
        TakingItem,
        WalkingToCheckout,
        WaitingForPayment,
        WalkingToExit,
        Completed
    }

    public class CustomerModel
    {
        public CustomerState CurrentState { get; set; } = CustomerState.Spawning;
        public int ItemsInBasket { get; private set; }
        public int PricePerItem { get; private set; }

        public void AddItems(int count, int price)
        {
            ItemsInBasket += count;
            PricePerItem = price;
        }

        public void ClearBasket()
        {
            ItemsInBasket = 0;
            PricePerItem = 0;
        }
    }
}