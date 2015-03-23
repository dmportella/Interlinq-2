using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using InterLinq.Expressions.Helpers;
using InterLinq.Types;
using InterLinq.Expressions.SerializableTypes;
using System.Reflection;

namespace InterLinq.Expressions
{

    /// <summary>
    /// A serializable version of <see cref="Expression"/>.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    [DataContract(Namespace="http://schemas.interlinq.com/2011/03/")]
    [KnownType(typeof(SerializableBinaryExpression))]
    [KnownType(typeof(SerializableConditionalExpression))]
    [KnownType(typeof(SerializableConstantExpression))]
    [KnownType(typeof(SerializableExpression))]
    [KnownType(typeof(SerializableExpressionTyped))]
    [KnownType(typeof(SerializableInvocationExpression))]
    [KnownType(typeof(SerializableLambdaExpression))]
    [KnownType(typeof(SerializableListInitExpression))]
    [KnownType(typeof(SerializableMemberExpression))]
    [KnownType(typeof(SerializableMemberInitExpression))]
    [KnownType(typeof(SerializableMethodCallExpression))]
    [KnownType(typeof(SerializableNewArrayExpression))]
    [KnownType(typeof(SerializableNewExpression))]
    [KnownType(typeof(SerializableParameterExpression))]
    [KnownType(typeof(SerializableTypeBinaryExpression))]
    [KnownType(typeof(InterLinq.Expressions.SerializableTypes.SerializableElementInit))]
    [KnownType(typeof(InterLinq.Expressions.SerializableTypes.SerializableMemberAssignment))]
    [KnownType(typeof(InterLinq.Expressions.SerializableTypes.SerializableMemberBinding))]
    [KnownType(typeof(InterLinq.Expressions.SerializableTypes.SerializableMemberListBinding))]
    [KnownType(typeof(InterLinq.Expressions.SerializableTypes.SerializableMemberMemberBinding))]
    [KnownType(typeof(SerializableUnaryExpression))]
    [KnownType(typeof(InterLinqQueryBase))]
    [KnownType(typeof(InterLinq.Types.Anonymous.AnonymousMetaProperty))]
    [KnownType(typeof(InterLinq.Types.Anonymous.AnonymousMetaType))]
    [KnownType(typeof(InterLinq.Types.Anonymous.AnonymousObject))]
    [KnownType(typeof(InterLinq.Types.Anonymous.AnonymousProperty))]
    [KnownType(typeof(InterLinq.Types.Anonymous.InterLinqGroupingBase))]
    [KnownType(typeof(InterLinqConstructorInfo))]
    [KnownType(typeof(InterLinqFieldInfo))]
    [KnownType(typeof(InterLinqMemberInfo))]
    [KnownType(typeof(InterLinqMethodBase))]
    [KnownType(typeof(InterLinqMethodInfo))]
    [KnownType(typeof(InterLinqPropertyInfo))]
    [KnownType(typeof(InterLinqType))]
    public abstract class SerializableExpression
    {

#region Constructor

        /// <summary>
        /// Default constructor for serialization.
        /// </summary>
        protected SerializableExpression() { }

        /// <summary>
        /// Constructor with an <see cref="Expression"/> and an <see cref="ExpressionConverter"/>.
        /// </summary>
        /// <param name="expression">The original, not serializable <see cref="Expression"/>.</param>
        /// <param name="expConverter">The <see cref="ExpressionConverter"/> to convert contained <see cref="Expression">Expressions</see>.</param>
        protected SerializableExpression(Expression expression, ExpressionConverter expConverter)
            : this(expression.NodeType, expression.Type, expConverter)
        {
            HashCode = expression.GetHashCode();
        }

        /// <summary>
        /// Constructor with an <see cref="ExpressionType"/>, a <see cref="Type"/> and an <see cref="ExpressionConverter"/>.
        /// </summary>
        /// <param name="nodeType">The <see cref="ExpressionType"/> of the <see cref="Expression"/>.</param>
        /// <param name="type">The <see cref="Type"/> of the <see cref="Expression"/>.</param>
        /// <param name="expConverter">The <see cref="ExpressionConverter"/> to convert contained <see cref="Expression">Expressions</see>.</param>
        private SerializableExpression(ExpressionType nodeType, Type type, ExpressionConverter expConverter)
            : this()
        {
            NodeType = nodeType;
#if !NETFX_CORE
            Type = InterLinqTypeSystem.Instance.GetInterLinqVersionOf<InterLinqType>(type);
#else
            Type = InterLinqTypeSystem.Instance.GetInterLinqVersionOf<InterLinqType>(type.GetTypeInfo());
#endif
        }

        #endregion

        #region Properties

        /// <summary>
        /// The hashcode of the original <see cref="Expression"/>.
        /// </summary>
        [DataMember]
        public int HashCode { get; set; }

        /// <summary>
        /// See <see cref="Expression.NodeType"/>
        /// </summary>
        [DataMember]
        public ExpressionType NodeType { get; set; }

        /// <summary>
        /// See <see cref="Expression.Type"/>
        /// </summary>
        [DataMember]
        public InterLinqType Type { get; set; }

#endregion

#region ToString() Methods

        /// <summary>
        /// Builds a <see langword="string"/> representing the <see cref="Expression"/>.
        /// </summary>
        /// <returns>A <see langword="string"/> representing the <see cref="Expression"/>.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            BuildString(sb);
            return sb.ToString();
        }

        /// <summary>
        /// Builds a <see langword="string"/> representing the <see cref="Expression"/>.
        /// </summary>
        /// <param name="builder">A <see cref="System.Text.StringBuilder"/> to add the created <see langword="string"/>.</param>
        internal virtual void BuildString(StringBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }
            builder.Append("[");
            builder.Append(NodeType.ToString());
            builder.Append("]");
        }

#endregion

    }
}