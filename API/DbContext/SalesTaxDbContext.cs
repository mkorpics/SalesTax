using API.Models;
using System.Collections.Generic;

namespace API.DbContext
{
    public class SalesTaxDbContext
    {
        public static List<InventoryItem> InventoryItems = new List<InventoryItem>();
        public static List<PurchaseItem> PurchaseItems = new List<PurchaseItem>();
        public static List<Order> Orders = new List<Order>();

        public static List<ItemType> ItemTypes = new List<ItemType>()
        {
            new ItemType() { ItemTypeId = 1, ItemTypeName = "Book", HasBasicSalesTax = false, IsImported = false },
            new ItemType() { ItemTypeId = 2, ItemTypeName = "Music CD", HasBasicSalesTax = true, IsImported = false },
            new ItemType() { ItemTypeId = 3, ItemTypeName = "Chocolate bar", HasBasicSalesTax = false, IsImported = false },
            new ItemType() { ItemTypeId = 4, ItemTypeName = "Imported box of chocolates", HasBasicSalesTax = false, IsImported = true },
            new ItemType() { ItemTypeId = 5, ItemTypeName = "Imported bottle of perfume", HasBasicSalesTax = true, IsImported = true },
            new ItemType() { ItemTypeId = 6, ItemTypeName = "Bottle of perfume", HasBasicSalesTax = true, IsImported = false },
            new ItemType() { ItemTypeId = 7, ItemTypeName = "Packet of headache pills", HasBasicSalesTax = false, IsImported = false },
        };
    }
}
