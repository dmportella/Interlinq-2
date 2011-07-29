using System;
using System.ServiceModel.Web;

namespace InterLinq.Tests.Server.Services
{
    public class CustomServiceHost : WebServiceHost
    {
        private readonly IServiceProvider container;

        public CustomServiceHost(Type serviceType, Uri[] baseAddresses, IServiceProvider container)
            : base(serviceType, baseAddresses)
        {
            this.container = container;
        }

        protected override void OnOpening()
        {
            if (Description.Behaviors.Find<CustomServiceBehavior>() == null)
            {
                Description.Behaviors.Add(new CustomServiceBehavior(this.container));
            }

            base.OnOpening();
        }
    }
}
