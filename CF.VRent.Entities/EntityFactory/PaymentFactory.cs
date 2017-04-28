using CF.VRent.Entities.PaymentService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Entities.EntityFactory
{
    public class PaymentFactory
    {
        /// <summary>
        /// Create a new entity with the value of PaymentID, LastPaymentID,Operation,State,CreatedOn,Retry,UniqueID,UserID
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public virtual PaymentExchangeMessage CreateFilterEntity(PaymentExchangeMessage original)
        {
            return new PaymentExchangeMessage() { 
                PaymentID = original.PaymentID,
                LastPaymentID = original.LastPaymentID,
                Operation = original.Operation,
                State = original.State,
                CreatedOn = original.CreatedOn,
                Retry = original.Retry,
                UniqueID = original.UniqueID,
                UserID = original.UserID
            };
        }
    }
}
