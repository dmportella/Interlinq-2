//Copyright © DMPortella.  All Rights Reserved.
//This code released under the terms of the 
//Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)

//Copyright © DMPortella.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterLinq.Tests.Entities;

namespace InterLinq.Tests.Client
{
    internal interface IMockClientContext
    {
        IQueryable<Supplier> Suppliers { get; }
        IQueryable<Customer> Customers { get; }
        IQueryable<Product> Products { get; }
        IQueryable<string> Digits { get; }

        IQueryable<Customer> CustomersByCity(string cityName);
    }
}
