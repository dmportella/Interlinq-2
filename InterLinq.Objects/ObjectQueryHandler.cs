using System;
using System.Linq;
using System.Reflection;

namespace InterLinq.Objects
{
    /// <summary>
    /// LINQ to Objects specific implementation of the
    /// <see cref="IQueryHandler"/>.
    /// </summary>
    /// <seealso cref="IQueryHandler"/>
    public class ObjectQueryHandler : IQueryHandler
    {

        #region Fields

        private readonly IObjectSource objectSource;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes this class.
        /// </summary>
        /// <param name="objectSource"><see cref="IObjectSource"/> used get all objects of a type.</param>
        public ObjectQueryHandler(IObjectSource objectSource)
        {
            if (objectSource == null)
            {
                throw new ArgumentNullException("objectSource");
            }
            this.objectSource = objectSource;
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
            MethodInfo getTableMethod = GetType().GetMethod("Get", BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null);
            MethodInfo genericGetTableMethod = getTableMethod.MakeGenericMethod(type);
            return (IQueryable)genericGetTableMethod.Invoke(this, new object[0]);
        }

        /// <summary>
        /// Returns an <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <typeparam name="T">Generic Argument of the returned <see cref="IQueryable{T}"/>.</typeparam>
        /// <returns>Returns an <see cref="IQueryable{T}"/>.</returns>
        public IQueryable<T> Get<T>() where T : class
        {
            return objectSource.GetObjects<T>(null).AsQueryable();
        }

        /// <summary>
        /// Tells the <see cref="IQueryHandler"/> to start a new the session.
        /// </summary>
        /// <returns>True, if the session creation was successful. False, if not.</returns>
        /// <seealso cref="IQueryHandler.StartSession"/>
        public bool StartSession()
        {
            return true;
        }

        /// <summary>
        /// Tells the <see cref="IQueryHandler"/> to close the current session.
        /// </summary>
        /// <returns>True, if the session closing was successful. False, if not.</returns>
        /// <seealso cref="IQueryHandler.CloseSession"/>
        public bool CloseSession()
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
        public IQueryable Get(Type type, string queryName, params object[] parameters)
        {
            MethodInfo getTableMethod = GetType().GetMethod("Get", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(string), typeof(object[]) }, null);
            MethodInfo genericGetTableMethod = getTableMethod.MakeGenericMethod(type);
            return (IQueryable)genericGetTableMethod.Invoke(this, new object[] { queryName, parameters });
        }

        /// <summary>
        /// Returns an <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <typeparam name="T">Generic Argument of the returned <see cref="IQueryable{T}"/>.</typeparam>
        /// <param name="queryName">The named query to call.</param>
        /// <param name="parameters">The parameters of the named query.</param>
        /// <returns>Returns an <see cref="IQueryable{T}"/>.</returns>
        public IQueryable<T> Get<T>(string queryName, params object[] parameters) where T : class
        {
            return this.objectSource.GetObjects<T>(queryName, parameters).AsQueryable();
        }
    }
}
