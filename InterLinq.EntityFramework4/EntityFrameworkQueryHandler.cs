using System;
using System.Data.Objects;
using System.Linq;
using System.Reflection;
using System.Data.Metadata.Edm;
using System.Collections.Generic;

namespace InterLinq.EntityFramework4
{
    /// <summary>
    /// LINQ to Entities specific implementation of the
    /// <see cref="IQueryHandler"/>.
    /// </summary>
    /// <seealso cref="IQueryHandler"/>
    public class EntityFrameworkQueryHandler : IQueryHandler
    {
        #region Fields

        private readonly ObjectContext objectContext;

        private readonly static MethodInfo getTableMethod;
        private readonly static MethodInfo getNamedQueryMethod;


        #endregion

        #region Constructors

        /// <summary>
        /// Initializes this class.
        /// </summary>
        /// <param name="objectContext">A entity <see cref="ObjectContext"/>.</param>
        public EntityFrameworkQueryHandler(ObjectContext objectContext)
        {
            if (objectContext == null)
            {
                throw new ArgumentNullException("objectContext");
            }
            this.objectContext = objectContext;
        }

        static EntityFrameworkQueryHandler()
        {
            getTableMethod = typeof(EntityFrameworkQueryHandler).GetMethod("Get", BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null);
            getNamedQueryMethod = typeof(EntityFrameworkQueryHandler).GetMethod("Get", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(string), typeof(object[]) }, null);
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
            return objectContext.CreateObjectSet<T>();
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

        public IQueryable Get(Type type, object additionalObject, string queryName, object sessionObject, params object[] parameters)
        {
            MethodInfo genericGetTableMethod = getNamedQueryMethod.MakeGenericMethod(type);
            return (IQueryable)genericGetTableMethod.Invoke(this, new object[] { queryName, parameters });
        }

        public IQueryable<T> Get<T>(object additionalObject, string queryName, object sessionObject, params object[] parameters) where T : class
        {
            return this.objectContext.ExecuteStoreQuery<T>(queryName, parameters).AsQueryable();
        }
    }
}
