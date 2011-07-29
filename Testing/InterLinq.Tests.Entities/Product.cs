//Copyright © DMPortella.  All Rights Reserved.
//This code released under the terms of the 
//Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)

//Copyright © DMPortella.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterLinq.Tests.Entities
{
    public class Product
    {
        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public string Category { get; set; }

        public decimal UnitPrice { get; set; }

        public int UnitsInStock { get; set; }
    }
}
