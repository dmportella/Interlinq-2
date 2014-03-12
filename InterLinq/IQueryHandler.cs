using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace InterLinq
{
    /// <summary>
    /// Interface providing methods to get <see cref="T:System.Linq.IQueryable`1"/>.
    /// </summary>
    public interface IQueryHandler
    {
        //IQueryable Get(Type type);
        //IQueryable<T> Get<T>() where T : class;

        /// <summary>
        /// Returns an <see cref="T:System.Linq.IQueryable"/>.
        /// </summary>
        /// <param name="type">Type of the returned <see cref="T:System.Linq.IQueryable"/>.</param>
        /// <param name="queryName">The named query to call.</param>
        /// <param name="sessionObject">The object that represents session.</param>
        /// <param name="parameters">The parameters of the named query.</param>
        /// <returns>Returns an <see cref="T:System.Linq.IQueryable"/>.</returns>
        IQueryable Get(Type type, object additionalObject, string queryName, object sessionObject, params object[] parameters);

        /// <summary>
        /// Returns an <see cref="T:System.Linq.IQueryable`1"/>.
        /// </summary>
        /// <typeparam name="T">Generic Argument of the returned <see cref="T:System.Linq.IQueryable`1"/>.</typeparam>
        /// <param name="queryName">The named query to call.</param>
        /// <param name="sessionObject">The object that represents session.</param>
        /// <param name="parameters">The parameters of the named query.</param>
        /// <returns>Returns an <see cref="T:System.Linq.IQueryable`1"/>.</returns>
        IQueryable<T> Get<T>(object additionalObject, string queryName, object sessionObject, params object[] parameters) where T : class;

        /// <summary>
        /// Tells the <see cref="T:System.Linq.IQueryHandler"/> to start a new the session.
        /// </summary>
        /// <returns>Session object, if the session creation was successful. <c>null</c>, if not.</returns>
        object StartSession();

        /// <summary>
        /// Tells the <see cref="T:System.Linq.IQueryHandler"/> to close the specified session.
        /// </summary>
        /// <returns><c>true</c>, if the session closing was successful. <c>false</c>, if not.</returns>
        bool CloseSession(object sessionObject);
    }
}
