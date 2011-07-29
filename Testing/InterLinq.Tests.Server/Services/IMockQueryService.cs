using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;

namespace InterLinq.Tests.Server.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(MockQueryServiceKnownTypes))]
    public interface IMockQueryService : IQueryRemoteHandler, IDisposable
    {

    }
}