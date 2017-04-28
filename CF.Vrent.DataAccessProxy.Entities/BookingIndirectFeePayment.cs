using CF.VRent.Common.Entities.DBEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.DataAccessProxy.Entities
{
    //BookingIndirectFeePayment
    public class BookingIndirectFeePayment : DBEntityObject
    {

        /// <summary>
        /// ID
        /// </summary>
        [DataMember]
        [Key]
        public virtual int ID
        {
            get;
            set;
        }
        /// <summary>
        /// BookingID
        /// </summary>
        [DataMember]
        public virtual int BookingID
        {
            get;
            set;
        }
        /// <summary>
        /// OrderItemID
        /// </summary>
        [DataMember]
        public virtual int OrderItemID
        {
            get;
            set;
        }
        /// <summary>
        /// UPPaymentID
        /// </summary>
        [DataMember]
        public virtual int UPPaymentID
        {
            get;
            set;
        }
        /// <summary>
        /// State
        /// </summary>
        [DataMember]
        public virtual int State
        {
            get;
            set;
        }
        /// <summary>
        /// CreateOn
        /// </summary>
        [DataMember]
        public virtual DateTime? CreateOn
        {
            get;
            set;
        }
        /// <summary>
        /// CreatedBy
        /// </summary>
        [DataMember]
        public virtual Guid? CreatedBy
        {
            get;
            set;
        }
        /// <summary>
        /// ModifiedOn
        /// </summary>
        [DataMember]
        public virtual DateTime? ModifiedOn
        {
            get;
            set;
        }
        /// <summary>
        /// ModifiedBy
        /// </summary>
        [DataMember]
        public virtual Guid? ModifiedBy
        {
            get;
            set;
        }
    }
}
