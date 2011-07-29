using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace InterLinq.Tests.Server.Services
{
    public class CustomInstanceProvider : IInstanceProvider
    {
        private readonly IServiceProvider container;
        private readonly Type serviceType;

        public CustomInstanceProvider(IServiceProvider container, Type serviceType)
        {
            this.container = container;
            this.serviceType = serviceType;
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return this.GetInstance(instanceContext, null);
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return this.container.GetService(this.serviceType);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            
        }
    }
}
