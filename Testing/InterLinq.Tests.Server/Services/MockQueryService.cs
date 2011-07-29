using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Activation;
using System.ServiceModel;
using InterLinq.Tests.Server.Model;
using InterLinq.Communication;

namespace InterLinq.Tests.Server.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class MockQueryService : IMockQueryService
    {
        private ServerQueryHandler serverQueryHandler;

        public MockQueryService()
        {
            this.serverQueryHandler = new Communication.ServerQueryHandler(new MockQueryHandler());
        }

        public object Retrieve(Expressions.SerializableExpression expression)
        {
            return this.serverQueryHandler.Retrieve(expression);
        }

        public void Dispose()
        {
            this.serverQueryHandler = null;
        }
    }
}