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
    public class ReturnResultAddIndirectFeeItems : ReturnResult
    {
        [DataMember]
        public int AffectedCnt { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ReturnResultRetrieveOrderItems : ReturnResult
    {
        [DataMember]
        public ProxyOrderItem[] Data { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ReturnResultBulkSink : ReturnResult
    {
        [DataMember]
        public int UpdatedCnt { get; set; }

        [DataMember]
        public int InsertedCnt { get; set; }

        [DataMember]
        public ProxyReservation[] SyncData { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ProxyOrder
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public int ProxyBookingID { get; set; }
        [DataMember]
        public string BookingUserID { get; set; }
        [DataMember]
        public int State { get; set; }
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
    [Serializable]
    public class ProxyOrderItem
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public int OrderID { get; set; }

        [DataMember]
        public string ProductCode { get; set; }     //length:20

        [DataMember]
        public string ProductName { get; set; }    //length:20

        [DataMember]
        public string SpecMode { get; set; } //length:20

        [DataMember]
        public string Category { get; set; } //length:20

        [DataMember]
        public int? TypeID { get; set; } //length:20

        [DataMember]
        public string Type { get; set; } //length:20

        [DataMember]
        public string UnitMeasure { get; set; }//length:10

        [DataMember]
        public decimal? SalesQuantity { get; set; } //numeruc(3,0) default 1

        [DataMember]
        public decimal? UnitPrice { get; set; }  //numeric(14,2)

        [DataMember]
        public decimal? AmountExclVAT { get; set; }  //amount numeric(14,2)

        [DataMember]
        public decimal? TaxRate { get; set; }    //tax rate numeric(5,4)

        [DataMember]
        public decimal? Tax { get; set; }    //numeric(14,2)

        [DataMember]
        public decimal? AmountIncVAT { get; set; }  //numeric(14,2)

        [DataMember]
        public string Remark { get; set; }  //nvarchar(50)

        [DataMember]
        public Int16 State { get; set; }
        [DataMember]
        public DateTime? CreatedOn { get; set; }
        [DataMember]
        public Guid? CreatedBy { get; set; }
        [DataMember]
        public DateTime? ModifiedOn { get; set; }
        [DataMember]
        public Guid? ModifiedBy { get; set; }

    }

    [Serializable]
    [DataContract]
    public class ProxyBookingPrice 
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public int ProxyBookingID { get; set; }

        [DataMember]
        public decimal Total { get; set; }

        [DataMember]
        public string Timestamp { get; set; }

        [DataMember]
        public string TagID { get; set; }
        
        [DataMember]
        public ProxyPrincingItem[] PrincingItems { get; set; }

        [DataMember]
        public Int16 State { get; set; }

        [DataMember]
        public DateTime? CreatedOn { get; set; }
        
        [DataMember]
        public Guid? CreatedBy { get; set; }
        
        [DataMember]
        public DateTime? ModifiedOn { get; set; }
        
        [DataMember]
        public Guid? ModifiedBy { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ProxyPrincingItem
    {
        [DataMember]
        public int ID { get; set; }
        
        [DataMember]
        public int BookingPriceID { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Group { get; set; }

        [DataMember]
        public string Category { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public decimal? UnitPrice { get; set; }

        [DataMember]
        public decimal? Quantity { get; set; }
        
        [DataMember]
        public decimal Total { get; set; }

        [DataMember]
        public int State { get; set; }

        [DataMember]
        public DateTime? CreatedOn { get; set; }
        
        [DataMember]
        public Guid? CreatedBy { get; set; }
        
        [DataMember]
        public DateTime? ModifiedOn { get; set; }
        
        [DataMember]
        public Guid? ModifiedBy { get; set; }
    }


    [Serializable]
    [DataContract]
    public class ProxyBookingPayment
    {
        [DataMember]
        public int ID { get; set; }
     
        [DataMember]
        public int ProxyBookingID { get; set; }

        [DataMember]
        public int UPPaymentID { get; set; }

        [DataMember]
        public int State { get; set; }
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
