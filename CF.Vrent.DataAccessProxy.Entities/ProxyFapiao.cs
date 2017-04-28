using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CF.VRent.DataAccessProxy.Entities
{
    /// <summary>
    /// Fapiao Entity
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProxyFapiao
    {
        #region FapiaoData Table
        [DataMember]
        public int ID { get; set; } //DB

        [DataMember]
        public int OrderID { get; set; } //DB order id or order item id
        #endregion

        [DataMember]
        public string UniqueID { get; set; }    // length: 20

        [DataMember]
        public string DealNumber { get; set; }  // length:20 , kemas Booking number

        [DataMember]
        public string ContractNumber { get; set; } //length:20 can be empty

        #region Kemas User Info
        [DataMember]
        public string CustomerCode { get; set; } //length:20, hold an user ID, currently it is a guid

        [DataMember]
        public string CustomerName { get; set; }  //length:80
        [DataMember]
        public string CustomerAddress { get; set; }  //length:120 kemas user info

        [DataMember]
        public string CustomerPhone { get; set; }  //length:20 kemas user info        

        [DataMember]
        public string TaxRegistrationID { get; set; }  //length:20

        [DataMember]
        public string BankName { get; set; }    //length :80

        [DataMember]
        public string BankAccount { get; set; } //length : 80

        #endregion

        #region FapiaoPreference ID
        [DataMember]
        public string FPCustomerName { get; set; }  //length:80

        [DataMember]
        public string FPMailType { get; set; } //express length: 80
        
        [DataMember]
        public string FPMailingAddress { get; set; } //length: 80  
        
        [DataMember]
        public string FPMailingPhone { get; set; } //length:20
    
        [DataMember]
        public string FPAddresseeName { get; set; } //length: 50
        #endregion

        #region Order or Order Item 
        [DataMember]
        public string ProductCode { get; set; }     //length:20

        [DataMember]
        public string ProductName { get; set; }    //length:20

        [DataMember]
        public string SpecMode { get; set; } //length:20

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

        #endregion
 
        [DataMember]
        public int FapiaoType { get; set; }//numeric(3,0)
        
        [DataMember]
        public string Remark { get; set; }  //nvarchar(230)

        #region Fapiao Printing System

        [DataMember]
        public int FapiaoNumber { get; set; }

        [DataMember]
        public int FapiaoCode { get; set; }

        [DataMember]
        public DateTime? IssueDate { get; set; }

        [DataMember]
        public string MailID { get; set; }

        #endregion

        [DataMember]
        public int FapiaoState { get; set; }

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
            if (obj != null && obj is ProxyFapiao)
            {
                ProxyFapiao pfdata = obj as ProxyFapiao;
                return ID.Equals(pfdata.ID);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return ID;
        }
    }
}
