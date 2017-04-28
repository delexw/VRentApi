using CF.VRent.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.DataAccessProxy.Entities
{
    public enum UserTransferState { Active = 0, Completed = 1};
    public enum UserTransferResult {Pending = 2, Approve = 0, Reject = 1 };


    [Serializable]
    [DataContract]
    public class UserTransferCUDResult: ReturnResult
    {
        [DataMember]
        public UserTransferRequest Data { get; set; }
    }

    [Serializable]
    [DataContract]
    public class UserTransferRResult : ReturnResult
    {
        [DataMember]
        public UserTransferRequest[] Data { get; set; }
    }

    [Serializable]
    [DataContract]
    public class UserTransferRequest
    {
//[ID]
//      ,[UserID]
//      ,[FirstName]
//      ,[LastName]
//      ,[Mail]
//      ,[ClientIDFrom]
//      ,[ClientIDTo]
//      ,[ApproverID]
//      ,[TransferResult]
//      ,[State]
//      ,[CreatedOn]
//      ,[CreatedBy]
//      ,[ModifiedOn]
//      ,[ModifiedBy]


        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public Guid UserID { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string Mail { get; set; }

        [DataMember]
        public Guid? ClientIDFrom { get; set; }
        [DataMember]
        public Guid? ClientIDTo { get; set; }

        [DataMember]
        public Guid? ApproverID { get; set; }

        [DataMember]
        public UserTransferResult TransferResult { get; set; }

        [DataMember]
        public UserTransferState State { get; set; }
        [DataMember]
        public DateTime? CreatedOn { get; set; }
        [DataMember]
        public Guid? CreatedBy { get; set; }
        [DataMember]
        public DateTime? ModifiedOn { get; set; }
        [DataMember]
        public Guid? ModifiedBy { get; set; }
    }
}
