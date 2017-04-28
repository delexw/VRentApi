using CF.VRent.Common;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Common.UserContracts;
using CF.VRent.DataAccessProxy.Entities;
using CF.VRent.UserRole;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CF.VRent.DAL
{
    public class UserTransferDAL
    {
        public const string Sp_UserTransfer_Pending = "Sp_UserTransfer_Pending";
        public const string Sp_UserTransfer_Retrieve = "Sp_UserTransfer_Retrieve";
        public const string Sp_UserTransfer_Add = "Sp_UserTransfer_Add";
        public const string Sp_UserTransfer_Update = "Sp_UserTransfer_Update";

        #region Add
        public static UserTransferCUDResult AddTransferRequest(UserTransferRequest transferRequest, ProxyUserSetting operatorInfo)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            //@UserID uniqueidentifier,
            //@FirstName nvarchar(50),
            //@LastName nvarchar(50),
            //@Mail nvarchar(50),
            //@ClientIDFrom uniqueidentifier,
            //@ClientIDTo uniqueidentifier,
            //@State [tinyint], --0: pending, 1: completed	@State tinyint, 
            //@CreatedOn datetime,
            //@CreatedBy uniqueidentifier,
            //@ret int output --0: success, 1: have pending request

            SqlParameter userIDParam = new SqlParameter("@UserID", transferRequest.UserID);
            parameters.Add(userIDParam);

            if (string.IsNullOrEmpty(transferRequest.FirstName))
            {
                SqlParameter firstNameParam = new SqlParameter("@FirstName", DBNull.Value);
                parameters.Add(firstNameParam);
            }
            else
            {
                SqlParameter firstNameParam = new SqlParameter("@FirstName", transferRequest.FirstName);
                parameters.Add(firstNameParam);
            }

            if (string.IsNullOrEmpty(transferRequest.LastName))
            {
                SqlParameter lastNameParam = new SqlParameter("@LastName", DBNull.Value);
                parameters.Add(lastNameParam);
            }
            else
            {
                SqlParameter lastNameParam = new SqlParameter("@LastName", transferRequest.LastName);
                parameters.Add(lastNameParam);
            }

            SqlParameter mailParam = new SqlParameter("@Mail", transferRequest.Mail);
            parameters.Add(mailParam);

            if (transferRequest.ClientIDFrom == null)
            {
                SqlParameter clientIDFromParam = new SqlParameter("@ClientIDFrom", DBNull.Value);
                parameters.Add(clientIDFromParam);
            }
            else
            {
                SqlParameter clientIDFromParam = new SqlParameter("@ClientIDFrom", transferRequest.ClientIDFrom);
                parameters.Add(clientIDFromParam);
            }

            SqlParameter clientIDToParam = new SqlParameter("@ClientIDTo", transferRequest.ClientIDTo);
            parameters.Add(clientIDToParam);

            SqlParameter transResultParam = new SqlParameter("@TransferResult", (byte)transferRequest.TransferResult);
            parameters.Add(transResultParam);


            SqlParameter stateParam = new SqlParameter("@State", (byte)transferRequest.State);
            parameters.Add(stateParam);

            SqlParameter createdOnParam = new SqlParameter("@CreatedOn", transferRequest.CreatedOn);
            parameters.Add(createdOnParam);

            SqlParameter createdByParam = new SqlParameter("@CreatedBy", transferRequest.CreatedBy);
            parameters.Add(createdByParam);

            SqlParameter retParam = new SqlParameter(DataAccessProxyConstantRepo.SPReturnParameter, -1);
            retParam.Direction = System.Data.ParameterDirection.Output;
            parameters.Add(retParam);

            //--@return value
            //-- 0: success
            //-- -1: is not ownere or operator
            //-- -2: order is not generated
            int addRet = -1;

            UserTransferRequest addResult = DataAccessProxyConstantRepo.ExecuteSPReturnReaderAdv<UserTransferRequest>(Sp_UserTransfer_Add, parameters.ToArray(), (sqldatareader) => ReadSingleUserTransFromDataReader(sqldatareader), ref addRet);

            return ProcessingAddResult(addResult, addRet, transferRequest);
        }

        private const string AddUserTransferFailedCode = "CVD000102";
        private const string AddUserTransferFailedMessage = "User {0} Add user transfer request failed for client{1}";
        private const string UserTransferRequestExistsCode = "CVD000103";
        private const string UserTransferRequestExistsMessage = "User {0} already have a pending transfer request for client {1}";

        private static UserTransferCUDResult ProcessingAddResult(UserTransferRequest updated, int result, UserTransferRequest original)
        {
            UserTransferCUDResult processingRet = new UserTransferCUDResult();
            processingRet.Data = updated;
            processingRet.Success = result;
            processingRet.Type = Common.Entities.ResultType.DATAACCESSProxy;

            //@ret int output --0: success, 1: add failed, 2, no pending user transfer requests
            if (result == 1)
            {
                processingRet.Code = UserTransferRequestExistsCode;
                processingRet.Message = string.Format(UserTransferRequestExistsMessage, original.UserID, original.ClientIDTo);
            }
            else if (result == 2)
            {

                processingRet.Code = AddUserTransferFailedCode;
                processingRet.Message = string.Format(AddUserTransferFailedMessage, original.UserID, original.ClientIDTo);
            }

            return processingRet;

        }

        #endregion

        #region update user transfer request

        public static UserTransferCUDResult UpdateTransferRequest(UserTransferRequest transferRequest, ProxyUserSetting operatorInfo)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            //@UserID uniqueidentifier,
            //@FirstName nvarchar(50),
            //@LastName nvarchar(50),
            //@Mail nvarchar(50),
            //@ApproverID uniqueidentifier,
            //@TransferResult tinyint, --0:approve, 1: reject, 2: pending
            //@State tinyint, --0: active, 1: completed	@State tinyint, 
            //@ModifiedOn datetime,
            //@ModifiedBy uniqueidentifier,
            //@ret int output --0: success, 1: update failed, 2, no pending user transfer requests

            ProxyRole primaryrole = operatorInfo.VrentRoles.FirstOrDefault();

            if (primaryrole != null)
            {
                //role parameters 
                SqlParameter rolePara = new SqlParameter("@Role", primaryrole.RoleMember);
                parameters.Add(rolePara);

                if (RoleUtility.IsServiceCenter(operatorInfo))
                {
                    SqlParameter clientPara = new SqlParameter("@VMClientID", transferRequest.ClientIDTo);
                    parameters.Add(clientPara);
                }
                else if (RoleUtility.IsVRentManager(operatorInfo))
                {
                    SqlParameter clientPara = new SqlParameter("@VMClientID", operatorInfo.ClientID);
                    parameters.Add(clientPara);
                }
            }

            SqlParameter userIDParam = new SqlParameter("@UserID", transferRequest.UserID);
            parameters.Add(userIDParam);

            if (string.IsNullOrEmpty(transferRequest.FirstName))
            {
                SqlParameter firstNameParam = new SqlParameter("@FirstName", DBNull.Value);
                parameters.Add(firstNameParam);
            }
            else
            {
                SqlParameter firstNameParam = new SqlParameter("@FirstName", transferRequest.FirstName);
                parameters.Add(firstNameParam);
            }

            if (string.IsNullOrEmpty(transferRequest.LastName))
            {
                SqlParameter lastNameParam = new SqlParameter("@LastName", DBNull.Value);
                parameters.Add(lastNameParam);
            }
            else
            {
                SqlParameter lastNameParam = new SqlParameter("@LastName", transferRequest.LastName);
                parameters.Add(lastNameParam);
            }

            SqlParameter mailParam = new SqlParameter("@Mail", transferRequest.Mail);
            parameters.Add(mailParam);

            SqlParameter approverIDParam = new SqlParameter("@ApproverID", operatorInfo.ID);
            parameters.Add(approverIDParam);

            SqlParameter transferResultParam = new SqlParameter("@TransferResult", (byte)transferRequest.TransferResult);
            parameters.Add(transferResultParam);

            SqlParameter stateParam = new SqlParameter("@State", (byte)transferRequest.State);
            parameters.Add(stateParam);

            SqlParameter modifiedOnParam = new SqlParameter("@ModifiedOn", transferRequest.ModifiedOn);
            parameters.Add(modifiedOnParam);

            SqlParameter modifiedByParam = new SqlParameter("@ModifiedBy", transferRequest.ModifiedBy);
            parameters.Add(modifiedByParam);

            SqlParameter retParam = new SqlParameter(DataAccessProxyConstantRepo.SPReturnParameter, -1);
            retParam.Direction = System.Data.ParameterDirection.Output;
            parameters.Add(retParam);

            int updateRet = -1;

            UserTransferRequest updated = DataAccessProxyConstantRepo.ExecuteSPReturnReaderAdv<UserTransferRequest>(Sp_UserTransfer_Update, parameters.ToArray(), (sqldatareader) => ReadSingleUserTransFromDataReader(sqldatareader), ref updateRet);

            return ProcessingUpdateResult(updated, updateRet, transferRequest, operatorInfo);
        }

        private const string UpdateUserTransferFailedCode = "CVD000100";
        private const string UpdateUserTransferFailedMessage = "VM {0}(Company {1}) Update user transfer request for employee {2} failed";

        private const string NoPendingUserTransferCode = "CVD000101";
        private const string NoPendingUserTransferMessage = "VM {0}(Company {1}) does not have a pending transfer request for employee {2}";

        private static UserTransferCUDResult ProcessingUpdateResult(UserTransferRequest updated, int result, UserTransferRequest original, ProxyUserSetting operatorInfo)
        {
            UserTransferCUDResult processingRet = new UserTransferCUDResult();
            processingRet.Data = updated;
            processingRet.Success = result;
            processingRet.Type = Common.Entities.ResultType.DATAACCESSProxy;

            //@ret int output --0: success, 1: update failed, 2, no pending user transfer requests
            if (result == 1)
            {
                processingRet.Code = UpdateUserTransferFailedCode;
                processingRet.Message = string.Format(UpdateUserTransferFailedMessage, operatorInfo.ID, operatorInfo.ClientID, original.UserID);
            }
            else if (result == 2)
            {
                processingRet.Code = NoPendingUserTransferCode;
                processingRet.Message = string.Format(NoPendingUserTransferMessage, operatorInfo.ID, operatorInfo.ClientID, original.UserID);
            }

            return processingRet;

        }

        #endregion

        #region Retrieve
        public static UserTransferRResult RetrieveTransferRequests(ProxyUserSetting operatorInfo)
        {
            UserTransferRequest[] requests = null;

            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlParameter userIDPara = new SqlParameter("@UserID", operatorInfo.ID);
            parameters.Add(userIDPara);

            //I really don't understand why only get the first role here....
            //this should be changed
            //If user is both a VM and a ordinary user, he will have a permission to retrieve data which represents user transfer request.
            //If role enity have a property that represents role's priority, it would be easy to modifiy code here.
            //By Adam
            ProxyRole primaryrole = operatorInfo.VrentRoles.FirstOrDefault(r => r.RoleMember == UserRoleConstants.VRentManagerKey);
            if (primaryrole == null)
            {
                //user is a ordinary user?
                primaryrole = operatorInfo.VrentRoles.FirstOrDefault(r => r.RoleMember == UserRoleConstants.BookingUserKey);
            }

            if (primaryrole != null)
            {
                //role parameters 
                SqlParameter rolePara = new SqlParameter("@Role", primaryrole.RoleMember);
                parameters.Add(rolePara);
                SqlParameter clientPara = new SqlParameter("@TargetClient", operatorInfo.ClientID);
                parameters.Add(clientPara);
            }

            SqlParameter retParam = new SqlParameter(DataAccessProxyConstantRepo.SPReturnParameter, SqlDbType.Int);
            retParam.Direction = System.Data.ParameterDirection.Output;
            parameters.Add(retParam);

            int readRet = -1;

            requests = DataAccessProxyConstantRepo.ExecuteSPReturnReaderAdv<UserTransferRequest[]>(Sp_UserTransfer_Retrieve, parameters.ToArray(), (sqldatareader) => ReadMultipleUserTransFromDataReader(sqldatareader), ref readRet);

            parameters.Clear();

            return ProcessingRetrieveeResult(requests, readRet, operatorInfo);
        }

        private const string NoPermissionCode = "CVD000104";
        private const string NoPermissionMessage = "User {0} does not have permission to retrieve pending transfer requests for client {1}";

        private static UserTransferRResult ProcessingRetrieveeResult(UserTransferRequest[] updated, int result, ProxyUserSetting operatorInfo)
        {
            UserTransferRResult processingRet = new UserTransferRResult();
            processingRet.Data = updated;
            processingRet.Success = result;
            processingRet.Type = Common.Entities.ResultType.DATAACCESSProxy;

            if (result == 1)
            {
                processingRet.Code = NoPermissionCode;
                processingRet.Message = string.Format(NoPermissionMessage, operatorInfo.ID, operatorInfo.ClientID);
            }

            return processingRet;

        }

        #endregion

        public static UserTransferRResult RetrievePendingTransferRequests(Guid userID)
        {
            UserTransferRequest[] requests = null;

            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlParameter userIDPara = new SqlParameter("@UserID", userID);
            parameters.Add(userIDPara);

            SqlParameter retParam = new SqlParameter(DataAccessProxyConstantRepo.SPReturnParameter, SqlDbType.Int);
            retParam.Direction = System.Data.ParameterDirection.Output;
            parameters.Add(retParam);

            int readRet = -1;

            requests = DataAccessProxyConstantRepo.ExecuteSPReturnReaderAdv<UserTransferRequest[]>(Sp_UserTransfer_Pending, parameters.ToArray(), (sqldatareader) => ReadMultipleUserTransFromDataReader(sqldatareader), ref readRet);

            parameters.Clear();

            UserTransferRResult pending = new UserTransferRResult();
            pending.Success = 0;
            pending.Data = requests;

            return pending;
        }


        #region Helper Method

        private static UserTransferRequest ReadSingleUserTransFromDataReader(SqlDataReader sqlReader)
        {
            UserTransferRequest userTrans = new UserTransferRequest();

            while (sqlReader.Read())
            {
                //[ID]
                //      ,[UserID]
                //      ,[FirstName]
                //      ,[LastName]
                //      ,[Mail]
                //      ,[ClientIDFrom]
                //      ,[ClientIDTo]
                //      ,[ApproverID]
                //      ,[TransferResult]
                //      ,[State]
                //      ,[CreatedOn]
                //      ,[CreatedBy]
                //      ,[ModifiedOn]
                //      ,[ModifiedBy]


                userTrans.ID = Convert.ToInt32(sqlReader[0].ToString());
                userTrans.UserID = Guid.Parse(sqlReader[1].ToString());

                if (!sqlReader[2].Equals(DBNull.Value))
                {
                    userTrans.FirstName = sqlReader[2].ToString();
                }

                if (!sqlReader[3].Equals(DBNull.Value))
                {
                    userTrans.LastName = sqlReader[3].ToString();
                }

                userTrans.Mail = sqlReader[4].ToString();

                if (!sqlReader[5].Equals(DBNull.Value))
                {
                    userTrans.ClientIDFrom = Guid.Parse(sqlReader[5].ToString());
                }
                userTrans.ClientIDTo = Guid.Parse(sqlReader[6].ToString());

                if (!sqlReader[7].Equals(DBNull.Value))
                {
                    userTrans.ApproverID = Guid.Parse(sqlReader[7].ToString());
                }

                userTrans.TransferResult = (UserTransferResult)Enum.Parse(typeof(UserTransferResult), sqlReader[8].ToString());

                userTrans.State = (UserTransferState)Enum.Parse(typeof(UserTransferState), sqlReader[9].ToString());
                userTrans.CreatedOn = Convert.ToDateTime(sqlReader[10].ToString());
                userTrans.CreatedBy = Guid.Parse(sqlReader[11].ToString());
                userTrans.ModifiedOn = sqlReader[12].Equals(DBNull.Value) ? new Nullable<DateTime>() : Convert.ToDateTime(sqlReader[12].ToString());
                userTrans.ModifiedBy = sqlReader[13].Equals(DBNull.Value) ? new Nullable<Guid>() : Guid.Parse(sqlReader[13].ToString());
            }

            return userTrans;
        }

        private static UserTransferRequest[] ReadMultipleUserTransFromDataReader(SqlDataReader sqlReader)
        {
            List<UserTransferRequest> userTransReqs = new List<UserTransferRequest>();

            while (sqlReader.Read())
            {
                UserTransferRequest userTrans = new UserTransferRequest();

                userTrans.ID = Convert.ToInt32(sqlReader[0].ToString());
                userTrans.UserID = Guid.Parse(sqlReader[1].ToString());

                userTrans.FirstName = sqlReader[2].ToString();
                userTrans.LastName = sqlReader[3].ToString();
                userTrans.Mail = sqlReader[4].ToString();

                if (!sqlReader[5].Equals(DBNull.Value))
                {
                    userTrans.ClientIDFrom = Guid.Parse(sqlReader[5].ToString());
                }
                userTrans.ClientIDTo = Guid.Parse(sqlReader[6].ToString());

                if (!sqlReader[7].Equals(DBNull.Value))
                {
                    userTrans.ApproverID = Guid.Parse(sqlReader[7].ToString());
                }

                userTrans.TransferResult = (UserTransferResult)Enum.Parse(typeof(UserTransferResult), sqlReader[8].ToString());

                userTrans.State = (UserTransferState)Enum.Parse(typeof(UserTransferState), sqlReader[9].ToString());
                userTrans.CreatedOn = Convert.ToDateTime(sqlReader[10].ToString());
                userTrans.CreatedBy = Guid.Parse(sqlReader[11].ToString());
                userTrans.ModifiedOn = sqlReader[12].Equals(DBNull.Value) ? new Nullable<DateTime>() : Convert.ToDateTime(sqlReader[12].ToString());
                userTrans.ModifiedBy = sqlReader[13].Equals(DBNull.Value) ? new Nullable<Guid>() : Guid.Parse(sqlReader[13].ToString());

                userTransReqs.Add(userTrans);
            }

            return userTransReqs.ToArray();
        }

        #endregion
    }
}
