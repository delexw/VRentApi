using CF.VRent.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace CF.VRent.Contract
{
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IVrentAppService
    {
        #region Register in Portal
        [OperationContract]
        UserExtension Register(UserExtension user, string tcId, string lang);

        [OperationContract]
        UserExtension UpdateProfile(string userID, UserExtension user, string lang);

        [OperationContract]
        UserExtension ChangePassword(string userID, UserExtension user, string lang);

        #endregion

    }
}
