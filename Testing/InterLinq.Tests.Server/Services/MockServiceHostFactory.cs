using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InterLinq.Tests.Server.Services
{
    public class MockServiceHostFactory : CustomServiceHostFactory
    {
        public MockServiceHostFactory()
            : base(ServiceLocator.Locator)
        {

        }
    }
}