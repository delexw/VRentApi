using CF.VRent.Entities.PaymentService;
using CF.VRent.UPSDK.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.Payment
{
    public interface ICreateOrder
    {
        /// <summary>
        /// After create method is called, the UPMessage property would have value
        /// </summary>
        UnionPayExchangeMessage UPMessage { get; }
        void Create(PaymentExchangeMessage payment, string operatorId);
    }
}
