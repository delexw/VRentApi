using CF.VRent.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.DataAccessProxy.Entities
{
    [Serializable]
    [DataContract]
    public class ReturnResultExt:ReturnResult
    {
        [DataMember]
        public ProxyFapiaoRequest Data { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ReturnResultExtRetrieve : ReturnResult
    {
        [DataMember]
        public ProxyFapiaoRequest[] Data { get; set; }
    }


    [Serializable]
    [DataContract]
    public class ProxyReservation
    {
      //  [ID]
      //  ,[BookingType]
      //,[KemasBookingID]
      //,[KemasBookingNumber]
      //,[DateBegin]
      //,[DateEnd]
      //,[TotalAmount]
      //,[UserID]
      //,[UserFirstName]
      //,[UserLastName]
      //,[CorporateID]
      //,[CorporateName]
      //,[CreatorID]
      //,[CreatorFirstName]
      //,[CreatorLastName]
      //,[State]
      //,[CreatedOn]
      //,[CreatedBy]
      //,[ModifiedOn]
      //,[ModifiedBy]
      //,[StartLocationID]
      //,[StartLocationName]

        [DataMember]
        public int ProxyBookingID { get; set; }

        [DataMember]
        public int BillingOption { get; set; }

        [DataMember]
        public Guid KemasBookingID { get; set; }

        [DataMember]
        public string KemasBookingNumber { get; set; }

        //display purpose
        [DataMember]
        public DateTime? DateBegin { get; set; }

        [DataMember]
        public DateTime? DateEnd { get; set; }

        [DataMember]
        public Decimal? TotalAmount { get; set; }

        [DataMember]
        public Decimal IndirectFeeAmount { get; set; }

        [DataMember]
        public Decimal CurrentTotalAmount { get; set; }

        [DataMember]
        public string PricingDetail { get; set; }

        //,[UserID]
        //,[UserFirstName]
        //,[UserLastName]
        [DataMember]
        public Guid UserID { get; set; }
        [DataMember]
        public string UserFirstName { get; set; }
        [DataMember]
        public string UserLastName { get; set; }

        //,[CorporateID]
        //,[CorporateName]
        [DataMember]
        public string CorporateID { get; set; }
        [DataMember]
        public string CorporateName { get; set; }

        //,[StartLocationID]
        //,[StartLocationName]
        [DataMember]
        public Guid? StartLocationID { get; set; }
        [DataMember]
        public string StartLocationName { get; set; }

        //,[CreatorID]
        //,[CreatorFirstName]
        //,[CreatorLastName]
        [DataMember]
        public Guid? CreatorID { get; set; }
        [DataMember]
        public string CreatorFirstName { get; set; }
        [DataMember]
        public string CreatorLastName { get; set; }

        
        [DataMember]
        public string State { get; set; }

        [DataMember]
        public DateTime? CreatedOn { get; set; }

        [DataMember]
        public Guid? CreatedBy { get; set; }

        [DataMember]
        public DateTime? ModifiedOn { get; set; }

        [DataMember]
        public Guid? ModifiedBy { get; set; }


        public override bool Equals(object obj)
        {
            if (obj != null && obj is ProxyReservation)
            {
                ProxyReservation pr = obj as ProxyReservation;

                return GetHashCode().Equals(pr.GetHashCode());
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return ProxyBookingID;
        }
    }

    [Serializable]
    [DataContract]
    public class ProxyReservationsWithPaging
    {
        [DataMember]
        public int ItemsPerPage { get; set; }

        [DataMember]
        public int PageNumber { get; set; }

        [DataMember]
        public int TotalPage { get; set; }

        [DataMember]
        public string[] RawWhereConditions { get; set; }

        [DataMember]
        public string[] RawOrderByConditions { get; set; }

        [DataMember]
        public ProxyReservation[] Reservations { get; set; }
    }
}
