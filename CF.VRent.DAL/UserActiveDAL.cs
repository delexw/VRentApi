using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CF.VRent.DAL
{
    public class UserActiveDAL
    {
        public bool GetUserActive(string email)
        {
            SqlParameter[] paras = new SqlParameter[]
            {
                new SqlParameter("@Active_Email", email)
            };

            int res = Convert.ToInt32(SQLHelper.ExecuteScalar(null, "Sp_User_Active_Check", System.Data.CommandType.StoredProcedure, paras));

            if (res > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ActiveUser(string email)
        {
            SqlParameter[] paras = new SqlParameter[]
            {
                new SqlParameter("@Active_Email", email)
            };

            int res = SQLHelper.ExecuteNonQuery(null, "Sp_User_Active", System.Data.CommandType.StoredProcedure, paras);

            if (res > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
