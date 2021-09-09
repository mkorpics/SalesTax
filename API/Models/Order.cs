using System;
using System.Collections.Generic;

namespace API.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public Guid OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal? TotalSalesTax { get; set; }
        public decimal? OrderTotal { get; set; }
        public IEnumerable<PurchaseItem> Items { get; set; }
    }
}
