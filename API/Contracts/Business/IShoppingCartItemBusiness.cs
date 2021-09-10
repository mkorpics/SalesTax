using API.Models;
using System;
using System.Collections.Generic;

namespace API.Contracts.Business
{
    public interface IShoppingCartItemBusiness
    {
        bool ShoppingCartItemExists(int itemId);
        bool ShoppingCartItemExists(Func<PurchaseItem, bool> predicate);
        IEnumerable<PurchaseItem> GetShoppingCartItems();
        PurchaseItem UpsertInventoryItemInShoppingCart(int inventoryItemId, bool increaseCount);
        void DeleteShoppingCartItem(int itemId);
    }
}
