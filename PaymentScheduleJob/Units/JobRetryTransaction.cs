using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.Common;
using CF.VRent.Job.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using VRentDJ.Utility;
using CF.VRent.BLL.BLLFactory;
using System.Collections.Specialized;

namespace VRentDJ.Units
{
    public class JobRetryTransaction : JobUnit
    {
        public string UserName { get; set; }
        public string UserPwd { get; set; }

        public override void Init()
        {
            if (Parameters != null)
            {
                UserName = Parameters["userName"];
                UserPwd = Parameters["userPwd"];
            }
        }

        public override void Finish()
        {
            
        }

        public override void Invoke(NameValueCollection containerParameters = null)
        {
            ProxyUserSetting setting = KemasLogin.Login(UserName, UserPwd);
            //Login user
            if (setting == null)
            {
                throw new FaultException<ReturnResult>(new ReturnResult()
                {
                    Code = MessageCode.CVCE000006.ToString(),
                    Message = MessageCode.CVCE000006.GetDescription(),
                    Type = ResultType.VRENT
                }, MessageCode.CVCE000006.GetDescription());
            }
            //retry

            var pbll = ServiceImpInstanceFactory.CreateTransactionInstance(setting);

            pbll.RetryTransactionOfBookings();
        }
    }
}
