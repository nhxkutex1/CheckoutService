using CheckoutService_TestTask.Enums;

namespace CheckoutService_TestTask.Models;

public class OrderItem
{
    public ItemType ItemType { get; set; }
    public int Quantity { get; set; }
    public DateTime OrderTime { get; set; }

    public OrderItem(ItemType type, int quantity, DateTime time)
    {
        ItemType = type;
        Quantity = quantity;
        OrderTime = time;
    }

    public double GetItemPrice()
    {
        switch (ItemType)
        {
            case ItemType.Starter: return 4.0;
            case ItemType.Main: return 7.0;
            case ItemType.Drink: return OrderTime.Hour < 19 ? 2.5 * 0.7 : 2.5;
            default: return 0;
        }
    }
}
