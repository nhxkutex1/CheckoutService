using CheckoutService_TestTask.Enums;

namespace CheckoutService_TestTask.Models;

public class Order
{
    public List<OrderItem> Items { get; set; } = new List<OrderItem>();

    public Order(List<OrderItem> Items)
    {
        foreach (var item in Items)
        {
            AddOrderItem(item.ItemType, item.Quantity, item.OrderTime);
        }
    }

    public void AddOrderItem(ItemType type, int quantity, DateTime time)
    {
        Items.Add(new OrderItem(type, quantity, time));
    }

    public void RemoveOrderItemEachByAmount(int amount)
    {
        Items[0].Quantity -= amount; // Remove starter
        Items[1].Quantity -= amount; // Remove main
        Items[2].Quantity -= amount; // Remove drink
    }

    public double GetFoodTotal()
    {
        return Items.Where(i => i.ItemType == ItemType.Starter || i.ItemType == ItemType.Main)
                    .Sum(i => i.GetItemPrice() * i.Quantity);
    }

    public double GetDrinkTotal()
    {
        return Items.Where(i => i.ItemType == ItemType.Drink)
                    .Sum(i => i.GetItemPrice() * i.Quantity);
    }

    public double GetServiceCharge()
    {
        return GetFoodTotal() * 0.1;
    }

    public double GetTotal()
    {
        return GetFoodTotal() + GetDrinkTotal() + GetServiceCharge();
    }

}
