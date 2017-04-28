using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using CF.VRent.DataAccessProxy.Entities;
using CF.VRent.Common;



namespace CF.VRent.DAL
{
    /// <summary>
    /// DAL for user setting
    /// </summary>
    public class UserSettingDAL
    {
        //public ProxyUserSetting GetUserSetting(string id)
        //{
        //    return null;
        //}



        /// <summary>
        /// Login the system
        /// </summary>
        /// <param name="uname">user name</param>
        /// <param name="upwd">user pwd</param>
        /// <returns>use setting object</returns>
        //public UserSetting Login(string uname, string upwd)
        //{
        //    upwd = ":md5:" + Encrypt.EncryptMD5(upwd);


        //    KEMASWSIF_AUTHRef.WSKemasPortTypeClient clientLogin = new KEMASWSIF_AUTHRef.WSKemasPortTypeClient();
        //    var res = clientLogin.authByLogin(uname, upwd);

        //    if (res!=null && Convert.ToInt32(res.Result) == 0)
        //    {
        //        //create a new usersetting
        //        UserSetting uSetting = new UserSetting()
        //        {
        //            AllowChangePwd = Convert.ToInt32(res.AllowChangePwd),
        //            Blocked = Convert.ToInt32(res.Blocked),
        //            ChangePwd = Convert.ToInt32(res.ChangePwd),
        //            Enabled = Convert.ToInt32(res.Enabled),
        //            ID = res.ID,
        //            Name = res.Name,
        //            Result = Convert.ToInt32(res.Result),
        //            VName = res.VName
        //        };

        //        return uSetting;
        //    }
        //    else
        //        return null;
        //}


        public int LoginWeb(string uname, string upwd)
        {
            upwd = ":md5:" + Encrypt.EncryptMD5(upwd);

            SqlParameter[] paras = new SqlParameter[]
            {
                new SqlParameter("@User_Email", uname),
                new SqlParameter("@User_Password",upwd)
            };

            return Convert.ToInt32(SQLHelper.ExecuteScalar(null, "Sp_User_login", System.Data.CommandType.StoredProcedure, paras));
        }
    }
}
