using CF.VRent.BLL;
using CF.VRent.BLL.BLLFactory;
using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Job.Interface;
using CF.VRent.Job.Units;
using CF.VRent.Log;
using CF.VRent.UPSDK;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.ServiceModel;
using System.Text;
using VRentDJ.Utility;

namespace VRentDJ.Units
{
    public class JobIndirectFeeDeduction : JobUnit
    {
        public string UserName { get; set; }
        public string UserPwd { get; set; }

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
            //Get uncomplished bookings
            var pbll = ServiceImpInstanceFactory.CreatePaymentInstance(setting);

            //Schedule job
            pbll.ScheduleJobIndirectFeeBookings(setting, UserPwd);
        }

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
    }
}
