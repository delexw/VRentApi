using CF.VRent.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.DataAccessProxy.Entities
{
    public enum PaymentState { Unknown = -1, Pending = 0, Due = 1, OverDue = 2, Paid = 3 };
    public enum ItemCategory { RentalFee = 0, IndirectFee = 1 };
    public enum SyncedRecordState { UnKnown = -1, NotRun = 0, Preview = 1, Final = 2, Closed = 3 };

    [DataContract]
    public class DebitNotePeriod 
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Period { get; set; }

        [DataMember]
        public DateTime PeriodStartDate { get; set; }

        [DataMember]
        public DateTime PeriodEndDate { get; set; }

        [DataMember]
        public DateTime BillingDate { get; set; }

        [DataMember]
        public DateTime DueDate { get; set; }        
        
        [DataMember]
        public SyncedRecordState State { get; set; }

        [DataMember]
        public DateTime? CreatedOn { get; set; }

        [DataMember]
        public Guid? CreatedBy { get; set; }

        [DataMember]
        public DateTime? ModifiedOn { get; set; }

        [DataMember]
        public Guid? ModifiedBy { get; set; }
    }

    [DataContract]
    public class DebitNotesSearchConditions
    {
        //@clientName nvarchar(50),
        //@status tinyint,
        //@PeriodBegin Datetime,
        //@PeriodEnd datetime
        [DataMember]
        public Guid? ClientID { get; set; }

        [DataMember]
        public string ClientName { get; set; }
        
        
        [DataMember]
        public PaymentState? Status { get; set; }

        [DataMember]
        public DateTime? PeriodBegin { get; set; }

        [DataMember]
        public DateTime? PeriodEnd { get; set; }

        [DataMember]
        public DateTime QueryTime { get; set; }

        [DataMember]
        public int ItemsPerPage { get; set; }

        [DataMember]
        public int PageNumber { get; set; }

        [DataMember]
        public int TotalPages { get; set; }

        [DataMember]
        public DebitNote[] Notes { get; set; }
    }

    [DataContract]
    public class DebitNote
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public Guid ClientID { get; set; }

        [DataMember]
        public int PeriodID { get; set; }

        [DataMember]
        public string ClientName { get; set; }

        [DataMember]
        public string ContactPerson { get; set; }

        [DataMember]
        public string Period { get; set; }

        [DataMember]
        public DateTime PeriodStartDate { get; set; }

        [DataMember]
        public DateTime PeriodEndDate { get; set; }

        //redundant data
        [DataMember]
        public DateTime BillingDate { get; set; }

        [DataMember]
        public DateTime DueDate { get; set; }

        [DataMember]
        public DateTime? PaymentDate { get; set; }

        [DataMember]
        public decimal TotalAmount { get; set; }

        [DataMember]
        public PaymentState PaymentStatus { get; set; }

        [DataMember]
        public string Note { get; set; }

        [DataMember]
        public CommonState State { get; set; }

        [DataMember]
        public DateTime? CreatedOn { get; set; }

        [DataMember]
        public Guid? CreatedBy { get; set; }

        [DataMember]
        public DateTime? ModifiedOn { get; set; }

        [DataMember]
        public Guid? ModifiedBy { get; set; }
    }

    [DataContract]
    public class DebitNoteDetailSearchConditions
    {
        [DataMember]
        public int DebitNoteID { get; set; }

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
        public int ItemsPerPage { get; set; }

        [DataMember]
        public int PageNumber { get; set; }

        [DataMember]
        public int TotalPage { get; set; }

        [DataMember]
        public DebitNoteDetail[] Items { get; set; }
    }

    [DataContract]
    public class BookingCompact 
    {
        [DataMember]
        public int? BookingID { get; set; }

        [DataMember]
        public Guid KemasBookingID { get; set; }

        [DataMember]
        public int? OrderID { get; set; }
    }

    [DataContract]
    public class DebitNoteDetail
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public int DebitNoteID { get; set; }

        [DataMember]
        public Guid ClientID { get; set; }

        [DataMember]
        public Guid UserID { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public int BookingID { get; set; }

        [DataMember]
        public Guid KemasBookingID { get; set; }

        [DataMember]
        public string KemasBookingNumber { get; set; }
        
        [DataMember]
        public int? OrderID { get; set; }

        [DataMember]
        public string ItemCategory { get; set; }

        [DataMember]
        public DateTime OrderDate { get; set; }

        [DataMember]
        public decimal? TotalAmount { get; set; }

        [DataMember]
        public PaymentState PaymentStatus { get; set; }
    }


}
