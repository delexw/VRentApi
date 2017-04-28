using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.UserCompany;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.UserMgmt
{
    public class EndUserValidator : IEndUserValidator
    {
        /// <summary>
        /// Validate current user clientID is end user or not
        /// </summary>
        /// <param name="currentClientID"></param>
        /// <param name="kemasSessionID"></param>
        /// <returns>if yes, return true</returns>
        public bool? Validate(string currentClientID, string kemasSessionID)
        {
            if (String.IsNullOrWhiteSpace(currentClientID))
            {
                return null;
            }
            //Get end user company id
            var kemasExtensionApi = KemasAccessWrapper.CreateKemasExtensionAPIInstance();
            var defaultEndUserCompany = UserCompanyContext.CreateCompanyManager().Companies[UserCompanyConstants.EndUserCompanyKey].GetDefaultKemasCompany();
            var kemasCompay = kemasExtensionApi.GetCompanyID(defaultEndUserCompany.Name, kemasSessionID);

            //end user
            return kemasCompay == currentClientID;
        }
    }
}
