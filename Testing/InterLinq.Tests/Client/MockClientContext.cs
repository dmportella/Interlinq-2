//Copyright © DMPortella.  All Rights Reserved.
//This code released under the terms of the 
//Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)

//Copyright © DMPortella.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterLinq.Communication;
using InterLinq.Tests.Server;
using InterLinq.Tests.Server.Model;

namespace InterLinq.Tests.Client
{
    internal sealed class MockClientContext : InterLinqContext, IMockClientContext
    {
        public MockClientContext()
            : base(MockClientContext.Create())
        {

        }

        private static IQueryHandler Create()
        {
            return new ClientQueryHandler(GetMockServerQueryHandler());
        }

        private static InterLinq.IQueryRemoteHandler GetMockServerQueryHandler()
        {
#warning Change here for memory or http
            //Server.MockQueryServiceClient client = new MockQueryServiceClient(); // run these tests to prove that the serialisation work across http

            MockService client = new MockService(); // memory test run these test to prove that the serialisation works

            return client;
        }

        public IQueryable<Entities.Supplier> Suppliers
        {
            get { return this.QueryHandler.Get<Entities.Supplier>(null, null); }
        }

        public IQueryable<Entities.Customer> Customers
        {
            get { return this.QueryHandler.Get<Entities.Customer>(null, null); }
        }

        public IQueryable<Entities.Product> Products
        {
            get { return this.QueryHandler.Get<Entities.Product>(null, null); }
        }

        public IQueryable<string> Digits
        {
            get { return this.QueryHandler.Get<string>(null, null); }
        }

        public IQueryable<Entities.Customer> CustomersByCity(string cityName)
        {
            return this.Customers.CustomersByCity(cityName);
        }
    }
}
