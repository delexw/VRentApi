using CF.VRent.BLL.BLLFactory;
using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.Entities.EntityFactory;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Log;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CF.VRent.Common;
using CF.VRent.Email.EmailSender.Entity;
using CF.VRent.Email;
using CF.VRent.UserStatus;
using CF.VRent.Email.EmailSender.Clients;
using CF.VRent.Email.EmailSender;

namespace CF.VRent.BLL.ReservationAOP
{
    public class ClientInterceptionBehavior : IInterceptionBehavior
    {
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            string clientId = "";
            if (input.MethodBase.Name == "EnableDisableCompany")
            {
                clientId = input.Arguments[0].ToStr();
            }

            var ret = getNext()(input, getNext);

            if (input.MethodBase.Name == "EnableDisableCompany" && ret.Exception == null)
            {
                var company = ret.ReturnValue as UserCompanyExtenstion;

                if (company.Status == "0")
                {
                    var api = KemasAccessWrapper.CreateKemasUserAPI2Instance();
                    //Get all users in this company
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            var userProperty = input.Target.GetType().GetProperty("UserInfo");
                            var propertyValue = userProperty.GetValue(input.Target, null) as ProxyUserSetting;

                            var page = 0;
                            var pageSize = 10;
                            var userFactory = new UserFactory();
                            var loginUser = userFactory.CreateEntity(propertyValue);
                            var disableCCBOp = ServiceImpInstanceFactory.CreateDisableCCBAccoutInstance(propertyValue);
                            var typeofJourneyStrategy = ServiceImpInstanceFactory.CreateTypeofJourneyStrategyInstance();

                            while (true)
                            {
                                IEnumerable<UserExtension> userExtension = userFactory.CreateEntity(api.getUsers2(new Entities.KEMASWSIF_USERRef.getUsers2Request()
                                {
                                    ItemsPerPage = pageSize,
                                    ItemsPerPageSpecified = true,
                                    Page = page,
                                    PageSpecified = true,
                                    SessionID = propertyValue.SessionID,
                                    SearchCondition = new Entities.KEMASWSIF_USERRef.getUsers2RequestSearchCondition()
                                    {
                                        ClientID = clientId
                                    }
                                }).Users);

                                if (userExtension == null || userExtension.Count() == 0)
                                {
                                    break;
                                }
                                else
                                {
                                    //CCB permission & CCB bookings
                                    foreach (UserExtension oneUser in userExtension)
                                    {
                                        bool permissionResult = false;
                                        bool bookingResult = false;
                                        try
                                        {
                                        //disable ccb permission
                                            permissionResult = disableCCBOp.DisableCCBPermission(oneUser, loginUser.RoleEntities);
                                        //disable user company status
                                        //var statusManager = UserStatusContext.CreateStatusManager(oneUser.Status);
                                        api.updateUser2(new Entities.KEMASWSIF_USERRef.updateUser2Request()
                                        {
                                            SessionID = propertyValue.SessionID,
                                            Language = "english",
                                            UserData = new Entities.KEMASWSIF_USERRef.updateUserData
                                            {
                                                ID = oneUser.ID,
                                                Mail = oneUser.Mail,
                                                TypeOfJourney = typeofJourneyStrategy.GetValueFromApiInputValue(oneUser.TypeOfJourney),
                                                TypeOfJourneySpecified = true
                                            }
                                        });
                                        //disable ccb bookings
                                            bookingResult = disableCCBOp.DisableCCBBooking(oneUser, loginUser.RoleEntities);
                                        }
                                        catch (Exception ex)
                                        {
                                            LogInfor.WriteError(MessageCode.CVB000033.ToString(),
                                                String.Format("UserId:{0},ErrorMessage:{1}", oneUser.ID, ex.ToStr()), propertyValue.ID);
                                        }

                                        //Send email to the user
                                        Task.Factory.StartNew(() => {
                                            try
                                            {
                                        if (permissionResult && bookingResult)
                                        {
                                            //Email
                                            IClientTerminalSender email = EmailSenderFactory.CreateClientTerminalSender();
                                            email.onSendEvent += email_onSendEvent;
                                            email.Send(new EmailParameterEntity()
                                            {
                                                FirstName = oneUser.Name,
                                                LastName = oneUser.VName,
                                                Company = oneUser.Clients[0].Name,
                                                CompanyTerminalDate = DateTime.Now.ToString("dd/MM/yyyy")
                                            }, oneUser.Mail);
                                        }
                                            }
                                            catch (Exception ex)
                                            {
                                                //Email
                                                LogInfor.WriteError(MessageCode.CVB000032.ToStr(),
                                                    String.Format("Exception:{0}", ex.ToStr()), "System");
                                            }
                                        }, TaskCreationOptions.PreferFairness);
                                    }

                                    page++;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogInfor.WriteError(MessageCode.CVB000033.ToString(), ex.ToString(), "");
                        }

                    }, TaskCreationOptions.LongRunning);
                }
            }
            return ret;
        }

        void email_onSendEvent(EmailParameterEntity arg1, EmailType arg2, string[] arg3)
        {
            DataAccessProxyManager manager = new DataAccessProxyManager();
            manager.SendPaymentEmail(arg1, arg2.ToString(), arg3);
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}
