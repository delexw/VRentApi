using CF.VRent.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace CF.VRent.Contract
{
    [ServiceContract]
    public interface IPortalLogin
    {
        [OperationContract]
        UserExtension Login(UserExtension postUserSetting);

        [OperationContract]
        UserExtension GetUser();
    }
}
