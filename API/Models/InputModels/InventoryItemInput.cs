namespace API.Models.InputModels
{
    public class InventoryItemInput
    {
        public string InventoryItemName { get; set; }
        public int ItemTypeId { get; set; }
        public decimal Price { get; set; }
    }
}
