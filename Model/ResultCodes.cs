using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Rest.Model
{
    public enum ResultCodes
    {    
        EmptyParam = -5,
        NotFound = -4,
        NotDeleted = -3,
        NotUpdated = -2,
        NotCreated = -1,
        OK = 0,
        Created = 1,
        Updated = 2,
        Deleted = 3
    }
}