using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.DataAccessProxyWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL
{
    public class UserTransferUtility 
    {
        public static UserTransferRequest CreateUserTransferRequest(ProxyUserSetting userInfo, string clientIDTo)
        {
            UserTransferRequest utr = new UserTransferRequest();
            utr.UserID = Guid.Parse(userInfo.ID);
            utr.ClientIDFrom = Guid.Parse(userInfo.ClientID);
            utr.ClientIDTo = Guid.Parse(clientIDTo);

            utr.FirstName = userInfo.Name;
            utr.LastName = userInfo.VName;

            utr.Mail = userInfo.Mail;
            utr.State = UserTransferState.Active;
            utr.TransferResult = UserTransferResult.Pending;

            utr.CreatedOn = DateTime.Now;
            utr.CreatedBy = Guid.Parse(userInfo.ID);

            return utr;
        }

        public static UserTransferRequest LoadPendingUserTransferRequest(string userID)
        {
            UserTransferRequest req = null;

            IDataService be = new DataAccessProxyManager();
            UserTransferRResult result = be.RetrievePendingTransferRequests(Guid.Parse(userID));

            if (result.Data != null && result.Data.Length > 0)
            {
                req = result.Data[0];
            }

            return req;
        }
    }

    public class UserTransferBLL: AbstractBLL
    {
        public UserTransferBLL(ProxyUserSetting profile)
            : base(profile)
        {
        }

        public UserTransferRequest RequestUserTransfer(string targetClient)
        {
            UserTransferRequest addRequest = UserTransferUtility.CreateUserTransferRequest(UserInfo, targetClient);
            IDataService be = new DataAccessProxyManager();
            UserTransferCUDResult result = be.AddTransferRequest(addRequest, UserInfo);

            if (result.Success != 0)
            {
                throw new VrentApplicationException(result);
            }

            return result.Data;
        }


        public UserTransferRequest DetermineUserTransfer(string userID, bool approve)
        {
            UserTransferRequest transResult = null;
            UserTransferRequest updateRequest = LoadPendingUserTransferRequest(userID);
            if (updateRequest != null)
            {
                updateRequest.ApproverID = Guid.Parse(UserInfo.ID);
                updateRequest.TransferResult = approve ? UserTransferResult.Approve : UserTransferResult.Reject;
                updateRequest.State = UserTransferState.Completed;
                updateRequest.ModifiedBy = Guid.Parse(UserInfo.ID);
                updateRequest.ModifiedOn = DateTime.Now;

                IDataService be = new DataAccessProxyManager();
                UserTransferCUDResult result = be.UpdateTransferRequest(updateRequest, UserInfo);

                if (result.Success != 0)
                {
                    throw new VrentApplicationException(result);
                }

                transResult = result.Data;
            }
            else
            {
                throw new VrentApplicationException(
                    ErrorConstants. PendingRequestNotExistCode,
                    string.Format(ErrorConstants.PendingRequestNotExistMessage,userID),
                    ResultType.VRENT
                    );
            }
            return transResult;
        }

        public UserTransferRequest[] LoadUserTransfer()
        {
            IDataService be = new DataAccessProxyManager();
            UserTransferRResult result = be.RetrieveTransferRequests(UserInfo);

            if (result.Success != 0)
            {
                throw new VrentApplicationException(result);
            }

            return result.Data;
        }

        public UserTransferRequest LoadPendingUserTransferRequest(string userID)
        {
            UserTransferRequest req = null;

            IDataService be = new DataAccessProxyManager();
            UserTransferRResult result = be.RetrievePendingTransferRequests(Guid.Parse(userID));

            if (result.Data != null && result.Data.Length > 0)
            {
                req = result.Data[0];
            }

            return req;
        }
    }
}
