using System;
using System.Collections.Generic;

namespace API.Contracts.Utilities
{
    public interface IDataUtility
    {
        int GenerateNextId<T>(List<T> collection, Func<T, int> idSelector);
    }
}
