using API.Models;
using System.Collections.Generic;

namespace API.Contracts.Business
{
    public interface IShoppingCartItemBusiness
    {
        bool ShoppingCartItemExists(int itemId);
        IEnumerable<PurchaseItem> GetShoppingCartItems();
        PurchaseItem AddInventoryItemToShoppingCart(int inventoryItemId);
        //PurchaseItem CreateShoppingCartItem(InventoryItemInput itemInput);
        void DeleteShoppingCartItem(int itemId);
    }
}
