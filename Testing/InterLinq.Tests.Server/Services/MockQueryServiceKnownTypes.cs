using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using InterLinq.Tests.Entities;

namespace InterLinq.Tests.Server.Services
{
    public static class MockQueryServiceKnownTypes
    {
        public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider provider)
        {
            return GetListOfKnownTypes(provider);
        }

        public static List<Type> GetListOfKnownTypes(ICustomAttributeProvider provider)
        {
            List<Type> knownTypes = new List<Type>();

            Type interLinqQuery = typeof(InterLinqQuery<>);
            Type genericList = typeof(List<>);

            List<Type> coreTypes = new List<Type>
            {
                typeof(Customer),
                typeof(Order),
                typeof(Product),
                typeof(Supplier),
            };

            coreTypes.ForEach(type =>
                {
                    knownTypes.Add(type);

                    knownTypes.Add(interLinqQuery.MakeGenericType(type));

                    knownTypes.Add(type.MakeArrayType());
                }
            );

            return knownTypes;
        }
    }
}
