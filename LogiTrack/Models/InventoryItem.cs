using System;

namespace LogiTrack.Models
{
    public class InventoryItem
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int ItemId { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public required string Name { get; set; }

        public int Quantity { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public required string Location { get; set; }

        // Foreign key for Order
        public int? OrderId { get; set; }

        // Navigation property
        public Order? Order { get; set; }

        // This class represents an inventory item with properties for the item ID, name, quantity, and location.

        public void DisplayInfo()
        {
            Console.WriteLine($"Item: {Name} | Quantity: {Quantity} | Location: {Location}");
        }
    }
}