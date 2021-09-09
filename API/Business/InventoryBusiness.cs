using API.Contracts.Business;
using API.Contracts.Utilities;
using API.DbContext;
using API.Models;
using API.Models.InputModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Business
{
    public class InventoryBusiness : IInventoryBusiness
    {
        private IDataUtility _dataUtility;

        public InventoryBusiness(IDataUtility dataUtility)
        {
            _dataUtility = dataUtility;
        }

        public bool InventoryItemExists(int itemId)
        {
            return SalesTaxDbContext.InventoryItems.Any(x => x.InventoryItemId == itemId);
        }

        public bool InventoryItemInUse(int itemId)
        {
            return SalesTaxDbContext.PurchaseItems.Any(x => x.InventoryItemId == itemId);
        }

        public IEnumerable<InventoryItem> GetInventoryItems()
        {
            var items = SalesTaxDbContext.InventoryItems;
            items.ForEach(x => x.CanDelete = !SalesTaxDbContext.PurchaseItems.Any(p => p.InventoryItemId == x.InventoryItemId));
            return items;
        }

        public InventoryItem CreateInventoryItem(InventoryItemInput itemInput)
        {
            var itemType = SalesTaxDbContext.ItemTypes.FirstOrDefault(x => x.ItemTypeId == itemInput.ItemTypeId);

            var createdItem = new InventoryItem()
            {
                InventoryItemId = _dataUtility.GenerateNextId(SalesTaxDbContext.InventoryItems, x => x.InventoryItemId),
                InventoryItemName = itemInput.InventoryItemName,
                ItemTypeId = itemInput.ItemTypeId,
                Price = itemInput.Price,
                SalesTax = CalculateSalesTax(itemInput.Price, itemType),
                ItemType = itemType,
                CanDelete = true,
            };
            createdItem.TotalPrice = createdItem.Price + createdItem.SalesTax;

            SalesTaxDbContext.InventoryItems.Add(createdItem);
            return createdItem;
        }

        public void DeleteInventoryItem(int itemId)
        {
            var item = SalesTaxDbContext.InventoryItems.FirstOrDefault(x => x.InventoryItemId == itemId);
            SalesTaxDbContext.InventoryItems.Remove(item);
        }

        private static decimal CalculateSalesTax(decimal price, ItemType itemType)
        {
            const decimal basicSalesTaxRate = .1m;
            const decimal importSalesTaxRate = .05m;

            decimal salesTax = 0;
            if (itemType.HasBasicSalesTax)
            {
                var basicSalesTax = basicSalesTaxRate * price;
                salesTax += basicSalesTax;
            }
            if (itemType.IsImported)
            {
                var importSalesTax = importSalesTaxRate * price;
                salesTax += importSalesTax;
            }
            // TODO: need?
            //var salesTaxRoundedToTwoPlaces = decimal.Round(salesTax, 2);
            var salesTaxRoundedUpToNearestFifthCent = Math.Ceiling(salesTax / .05m) * .05m;
            return salesTaxRoundedUpToNearestFifthCent;
        }
    }
}
