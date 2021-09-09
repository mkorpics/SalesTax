using API.Models;
using API.Models.InputModels;
using System;
using System.Collections.Generic;

namespace API.Contracts.Business
{
    public interface IInventoryBusiness
    {
        bool InventoryItemExists(int itemId);
        bool InventoryItemInUse(int itemId);
        IEnumerable<InventoryItem> GetInventoryItems();
        InventoryItem CreateInventoryItem(InventoryItemInput itemInput);
        void DeleteInventoryItem(int itemId);
    }
}
