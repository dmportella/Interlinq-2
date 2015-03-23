using System;
using System.ServiceModel;
using InterLinq.Expressions;
using System.Collections.Generic;
using System.Reflection;

namespace InterLinq
{
    /// <summary>
    /// Interface providing methods to communicate with
    /// the InterLINQ service.
    /// </summary>
    [ServiceContract(Namespace="http://schemas.interlinq.com/2011/03/server")]
    [ServiceKnownType("GetKnownTypes", typeof(Communication.InterLinqKnowTypes))]
    public interface IQueryRemoteHandler
    {
        /// <summary>
        /// Retrieves data from the server by an <see cref="SerializableExpression">Expression</see> tree.
        /// </summary>
        /// <remarks>
        /// This method's return type depends on the submitted 
        /// <see cref="SerializableExpression">Expression</see> tree.
        /// Here some examples ('T' is the requested type):
        /// <list type="list">
        ///     <listheader>
        ///         <term>Method</term>
        ///         <description>Return Type</description>
        ///     </listheader>
        ///     <item>
        ///         <term>Select(...)</term>
        ///         <description>T[]</description>
        ///     </item>
        ///     <item>
        ///         <term>First(...), Last(...)</term>
        ///         <description>T</description>
        ///     </item>
        ///     <item>
        ///         <term>Count(...)</term>
        ///         <description><see langword="int"/></description>
        ///     </item>
        ///     <item>
        ///         <term>Contains(...)</term>
        ///         <description><see langword="bool"/></description>
        ///     </item>
        /// </list>
        /// </remarks>
        /// <param name="expression">
        ///     <see cref="SerializableExpression">Expression</see> tree 
        ///     containing selection and projection.
        /// </param>
        /// <returns>Returns requested data.</returns>
#if !SILVERLIGHT        
        [OperationContract]
        [FaultContract(typeof(Exception))]
        object Retrieve(SerializableExpression expression);
#else
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(Exception))]
        IAsyncResult BeginRetrieve(SerializableExpression expression, AsyncCallback callback, object state);

        object EndRetrieve(IAsyncResult result);
#endif
    }
}
