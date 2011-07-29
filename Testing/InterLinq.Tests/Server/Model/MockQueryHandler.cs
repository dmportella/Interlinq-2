//Copyright © DMPortella.  All Rights Reserved.
//This code released under the terms of the 
//Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)

//Copyright © DMPortella.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterLinq.Objects;

namespace InterLinq.Tests.Server.Model
{
    internal sealed class MockQueryHandler : Objects.ObjectQueryHandler
    {
        public MockQueryHandler() : base(new MockObjectRepository())
        {

        }
    }
}
