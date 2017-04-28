using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.DataAccessProxy.Entities
{
    public enum StagingState { Created = 0,Merged,Discarded,Archived };
    public enum MatchState {Unknown = 0, NoDifference = 1, Missing, StateMisMatch, IDMismatch, MinorFieldsMisMatch, PriceMisMatch };

    [DataContract]
    public class StagedBookings
    {
        [DataMember]
        public CompletedBooking[] Items { get; set; }

        [DataMember]
        public DateTime BeginDate { get; set; }

        [DataMember]
        public DateTime EndDate { get; set; }
    }

    [DataContract]
    public class CompletedBooking
    {
        [DataMember]
        public int ID { get; set; }
        // ID string (unique internal reservation id)
        // DateBegin string (date when reservation starts in format YYYY-MM-DD HH:ii:ss )
        // DateEnd string (date when reservations ends in format YYYY-MM-DD HH:ii:ss )
        // StartLocation Structure type Location

        [DataMember]
        public Guid KemasBookingID { get; set; }

        [DataMember]
        public int? BookingID { get; set; }

        [DataMember]
        public DateTime? DateBegin { get; set; }
        [DataMember]
        public DateTime? DateEnd { get; set; }
        [DataMember]
        public Guid? StartLocationID { get; set; }
        [DataMember]
        public string StartLocationName { get; set; }

        // BillingOption Structure type BillingOption
        [DataMember]
        public int BillingOption { get; set; }
        // Category string (name of category)
        [DataMember]
        public string Category { get; set; }
        // Creator Structure type UserData
        [DataMember]
        public Guid? CreatorID { get; set; }
        // Driver Structure type UserData
        [DataMember]
        public Guid UserID { get; set; }
        [DataMember]
        public string CorporateID { get; set; }

        [DataMember]
        public Guid? CarID { get; set; }

        [DataMember]
        public string CarName { get; set; }

        [DataMember]
        public string CorporateName { get; set; }
        // CarID string (unique internal number of vehicle)
        // Car string (name of vehicle in format: model [license plate])
        // Number string (booking number, if congure it will be auto gen-erated)
        [DataMember]
        public string KemasBookingNumber { get; set; }
        // PickupBegin string (date when vehicle key can be taken from ter-minal in format YYYY-MM-DD HH:ii:ss )
        // PickupEnd string (date until vehicle key can be taken from termi-nal in format YYYY-MM-DD HH:ii:ss )
        // KeyOut string (date when vehicle key was taken from terminal in format YYYY-MM-DD HH:ii:ss )
        // KeyIn string (date when vehicle key was returned to terminal in format YYYY-MM-DD HH:ii:ss )
        [DataMember]
        public DateTime? PickupBegin { get; set; }
        [DataMember]
        public DateTime? PickupEnd { get; set; }
        
        [DataMember]
        public DateTime? KeyIn { get; set; }
        [DataMember]
        public DateTime? KeyOut { get; set; }
        // State string (state of reservation for meaning see states of reservation)
        [DataMember]
        public string State { get; set; }
        // Price string total costs of booking
        // PriceDetail string xml string of detailed costs
        [DataMember]
        public Decimal? Price { get; set; }
        [DataMember]
        public string PricingDetail { get; set; }

        [DataMember]
        public string PaymentStatus { get; set; }

        [DataMember]
        public StagingState SyncState { get; set; }

        [DataMember]
        public MatchState CompareResult { get; set; } 

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
    public class RangedBookings 
    {
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }

        public CompletedBooking[] BookingsToClose { get; set; }
    }
}
