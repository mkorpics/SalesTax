using API.Contracts.Business;
using API.Contracts.Utilities;
using API.DbContext;
using API.Models;
using System;
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

        public bool ShoppingCartItemExists(Func<PurchaseItem, bool> predicate)
        {
            return SalesTaxDbContext.PurchaseItems.Any(predicate);
        }

        public IEnumerable<PurchaseItem> GetShoppingCartItems()
        {
            return SalesTaxDbContext.PurchaseItems.Where(x => x.IsInShoppingCart);
        }

        public PurchaseItem UpsertInventoryItemInShoppingCart(int inventoryItemId, bool increaseCount)
        {
            var purchaseItem = SalesTaxDbContext.PurchaseItems.FirstOrDefault(x => x.InventoryItemId == inventoryItemId && x.IsInShoppingCart);

            if (increaseCount)
            {
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
            }
            else
            {
                if (purchaseItem == null) { return null; }
                else if (purchaseItem.Quantity == 1)
                {
                    SalesTaxDbContext.PurchaseItems.Remove(purchaseItem);
                    return null;
                }
                else
                {
                    purchaseItem.Quantity--;
                }
            }

            purchaseItem.TotalPrice = purchaseItem.InventoryItem.TotalPrice * purchaseItem.Quantity;

            return purchaseItem;
        }

        public void DeleteShoppingCartItem(int itemId)
        {
            var item = SalesTaxDbContext.PurchaseItems.FirstOrDefault(x => x.PurchaseItemId == itemId && x.IsInShoppingCart);
            SalesTaxDbContext.PurchaseItems.Remove(item);
        }
    }
}
