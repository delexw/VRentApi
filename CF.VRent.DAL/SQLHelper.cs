using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using CF.VRent.Common;
using CF.VRent.Common.Entities.DBEntity;

namespace CF.VRent.DAL
{
    public abstract class SQLHelper
    {
        //Database connection strings
        //public static readonly string ConnectionStringDefault = ConfigurationManager.ConnectionStrings["SQLConnString"].ConnectionString;
        public static readonly string ConnectionStringDefault = ConfigurationManager.ConnectionStrings[Constants.SqlConnStrKey].ConnectionString;
        //public static string ConnectionStringDefault = null;

        //Hashtable to store cached parameters
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// add parameter array to the cache
        /// </summary>
        /// <param name="cacheKey">Key to the parameter cache</param>
        /// <param name="cmdParameters">an array of SqlParamters to be cached</param>
        public static void SetCacheParameters(string cacheKey, params SqlParameter[] parameters)
        {
            parmCache[cacheKey] = parameters;
        }

        /// <summary>
        /// Retrieve cached parameters
        /// </summary>
        /// <param name="cacheKey">key used to lookup parameters</param>
        /// <returns>Cached SqlParamters array</returns>
        public static SqlParameter[] GetCacheParameters(string cacheKey)
        {
            SqlParameter[] cachedParms = (SqlParameter[])parmCache[cacheKey];
            if (cachedParms == null)
            {
                return null;
            }
            SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];
            for (int i = 0; i < cachedParms.Length; i++)
            {
                clonedParms[i] = (SqlParameter)((ICloneable)cachedParms[i]).Clone();
            }
            return clonedParms;
        }

        /// <summary>
        /// Prepare a command for execution
        /// </summary>
        /// <param name="cmd">SqlCommand object</param>
        /// <param name="conn">SqlConneciotn object</param>
        /// <param name="trans">SqlTransaction object</param>
        /// <param name="cmdType">Cmd type e.g. stored procedure or text</param>
        /// <param name="cmdText">Command text, e.g. Select * From Products</param>
        /// <param name="cmdParms">SqlParameters to use in the command</param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            cmd.CommandType = cmdType;
            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                {
                    cmd.Parameters.Add(parm);
                }
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the database specified in the connection string using the provided parameters.
        /// </summary>
        /// <param name="connectionString">a valid conneciotn string for a SqlConnection</param>
        /// <param name="cmdText">the stored procedure name or T-SQL command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string connectionString, string cmdText)
        {
            if (connectionString == null)
            {
                connectionString = ConnectionStringDefault;
            }
            return ExecuteNonQuery(connectionString, cmdText, CommandType.Text, null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the database specified in the connection string using the provided parameters.
        /// </summary>
        /// <param name="connectionString">a valid conneciotn string for a SqlConnection</param>
        /// <param name="cmdType">the CommandType (stored procedure,text,etc.)</param>
        /// <param name="cmdText">the stored procedure name or T-SQL command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string connectionString, string cmdText, CommandType cmdType)
        {
            if (connectionString == null)
            {
                connectionString = ConnectionStringDefault;
            }
            return ExecuteNonQuery(connectionString, cmdText, cmdType, null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the database specified in the connection string using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:
        /// int result=ExecuteNonQuery(connString,CommandType.StoredProcedure,"PublishOrders",new SqlParameter("@prodid",24));
        /// </remarks>
        /// <param name="connectionString">a valid conneciotn string for a SqlConnection</param>
        /// <param name="cmdType">the CommandType (stored procedure,text,etc.)</param>
        /// <param name="cmdText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParameters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string connectionString, string cmdText, CommandType cmdType, params SqlParameter[] commandParameters)
        {
            if (connectionString == null)
            {
                connectionString = ConnectionStringDefault;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                    int val = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return val;
                }
            }
        }

        /// <summary>
        /// Insert records in batch
        /// The sequence of the properties of Obj T must be the same as the sequence of the comlums of relative DataTable 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="records"></param>
        public static bool InsertNonQueryBatch<T>(IEnumerable<T> records)
        {
            var table = records.ConvertToDataTable();
            using (SqlConnection conn = new SqlConnection(ConnectionStringDefault))
            {
                SqlTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();
                    using (var sqlBulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                    {
                        sqlBulkCopy.BatchSize = table.Rows.Count;
                        sqlBulkCopy.DestinationTableName = table.TableName;
                        sqlBulkCopy.WriteToServer(table);
                    }
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                    throw;
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) using an existing SQL Transaction using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.: 
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="trans">an existing sql transaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        //public static int ExecuteNonQuery(SqlTransaction trans, string cmdText, CommandType cmdType, params SqlParameter[] commandParameters)
        //{

        //    int val = 0;
        //    using (SqlCommand cmd = new SqlCommand())
        //    {
        //        PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
        //        val = cmd.ExecuteNonQuery();
        //        cmd.Parameters.Clear();
        //    }
        //    return val;
        //}

        /// <summary>
        /// Execute a SqlCommand that returns a resultset against the database specified in the connection string using the provided parameters
        /// </summary>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="cmdType">the CommandType(stored procedure,text,etc.)</param>
        /// <param name="cmdText">the stroed procedure name or T-SQL command</param>
        /// <param name="cmdParameters">an array of SqlParameters used to execute the command</param>
        /// <returns>A SqlDataReader containing the results</returns>
        public static SqlDataReader ExecuteReader(string connectionString, string cmdText, CommandType cmdType, params SqlParameter[] cmdParameters)
        {
            if (connectionString == null)
            {
                connectionString = ConnectionStringDefault;
            }
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connectionString);
            // we use a try/catch here because if the method throws an exception we want to
            // close the connection throw code, because no datareader will exist, hence the
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParameters);
                  SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                //SqlDataReader dr = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                return dr;
            }
            catch (Exception)
            {
                conn.Close();
                throw;
            }
        }

        /// <summary>
        /// Execute a SqlCommand that return a resultset against the database specified in the connection string using the provided parameters
        /// </summary>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="cmdType">the CommandType(stored procedure,text,etc.)</param>
        /// <param name="cmdText">the stored procedure name or T-SQL command</param>
        /// <param name="cmdParameters">an array of SqlParameters userd to execute the command</param>
        /// <returns>A DataTable containing the results</returns>
        public static DataTable ExecuteDataTable(string connectionString, string cmdText, CommandType cmdType, params SqlParameter[] cmdParameters)
        {
            if (connectionString == null)
            {
                connectionString = ConnectionStringDefault;
            }
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParameters);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        /// <summary>
        /// Execute a SqlCommand that returns the first column of the first record against the database specified in the connection string
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.: 
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParameters used to execute the command</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        public static object ExecuteScalar(string connectionString, string cmdText, CommandType cmdType, params SqlParameter[] commandParameters)
        {
            if (connectionString == null)
            {
                connectionString = ConnectionStringDefault;
            }
            object val = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                    val = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                }
                return val;
            }
        }

        public static object ExecuteScalar(string connectionString, string cmdText, CommandType cmdType, SqlParameter[] commandParameters,string outParametersName)
        {
            if (connectionString == null)
            {
                connectionString = ConnectionStringDefault;
            }
            object val = null;
            object res = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                    val = cmd.ExecuteScalar();
                    res = cmd.Parameters[outParametersName].Value;
                    cmd.Parameters.Clear();
                }
                return res;
            }
        }

    }
}