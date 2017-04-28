using CF.VRent.Common;
using CF.VRent.DataAccessProxy.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CF.VRent.DAL
{
    //public class UserDAL
    //{

    //    /// <summary>
    //    /// Register function for web user
    //    /// </summary>
    //    /// <param name="user"></param>
    //    /// <returns></returns>
    //    //public int Register(ProxyUser user)
    //    //{

    //    //    user.Password = ":md5:" + Encrypt.EncryptMD5(user.Password);

    //    //    //check web user database and insert
    //    //    SqlParameter[] paras = new SqlParameter[]
    //    //    {
    //    //        new SqlParameter("@User_Email", EscapeHelper.Escape(user.Mail)),
    //    //        new SqlParameter("@User_FirstName",EscapeHelper.Escape(user.VName)),
    //    //        new SqlParameter("@User_LastName",EscapeHelper.Escape(user.Name)), // map to last name
    //    //        new SqlParameter("@User_ContactNumber",EscapeHelper.Escape(user.Phone)),
    //    //        new SqlParameter("@User_Company",EscapeHelper.Escape(user.Company)),
    //    //        new SqlParameter("@User_Password",EscapeHelper.Escape(user.Password)),

                
    //    //        //@User_JobTitle nvarchar(50),
    //    //        //@User_OfficeLocation nvarchar(100),
    //    //        //@User_IsAcceptAppointment bit,
    //    //        //@User_IsShareInfo bit,
    //    //        //@User_IsNoImmediateNeed bit,

    //    //        user.UserJobTitle == null? new SqlParameter("@User_JobTitle",DBNull.Value):new SqlParameter("@User_JobTitle",EscapeHelper.Escape(user.UserJobTitle)),
    //    //        user.UserOfficeLocation == null? new SqlParameter("@User_OfficeLocation",DBNull.Value):new SqlParameter("@User_OfficeLocation",EscapeHelper.Escape(user.UserOfficeLocation)), //used original address
    //    //        user.UserLeadSource == null? new SqlParameter("@User_LeadSource",DBNull.Value):new SqlParameter("@User_LeadSource",EscapeHelper.Escape(user.UserLeadSource)),
    //    //        user.UserIsAcceptAppointment == null? new SqlParameter("@User_IsAcceptAppointment",DBNull.Value):new SqlParameter("@User_IsAcceptAppointment",EscapeHelper.Escape(user.UserIsAcceptAppointment)), //used original address
    //    //        user.UserIsShareInfo == null? new SqlParameter("@User_IsShareInfo",DBNull.Value):new SqlParameter("@User_IsShareInfo",EscapeHelper.Escape(user.UserIsShareInfo)),
    //    //        user.UserIsNoImmediateNeed == null? new SqlParameter("@User_IsNoImmediateNeed",DBNull.Value):new SqlParameter("@User_IsNoImmediateNeed",EscapeHelper.Escape(user.UserIsNoImmediateNeed)), //used original address

    //    //        //output parameter
    //    //        new SqlParameter("@res",System.Data.SqlDbType.Int)
    //    //    };

    //    //    paras[12].Direction = System.Data.ParameterDirection.Output;

    //    //    //DB stored procedure Sp_User_Register
    //    //    return Convert.ToInt32(SQLHelper.ExecuteScalar(null, "Sp_User_Register", System.Data.CommandType.StoredProcedure, paras, "@res"));
    //    //}

    //    /// <summary>
    //    /// Check the web user email exist
    //    /// </summary>
    //    /// <param name="email"></param>
    //    /// <returns></returns>
    //    //public bool WebUserEmailCheck(string email)
    //    //{
    //    //    SqlParameter[] paras = new SqlParameter[]
    //    //    {
    //    //        new SqlParameter("@User_Email", EscapeHelper.Escape(email))
    //    //    };

    //    //    int res = Convert.ToInt32(SQLHelper.ExecuteScalar(null, "Sp_User_CheckEmailExist", System.Data.CommandType.StoredProcedure, paras));
    //    //    if (res > 0)
    //    //    {
    //    //        return true;
    //    //    }
    //    //    else
    //    //    {
    //    //        return false;
    //    //    }
    //    //}

    //    /// <summary>
    //    /// Get web user information
    //    /// </summary>
    //    /// <param name="email"></param>
    //    /// <returns></returns>
    //    //public ProxyUser GetWebUserInfo(string email)
    //    //{
    //    //    ProxyUser user = null;
    //    //    SqlParameter[] paras = new SqlParameter[]
    //    //    {
    //    //        new SqlParameter("@User_Email", email)
               
    //    //    };


    //    //    DataTable dt = SQLHelper.ExecuteDataTable(null, "Sp_User_Read", System.Data.CommandType.StoredProcedure, paras);
    //    //    if (dt.Rows.Count > 0)
    //    //    {
    //    //        user = new ProxyUser();
    //    //        user.Mail = dt.Rows[0]["User_Email"].ToString();
    //    //        user.VName = dt.Rows[0]["User_FirstName"].ToString();
    //    //        user.Name = dt.Rows[0]["User_LastName"].ToString();
    //    //        user.Phone = dt.Rows[0]["User_ContactNumber"].ToString();
    //    //        user.Company = dt.Rows[0]["User_Company"].ToString();
    //    //        user.CreateDate = dt.Rows[0]["User_CreatOn"].ToString();
    //    //        user.IsWebUser = true;

    //    //        //Vrent CR(compaign lead generation)
    //    //        user.UserJobTitle = dt.Rows[0]["User_JobTitle"].ToString();
    //    //        user.UserOfficeLocation = dt.Rows[0]["User_OfficeLocation"].ToString();
    //    //        user.UserLeadSource = dt.Rows[0]["User_LeadSource"].ToString();
    //    //        user.UserIsAcceptAppointment = dt.Rows[0]["User_IsAcceptAppointment"].ToString();
    //    //        user.UserIsShareInfo = dt.Rows[0]["User_IsShareInfo"].ToString();
    //    //        user.UserIsNoImmediateNeed = dt.Rows[0]["User_IsNoImmediateNeed"].ToString();
    //    //    }
    //    //    //using (SqlDataReader dr = SQLHelper.ExecuteReader(null, "Sp_User_Read", System.Data.CommandType.StoredProcedure, paras))
    //    //    //{
    //    //    //    while (dr.Read())
    //    //    //    {
    //    //    //        user.Mail = dr["User_Email"].ToString();
    //    //    //        user.VName = dr["User_FirstName"].ToString();
    //    //    //        user.Name = dr["User_LastName"].ToString();
    //    //    //        user.Phone = dr["User_ContactNumber"].ToString();
    //    //    //        user.Company = dr["User_Company"].ToString();
    //    //    //        user.CreateDate = dr["User_CreatOn"].ToString();

    //    //    //        break;
    //    //    //    }
    //    //    //}

    //    //    return user;
    //    //}
    //}
}
