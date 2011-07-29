//Copyright © DMPortella.  All Rights Reserved.
//This code released under the terms of the 
//Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)

//Copyright © DMPortella.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterLinq.Tests.Client
{
    internal static class MockClientExtensions
    {
        public static IQueryable<Entities.Customer> CustomersByCity(this IQueryable<Entities.Customer> query, string cityName)
        {
            InterLinqQuery<Entities.Customer> interLinqQuery = new InterLinqQuery<Entities.Customer>(query.Provider, null, "CustomersByCity", cityName);
            
            return interLinqQuery;
        }
    }
}
