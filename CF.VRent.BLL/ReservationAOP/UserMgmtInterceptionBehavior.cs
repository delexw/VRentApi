using CF.VRent.BLL.BLLFactory;
using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Common.UserContracts;
using CF.VRent.Email;
using CF.VRent.Email.EmailSender;
using CF.VRent.Email.EmailSender.Entity;
using CF.VRent.Email.EmailSender.UserMgmt;
using CF.VRent.Entities;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.Entities.EntityFactory;
using CF.VRent.Log;
using CF.VRent.UserStatus;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace CF.VRent.BLL.ReservationAOP
{
    public class UserMgmtInterceptionBehavior : IInterceptionBehavior
    {
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            //Get the input status from FE
            var inputStatus = new Dictionary<string, string[]>();
            var statusStr = "";
            UserExtension inputUser = new UserExtension();
            if (input.MethodBase.Name == "CreateCorpUser" || input.MethodBase.Name == "UpdateUser")
            {
                inputUser = input.Arguments[0] as UserExtension;
                statusStr = inputUser.Status;
                if (String.IsNullOrWhiteSpace(statusStr) && input.MethodBase.Name == "CreateCorpUser")
                {
                    statusStr = "1";
                }
                if (!String.IsNullOrWhiteSpace(statusStr))
                {
                    inputStatus.Add(statusStr, null);
                }
            }

            //Run the method body
            var returnObj = getNext()(input, getNext);

            if (returnObj.Exception == null)
            {
                if (input.MethodBase.Name == "CreateCorpUser" || input.MethodBase.Name == "UpdateUser")
                {
                    var user = returnObj.ReturnValue as UserExtension;

                    #region Disable ccb/dub bookings
                    Task.Factory.StartNew(() => {

                        //Booking_Deactived
                        if (input.MethodBase.Name == "UpdateUser" &&
                            (inputUser.StatusEntities["9"].Value == 1 ||
                             inputUser.StatusEntities["B"].Value == 1 ||
                             inputUser.StatusEntities["C"].Value == 1))
                        {
                            var userProperty = input.Target.GetType().GetProperty("UserInfo");
                            var propertyValue = userProperty.GetValue(input.Target, null) as ProxyUserSetting;
                            var disableCCBOp = ServiceImpInstanceFactory.CreateDisableCCBAccoutInstance(propertyValue);
                            var currentUser = new UserFactory().CreateEntity(propertyValue);
                            //Disable CCB
                            disableCCBOp.DisableCCBBooking(user, currentUser.RoleEntities);
                            //Disable DUB
                            disableCCBOp.DisableDUBBooking(user, currentUser.RoleEntities);
                        }

                    }, TaskCreationOptions.PreferFairness);
                    
                    #endregion

                    #region Send Email
                    var pwd = user.Password;
                    
                    Task.Factory.StartNew(() =>
                                {
                                    try
                                    {
                                        UserExtension vrentManager=  new UserExtension();
                                        //Send email to inform user
                                        var userProperty = input.Target.GetType().GetProperty("UserInfo");
                                        if (userProperty != null)
                                        {
                                            var propertyValue = userProperty.GetValue(input.Target, null);
                                            //User transfer from end 2 corporater
                                            var statuManager = UserStatusContext.CreateStatusManager(user.Status);
                                            if (statuManager.Status["6"].Value == 1)
                                            {
                                                //Get the vrent manager mail
                                                var transferEamil = ServiceImpInstanceFactory.CreateSendUserTransferEmailInstance(((ProxyUserSetting)propertyValue).SessionID, user);
                                                transferEamil.SendToManager();
                                            }

                                            //User transfer from corporater 2 end
                                            if (!user.IsPrivateUserBefore && user.IsPrivateUser)
                                            {
                                                var transferEamil = ServiceImpInstanceFactory.CreateSendUserTransferEmailInstance(((ProxyUserSetting)propertyValue).SessionID, user);
                                                transferEamil.SendToUser();
                                            }
                                        }

                                        if (user != null && !String.IsNullOrWhiteSpace(statusStr))
                                        {
                                            inputStatus[statusStr] = new string[] { user.Mail };

                                            IUserMgmtSender sender = EmailSenderFactory.CreateUserMgmtSuccessSender(inputStatus, user.Status, user.OriginalStatusEntities);
                                            sender.onSendEvent += sender_onSendEvent;
                                            sender.Send(new EmailParameterEntity()
                                            {
                                                FirstName = user.Name,
                                                LastName = user.VName,
                                                Password = pwd,
                                                Mail = user.Mail,
                                                IOSUrl = "<a href=\"" + ConfigurationManager.AppSettings["IOS"] + "\">ISO APP</a>",
                                                AndroidUrl = "<a href=\"" + ConfigurationManager.AppSettings["Android"] + "\">Android APP</a>",
                                                VRentManagerName = String.Format("{0} {1}", vrentManager.Name, vrentManager.VName)
                                            });

                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        //Email
                                        LogInfor.EmailLogWriter.WriteError(MessageCode.CVB000032.ToStr(),
                                            String.Format("Exception:{0}", ex.ToStr()), "System");
                                    }
                                }, TaskCreationOptions.LongRunning);
                    #endregion

                    user.Password = "";
                    returnObj.ReturnValue = user;
                }
            }
            return returnObj;
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
