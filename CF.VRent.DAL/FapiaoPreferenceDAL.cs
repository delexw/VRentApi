using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using CF.VRent.DataAccessProxy.Entities;
using CF.VRent.Log;
using CF.VRent.Common;
using System.ServiceModel;
using CF.VRent.Common.Entities;

namespace CF.VRent.DAL
{    

    public class FapiaoPreferenceDAL
    {
        public const string GetAllFapiaoPreference_SP = "Sp_GetFapiaoPreferences";
        public const string GetFapiaoPreferenceByUniqueID_SP = "Sp_GetFapiaoPreferenceByUniqueID";
        public const string CreateFapiaoPreferences_SP = "Sp_FapiaoPreferences_Create";
        public const string UpdateFapiaoPreferences_SP = "Sp_FapiaoPreferences_Update";
        public const string DeleteFapiaoPreferences_SP = "Sp_FapiaoPreferences_Delete";

        public const string UpdateExistingFapiaoPreferences_SP = "Sp_FapiaoPreferences_UpdateExisting";

        public static List<ProxyFapiaoPreference> GetAllFapiaoPreference(string uid)
        {
            List<ProxyFapiaoPreference> fpList = new List<ProxyFapiaoPreference>();

            try
            {
                SqlParameter[] paras = new SqlParameter[]
            {
                new SqlParameter("@uid", uid)               
            };

                DataTable dt = SQLHelper.ExecuteDataTable(null, GetAllFapiaoPreference_SP, System.Data.CommandType.StoredProcedure, paras);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        ProxyFapiaoPreference fp = new ProxyFapiaoPreference();
                        fp.ID = row["Unique_ID"].ToString();
                        fp.UserID = row["User_ID"].ToString();
                        fp.CustomerName = row["Customer_Name"].ToString();
                        fp.MailType = row["Mail_Type"].ToString();
                        fp.MailAddress = row["Mail_Address"].ToString();
                        fp.MailPhone = row["Mail_Phone"].ToString();
                        fp.AddresseeName = row["Addressee_Name"].ToString();
                        fp.FapiaoType = Convert.ToInt32(row["Fapiao_Type"]);
                        fp.State = Convert.ToInt32(row["State"].ToString());

                        fp.CreatedBy = (Guid)row["CreatedBy"];
                        fp.CreatedOn = Convert.ToDateTime(row["CreatedOn"]);
                        fp.ModifiedBy = row["ModifiedBy"].Equals(DBNull.Value) ? new Nullable<Guid>() : (Guid)row["ModifiedBy"];
                        fp.ModifiedOn = row["ModifiedOn"].Equals(DBNull.Value) ? new Nullable<DateTime>() : Convert.ToDateTime(row["ModifiedOn"]);

                        fpList.Add(fp);
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
            

          
            return fpList;
        }

        public static ProxyFapiaoPreference GetFapiaoPreferenceDetail(string fpid)
        {
            ProxyFapiaoPreference fp = null;

                SqlParameter[] paras = new SqlParameter[]
            {
                new SqlParameter("@UniqueID", fpid)               
            };

                DataTable dt = SQLHelper.ExecuteDataTable(null, GetFapiaoPreferenceByUniqueID_SP, System.Data.CommandType.StoredProcedure, paras);
                if (dt.Rows.Count > 0)
                {
                    fp = new ProxyFapiaoPreference();
                    DataRow row = dt.Rows[0];
                    fp.ID = row["Unique_ID"].ToString();
                    fp.UserID = row["User_ID"].ToString();
                    fp.CustomerName = row["Customer_Name"].ToString();
                    fp.MailType = row["Mail_Type"].ToString();
                    fp.MailAddress = row["Mail_Address"].ToString();
                    fp.MailPhone = row["Mail_Phone"].ToString();
                    fp.AddresseeName = row["Addressee_Name"].ToString();
                    fp.FapiaoType = Convert.ToInt32(row["Fapiao_Type"]);

                    fp.CreatedBy = (Guid)row["CreatedBy"];
                    fp.CreatedOn = Convert.ToDateTime(row["CreatedOn"]);
                    fp.ModifiedBy = row["ModifiedBy"].Equals(DBNull.Value) ? new Nullable<Guid>() : (Guid)row["ModifiedBy"];
                    fp.ModifiedOn = row["ModifiedOn"].Equals(DBNull.Value) ? new Nullable<DateTime>() : Convert.ToDateTime(row["ModifiedOn"]);
                    fp.State = Convert.ToInt32(row["State"].ToString());
                } 

            return fp;
        }

        public static ProxyFapiaoPreference SaveFapiaoPreference(ProxyFapiaoPreference fp)
        {
            ProxyFapiaoPreference fapiaoPreference = default(ProxyFapiaoPreference);

                SqlParameter[] paras = new SqlParameter[]
                {
                    new SqlParameter("@Unique_ID", fp.ID),                
                    new SqlParameter("@Customer_Name", fp.CustomerName),
                    new SqlParameter("@User_ID", fp.UserID),
                    new SqlParameter("@Mail_Type", fp.MailType),
                    new SqlParameter("@Mail_Address", fp.MailAddress),
                    new SqlParameter("@Mail_Phone", fp.MailPhone),
                    new SqlParameter("@Addressee_Name", fp.AddresseeName),
                    new SqlParameter("@Fapiao_Type", fp.FapiaoType),
                    new SqlParameter("@CreatedOn", fp.CreatedOn),
                    new SqlParameter("@CreatedBy", fp.CreatedBy),
                    new SqlParameter("@State", fp.State)
                };

                int res = SQLHelper.ExecuteNonQuery(null, CreateFapiaoPreferences_SP, System.Data.CommandType.StoredProcedure, paras);

                if (res > 0)
                {
                    fapiaoPreference = GetFapiaoPreferenceDetail(fp.ID);
                }
 

            return fapiaoPreference;
        }

        public static ProxyFapiaoPreference UpdateFapiaoPreference(ProxyFapiaoPreference oldfp, ProxyFapiaoPreference newFP)
        {
            ProxyFapiaoPreference fapiaoPreference = default(ProxyFapiaoPreference);


                SqlParameter[] paras = new SqlParameter[]
            {
                //--insert new one
                //    @NewUnique_ID nvarchar(200),
                //    @User_ID nvarchar(160),
                //    @Customer_Name nvarchar(80),
                //    @Mail_Type nvarchar(80),
                //    @Mail_Address nvarchar(80),
                //    @Mail_Phone nvarchar(20),
                //    @Addressee_Name nvarchar(50),
                //    @Fapiao_Type numeric(3,0),
                //    @NewState tinyint,
                //    @CreatedOn datetime,
                //    @CreatedBy uniqueidentifier,
                //--deactivate old one

                //    @OldUnique_ID nvarchar(200)

                //create a new one and mark the old one as deleted
                new SqlParameter("@NewUnique_ID", newFP.ID),                
                new SqlParameter("@User_ID", newFP.UserID),                
                new SqlParameter("@Customer_Name", newFP.CustomerName),
                new SqlParameter("@Mail_Type", newFP.MailType),
                new SqlParameter("@Mail_Address", newFP.MailAddress),
                new SqlParameter("@Mail_Phone", newFP.MailPhone),
                new SqlParameter("@Addressee_Name", newFP.AddresseeName),
                new SqlParameter("@Fapiao_Type", newFP.FapiaoType),
                new SqlParameter("@NewState",newFP.State),
                new SqlParameter("@CreatedOn", newFP.CreatedOn),
                new SqlParameter("@CreatedBy", newFP.CreatedBy),

                //old one
                new SqlParameter("@OldUnique_ID", oldfp.ID),
            };

                int res = SQLHelper.ExecuteNonQuery(null, UpdateExistingFapiaoPreferences_SP, System.Data.CommandType.StoredProcedure, paras);

                if (res < 0)
                {
                    fapiaoPreference = GetFapiaoPreferenceDetail(newFP.ID);
                }
 

            return fapiaoPreference;
        }

        public static void DeleteFapiaoPreference(string fpid)
        {

                SqlParameter[] paras = new SqlParameter[]
            {
                new SqlParameter("@UniqueID", fpid)               
            };

                int res = SQLHelper.ExecuteNonQuery(null, DeleteFapiaoPreferences_SP, System.Data.CommandType.StoredProcedure, paras); 
        }
    }

}
