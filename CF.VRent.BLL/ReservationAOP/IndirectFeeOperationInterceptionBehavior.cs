using CF.VRent.Entities.DataAccessProxyWrapper;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Common;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Common.UserContracts;
using System.Threading.Tasks;
using CF.VRent.Log;
using CF.VRent.Common.Entities;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Email.EmailSender.Entity;
using CF.VRent.Email.EmailSender.Payment;
using CF.VRent.Email.EmailSender;
using CF.VRent.Email;

namespace CF.VRent.BLL.ReservationAOP
{
    public class IndirectFeeOperationInterceptionBehavior : IInterceptionBehavior
    {
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var ret = getNext()(input, getNext);

            if (ret.Exception == null)
            {
                if (input.MethodBase.Name == "AddOrderItems")
                {
                    Task.Factory.StartNew(() =>
                    {
                        ProxyUserSetting loginUser = new ProxyUserSetting();
                        try
                        {
                            //Send email to inform user
                            DataAccessProxyManager ds = new DataAccessProxyManager();
                            var booking = ds.RetrieveReservationByBookingID(input.Arguments[0].ToInt());
                            //Only for dub bookings
                            if (booking.BillingOption == BookingType.Private.GetValue())
                            {
                                var api = KemasAccessWrapper.CreateKemasUserAPI2Instance();

                                var mail = "";
                                var userProperty = input.Target.GetType().GetProperty("UserInfo");
                                if (userProperty != null)
                                {
                                    var propertyValue = userProperty.GetValue(input.Target, null);
                                    if (propertyValue != null)
                                    {
                                        loginUser = propertyValue as ProxyUserSetting;
                                        var kemasUser = api.findUser2(booking.UserID.ToStr(), loginUser.SessionID);
                                        if (kemasUser.UserData != null)
                                        {
                                            mail = kemasUser.UserData.Mail;
                                            ProxyOrderItem[] itmes = input.Arguments[1] as ProxyOrderItem[];
                                            var price = itmes.Sum(r => r.AmountIncVAT);

                                            //Send email
                                            IIndirectFeeRemainder sender = EmailSenderFactory.CreateIndirectFeeRemainderSender();
                                            sender.onSendEvent += sender_onSendEvent;
                                            sender.Send(new EmailParameterEntity()
                                            {
                                                LastName = kemasUser.UserData.VName,
                                                FirstName = kemasUser.UserData.Name,
                                                Price = price.GetValueOrDefault().ToStr()
                                            }, mail);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //Email
                            LogInfor.EmailLogWriter.WriteError(MessageCode.CVB000032.ToStr(),
                                String.Format("Exception:{0}", ex.ToStr()), loginUser.ID);
                        }
                    }, TaskCreationOptions.PreferFairness);
                }
            }
            return ret;
        }

        void sender_onSendEvent(EmailParameterEntity arg1, EmailType arg2, string[] arg3)
        {
            DataAccessProxyManager ds = new DataAccessProxyManager();
            ds.SendPaymentEmail(arg1, arg2.ToStr(), arg3);
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}
