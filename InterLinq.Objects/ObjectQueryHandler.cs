using System;
using System.Linq;
using System.Reflection;

namespace InterLinq.Objects
{
    /// <summary>
    /// LINQ to Objects specific implementation of the
    /// <see cref="T:InterLinq.Objects.IQueryHandler"/>.
    /// </summary>
    /// <seealso cref="T:InterLinq.Objects.IQueryHandler"/>
    public class ObjectQueryHandler : IQueryHandler
    {
        #region Fields

        private static readonly MethodInfo getMethod;
        private static readonly MethodInfo getByNameMethod;

        private readonly IObjectSource objectSource;

        #endregion

        #region Constructors

        static ObjectQueryHandler()
        {
            Type type = MethodBase.GetCurrentMethod().DeclaringType;
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
            getMethod = type.GetMethod("Get", flags, null, Type.EmptyTypes, null);
            getByNameMethod = type.GetMethod("Get", flags, null, new Type[] { typeof(string), typeof(object), typeof(object[]) }, null);
            
        }

        /// <summary>
        /// Initializes this class.
        /// </summary>
        /// <param name="objectSource"><see cref="T:InterLinq.Objects.IObjectSource"/> used get all objects of a type.</param>
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
        /// Returns an <see cref="T:System.Linq.IQueryable`1"/>.
        /// </summary>
        /// <param name="type">Type of the returned <see cref="T:System.Linq.IQueryable`1"/>.</param>
        /// <returns>Returns an <see cref="T:System.Linq.IQueryable`1"/>.</returns>
        public IQueryable Get(Type type)
        {
            MethodInfo genericGetTableMethod = getMethod.MakeGenericMethod(type);
            return (IQueryable)genericGetTableMethod.Invoke(this, new object[0]);
        }

        /// <summary>
        /// Returns an <see cref="T:System.Linq.IQueryable`1"/>.
        /// </summary>
        /// <typeparam name="T">Generic Argument of the returned <see cref="T:System.Linq.IQueryable`1"/>.</typeparam>
        /// <returns>Returns an <see cref="T:System.Linq.IQueryable`1"/>.</returns>
        public IQueryable<T> Get<T>() where T : class
        {
            return objectSource.GetObjects<T>(null).AsQueryable();
        }

        /// <summary>
        /// Tells the <see cref="T:System.Linq.IQueryHandler"/> to start a new the session.
        /// </summary>
        /// <returns>Session object, if the session creation was successful. <c>null</c>, if not.</returns>
        /// <seealso cref="M:InterLinq.Objects.IQueryHandler.StartSession"/>
        public object StartSession()
        {
            return new object();
        }

        /// <summary>
        /// Tells the <see cref="T:System.Linq.IQueryHandler"/> to close the specified session.
        /// </summary>
        /// <returns><c>true</c>, if the session closing was successful. <c>false</c>, if not.</returns>
        /// <seealso cref="M:InterLinq.Objects.IQueryHandler.CloseSession"/>
        public bool CloseSession(object sessionObject)
        {
            return true;
        }

        #endregion

        /// <summary>
        /// Returns an <see cref="T:System.Linq.IQueryable"/>.
        /// </summary>
        /// <param name="type">Type of the returned <see cref="T:System.Linq.IQueryable"/>.</param>
        /// <param name="queryName">The named query to call.</param>
        /// <param name="sessionObject">The object that represents session.</param>
        /// <param name="parameters">The parameters of the named query.</param>
        /// <returns>Returns an <see cref="T:System.Linq.IQueryable"/>.</returns>
        /// <seealso cref="IQueryHandler.Get(System.Type, System.String, System.Object, System.Object[])"/>
        public IQueryable Get(Type type, object additionalObject, string queryName, object sessionObject, params object[] parameters)
        {
            MethodInfo genericGetTableMethod = getByNameMethod.MakeGenericMethod(type);
            return (IQueryable)genericGetTableMethod.Invoke(this, new object[] { additionalObject, queryName, parameters });
        }

        /// <summary>
        /// Returns an <see cref="T:System.Linq.IQueryable`1"/>.
        /// </summary>
        /// <typeparam name="T">Generic Argument of the returned <see cref="T:System.Linq.IQueryable`1"/>.</typeparam>
        /// <param name="queryName">The named query to call.</param>
        /// <param name="sessionObject">The object that represents session.</param>
        /// <param name="parameters">The parameters of the named query.</param>
        /// <returns>Returns an <see cref="T:System.Linq.IQueryable`1"/>.</returns>
        /// <seealso cref="IQueryHandler.Get{T}(System.String, System.Object, System.Object[])"/>
        public IQueryable<T> Get<T>(object additionalObject, string queryName, object sessionObject, params object[] parameters) where T : class
        {
            return this.objectSource.GetObjects<T>(queryName, parameters).AsQueryable();
        }
    }
}
