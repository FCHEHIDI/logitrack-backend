using System;

namespace LogiTrack.Models
{
    public class Order
    {
        /// <summary>
        /// For cart-like or temporary order data, this field can store a session ID or user token.
        /// </summary>
        public string? SessionId { get; set; }
        [System.ComponentModel.DataAnnotations.Key]
        public int OrderId { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public required string CustomerName { get; set; }

        public DateTime DatePlaced { get; set; }



        // Navigation property for one-to-many relationship
        public List<InventoryItem> Items { get; set; } = new();

        public void DisplayOrderInfo()
        {
            // Note: CustomerId, OrderDate, and TotalAmount are not defined in this class.
            // Adjusted to use available properties.
            Console.WriteLine($"Order ID: {OrderId} | Customer Name: {CustomerName} | Order Date: {DatePlaced}");
        }

        public void AddItem(InventoryItem item)
        {
            if (Items == null)
            {
                Items = new List<InventoryItem>();
            }
            Items.Add(item);
        }

        public void RemoveItem(InventoryItem item)
        {
            if (Items != null && Items.Contains(item))
            {
                Items.Remove(item);
            }
        }

        public void GetOrderSummary()
        {
            int itemCount = Items != null ? Items.Count : 0;
            string summary = $"Order #{OrderId} for {CustomerName} | Items: {itemCount} | Placed: {DatePlaced:MM/dd/yyyy}";
            Console.WriteLine(summary);
        }
    }
}
