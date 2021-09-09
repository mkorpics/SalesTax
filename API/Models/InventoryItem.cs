namespace API.Models
{
    public class InventoryItem
    {
        public int InventoryItemId { get; set; }
        public string InventoryItemName { get; set; }
        public int ItemTypeId { get; set; }
        public decimal Price { get; set; }
        public decimal? SalesTax { get; set; }
        public decimal? TotalPrice { get; set; }
        public bool CanDelete { get; set; }

        public ItemType ItemType { get; set; }
    }
}
