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
    /// Base Class for InterLinq Queries.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    [DataContract(Namespace = "http://schemas.interlinq.com/2011/03/")]
    public abstract class InterLinqQueryBase
    {
        #region Properties

        /// <summary>
        /// The name of the query. Used for named queries on the server.
        /// </summary>
        [DataMember(Order = 1)]
        public string QueryName
        { get; set; }

        /// <summary>
        /// The parameters for the query. User for named queries on the server.
        /// </summary>
        [DataMember(Order = 1)]
        public List<SerializableExpression> QueryParameters
        { get; set; }

        #region Property ElementType

        /// <summary>
        /// See <see cref="Type">Element Type</see> of the <see cref="Expression"/>.
        /// </summary>
#if !SILVERLIGHT
        [NonSerialized]
#endif
        protected Type elementType;
        /// <summary>
        /// Gets or sets a <see cref="Type">Element Type</see> of the <see cref="Expression"/>.
        /// </summary>
        public abstract Type ElementType { get; }

        #endregion

        #region Property InterLinqElementType

        /// <summary>
        /// See <see cref="InterLinqType">InterLINQ Element Type</see> of the <see cref="Expression"/>.
        /// </summary>
        protected InterLinqType elementInterLinqType;
        /// <summary>
        /// Gets or sets a <see cref="InterLinqType">InterLINQ Element Type</see> of the <see cref="Expression"/>.
        /// </summary>
        [DataMember]
        public InterLinqType ElementInterLinqType
        {
            get { return elementInterLinqType; }
            set { elementInterLinqType = value; }
        }

        #endregion

        #endregion
    }
}
