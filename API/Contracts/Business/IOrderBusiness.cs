using API.Models;
using System.Collections.Generic;

namespace API.Contracts.Business
{
    public interface IOrderBusiness
    {
        IEnumerable<Order> GetOrders();
        Order GetOrder(int orderId);
        Order CreateOrder(IEnumerable<int> items);
    }
}
