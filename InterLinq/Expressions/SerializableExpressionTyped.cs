using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using InterLinq.Expressions.Helpers;
using InterLinq.Types;
using System.Reflection;

namespace InterLinq.Expressions
{

    /// <summary>
    /// A serializable version of <see cref="LambdaExpression"/>.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    [DataContract(Namespace="http://schemas.interlinq.com/2011/03/")]
    public class SerializableExpressionTyped : SerializableLambdaExpression
    {

        #region Constructors

        /// <summary>
        /// Default constructor for serialization.
        /// </summary>
        public SerializableExpressionTyped() { }

        /// <summary>
        /// Constructor with an <see cref="LambdaExpression"/> and an <see cref="ExpressionConverter"/>.
        /// </summary>
        /// <param name="expression">The original, not serializable <see cref="Expression"/>.</param>
        /// <param name="delegateType"><see cref="Type"/> of the delegate.</param>
        /// <param name="expConverter">The <see cref="ExpressionConverter"/> to convert contained <see cref="Expression">Expressions</see>.</param>
        public SerializableExpressionTyped(LambdaExpression expression, Type delegateType, ExpressionConverter expConverter)
            : base(expression, expConverter)
        {
#if !NETFX_CORE
            Type = InterLinqTypeSystem.Instance.GetInterLinqVersionOf<InterLinqType>(delegateType);
#else
            Type = InterLinqTypeSystem.Instance.GetInterLinqVersionOf<InterLinqType>(delegateType.GetTypeInfo());
#endif
        }

        #endregion
    }
}
