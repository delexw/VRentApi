using CF.VRent.BLL.BLLFactory;
using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Log;
using CF.VRent.UPSDK;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF.VRent.BLL.ReservationAOP
{
    public class ReservationInterceptionBehavior : IInterceptionBehavior
    {
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var returnObj = getNext()(input, getNext);

            //Exception is threw by the function. invoke CancelPreauthOnce in PaymentBLL
            if (input.MethodBase.Name == "CreateReservation")
            {
                if (returnObj.Exception != null)
                {
                    var user = ServiceUtility.RetrieveUserInfoFromSession();
                    Task.Factory.StartNew(() =>
                    {
                        var booking = input.Arguments[1] as KemasReservationEntity;
                        try
                        {
                            //Preauth cancellation
                            if (BookingUtility.TransformToProxyBookingType(booking.BillingOption.ToString()) == BookingType.Private)
                            {
                                var pbll = ServiceImpInstanceFactory.CreatePaymentInstance(new ProxyUserSetting());
                                var payment = pbll.GetPaymentExchangeInfo(booking.UPPaymentID, user.ID);
                                pbll.CancelPreauth(0, payment.PaymentID, payment.PreAuthPrice,
                                    new ProxyUserSetting()
                                    {
                                        ID = user.ID,
                                        Name = user.Name,
                                        VName = user.VName,
                                        Mail = user.Mail
                                    });
                            }
                        }
                        catch (Exception ex)
                        {
                            //Preauth cancellation
                            LogInfor.WriteError(MessageCode.CVB000022.ToStr(), String.Format("User:{0},UserID:{1},BookingNo:{2},Message:{3}",
                                booking.CreatedBy, booking.CreatorID, booking.Number, ex.ToString()), booking.CreatorID);
                        }
                    }, TaskCreationOptions.PreferFairness);
                }
            }
            else if (input.MethodBase.Name == "ProxyReservationDetail")
            {
                if (returnObj.Exception == null)
                {
                    if (returnObj.ReturnValue is KemasReservationEntity)
                    {
                        var ke = (KemasReservationEntity)returnObj.ReturnValue;
                        var pbll = ServiceImpInstanceFactory.CreatePaymentInstance(new ProxyUserSetting());
                        var status = pbll.GetPaymentStatusByBookingId(ke.ProxyBookingID);
                        if (status > -2)
                        {
                            ke.PaymentStatus.RestfulResult = new Common.Entities.ReturnResult(false)
                            {
                                Code = status.ToString(),
                                Message = ((PaymentStatusEnum)Enum.Parse(typeof(PaymentStatusEnum), status.ToString())).GetDescription(),
                                Success = 1
                            };
                        }
                        else
                        {
                            ke.PaymentStatus.RestfulResult = new ReturnResult(false) { Message = "No status", Code = "" };
                        }
                    }
                }
            }

            return returnObj;
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}
