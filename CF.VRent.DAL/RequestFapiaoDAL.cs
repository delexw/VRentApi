using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.DataAccessProxy.Entities;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CF.VRent.DAL
{


    public class RequestFapiaoDAL
    {
        private const string RetrieveFapiaoRequestsByProxyBookingID = "Sp_RetrieveFapiaoRequestsByProxyBookingID";
        
        //fapiao-request and fp
        private const string RetrieveFapiaoRequestsFullByProxyBookingID = "Sp_RetrieveFapiaoRequestsFullByProxyBookingID";
        private const string RetrieveFapiaoRequestsByFapiaoSource = "Sp_RetrieveFapiaoRequestsBySource";
        private const string UpdateFaPiaoRequest = "Sp_UpdateFaPiaoRequest";

        #region Update Fapiao Request

        //user API
        public static ReturnResultExt UpdateFapiaoRequest(ProxyFapiaoRequest request, UserProfile profile)
        {

            ProxyFapiaoRequest fapiaoRequest = null;
            List<SqlParameter> parameters = new List<SqlParameter>();


            SqlParameter proxyBookingIDPara = new SqlParameter("@ProxyBookingID", request.ProxyBookingID);
            parameters.Add(proxyBookingIDPara);

            #region identity parameter
            SqlParameter userIDPara = new SqlParameter("@OperatorID", profile.UserID);
            parameters.Add(userIDPara);

            if (profile.CorporateID == null)
            {
                SqlParameter corporateIDPara = new SqlParameter("@CorporateID", DBNull.Value);
                parameters.Add(corporateIDPara);
            }
            else
            {
                SqlParameter corporateIDPara = new SqlParameter("@CorporateID", profile.CorporateID);
                parameters.Add(corporateIDPara); 
            }
            #endregion

            SqlParameter FPIDPara = null;
            if (request.FapiaoPreferenceID == null)
            {
                FPIDPara = new SqlParameter("@FapiaoPreferenceID", DBNull.Value);
            }
            else
            {
                FPIDPara = new SqlParameter("@FapiaoPreferenceID", request.FapiaoPreferenceID);
            }

            parameters.Add(FPIDPara);

            SqlParameter fapiaoSourcePara = new SqlParameter("@FapiaoSource", request.FapiaoSource);
            parameters.Add(fapiaoSourcePara);

            SqlParameter statePara = new SqlParameter("@State", request.State);
            parameters.Add(statePara);

            SqlParameter modifiedOnPara = new SqlParameter("@ModifiedOn", request.ModifiedOn);
            parameters.Add(modifiedOnPara);

            SqlParameter modifiedByPara = new SqlParameter("@ModifiedBy", request.ModifiedBy);
            parameters.Add(modifiedByPara);

            SqlParameter retPara = new SqlParameter("@return_value", -1);
            retPara.Direction = ParameterDirection.InputOutput;
            parameters.Add(retPara);

            fapiaoRequest = DataAccessProxyConstantRepo.ExecuteSPReturnReader<ProxyFapiaoRequest>(UpdateFaPiaoRequest, parameters.ToArray(), (sqldatareader) => ReadSingleFapiaoRequestFromDataReader(sqldatareader));

            int returnValue = Convert.ToInt32(retPara.Value);
            parameters.Clear();

            ReturnResultExt operationResult = new ReturnResultExt();
            operationResult.Data = fapiaoRequest;
            operationResult.Success = returnValue;

            ProcessingResult(operationResult,profile,request);

            return operationResult;
        }

        private static void ProcessingResult(ReturnResultExt operationResult,UserProfile profile,ProxyFapiaoRequest request) 
        {
            switch (operationResult.Success)
            {
                //-- 0: update one
                //-- 1: invalid operator
                //-- 2: invalid FapiaoPreference
                //-- 3: bad data exists
                //-- 4: in unchangeable state
                case (int)UpdateFapiaoRequestResult.Success:
                    operationResult.Code = FapiaoRequestConst.SuccessCode;
                    operationResult.Message = FapiaoRequestConst.successMessage;
                    break;
                case (int)UpdateFapiaoRequestResult.InvalidOperator: //
                    operationResult.Code = FapiaoRequestConst.OperatorIsNotBookingOwnerCode;
                    operationResult.Message = string.Format(FapiaoRequestConst.OperatorIsNotBookingOwnerMessage, profile.UserID, request.ProxyBookingID);
                    break;
                case (int)UpdateFapiaoRequestResult.InvalidFP:
                    operationResult.Code = FapiaoRequestConst.OperatorIsNotFPOwnerCode;
                    operationResult.Message = string.Format(FapiaoRequestConst.OperatorIsNotFPOwnerMessage, profile.UserID, request.FapiaoPreferenceID);
                    break;
                case (int)UpdateFapiaoRequestResult.BadDataExist:
                    operationResult.Code = FapiaoRequestConst.OperatorIsNotFPOwnerCode;
                    operationResult.Message = string.Format(FapiaoRequestConst.OperatorIsNotFPOwnerMessage, profile.UserID, "FapiaoRequests");
                    break;
                case (int)UpdateFapiaoRequestResult.UnChangeableState:
                    operationResult.Code = FapiaoRequestConst.FapiaoRequestsInProcessingCode;
                    operationResult.Message = string.Format(FapiaoRequestConst.FapiaoRequestsInProcessingCode, request.ProxyBookingID, profile.UserID);
                    break;
                default:
                    operationResult.Code = FapiaoRequestConst.UnknownResultCode;
                    operationResult.Message = string.Format(FapiaoRequestConst.UnknownResultMessage, request.ProxyBookingID, request.FapiaoSource);
                    break;
            }

        }

        #endregion

        #region Retrieve Fapiao Request
        public static ReturnResultExtRetrieve RetrieveFapiaoRequestDetailFullByFapiaoSource(int proxyBookingID, int[] fapiaoSource, UserProfile profile)
        {

            ProxyFapiaoRequest[] fapiaoRequests = null;
            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlParameter proxyBookingIDPara = new SqlParameter("@ProxyBookingID", proxyBookingID);
            parameters.Add(proxyBookingIDPara);

            #region identity parameter
            SqlParameter userIDPara = new SqlParameter("@OperatorID", SqlDbType.UniqueIdentifier);
            userIDPara.Value = profile.UserID;
            parameters.Add(userIDPara);
            #endregion

            SqlDataRecord[] fapiaoSourceRec = CreateFapiaoSourceRecord(fapiaoSource);
            SqlParameter fapiaoSourcePara = null;
            if (fapiaoSourceRec.Length == 0)
            {
                fapiaoSourcePara = new SqlParameter("@FapiaoSourceInput", DBNull.Value);
            }
            else
            {
                fapiaoSourcePara = new SqlParameter("@FapiaoSourceInput", fapiaoSourceRec);
            }
            fapiaoSourcePara.SqlDbType = SqlDbType.Structured;
            fapiaoSourcePara.TypeName = "dbo.FapiaoSource";
            parameters.Add(fapiaoSourcePara);

            SqlParameter retPara = new SqlParameter("@return_value", -1);
            retPara.Direction = ParameterDirection.InputOutput;
            parameters.Add(retPara);

            fapiaoRequests = DataAccessProxyConstantRepo.ExecuteSPReturnReader<ProxyFapiaoRequest[]>(RetrieveFapiaoRequestsByFapiaoSource, parameters.ToArray(), (sqldatareader) => ReadMultipleFapiaoRequestFullFromDataReader (sqldatareader));

            int returnValue = Convert.ToInt32(retPara.Value);
            parameters.Clear();

            ReturnResultExtRetrieve operationResult = new ReturnResultExtRetrieve();
            operationResult.Data = fapiaoRequests;
            operationResult.Success = returnValue;

            ProcessingResult(operationResult, profile, proxyBookingID, fapiaoSource);

            return operationResult;
        }


        //public static ReturnResultExtRetrieve RetrieveFapiaoRequestDetailByFapiaoSource(int proxyBookingID, int[] fapiaoSource, UserProfile profile)
        //{

        //    ProxyFapiaoRequest[] fapiaoRequests = null;
        //    List<SqlParameter> parameters = new List<SqlParameter>();

        //    SqlParameter proxyBookingIDPara = new SqlParameter("@ProxyBookingID", proxyBookingID);
        //    parameters.Add(proxyBookingIDPara);

        //    #region identity parameter
        //    SqlParameter userIDPara = new SqlParameter("@OperatorID", SqlDbType.UniqueIdentifier);
        //    userIDPara.Value = profile.UserID;
        //    parameters.Add(userIDPara);
        //    #endregion

        //    SqlDataRecord[] fapiaoSourceRec = CreateFapiaoSourceRecord(fapiaoSource);
        //    SqlParameter fapiaoSourcePara = null;
        //    if (fapiaoSourceRec.Length == 0)
        //    {
        //        fapiaoSourcePara = new SqlParameter("@FapiaoSourceInput", DBNull.Value);
        //    }
        //    else
        //    {
        //        fapiaoSourcePara = new SqlParameter("@FapiaoSourceInput", fapiaoSourceRec); 
        //    }
        //    fapiaoSourcePara.SqlDbType = SqlDbType.Structured;
        //    fapiaoSourcePara.TypeName = "dbo.FapiaoSource";
        //    parameters.Add(fapiaoSourcePara);

        //    SqlParameter retPara = new SqlParameter("@return_value", -1);
        //    retPara.Direction = ParameterDirection.InputOutput;
        //    parameters.Add(retPara);

        //    fapiaoRequests = DataAccessProxyConstantRepo.ExecuteSPReturnReader<ProxyFapiaoRequest[]>(RetrieveFapiaoRequestsByFapiaoSource, parameters.ToArray(), (sqldatareader) => ReadMultipleFapiaoRequestFullFromDataReader (sqldatareader));

        //    int returnValue = Convert.ToInt32(retPara.Value);
        //    parameters.Clear();

        //    ReturnResultExtRetrieve operationResult = new ReturnResultExtRetrieve();
        //    operationResult.Data = fapiaoRequests;
        //    operationResult.Success = returnValue;

        //    ProcessingResult(operationResult,profile,proxyBookingID, fapiaoSource);

        //    return operationResult;
        //}

        private static void ProcessingResult(ReturnResultExtRetrieve operationResult, UserProfile profile, int proxyBookingID, int[] fapiaoSource) 
        {
            switch (operationResult.Success)
            {
                //-- 0: update one
                //-- 1: invalid operator
                //-- 2: invalid FapiaoPreference
                //-- 3: bad data exists
                //-- 4: in unchangeable state
                case (int)UpdateFapiaoRequestResult.Success:
                    operationResult.Code = FapiaoRequestConst.SuccessCode;
                    operationResult.Message = FapiaoRequestConst.successMessage;
                    break;
                case (int)UpdateFapiaoRequestResult.InvalidOperator: //
                    operationResult.Code = FapiaoRequestConst.OperatorIsNotBookingOwnerCode;
                    operationResult.Message = string.Format(FapiaoRequestConst.OperatorIsNotBookingOwnerMessage, profile.UserID, proxyBookingID);
                    break;
                default:
                    operationResult.Code = FapiaoRequestConst.UnknownResultCode;
                    operationResult.Message = string.Format(FapiaoRequestConst.UnknownResultMessage, proxyBookingID, fapiaoSource);
                    break;
            }

        }

        private static SqlDataRecord[] CreateFapiaoSourceRecord(int[] fapiaoSource)
        {
            //price.BookingID,
            //price.[Total],
            //price.[TagID],
            //price.[TimeStamp],
            //price.[state],
            //price.[CreatedOn],
            //price.[CreatedBy],
            //price.[ModifiedOn],
            //price.[ModifiedBy]
            SqlMetaData[] metaData = new SqlMetaData[1]
            {
                new SqlMetaData("FapiaoSource",SqlDbType.SmallInt)
            };

            return fapiaoSource.Select(m => SetFapiaoSourceValue(metaData, m)).ToArray();
        }

        private static SqlDataRecord SetFapiaoSourceValue(SqlMetaData[] metaData, int fapiaoSource)
        {
            SqlDataRecord fapiaoSourceRecord = new SqlDataRecord(metaData);
            fapiaoSourceRecord.SetInt16(0, (Int16)fapiaoSource);

            return fapiaoSourceRecord;
        }


        #endregion

        #region Common API
        //public static ProxyFapiaoRequest[] RetrieveAllFapiaoRequests(int proxyBookingID)
        //{
        //    ProxyFapiaoRequest[] fapiaoData = null;
        //    List<SqlParameter> parameters = new List<SqlParameter>();

        //    SqlParameter proxyBookingIDPara = new SqlParameter("@ProxyBookingID", proxyBookingID);
        //    parameters.Add(proxyBookingIDPara);

        //    fapiaoData = DataAccessProxyConstantRepo.ExecuteSPReturnReader<ProxyFapiaoRequest[]>(RetrieveFapiaoRequestsByProxyBookingID, parameters.ToArray(), (sqldatareader) => ReadMultipleFapiaoRequestFromDataReader(sqldatareader));

        //    parameters.Clear();

        //    return fapiaoData;
        //}

        #endregion

        #region Admin API
        private const string RetrieveFaPiaoRequestDetail = "Sp_RetrieveFapiaoRequestsByFapiaoRequestID";
        private const string UpdateFaPiaoRequestState = "Sp_UpdateFaPiaoRequestState";

        //admin API
        public static ProxyFapiaoRequest RetrieveFapiaoRequestDetailByID(int fapiaoRequestID)
        {

            ProxyFapiaoRequest fapiaoRequest = null;
            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlParameter fapiaoRequestIDPara = new SqlParameter("@ID", fapiaoRequestID);
            parameters.Add(fapiaoRequestIDPara);

            fapiaoRequest = DataAccessProxyConstantRepo.ExecuteSPReturnReader<ProxyFapiaoRequest>(RetrieveFaPiaoRequestDetail, parameters.ToArray(), (sqldatareader) => ReadSingleFapiaoRequestFromDataReader(sqldatareader));

            parameters.Clear();

            return fapiaoRequest;
        }

        #endregion

        #region Helper Methods
        private static ProxyFapiaoRequest ReadSingleFapiaoRequestFromDataReader(SqlDataReader sqlReader)
        {
            ProxyFapiaoRequest requestedFapiao = new ProxyFapiaoRequest();

            while (sqlReader.Read())
            {
                //[ID] [int] IDENTITY(1,1) NOT NULL,
                //[ProxyBookingID] [int] NOT NULL,
                //[FapiaoPreferenceID] [uniqueidentifier] NOT NULL,
                //[FapiaoSource] [smallint] NOT NULL,

                //[State] [tinyint] NOT NULL,
                //[CreatedOn] [datetime] NOT NULL,
                //[CreatedBy] [uniqueidentifier] NOT NULL,
                //[ModifiedOn] [datetime] NULL,
                //[ModifiedBy] [uniqueidentifier] NULL,

                requestedFapiao.ID = Convert.ToInt32(sqlReader[0].ToString());

                requestedFapiao.ProxyBookingID = Convert.ToInt32(sqlReader[1].ToString());

                requestedFapiao.FapiaoPreferenceID = sqlReader[2].Equals(DBNull.Value) ? new Nullable<Guid>() : Guid.Parse(sqlReader[2].ToString());;
                requestedFapiao.FapiaoSource = Convert.ToInt32(sqlReader[3].ToString());

                requestedFapiao.State = Convert.ToInt32(sqlReader[4].ToString());

                requestedFapiao.CreatedOn = Convert.ToDateTime(sqlReader[5].ToString());
                requestedFapiao.CreatedBy = Guid.Parse(sqlReader[6].ToString());

                requestedFapiao.ModifiedOn = sqlReader[7].Equals(DBNull.Value) ? new Nullable<DateTime>() : Convert.ToDateTime(sqlReader[7].ToString());
                requestedFapiao.ModifiedBy = sqlReader[8].Equals(DBNull.Value) ? new Nullable<Guid>() : Guid.Parse(sqlReader[8].ToString());
            }

            return requestedFapiao;
        }

        private static ProxyFapiaoRequest[] ReadMultipleFapiaoRequestFromDataReader(SqlDataReader sqlReader)
        {
            List<ProxyFapiaoRequest> requestedFapiaos = new List<ProxyFapiaoRequest>();


            while (sqlReader.Read())
            {
                ProxyFapiaoRequest requestedFapiao = new ProxyFapiaoRequest();
                //[ID] [int] IDENTITY(1,1) NOT NULL,
                //[ProxyBookingID] [int] NOT NULL,
                //[FapiaoPreferenceID] [uniqueidentifier] NOT NULL,
                //[FapiaoSource] [smallint] NOT NULL,

                //[State] [tinyint] NOT NULL,
                //[CreatedOn] [datetime] NOT NULL,
                //[CreatedBy] [uniqueidentifier] NOT NULL,
                //[ModifiedOn] [datetime] NULL,
                //[ModifiedBy] [uniqueidentifier] NULL,

                requestedFapiao.ID = Convert.ToInt32(sqlReader[0].ToString());

                requestedFapiao.ProxyBookingID = Convert.ToInt32(sqlReader[1].ToString());

                requestedFapiao.FapiaoPreferenceID = sqlReader[2].Equals(DBNull.Value)? new Nullable<Guid>() : Guid.Parse(sqlReader[2].ToString());

                requestedFapiao.FapiaoSource = Convert.ToInt32(sqlReader[3].ToString());

                requestedFapiao.State = Convert.ToInt32(sqlReader[4].ToString());

                requestedFapiao.CreatedOn = Convert.ToDateTime(sqlReader[5].ToString());
                requestedFapiao.CreatedBy = Guid.Parse(sqlReader[6].ToString());

                requestedFapiao.ModifiedOn = sqlReader[7].Equals(DBNull.Value) ? new Nullable<DateTime>() : Convert.ToDateTime(sqlReader[7].ToString());
                requestedFapiao.ModifiedBy = sqlReader[8].Equals(DBNull.Value) ? new Nullable<Guid>() : Guid.Parse(sqlReader[8].ToString());

                requestedFapiaos.Add(requestedFapiao);
            }

            return requestedFapiaos.ToArray();
        }

        private static ProxyFapiaoRequest[] ReadMultipleFapiaoRequestFullFromDataReader(SqlDataReader sqlReader)
        {
            List<ProxyFapiaoRequest> requestedFapiaos = new List<ProxyFapiaoRequest>();

            while (sqlReader.Read())
            {
                ProxyFapiaoRequest requestedFapiao = new ProxyFapiaoRequest();
                //[ID] [int] IDENTITY(1,1) NOT NULL,
                //[ProxyBookingID] [int] NOT NULL,
                //[FapiaoPreferenceID] [uniqueidentifier] NOT NULL,
                //[FapiaoSource] [smallint] NOT NULL,

                //[State] [tinyint] NOT NULL,
                //[CreatedOn] [datetime] NOT NULL,
                //[CreatedBy] [uniqueidentifier] NOT NULL,
                //[ModifiedOn] [datetime] NULL,
                //[ModifiedBy] [uniqueidentifier] NULL,

                requestedFapiao.ID = Convert.ToInt32(sqlReader[0].ToString());

                requestedFapiao.ProxyBookingID = Convert.ToInt32(sqlReader[1].ToString());

                requestedFapiao.FapiaoPreferenceID = sqlReader[2].Equals(DBNull.Value) ? new Nullable<Guid>() : Guid.Parse(sqlReader[2].ToString());

                requestedFapiao.FapiaoSource = Convert.ToInt32(sqlReader[3].ToString());

                requestedFapiao.State = Convert.ToInt32(sqlReader[4].ToString());

                requestedFapiao.CreatedOn = Convert.ToDateTime(sqlReader[5].ToString());
                requestedFapiao.CreatedBy = Guid.Parse(sqlReader[6].ToString());

                requestedFapiao.ModifiedOn = sqlReader[7].Equals(DBNull.Value) ? new Nullable<DateTime>() : Convert.ToDateTime(sqlReader[7].ToString());
                requestedFapiao.ModifiedBy = sqlReader[8].Equals(DBNull.Value) ? new Nullable<Guid>() : Guid.Parse(sqlReader[8].ToString());

                //FapiaoPreference Section
                //,fp.[Unique_ID]
                //,fp.[User_ID]
                //,fp.[Customer_Name]
                //,fp.[Mail_Type]
                //,fp.[Mail_Address]

                //,fp.[Mail_Phone]
                //,fp.[Addressee_Name]
                //,fp.[Fapiao_Type]
                
                //,fp.[State]
                //,fp.[CreatedOn]
                //,fp.[CreatedBy]
                //,fp.[ModifiedOn]
                //,fp.[ModifiedBy]

                if (requestedFapiao.FapiaoPreferenceID != null)
                {
                    ProxyFapiaoPreference FapiaoPreference = new ProxyFapiaoPreference();
                    FapiaoPreference.ID = sqlReader[9].ToString();
                    FapiaoPreference.UserID = sqlReader[10].ToString();
                    FapiaoPreference.CustomerName = sqlReader[11].ToString();
                    FapiaoPreference.MailType = sqlReader[12].ToString();
                    FapiaoPreference.MailAddress = sqlReader[13].ToString();

                    FapiaoPreference.MailPhone = sqlReader[14].ToString();
                    FapiaoPreference.AddresseeName = sqlReader[15].ToString();
                    FapiaoPreference.FapiaoType = Convert.ToInt32(sqlReader[16].ToString());

                    FapiaoPreference.State = Convert.ToInt32(sqlReader[17].ToString());

                    FapiaoPreference.CreatedOn = Convert.ToDateTime(sqlReader[18].ToString());
                    FapiaoPreference.CreatedBy = Guid.Parse(sqlReader[19].ToString());

                    requestedFapiao.ModifiedOn = sqlReader[20].Equals(DBNull.Value) ? new Nullable<DateTime>() : Convert.ToDateTime(sqlReader[20].ToString());
                    FapiaoPreference.ModifiedBy = sqlReader[21].Equals(DBNull.Value) ? new Nullable<Guid>() : Guid.Parse(sqlReader[21].ToString());
                    requestedFapiao.FapiaoPreference = FapiaoPreference; 
                }

                requestedFapiaos.Add(requestedFapiao);
            }

            return requestedFapiaos.ToArray();
        }


        #endregion
    }
}
