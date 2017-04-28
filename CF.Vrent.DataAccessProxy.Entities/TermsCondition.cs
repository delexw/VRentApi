using CF.VRent.Common.Entities.DBEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.DataAccessProxy.Entities
{
    public class TermsCondition : DBEntityObject
    {
        #region Model
        [DataMember]
        public int ID
        {
            get;
            set;
        }

        [DataMember]
        public int IsActive
        {
            get;
            set;
        }
        
        [DataMember]
        public DateTime? CreatedOn
        {
            get;
            set;
        }
        [DataMember]
        public Guid? CreatedBy
        {
            get;
            set;
        }
       [DataMember]
        public DateTime? ModifiedOn
        {
            get;
            set;
        }
        [DataMember]
        public Guid? ModifiedBy
        {
            get;
            set;
        }
        [DataMember]
        public string Title
        {
            get;
            set;
        }
        [DataMember]
        public string DisplayTitle
        {
            get;
            set;
        }
        [DataMember]
        public string Content
        {
            get;
            set;
        }
        [DataMember]
        public Guid Key
        {
            get;
            set;
        }
        [DataMember]
        public int Type
        {
            get;
            set;
        }
        [DataMember]
        public int Category
        {
            get;
            set;
        }
        [DataMember]
        public string Version
        {
            get;
            set;
        }
        [DataMember]
        public int IsNewVersion
        {
            get;
            set;
        }
        #endregion Model
    }

    public class UserTermsConditionAgreement:DBEntityObject
    {
        [DataMember]
        public int ID
        {
            get;
            set;
        }
        [DataMember]
        public Guid UserID
        {
            get;
            set;
        }
        [DataMember]
        public int TCID
        {
            get;
            set;
        }
        [DataMember]
        public DateTime? CreatedOn
        {
            get;
            set;
        }
        [DataMember]
        public Guid? CreatedBy
        {
            get;
            set;
        }
        [DataMember]
        public DateTime? ModifiedOn
        {
            get;
            set;
        }
        [DataMember]
        public Guid? ModifiedBy
        {
            get;
            set;
        }

    }
}
