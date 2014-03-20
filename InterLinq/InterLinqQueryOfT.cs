using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using InterLinq.Types;
using InterLinq.Expressions;

namespace InterLinq
{
    /// <summary>
    /// Standard implementation of an InterLinqQuery.
    /// </summary>
    /// <typeparam name="T">The type of the content of the data source.</typeparam>
    /// <seealso cref="InterLinqQueryBase"/>
#if !SILVERLIGHT
    [Serializable]
#endif
    [DataContract(Name = "InterLinqQueryOf{0}", Namespace = "http://schemas.interlinq.com/2011/03/")]
    public class InterLinqQuery<T> : InterLinqQueryBase, IOrderedQueryable<T>
    {
        #region Fields

#if !SILVERLIGHT
        [NonSerialized]
#endif
        private IQueryProvider provider;
#if !SILVERLIGHT
        [NonSerialized]
#endif
        private Expression expression;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes this Query with the arguments.
        /// </summary>
        /// <remarks>
        /// This constructor creates a <see cref="ConstantExpression"/>.
        /// The arguments will be checked. These exceptions will be thrown...
        ///     ... when "provider" is null:    <see cref="ArgumentNullException"/>
        /// </remarks>
        /// <param name="provider"><see cref="IQueryProvider"/> to set.</param>
        public InterLinqQuery(IQueryProvider provider)
        {
            Initialize(provider, Expression.Constant(this), null, null);
        }

        /// <summary>
        /// Initializes this Query with the arguments.
        /// </summary>
        /// <remarks>
        /// The arguments will be checked. These exceptions will be thrown...
        /// <list type="list">
        ///     <listheader>
        ///         <term>Condition</term>
        ///         <description>Thrown Exception</description>
        ///     </listheader>
        ///     <item>
        ///         <term>... when "provider" is null</term>
        ///         <description><see cref="ArgumentNullException"/></description>
        ///     </item>
        ///     <item>
        ///         <term>... when "expression" is null</term>
        ///         <description><see cref="ArgumentNullException"/></description>
        ///     </item>
        ///     <item>
        ///         <term>... when "expression" is not assignable from <see cref="IQueryable{T}"/></term>
        ///         <description><see cref="ArgumentException"/></description>
        ///     </item>
        /// </list>                                  
        /// </remarks>
        /// <param name="provider"><see cref="IQueryProvider"/> to set.</param>
        /// <param name="expression"><see cref="Expression"/> to set.</param>
        public InterLinqQuery(IQueryProvider provider, Expression expression)
        {
            Initialize(provider, expression, null, null);
        }

        /// <summary>
        /// Create an instance of InterLinq query with the specified name and parameters.
        /// </summary>
        /// <param name="iQueryProvider"><see cref="IQueryProvider"/> to set.</param>
        /// <param name="expr"><see cref="Expression"/> to set.</param>
        /// <param name="queryName">The Query name. This cant be null.</param>
        /// <param name="parameters">The parameters for the query. Dont pass anything here is the named query has no parameters.</param>
        public InterLinqQuery(IQueryProvider iQueryProvider, Expression expr, object additionalObject, string queryName, params object[] parameters)
        {
            Initialize(iQueryProvider, expr ?? Expression.Constant(this), additionalObject, queryName, parameters);
        }

        /// <summary>
        /// Initialise the instance of <see cref="InterLinqQuery{T}"/> with the specified name and parameters.
        /// </summary>
        /// <param name="iQueryProvider"><see cref="IQueryProvider"/> to set.</param>
        /// <param name="expr"><see cref="Expression"/> to set.</param>
        /// <param name="queryName">The Query name. This cant be null.</param>
        /// <param name="parameters">The parameters for the query. Dont pass anything here is the named query has no parameters.</param>
        private void Initialize(IQueryProvider iQueryProvider, Expression expr, object additionalObject, string queryName, params object[] parameters)
        {
            if (iQueryProvider == null)
            {
                throw new ArgumentNullException("iQueryProvider");
            }
            if (expr == null)
            {
                throw new ArgumentNullException("expr");
            }

            if (!typeof(IQueryable<T>).IsAssignableFrom(expr.Type))
            {
                throw new ArgumentException("expr");
            }
            provider = iQueryProvider;
            expression = expr;
            elementType = typeof(T);
            elementInterLinqType = InterLinqTypeSystem.Instance.GetInterLinqVersionOf<InterLinqType>(elementType);
            AdditionalObject = additionalObject;
            QueryName = queryName;

            if (!string.IsNullOrEmpty(queryName))
                QueryParameters = (parameters != null && parameters.Count() != 0)
                    ? new List<SerializableExpression>(
                        parameters.Select(s => System.Linq.Expressions.Expression.Constant(s).MakeSerializable())
                            .ToArray())
                    : null;
            else
                Parameters = parameters;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a <see langword="string"/> that represents this instance.
        /// </summary>
        /// <remarks>
        /// The following <see langword="string"/> is returned:
        /// <c>Type&lt;GenericArgumentType&gt;</c>
        /// </remarks>
        /// <returns>A <see langword="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("{0}<{1}>", GetType().Name, typeof(T));
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// Returns an <see cref="IEnumerator{T}"/> that iterates through the returned result
        /// of this query.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator{T}"/> object that can be used to iterate 
        /// through the returned result of this query.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            IEnumerable retrievedObjects = (IEnumerable)provider.Execute(expression);
            object returnValue = TypeConverter.ConvertFromSerializable(typeof(IEnumerable<T>), retrievedObjects);
            if (returnValue == null)
                return (new List<T>()).GetEnumerator();
            return ((IEnumerable<T>)returnValue).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an <see cref="IEnumerator"/> that iterates through the returned result
        /// of this query.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> object that can be used to iterate 
        /// through the returned result of this query.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IQueryable Members

        /// <summary>
        /// Gets the type of the element(s) that are returned when the 
        /// <see cref="Expression"/> tree associated with this instance of 
        /// <see cref="IQueryable"/> is executed.
        /// </summary>
        /// <remarks>
        /// A <see cref="Type"/> that represents the type of the element(s) that are returned
        /// when the <see cref="Expression"/> tree associated with this object is executed.
        /// </remarks>
        public override Type ElementType
        {
            get
            {
                if (elementType == null && elementInterLinqType != null)
                {
                    elementType = (Type)elementInterLinqType.GetClrVersion();
                }
                return elementType;
            }
        }

        /// <summary>
        /// Gets the <see cref="IQueryProvider"/> that is associated with this data source.
        /// </summary>
        /// <remarks>
        /// The <see cref="IQueryProvider"/> that is associated with this data source.
        /// </remarks>
        public IQueryProvider Provider
        {
            get { return provider; }
        }

        /// <summary>
        /// Gets the <see cref="Expression"/> tree that is associated with the instance of 
        /// <see cref="IQueryable"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="Expression"/> that is associated with this instance of 
        /// <see cref="IQueryable"/>.
        /// </remarks>
        public Expression Expression
        {
            get { return expression; }
        }

        #endregion
    }
}
