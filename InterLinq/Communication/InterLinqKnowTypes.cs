using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace InterLinq.Communication
{
    /// <summary>
    /// Static Class for keeping all the known types for interlinq's framework.
    /// </summary>
    public static class InterLinqKnowTypes
    {
        private static Type[] _knownTypes;

        static InterLinqKnowTypes()
        {
            List<Type> types = new List<Type>();
            types.Add(typeof(InterLinq.Expressions.SerializableInvocationExpression));
            types.Add(typeof(InterLinq.Expressions.SerializableNewArrayExpression));
            types.Add(typeof(InterLinq.Expressions.SerializableConstantExpression));
            types.Add(typeof(InterLinq.Expressions.SerializableListInitExpression));
            types.Add(typeof(InterLinq.Expressions.SerializableNewExpression));
            types.Add(typeof(InterLinq.Expressions.SerializableMemberExpression));
            types.Add(typeof(InterLinq.Expressions.SerializableMemberInitExpression));
            types.Add(typeof(InterLinq.Expressions.SerializableMethodCallExpression));
            types.Add(typeof(InterLinq.Expressions.SerializableLambdaExpression));
            types.Add(typeof(InterLinq.Expressions.SerializableParameterExpression));
            types.Add(typeof(InterLinq.Expressions.SerializableExpressionTyped));
            types.Add(typeof(InterLinq.Expressions.SerializableTypeBinaryExpression));
            types.Add(typeof(InterLinq.Expressions.SerializableUnaryExpression));
            types.Add(typeof(InterLinq.Expressions.SerializableBinaryExpression));
            types.Add(typeof(InterLinq.Expressions.SerializableConditionalExpression));
            types.Add(typeof(InterLinq.Types.Anonymous.AnonymousMetaType));
            types.Add(typeof(InterLinq.Types.InterLinqType));
            types.Add(typeof(InterLinq.Types.InterLinqMemberInfo));
            types.Add(typeof(InterLinq.Types.InterLinqMethodBase));
            types.Add(typeof(System.Collections.Generic.List<InterLinq.Types.InterLinqType>));
            types.Add(typeof(InterLinq.Types.InterLinqMethodInfo));
            types.Add(typeof(InterLinq.Types.InterLinqConstructorInfo));
            types.Add(typeof(InterLinq.Types.InterLinqPropertyInfo));
            types.Add(typeof(InterLinq.Types.InterLinqFieldInfo));
            types.Add(typeof(System.Collections.Generic.List<InterLinq.Types.InterLinqMemberInfo>));
            types.Add(typeof(System.Collections.Generic.List<InterLinq.Types.Anonymous.AnonymousMetaProperty>));
            types.Add(typeof(InterLinq.Types.Anonymous.AnonymousMetaProperty));
            types.Add(typeof(InterLinq.Types.Anonymous.AnonymousObject));
            types.Add(typeof(System.Collections.Generic.List<InterLinq.Types.Anonymous.AnonymousProperty>));
            types.Add(typeof(InterLinq.Types.Anonymous.AnonymousProperty));
            types.Add(typeof(System.Collections.Generic.List<InterLinq.Types.Anonymous.AnonymousObject>));
            types.Add(typeof(InterLinq.Expressions.SerializableExpression));
            types.Add(typeof(System.Collections.Generic.List<InterLinq.Expressions.SerializableExpression>));
            types.Add(typeof(System.Collections.Generic.List<InterLinq.Expressions.SerializableParameterExpression>));
            types.Add(typeof(System.Linq.Expressions.ExpressionType));
            types.Add(typeof(System.Linq.Expressions.MemberBindingType));
            types.Add(typeof(System.Collections.Generic.List<InterLinq.Expressions.SerializableTypes.SerializableElementInit>));
            types.Add(typeof(InterLinq.Expressions.SerializableTypes.SerializableElementInit));
            types.Add(typeof(System.Collections.Generic.List<InterLinq.Expressions.SerializableTypes.SerializableMemberBinding>));
            types.Add(typeof(InterLinq.Expressions.SerializableTypes.SerializableMemberBinding));
            types.Add(typeof(System.Reflection.MethodInfo));
            types.Add(typeof(System.Reflection.MethodBase));
            types.Add(typeof(System.Reflection.MemberInfo));
            types.Add(typeof(System.Exception));
            types.Add(typeof(object[]));
			types.Add(typeof(InterLinq.InterLinqQuery<string>));

            _knownTypes = types.ToArray();
        }
        /// <summary>
        /// Return a list of all known types for the interlinq framework.
        /// </summary>
        /// <param name="provider">The instance of the object that support custom known types.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of all known <see cref="Type"/>.</returns>
        /// 
        
        public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider provider)
        {
            return _knownTypes;
        }
    }
}
