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
    public class Order
    {
        public int OrderID { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal Total { get; set; }
    }
}
