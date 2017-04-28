
using CF.VRent.Entities;
using CF.VRent.Entities.DataAccessProxyWrapper;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Common;
using CF.VRent.Log;
using System.Threading.Tasks;
using System.Threading;
using CF.VRent.Common.Entities;
using CF.VRent.BLL.BLLFactory;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.EntityFactory;
using CF.VRent.Email.EmailSender.Entity;
using CF.VRent.Email;

namespace CF.VRent.BLL.ReservationAOP
{
    public class AppRegistrationInterceptionBehavior : IInterceptionBehavior
    {
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            #region Get the original value of user
		    UserExtension originalUser = new UserExtension();
            string clientId = null;
            //Get the original user
            if (input.MethodBase.Name == "UpdateProfile")
            {
                var userProperty = input.Target.GetType().GetProperty("UserInfo");
                if (userProperty != null)
                {
                    var propertyValue = userProperty.GetValue(input.Target, null) as ProxyUserSetting;
                    originalUser = new UserFactory().CreateEntity(KemasAccessWrapper.CreateKemasUserAPI2Instance().findUser2(propertyValue.ID, propertyValue.SessionID).UserData);
                    clientId = (input.Arguments[0] as UserExtension).ClientID;
                }
                
            } 
	        #endregion

            var ret = getNext()(input, getNext);

            if (ret.Exception == null)
            {
                if (input.MethodBase.Name == "UserRegistration")
                {
                    //var task = Task.Factory.StartNew(() =>
                    //{
                    //    //Send email to inform user
                    //    var user = ret.ReturnValue as UserExtension;
                    //    try
                    //    {
                    //        if (user != null)
                    //        {
                    //            IUserRegistrationSender sender = EmailSenderFactory.CreateUserRegistrationSender();
                    //            sender.onSendEvent += sender_onSendEvent;
                    //            sender.Send(new EmailParameterEntity() {
                    //                FirstName = user.Name,
                    //                LastName = user.VName
                    //            }, user.Mail);
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        //Email
                    //        LogInfor.WriteError(MessageCode.CVB000032.ToStr(),
                    //            String.Format("Mail:{0}, UserName:{1}, Exception:{2}", user.Mail, user.Name + user.VName, ex.ToStr()), user.ID);
                    //    }
                    //}, TaskCreationOptions.PreferFairness);
                }
                else if (input.MethodBase.Name == "UpdateProfile")
                {
                    var task = Task.Factory.StartNew(() =>
                    {
                        //Send email to inform user
                        var user = ret.ReturnValue as UserExtension;
                        try
                        {
                            if (user != null)
                            {
                                
                                //Send email when user is transfering
                                if (user.StatusEntities["6"].Value == 1)
                                {
                                    var admin = KemasAdmin.SignOn();
                                    var inputUser = input.Arguments[0] as UserExtension;
                                    var transferEmail = ServiceImpInstanceFactory.CreateSendUserTransferEmailInstance(admin.SessionID, user);
                                    transferEmail.SendToManager();
                                }
                                else
                                {
                                    var admin = KemasAdmin.SignOn();
                                    var endUserValidator = ServiceImpInstanceFactory.CreateEndUserValidatorInstance();
                                    var validatedUser = endUserValidator.Validate((originalUser.Clients != null && originalUser.Clients.Length > 0) ?
                                        originalUser.Clients[0].ID : null, admin.SessionID);
                                    var inputValidatedUser = endUserValidator.Validate(clientId,admin.SessionID);

                                    bool isEndUserBefore = validatedUser.HasValue ? validatedUser.Value : false;
                                    bool isEndUser = inputValidatedUser.HasValue ? inputValidatedUser.Value : false;

                                    if (!isEndUserBefore && isEndUser)
                                    {
                                        var transferEmail = ServiceImpInstanceFactory.CreateSendUserTransferEmailInstance(admin.SessionID, user);
                                        transferEmail.SendToUser();
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //Email
                            LogInfor.EmailLogWriter.WriteError(MessageCode.CVB000032.ToStr(),
                                String.Format("Mail:{0}, UserName:{1}, Exception:{2}", user.Mail, user.Name + user.VName, ex.ToStr()), user.ID);
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
