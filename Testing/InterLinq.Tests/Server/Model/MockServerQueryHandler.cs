//Copyright © DMPortella.  All Rights Reserved.
//This code released under the terms of the 
//Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)

//Copyright © DMPortella.  All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterLinq.Tests.Server.Model
{
    internal sealed class MockService : IQueryRemoteHandler
    {
        private Communication.ServerQueryHandler serverQueryHandler;

        public MockService()
        {
            this.serverQueryHandler = new Communication.ServerQueryHandler(new MockQueryHandler());
        }

        public object Retrieve(Expressions.SerializableExpression expression)
        {
            return this.serverQueryHandler.Retrieve(expression);
        }
    }
}
