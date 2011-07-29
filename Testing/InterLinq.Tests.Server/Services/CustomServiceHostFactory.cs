using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Description;

namespace InterLinq.Tests.Server.Services
{
    public class CustomServiceHostFactory : WebServiceHostFactory
    {
        private readonly IServiceProvider container;

        public CustomServiceHostFactory(IServiceProvider container)
        {
            this.container = container;
        }

        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var host = new CustomServiceHost(serviceType, baseAddresses, this.container);

            return host;
        }
    }
}
