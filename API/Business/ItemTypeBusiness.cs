using API.Contracts.Business;
using API.DbContext;
using API.Models;
using System.Collections.Generic;
using System.Linq;

namespace API.Business
{
    public class ItemTypeBusiness : IItemTypeBusiness
    {
        public bool ItemTypeExists(int itemTypeId)
        {
            return SalesTaxDbContext.ItemTypes.Any(x => x.ItemTypeId == itemTypeId);
        }

        public IEnumerable<ItemType> GetItemTypes()
        {
            return SalesTaxDbContext.ItemTypes.OrderBy(x => x.ItemTypeName);
        }
    }
}
