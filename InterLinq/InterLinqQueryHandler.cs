using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using InterLinq.Expressions;
using System.Linq.Expressions;

namespace InterLinq
{
    /// <summary>
    /// Abstract implementation of an <see cref="IQueryHandler"/>.
    /// This class provides methods to get an <see cref="IQueryable{T}"/>.
    /// </summary>
    /// <seealso cref="IQueryHandler"/>
    public abstract class InterLinqQueryHandler : IQueryHandler
    {
        protected static MethodInfo getTableMethodWithoutPara;
        protected static MethodInfo getTableMethod;

        static InterLinqQueryHandler()
        {
#if !NETFX_CORE
            getTableMethodWithoutPara = typeof(InterLinqQueryHandler).GetMethod("Get", new Type[] { });
            getTableMethod = typeof(InterLinqQueryHandler).GetMethod("Get", new Type[] { typeof(object), typeof(string), typeof(object), typeof(object[]) });    
#else
            getTableMethodWithoutPara = typeof(InterLinqQueryHandler).GetTypeInfo().GetDeclaredMethods("Get").FirstOrDefault(x=>x.GetParameters().Count() == 0);
            getTableMethod = typeof(InterLinqQueryHandler).GetTypeInfo().GetDeclaredMethods("Get").FirstOrDefault(x => x.GetParameters().Count() == 4);
#endif
        }

        #region Fields

        /// <summary>
        /// Gets the <see cref="IQueryProvider"/>.
        /// </summary>
        public abstract IQueryProvider QueryProvider { get; }

        #endregion

        #region IQueryHandler Members

        private Dictionary<Type, MethodInfo> genericMethodsCache1 = new Dictionary<Type, MethodInfo>();
        /// <summary>
        /// Returns an <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <param name="type">Type of the returned <see cref="IQueryable{T}"/>.</param>
        /// <returns>Returns an <see cref="IQueryable{T}"/>.</returns>
        public IQueryable Get(Type type)
        {
            MethodInfo genericGetTableMethod;

            if (!genericMethodsCache1.TryGetValue(type, out genericGetTableMethod))
            {
                genericGetTableMethod = getTableMethodWithoutPara.MakeGenericMethod(type);
                genericMethodsCache1.Add(type, genericGetTableMethod);
            }
            return (IQueryable)genericGetTableMethod.Invoke(this, new object[] { });
        }

        /// <summary>
        /// Returns an <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <typeparam name="T">Generic Argument of the returned <see cref="IQueryable{T}"/>.</typeparam>
        /// <returns>Returns an <see cref="IQueryable{T}"/>.</returns>
        public IQueryable<T> Get<T>() where T : class
        {
            return new InterLinqQuery<T>(QueryProvider);
        }

        /// <summary>
        /// Tells the <see cref="IQueryHandler"/> to start a new the session.
        /// </summary>
        /// <returns>True, if the session creation was successful. False, if not.</returns>
        /// <seealso cref="IQueryHandler.StartSession"/>
        public virtual object StartSession()
        {
            return new object();
        }

        /// <summary>
        /// Tells the <see cref="IQueryHandler"/> to close the current session.
        /// </summary>
        /// <returns>True, if the session closing was successful. False, if not.</returns>
        /// <seealso cref="IQueryHandler.CloseSession"/>
        public virtual bool CloseSession(object sessionObject)
        {
            return true;
        }

        private Dictionary<Type, MethodInfo> genericMethodsCache2 = new Dictionary<Type, MethodInfo>();
        /// <summary>
        /// Returns a <see cref="IQueryable{T}"/>
        /// </summary>
        /// <param name="type">Generic Argument of the returned <see cref="IQueryable{T}"/>.</param>
        /// <param name="name">The name of the query.</param>
        /// <param name="parameters">Parameters for the quey.</param>
        /// <returns>Returns a <see cref="IQueryable{T}"/>.</returns>
        public virtual IQueryable Get(Type type, object additionalObject, string name, object sessionObject, params object[] parameters)
        {
            MethodInfo genericGetTableMethod;

            if (!genericMethodsCache2.TryGetValue(type, out genericGetTableMethod))
            {
                genericGetTableMethod = getTableMethod.MakeGenericMethod(type);
                genericMethodsCache2.Add(type, genericGetTableMethod);
            }

            return (IQueryable)genericGetTableMethod.Invoke(this, new object[] { additionalObject, name, sessionObject, parameters });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">The entity used on <see cref="IQueryable{T}"/></typeparam>
        /// <param name="name">The name of the query.</param>
        /// <param name="parameters">Parameters for the quey.</param>
        /// <returns>Returns a <see cref="IQueryable{T}"/>.</returns>
        public virtual IQueryable<T> Get<T>(object additionalObject, string name, object sessionObject, params object[] parameters) where T : class
        {
            return new InterLinqQuery<T>(QueryProvider, null, additionalObject, name, parameters);               
        }

        #endregion
    }
}
