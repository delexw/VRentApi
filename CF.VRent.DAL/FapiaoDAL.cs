using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.DataAccessProxy.Entities;
using CF.VRent.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace CF.VRent.DAL
{
    public class FapiaoDataDAL
    {

        private const string RetrieveMyFaPiaos = "Sp_GetMyFapiaoDataByUserID";
        private const string RetrieveFaPiaoDetail = "Sp_RetrieveFapiaoDataDetail";

        private const string CreateFaPiaoDataRecord = "Sp_CreateFaPiaoData";
        private const int ValidRecordCount = 1;


        public static ProxyFapiao[] RetrieveAllMyFapiaoData(Guid BookingUserID)
        {
            ProxyFapiao[] fapiaoData = null;
            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlParameter userPara = new SqlParameter("@UserID", BookingUserID);
            parameters.Add(userPara);

            fapiaoData = DataAccessProxyConstantRepo.ExecuteSPReturnReader<ProxyFapiao[]>(RetrieveMyFaPiaos, parameters.ToArray(), (sqldatareader) => ReadMutipleFapiaoDataFromDataReader(sqldatareader));

            parameters.Clear();

            return fapiaoData;
        }

        public static ProxyFapiao RetrieveFapiaoDataDetail(int faPiaoDataID)
        {

            ProxyFapiao fapiaoData = null;
            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlParameter userPara = new SqlParameter("@FapiaoDataID", faPiaoDataID);
            parameters.Add(userPara);

            fapiaoData = DataAccessProxyConstantRepo.ExecuteSPReturnReader<ProxyFapiao>(RetrieveFaPiaoDetail, parameters.ToArray(), (sqldatareader) => ReadSingleFapiaoDataFromDataReader(sqldatareader));

            parameters.Clear();

            return fapiaoData;
        }

        #region helper Method

        private static ProxyFapiao[] ReadMutipleFapiaoDataFromDataReader(SqlDataReader sqlReader) 
        {

            List<ProxyFapiao> fapiaoData = new List<ProxyFapiao>();
            while(sqlReader.Read())
            {
                ProxyFapiao pfDetail = new ProxyFapiao();
                pfDetail.ID = Convert.ToInt32(sqlReader[0]);
                pfDetail.OrderID = Convert.ToInt32(sqlReader[1]);

                pfDetail.UniqueID = sqlReader[2].Equals(DBNull.Value) ? null : sqlReader[2].ToString();
                pfDetail.DealNumber = sqlReader[3].Equals(DBNull.Value) ? null : sqlReader[3].ToString();
                pfDetail.ContractNumber = sqlReader[4].Equals(DBNull.Value) ? null : sqlReader[4].ToString();

                pfDetail.CustomerCode = sqlReader[5].ToString();
                pfDetail.CustomerName = sqlReader[6].Equals(DBNull.Value) ? null : sqlReader[6].ToString();
                pfDetail.TaxRegistrationID = sqlReader[7].Equals(DBNull.Value) ? null : sqlReader[7].ToString();
                pfDetail.CustomerAddress = sqlReader[8].Equals(DBNull.Value) ? null : sqlReader[8].ToString();
                pfDetail.CustomerPhone = sqlReader[9].Equals(DBNull.Value) ? null : sqlReader[9].ToString();

                pfDetail.BankName = sqlReader[10].Equals(DBNull.Value) ? null : sqlReader[10].ToString();
                pfDetail.BankAccount = sqlReader[11].Equals(DBNull.Value) ? null : sqlReader[11].ToString();

                pfDetail.FPCustomerName = sqlReader[12].Equals(DBNull.Value) ? null : sqlReader[12].ToString();
                pfDetail.FPMailType = sqlReader[13].Equals(DBNull.Value) ? null : sqlReader[13].ToString();
                pfDetail.FPMailingAddress = sqlReader[14].Equals(DBNull.Value) ? null : sqlReader[14].ToString();
                pfDetail.FPMailingPhone = sqlReader[15].Equals(DBNull.Value) ? null : sqlReader[15].ToString();
                pfDetail.FPAddresseeName = sqlReader[16].Equals(DBNull.Value) ? null : sqlReader[16].ToString();

                pfDetail.ProductCode = sqlReader[17].Equals(DBNull.Value) ? null : sqlReader[17].ToString();
                pfDetail.SpecMode = sqlReader[18].Equals(DBNull.Value) ? null : sqlReader[18].ToString();
                pfDetail.UnitMeasure = sqlReader[19].Equals(DBNull.Value) ? null : sqlReader[19].ToString();

                pfDetail.SalesQuantity = sqlReader[20].Equals(DBNull.Value) ? default(int) : Convert.ToInt32(sqlReader[20]);
                pfDetail.UnitPrice = sqlReader[21].Equals(DBNull.Value) ? default(decimal) : Convert.ToDecimal(sqlReader[21]);
                pfDetail.AmountExclVAT = sqlReader[22].Equals(DBNull.Value) ? default(decimal) : Convert.ToDecimal(sqlReader[22]);
                pfDetail.TaxRate = sqlReader[23].Equals(DBNull.Value) ? default(decimal) : Convert.ToDecimal(sqlReader[23]);
                pfDetail.Tax = sqlReader[24].Equals(DBNull.Value) ? default(decimal) : Convert.ToDecimal(sqlReader[24]);
                pfDetail.AmountIncVAT = sqlReader[25].Equals(DBNull.Value) ? default(int) : Convert.ToDecimal(sqlReader[25]);

                pfDetail.FapiaoType = Convert.ToInt32(sqlReader[26]);
                pfDetail.Remark = sqlReader[27].Equals(DBNull.Value) ? null : sqlReader[27].ToString();

                pfDetail.FapiaoNumber = sqlReader[28].Equals(DBNull.Value) ? default(int) : Convert.ToInt32(sqlReader[28]);
                pfDetail.FapiaoCode = sqlReader[29].Equals(DBNull.Value) ? default(int) : Convert.ToInt32(sqlReader[29]);
                pfDetail.IssueDate = sqlReader[30].Equals(DBNull.Value) ? new Nullable<DateTime>() : Convert.ToDateTime(sqlReader[30]);
                pfDetail.MailID = sqlReader[31].Equals(DBNull.Value) ? null : sqlReader[31].ToString();

                pfDetail.FapiaoState = Convert.ToInt32(sqlReader[32]);

                pfDetail.CreatedOn = Convert.ToDateTime(sqlReader[33].ToString());
                pfDetail.CreatedBy = Guid.Parse(sqlReader[34].ToString());
                pfDetail.ModifiedOn = sqlReader[35].Equals(DBNull.Value) ? new Nullable<DateTime>() : Convert.ToDateTime(sqlReader[35].ToString());
                pfDetail.ModifiedBy = sqlReader[36].Equals(DBNull.Value) ? new Nullable<Guid>() : Guid.Parse(sqlReader[36].ToString());

                fapiaoData.Add(pfDetail);
            }

            return fapiaoData.ToArray();
        }

        private static ProxyFapiao ReadSingleFapiaoDataFromDataReader(SqlDataReader sqlReader)
        {
            ProxyFapiao pfDetail = null;
            while (sqlReader.Read())
            {
 
            pfDetail = new ProxyFapiao();
              //vbs.[ID]
              //,[ItemID]

              //,[UniqueID]
              //,[DealNumber]
              //,[ContractNumber]

              //,[CustomerID]
              //,[CustomerName]
              //,[TaxID]
              //,[CustomerAddress]
              //,[CustomerPhone]

              //,[BankName]
              //,[BankAccount]

              //,[FPCustomerName]
              //,[FPMailType]
              //,[FPMailAddress]
              //,[FPMailPhone]
              //,[FPAddresseeName]

              //,[ProductCode]
              //,[Spec_Mode]
              //,[UnitMeasure]
              //,[SalesQuantity]
              //,[UnitPrice]
              //,[NetAmount]
              //,[TaxRate]
              //,[Tax]
              //,[TotalAmount]

              //,[FapiaoType]
              //,[Remark]

              //,[GeneratedFapiaoNumber]
              //,[GeneratedFapiaoCode]
              //,[FapiaoIssueDate]
              //,[DeliverID]

              //,vbs.[State]
              //,vbs.[CreatedOn]
              //,vbs.[CreatedBy]
              //,vbs.[ModifiedOn]
              //,vbs.[ModifiedBy]

            pfDetail.ID = Convert.ToInt32(sqlReader[0]);
            pfDetail.OrderID = Convert.ToInt32(sqlReader[1]);

            pfDetail.UniqueID = sqlReader[2].Equals(DBNull.Value) ? null : sqlReader[2].ToString();
            pfDetail.DealNumber = sqlReader[3].Equals(DBNull.Value) ? null : sqlReader[3].ToString();
            pfDetail.ContractNumber = sqlReader[4].Equals(DBNull.Value) ? null : sqlReader[4].ToString();

            pfDetail.CustomerCode = sqlReader[5].ToString();
            pfDetail.CustomerName = sqlReader[6].Equals(DBNull.Value) ? null : sqlReader[6].ToString();
            pfDetail.TaxRegistrationID = sqlReader[7].Equals(DBNull.Value) ? null : sqlReader[7].ToString();
            pfDetail.CustomerAddress = sqlReader[8].Equals(DBNull.Value) ? null : sqlReader[8].ToString();
            pfDetail.CustomerPhone = sqlReader[9].Equals(DBNull.Value) ? null : sqlReader[9].ToString();

            pfDetail.BankName = sqlReader[10].Equals(DBNull.Value) ? null : sqlReader[10].ToString();
            pfDetail.BankAccount = sqlReader[11].Equals(DBNull.Value) ? null : sqlReader[11].ToString();

            pfDetail.FPCustomerName = sqlReader[12].Equals(DBNull.Value) ? null : sqlReader[12].ToString();
            pfDetail.FPMailType = sqlReader[13].Equals(DBNull.Value) ? null : sqlReader[13].ToString();
            pfDetail.FPMailingAddress = sqlReader[14].Equals(DBNull.Value) ? null : sqlReader[14].ToString();
            pfDetail.FPMailingPhone = sqlReader[15].Equals(DBNull.Value) ? null : sqlReader[15].ToString();
            pfDetail.FPAddresseeName = sqlReader[16].Equals(DBNull.Value) ? null : sqlReader[16].ToString();

            pfDetail.ProductCode = sqlReader[17].Equals(DBNull.Value) ? null : sqlReader[17].ToString();
            pfDetail.SpecMode = sqlReader[18].Equals(DBNull.Value) ? null : sqlReader[18].ToString();
            pfDetail.UnitMeasure = sqlReader[19].Equals(DBNull.Value) ? null : sqlReader[19].ToString();

            pfDetail.SalesQuantity = sqlReader[20].Equals(DBNull.Value) ? default(int) : Convert.ToInt32(sqlReader[20]);
            pfDetail.UnitPrice = sqlReader[21].Equals(DBNull.Value) ? default(decimal) : Convert.ToDecimal(sqlReader[21]);
            pfDetail.AmountExclVAT = sqlReader[22].Equals(DBNull.Value) ? default(decimal) : Convert.ToDecimal(sqlReader[22]);
            pfDetail.TaxRate = sqlReader[23].Equals(DBNull.Value) ? default(decimal) : Convert.ToDecimal(sqlReader[23]);
            pfDetail.Tax = sqlReader[24].Equals(DBNull.Value) ? default(decimal) : Convert.ToDecimal(sqlReader[24]);
            pfDetail.AmountIncVAT = sqlReader[25].Equals(DBNull.Value) ? default(int) : Convert.ToDecimal(sqlReader[25]);

            pfDetail.FapiaoType = Convert.ToInt32(sqlReader[26]);
            pfDetail.Remark = sqlReader[27].Equals(DBNull.Value) ? null : sqlReader[27].ToString();

            pfDetail.FapiaoNumber = sqlReader[28].Equals(DBNull.Value) ? default(int) : Convert.ToInt32(sqlReader[28]);
            pfDetail.FapiaoCode = sqlReader[29].Equals(DBNull.Value) ? default(int) : Convert.ToInt32(sqlReader[29]);
            pfDetail.IssueDate = sqlReader[30].Equals(DBNull.Value) ? new Nullable<DateTime>() : Convert.ToDateTime(sqlReader[30]);
            pfDetail.MailID = sqlReader[31].Equals(DBNull.Value) ? null : sqlReader[31].ToString();

            pfDetail.FapiaoState = Convert.ToInt32(sqlReader[32]);

            pfDetail.CreatedOn = Convert.ToDateTime(sqlReader[33].ToString());
            pfDetail.CreatedBy = Guid.Parse(sqlReader[34].ToString());
            pfDetail.ModifiedOn = sqlReader[35].Equals(DBNull.Value) ? new Nullable<DateTime>() : Convert.ToDateTime(sqlReader[35].ToString());
            pfDetail.ModifiedBy = sqlReader[36].Equals(DBNull.Value) ? new Nullable<Guid>() : Guid.Parse(sqlReader[36].ToString());
            }

            return pfDetail;
        }

        public static ProxyFapiao CreateFapiaoData(ProxyFapiao fapiaoData)
        {
            ProxyFapiao FapiaoData = null;

            //refactor codes later
            SqlConnection sqlConn = null;
            SqlDataReader sqlReader = null;
            int cnt = 0;

            try
            {
                sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings[Constants.SqlConnStrKey].ConnectionString);
                using (SqlCommand cmd = new SqlCommand(CreateFaPiaoDataRecord, sqlConn))
                {
                    //
                    //    @ItemID int,
                    //    @UniqueID nvarchar(20),
                    //    @DealNumber nvarchar(20),
                    //    @ContractNumber nvarchar(20),
                    //    @CustomerID nvarchar(20),
                    //    @CustomerName nvarchar(20),
                    //    @TaxID nvarchar(20),
                    //    @CustomerAddress nvarchar(120),
                    //    @CustomerPhone nvarchar(20),
                    //    @BankName nvarchar(80),
                    //    @BankAccount nvarchar(80),
                    //    @FPCustomerName nvarchar(20),
                    //    @FPMailType nvarchar(80),
                    //    @FPMailAddress nvarchar(80),
                    //    @FPMailPhone nvarchar(20),
                    //    @FPAddresseeName nvarchar(50),
                    //    @ProductCode nvarchar(20),
                    //    @Spec_Mode nvarchar(20),
                    //    @UnitMeasure nvarchar(10),
                    //    @SalesQuantity numeric(3,0),
                    //    @UnitPrice numeric(14,2),
                    //    @NetAmount numeric(14,2),
                    //    @TaxRate numeric(5,4),
                    //    @Tax numeric(14,2),
                    //    @TotalAmount numeric(14,2),
                    //    @FapiaoType numeric(3,0),
                    //    @Remark nvarchar(230),
                    //    @GeneratedFapiaoNumber nvarchar(20),
                    //    @GeneratedFapiaoCode nvarchar(20),
                    //    @FapiaoIssueDate datetime,
                    //    @DeliverID nvarchar(100),
                    //    @State tinyint,
                    //    @CreatedOn datetime,
                    //    @CreatedBy uniqueidentifier
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ItemID", fapiaoData.OrderID);

                    cmd.Parameters.AddWithValue("@UniqueID", fapiaoData.UniqueID);
                    cmd.Parameters.AddWithValue("@DealNumber", fapiaoData.DealNumber);
                    cmd.Parameters.AddWithValue("@ContractNumber", fapiaoData.ContractNumber);

                    cmd.Parameters.AddWithValue("@CustomerID", fapiaoData.CustomerCode);
                    cmd.Parameters.AddWithValue("@CustomerName", fapiaoData.CustomerName);

                    cmd.Parameters.AddWithValue("@TaxID", fapiaoData.TaxRegistrationID);
                    cmd.Parameters.AddWithValue("@CustomerAddress", fapiaoData.CustomerAddress);
                    cmd.Parameters.AddWithValue("@CustomerPhone", fapiaoData.CustomerPhone);

                    cmd.Parameters.AddWithValue("@BankName", fapiaoData.BankName);
                    cmd.Parameters.AddWithValue("@BankAccount", fapiaoData.BankAccount);

                    cmd.Parameters.AddWithValue("@FPCustomerName", fapiaoData.FPCustomerName);
                    cmd.Parameters.AddWithValue("@FPMailType", fapiaoData.FPMailType);
                    cmd.Parameters.AddWithValue("@FPMailAddress", fapiaoData.FPMailingAddress);
                    cmd.Parameters.AddWithValue("@FPMailPhone", fapiaoData.FPMailingPhone);
                    cmd.Parameters.AddWithValue("@FPAddresseeName", fapiaoData.FPAddresseeName);

                    cmd.Parameters.AddWithValue("@ProductCode", fapiaoData.ProductCode);
                    cmd.Parameters.AddWithValue("@Spec_Mode", fapiaoData.SpecMode);
                    cmd.Parameters.AddWithValue("@UnitMeasure", fapiaoData.UnitMeasure);

                    cmd.Parameters.AddWithValue("@SalesQuantity", fapiaoData.SalesQuantity);
                    cmd.Parameters.AddWithValue("@UnitPrice", fapiaoData.UnitPrice);
                    cmd.Parameters.AddWithValue("@NetAmount", fapiaoData.AmountExclVAT);
                    cmd.Parameters.AddWithValue("@TaxRate", fapiaoData.TaxRate);
                    cmd.Parameters.AddWithValue("@Tax", fapiaoData.Tax);

                    cmd.Parameters.AddWithValue("@TotalAmount", fapiaoData.AmountIncVAT);
                    cmd.Parameters.AddWithValue("@FapiaoType", fapiaoData.FapiaoType);
                    cmd.Parameters.AddWithValue("@Remark", fapiaoData.Remark);
                    cmd.Parameters.AddWithValue("@GeneratedFapiaoNumber", fapiaoData.FapiaoNumber);

                    cmd.Parameters.AddWithValue("@GeneratedFapiaoCode", fapiaoData.FapiaoCode);
                    cmd.Parameters.AddWithValue("@FapiaoIssueDate", fapiaoData.IssueDate);
                    cmd.Parameters.AddWithValue("@DeliverID", fapiaoData.MailID);
                    cmd.Parameters.AddWithValue("@State", fapiaoData.FapiaoState);

                    cmd.Parameters.AddWithValue("@CreatedOn", fapiaoData.CreatedOn);
                    cmd.Parameters.AddWithValue("@CreatedBy", fapiaoData.CreatedBy);

                    sqlConn.Open();
                    sqlReader = cmd.ExecuteReader();
                    if (sqlReader != null && sqlReader.Read())
                    {
                        FapiaoData = ReadSingleFapiaoDataFromDataReader(sqlReader);
                    }
                }
            }
            catch (SqlException sqle)
            {
                ReturnResult ret = DataAccessProxyConstantRepo.ConvertSqlExceptionToReturnRet(sqle);
                throw new FaultException<ReturnResult>(ret,ret.Message);
            }
            catch (Exception e)
            {
                ReturnResult ret = DataAccessProxyConstantRepo.ConvertUnExpectedExceptionToReturnRet(e);
                throw new FaultException<ReturnResult>(ret,ret.Message);
            }

            finally
            {
                if (sqlReader != null)
                {
                    sqlReader.Close();
                    sqlReader.Dispose();
                    sqlReader = null;
                }
                if (sqlConn != null)
                {
                    sqlConn.Close();
                    sqlConn.Dispose();
                }
            }

            if (cnt != ValidRecordCount)
            {
                FapiaoData = null;
            }
            return fapiaoData;
        }
        #endregion
    }
}
