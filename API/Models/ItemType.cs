namespace API.Models
{
    public class ItemType
    {
        public int ItemTypeId { get; set; }
        public string ItemTypeName { get; set; }
        public bool HasBasicSalesTax { get; set; }
        public bool IsImported { get; set; }
    }
}
