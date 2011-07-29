using System;
using System.Linq;
using InterLinq.Expressions;

namespace InterLinq
{
    /// <summary>
    /// Abstract base class holding an <see cref="IQueryHandler"/>.
    /// The usage of the <see cref="InterLinqContext"/> is comparable
    /// with <see cref="System.Data.Linq.DataContext"/>.
    /// </summary>
    /// <example>
    /// The following code illustrates a possible implementation of <see cref="InterLinqContext"/>.
    /// <code>
    ///     public class CompanyContext : InterLinqContext {
    ///        public CompanyContext( IQueryHandler queryHandler ) : base( queryHandler ) { }
    ///       
    ///         public IQueryable&lt;Company&gt; Companies {
    ///             get { return QueryHander.GetTable&lt;Company&gt;(); }
    ///         }
    ///     
    ///         public IQueryable&lt;Company&gt; Departments {
    ///             get { return QueryHander.GetTable&lt;Departments&gt;(); }
    ///         }
    ///     
    ///         public IQueryable&lt;Company&gt; Employees {
    ///             get { return QueryHander.GetTable&lt;Employee&gt;(); }
    ///         }
    ///     }
    /// </code>
    /// </example>
    public abstract class InterLinqContext
    {

        #region Properties

        /// <summary>
        /// Gets the <see cref="IQueryHandler"/>.
        /// </summary>
        protected IQueryHandler QueryHandler { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes this class.
        /// </summary>
        /// <param name="queryHandler"><see cref="IQueryHandler"/> instance.</param>
        protected InterLinqContext(IQueryHandler queryHandler)
        {
            if (queryHandler == null)
            {
                throw new ArgumentException("queryHandler");
            }
            QueryHandler = queryHandler;
        }

        #endregion

        /// <summary>
        /// Executes a named query on the server.
        /// </summary>
        /// <typeparam name="T">The entity type expected.</typeparam>
        /// <param name="name">The name of the query.</param>
        /// <param name="parameters">A list of Expression parameters to be passed into the query.</param>
        /// <returns>The result of the query.</returns>
        public IQueryable<T> ExecuteMethod<T>(string name, params System.Linq.Expressions.Expression[] parameters) where T: class
        {
            return this.QueryHandler.Get<T>(name, parameters);
        }
    }
}
