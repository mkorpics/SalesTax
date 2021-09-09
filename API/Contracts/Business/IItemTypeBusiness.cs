using API.Models;
using System.Collections.Generic;

namespace API.Contracts.Business
{
    public interface IItemTypeBusiness
    {
        bool ItemTypeExists(int itemTypeId);
        IEnumerable<ItemType> GetItemTypes();
    }
}
