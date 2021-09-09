using API.Contracts.Business;
using API.Contracts.Utilities;
using API.DbContext;
using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Business
{
    public class OrderBusiness : IOrderBusiness
    {
        private IDataUtility _dataUtility;

        public OrderBusiness(IDataUtility dataUtility)
        {
            _dataUtility = dataUtility;
        }

        public IEnumerable<Order> GetOrders()
        {
            return SalesTaxDbContext.Orders;
        }

        public Order GetOrder(int orderId)
        {
            return SalesTaxDbContext.Orders.FirstOrDefault(x => x.OrderId == orderId);
        }

        public Order CreateOrder(IEnumerable<int> itemIds)
        {
            var purchaseItems = SalesTaxDbContext.PurchaseItems.Where(x => x.IsInShoppingCart && itemIds.Contains(x.PurchaseItemId)).ToList();

            var createdOrder = new Order()
            {
                OrderId = _dataUtility.GenerateNextId(SalesTaxDbContext.Orders, x => x.OrderId),
                OrderNumber = Guid.NewGuid(),
                OrderDate = DateTime.Now,
                Items = purchaseItems,
                TotalSalesTax = purchaseItems.Sum(x => x.InventoryItem.SalesTax * x.Quantity),
            };
            createdOrder.OrderTotal = purchaseItems.Sum(x => x.InventoryItem.Price * x.Quantity) + createdOrder.TotalSalesTax;

            purchaseItems.ForEach(x => x.OrderId = createdOrder.OrderId);

            SalesTaxDbContext.Orders.Add(createdOrder);
            return createdOrder;
        }
    }
}
