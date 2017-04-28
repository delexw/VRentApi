using CF.VRent.Entities.DataAccessProxyWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Common;
using CF.VRent.UPSDK.Entities;
using CF.VRent.UPSDK;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Log;
using CF.VRent.Common.Entities;
using CF.VRent.Entities.PaymentService;

namespace CF.VRent.BLL.BLLFactory.Payment
{
    public class CreateOrder : ICreateOrder
    {
        public void Create(PaymentExchangeMessage payment, string operatorId)
        {
            //Get exchange message
            DataAccessProxyManager manager = new DataAccessProxyManager();
            var exchangeMessage = payment;

            if (!String.IsNullOrWhiteSpace(exchangeMessage.Message))
            {
                UnionPayExchangeMessage exchangeDic = exchangeMessage.Message.JsonDeserialize<UnionPayExchangeMessage>();
                this.UPMessage = exchangeDic;

                //only avaliable for fee dedcution op
                if (String.Equals(PayOperationEnum.FeeDeduction.ToString(), exchangeDic.type))
                {
                    //Add orders
                    manager.AddOrderAfterPayment(new ProxyOrder()
                    {
                        ProxyBookingID = exchangeDic.bookingId.ToInt(),
                        BookingUserID = exchangeDic.userId,
                        State = CommonState.Active.GetValue(),
                        CreatedBy = exchangeDic.adminUserId.ToGuidNull()
                    }, operatorId);
                }
            }
            else
            {
                LogInfor.WriteInfo(MessageCode.CVB000024.ToStr(), String.Format(MessageCode.CVB000024.GetDescription(), exchangeMessage.PaymentID), operatorId);
                this.UPMessage = new UnionPayExchangeMessage();
            }
        }

        public UnionPayExchangeMessage UPMessage
        {
            get;
            private set;
        }
    }
}
