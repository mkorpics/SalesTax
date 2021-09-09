using API.Contracts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Business.Utilities
{
    public class DataUtility : IDataUtility
    {
        public int GenerateNextId<T>(List<T> collection, Func<T, int> idSelector)
        {
            if (!collection.Any()) { return 1; }

            var nextId = collection.Max(idSelector) + 1;
            return nextId;
        }
    }
}
