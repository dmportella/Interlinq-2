using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InterLinq
{
    public static class ExtensionMethods
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach(var e in enumerable)
            {
                action(e);
            }
        }

        public static MemberTypes GetMemberType(this MemberInfo member)
        {
            if (member is FieldInfo)
                return MemberTypes.Field;
            if (member is ConstructorInfo)
                return MemberTypes.Constructor;
            if (member is PropertyInfo)
                return MemberTypes.Property;
            if (member is EventInfo)
                return MemberTypes.Event;
            if (member is MethodInfo)
                return MemberTypes.Method;

            var typeInfo = member as TypeInfo;
            //Debug.Assert(typeInfo != null);
            if (!typeInfo.IsPublic && !typeInfo.IsNotPublic)
                return MemberTypes.NestedType;

            return MemberTypes.TypeInfo;
        }
    }
}
