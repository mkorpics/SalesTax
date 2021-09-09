namespace API.Models
{
    public class PurchaseItem
    {
        public int PurchaseItemId { get; set; }
        public int InventoryItemId { get; set; }
        public int Quantity { get; set; }
        public int? OrderId { get; set; }
        public decimal? TotalPrice { get; set; }

        public InventoryItem InventoryItem { get; set; }

        public bool IsInShoppingCart
        {
            get { return OrderId is null; }
        }
    }
}
