using CF.VRent.Common.UserContracts;
using CF.VRent.Entities;
using CF.VRent.Entities.DataAccessProxyWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Common;
using CF.VRent.Email;
using CF.VRent.Email.EmailSender.Entity;
using CF.VRent.Email.EmailSender.UserMgmt;
using CF.VRent.Email.EmailSender;

namespace CF.VRent.BLL.BLLFactory.UserMgmt
{
   public class SendUserTransferEmail:ISendUserTransferEmail
    {
       private string _adminSessionId;
       private UserExtension _inputUser;

       public SendUserTransferEmail(string adminSessionId, UserExtension inputUser)
       {
           _adminSessionId = adminSessionId;
           _inputUser = inputUser;
       }

        public void SendToManager()
        {
            var vrentManager = ServiceImpInstanceFactory.CreateGetVRentManagerInfoInstance(new ProxyUserSetting() { SessionID = _adminSessionId }).Get(_inputUser.ClientID);
            var statusDic = new Dictionary<int, string[]>();
            statusDic.Add(1, new string[] { vrentManager.Mail });
            IUserTransferSender sender = EmailSenderFactory.CreateUserTransferSender(statusDic);
            sender.onSendEvent += sender_onSendEvent;
            sender.Send(new EmailParameterEntity()
            {
                FirstName = _inputUser.Name,
                LastName = _inputUser.VName,
                VRentManagerName = String.Format("{0} {1}", vrentManager.Name, vrentManager.VName)
            }, _inputUser.Mail);
        }

        public void SendToUser()
        {
            var statusDic = new Dictionary<int, string[]>();
            statusDic.Add(0, new string[] { _inputUser.Mail });
            IUserTransferSender sender = EmailSenderFactory.CreateUserTransferSender(statusDic);
            sender.onSendEvent += sender_onSendEvent;
            sender.Send(new EmailParameterEntity()
            {
                FirstName = _inputUser.Name,
                LastName = _inputUser.VName,
                ResignDate = DateTime.Now.ToString("dd/MM/yyyy")
            }, _inputUser.Mail);
        }

        void sender_onSendEvent(EmailParameterEntity arg1, EmailType arg2, string[] arg3)
        {
            DataAccessProxyManager ds = new DataAccessProxyManager();
            ds.SendPaymentEmail(arg1, arg2.ToStr(), arg3);
        }
    }
}
