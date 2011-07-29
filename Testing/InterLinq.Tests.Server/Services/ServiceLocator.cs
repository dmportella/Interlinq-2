using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.Design;

namespace InterLinq.Tests.Server.Services
{
    public static class ServiceLocator
    {
        private static IServiceProvider serviceProvider;

        public static IServiceProvider Locator
        {
            get { return (serviceProvider != null) ? serviceProvider : ServiceLocator.CreateInstance(); }
        }

        private static IServiceProvider CreateInstance()
        {
            ServiceContainer serviceContainer = new ServiceContainer();

            serviceProvider = serviceContainer;

            serviceContainer.AddService(typeof(MockQueryService), new ServiceCreatorCallback((container, serviceType) => new MockQueryService()));

            return serviceContainer;
        }
    }
}