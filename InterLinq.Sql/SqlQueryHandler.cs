using System;
using System.Data.Linq;
using System.Linq;

namespace InterLinq.Sql
{
    /// <summary>
    /// LINQ to SQL specific implementation of the
    /// <see cref="IQueryHandler"/>.
    /// </summary>
    /// <seealso cref="IQueryHandler"/>
    public class SqlQueryHandler : IQueryHandler
    {

        #region Fields

        private readonly DataContext dataContext;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes this class.
        /// </summary>
        /// <param name="dataContext"><see cref="DataContext"/> used for database operations.</param>
        public SqlQueryHandler(DataContext dataContext)
        {
            if (dataContext == null)
            {
                throw new ArgumentNullException("dataContext");
            }
            this.dataContext = dataContext;
        }

        #endregion

        #region IQueryHandler Members

        /// <summary>
        /// Returns an <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <param name="type">Type of the returned <see cref="IQueryable{T}"/>.</param>
        /// <returns>Returns an <see cref="IQueryable{T}"/>.</returns>
        public IQueryable Get(Type type)
        {
            return dataContext.GetTable(type);
        }

        /// <summary>
        /// Returns an <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <typeparam name="T">Generic Argument of the returned <see cref="IQueryable{T}"/>.</typeparam>
        /// <returns>Returns an <see cref="IQueryable{T}"/>.</returns>
        public IQueryable<T> Get<T>() where T : class
        {
            return dataContext.GetTable<T>();
        }

        /// <summary>
        /// Tells the <see cref="IQueryHandler"/> to start a new the session.
        /// </summary>
        /// <returns>True, if the session creation was successful. False, if not.</returns>
        /// <seealso cref="IQueryHandler.StartSession"/>
        public object StartSession()
        {
            return new object();
        }

        /// <summary>
        /// Tells the <see cref="IQueryHandler"/> to close the current session.
        /// </summary>
        /// <returns>True, if the session closing was successful. False, if not.</returns>
        /// <seealso cref="IQueryHandler.CloseSession"/>
        public bool CloseSession(object sessionObject)
        {
            return true;
        }

        #endregion


        /// <summary>
        /// Returns an <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <param name="type">Type of the returned <see cref="IQueryable{T}"/>.</param>
        /// <param name="queryName">The named query to call.</param>
        /// <param name="parameters">The parameters of the named query.</param>
        /// <returns>Returns an <see cref="IQueryable{T}"/>.</returns>
        public IQueryable Get(Type type, string queryName, object sessionObject, params object[] parameters)
        {// TODO  must implement named queries on linq to sql
            throw new NotImplementedException("NOT IMPLEMENTED YET On linq to sql");
        }

        /// <summary>
        /// Returns an <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <typeparam name="T">Generic Argument of the returned <see cref="IQueryable{T}"/>.</typeparam>
        /// <param name="queryName">The named query to call.</param>
        /// <param name="parameters">The parameters of the named query.</param>
        /// <returns>Returns an <see cref="IQueryable{T}"/>.</returns>
        public IQueryable<T> Get<T>(string queryName, object sessionObject, params object[] parameters) where T : class
        {// TODO  must implement named queries on linq to sql
            throw new NotImplementedException("NOT IMPLEMENTED YET On linq to sql");
        }
    }
}
