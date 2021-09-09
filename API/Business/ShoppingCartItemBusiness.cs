using API.Contracts.Business;
using API.Contracts.Utilities;
using API.DbContext;
using API.Models;
using System.Collections.Generic;
using System.Linq;

namespace API.Business
{
    public class ShoppingCartItemBusiness : IShoppingCartItemBusiness
    {
        private IDataUtility _dataUtility;

        public ShoppingCartItemBusiness(IDataUtility dataUtility)
        {
            _dataUtility = dataUtility;
        }

        public bool ShoppingCartItemExists(int itemId)
        {
            return SalesTaxDbContext.PurchaseItems.Any(x => x.PurchaseItemId == itemId && x.IsInShoppingCart);
        }

        public IEnumerable<PurchaseItem> GetShoppingCartItems()
        {
            return SalesTaxDbContext.PurchaseItems.Where(x => x.IsInShoppingCart);
        }

        public PurchaseItem AddInventoryItemToShoppingCart(int inventoryItemId)
        {
            var purchaseItem = SalesTaxDbContext.PurchaseItems.FirstOrDefault(x => x.InventoryItemId == inventoryItemId && x.IsInShoppingCart);
            if (purchaseItem == null)
            {
                var inventoryItem = SalesTaxDbContext.InventoryItems.FirstOrDefault(x => x.InventoryItemId == inventoryItemId);

                purchaseItem = new PurchaseItem()
                {
                    PurchaseItemId = _dataUtility.GenerateNextId(SalesTaxDbContext.PurchaseItems, x => x.PurchaseItemId),
                    InventoryItemId = inventoryItemId,
                    InventoryItem = inventoryItem,
                    Quantity = 1,
                    OrderId = null
                };
                SalesTaxDbContext.PurchaseItems.Add(purchaseItem);
            }
            else
            {
                purchaseItem.Quantity++;
            }

            purchaseItem.TotalPrice = purchaseItem.InventoryItem.TotalPrice * purchaseItem.Quantity;

            return purchaseItem;
        }

        //public PurchaseItem CreateShoppingCartItem(InventoryItemInput itemInput)
        //{
        //    var itemType = SalesTaxDbContext.ItemTypes.FirstOrDefault(x => x.ItemTypeId == itemInput.ItemTypeId);

        //    var createdItem = new PurchaseItem()
        //    {
        //        ItemId = GenerateNextId(SalesTaxDbContext.Items, x => x.ItemId),
        //        ItemName = itemInput.ItemName,
        //        ItemTypeId = itemInput.ItemTypeId,
        //        Price = itemInput.Price,
        //        SalesTax = CalculateSalesTax(itemInput.Price, itemType),
        //        ItemType = itemType,
        //        OrderId = null,
        //    };

        //    SalesTaxDbContext.Items.Add(createdItem);
        //    return createdItem;
        //}

        public void DeleteShoppingCartItem(int itemId)
        {
            var item = SalesTaxDbContext.PurchaseItems.FirstOrDefault(x => x.PurchaseItemId == itemId && x.IsInShoppingCart);
            SalesTaxDbContext.PurchaseItems.Remove(item);
        }
    }
}
