using CF.VRent.Common;
using CF.VRent.UPSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.DataAccessProxy.Entities
{
    [DataContract]
    public class DUBDetailSearchConditions 
    {
        [DataMember]
        public string KemasBookingNumber { get; set; }

        [DataMember]
        public Guid? UserID { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public DateTime? DateBegin { get; set; }

        [DataMember]
        public DateTime? DateEnd { get; set; }

        [DataMember]
        public UPProcessingState? UPState { get; set; }

        [DataMember]
        public int ItemsPerPage { get; set; }

        [DataMember]
        public int PageNumber { get; set; }

        [DataMember]
        public int TotalPages { get; set; }

        [DataMember]
        public DUBDetail[] Items { get; set; }

    }

    #region UPState
    public enum UPProcessingState { Processing = 0, PreAuthSuccess,PreAuthFail, PreAuthCancelSuccess, PreAuthCancelFail, FeeDeductionSuccess, FeeDeductionFail, NoFee };
        //[StatusGroupAttribute(Description = "NULL")]
        //NULL = -2,
        //[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
        //BeforePreAuth = -1,
        //[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
        //PreAuthorizing = 0,
        //[StatusGroupAttribute(Description = "Pre-authorization - Success", IsMiddleStatus = false, IsSuccessStatus = true)]
        //PreAuthorized = 1,
        //[StatusGroupAttribute(Description = "Pre-authorization - Failed", IsMiddleStatus = false, IsFailedStatus = true)]
        //PreAuthFailed = 2,

        //[StatusGroupAttribute(Description = "Pre-authorization cancellation - Success", IsMiddleStatus = false, IsSuccessStatus = true)]
        //PreAuthCanceled = 3,
        //[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
        //PreAuthCancelling = 4,
        //[StatusGroupAttribute(Description = "Pre-authorization cancellation - Failed", IsMiddleStatus = false, IsFailedStatus = true)]
        //PreAuthCancelFailed = 5,
        //[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
        //PreAuthCompleting = 6,
        //[StatusGroupAttribute(Description = "Fee deduction - Failed", IsMiddleStatus = false, IsFailedStatus = true)]
        //PreAuthCompleteFailed = 7,
        //[StatusGroupAttribute(Description = "Fee deduction - Success", IsMiddleStatus = false, IsSuccessStatus = true)]
        //PreAuthCompleted = 8,

        //[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
        //PreAuthRetryShortTime = 9,
        //[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
        //PreAuthCancelRetryShortTime = 10,
        //[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
        //PreAuthCompleteRetryShortTime = 11,

        ////Deduction
        //[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
        //PreDeduction = 12,
        //[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
        //Deducting = 13,
        //[StatusGroupAttribute(Description = "Fee deduction - Success", IsMiddleStatus = false, IsSuccessStatus = true)]
        //Deducted = 14,
        //[StatusGroupAttribute(Description = "Fee deduction - Failed", IsMiddleStatus = false, IsFailedStatus = true)]
        //DeductionFailed = 15,

        //[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
        //PreAuthCancelRetryLongTime = 16,
        //[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
        //PreAuthCompleteRetryLongTime = 17,
        //[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
        //DeductionRetryShortTime = 18,
        //[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
        //DeductionRetryLongTime = 19,
        //[StatusGroupAttribute(Description = "Processing", IsMiddleStatus = true)]
        //PreAuthRetryLongTime = 20,
        //[StatusGroupAttribute(Description = "No fee", IsMiddleStatus = false)]
        //NoFee = 21
    #endregion

    [DataContract]
    public class DUBDetail 
    {
      // [ID]
      //,[DUBClosingID]
      //,[BookingID]
      //,[KemasBookingID]
      //,[KemasBookingNumber]
      //,[UserID]
      //,[UserFirstName]
      //,[UserLastName]
      //,[OrderDate]
      //,[PaymentID]
      //,[OrderID]
      //,[OrderItemID]
      //,[Category]
      //,[TotalAmount]
      //,[State]
      //,[CreatedOn]
      //,[CreatedBy]
      //,[ModifiedOn]
      //,[ModifiedBy]

        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public int BookingID { get; set; }

        [DataMember]
        public Guid KemasBookingID { get; set; }

        [DataMember]
        public string KemasBookingNumber { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public Guid UserID { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public DateTime OrderDate { get; set; }

        [DataMember]
        public int? PaymentID { get; set; }

        [DataMember]
        public UPProcessingState? UPstate { get; set; }
        
        [DataMember]
        public int? OrderID { get; set; }

        [DataMember]
        public string Category { get; set; }

        [DataMember]
        public decimal? TotalAmount { get; set; }
    }
}
