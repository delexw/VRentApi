using CF.VRent.Common;
using CF.VRent.Common.Entities;
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
    public class DataAccessProxyConstantRepo
    {
        public const string DataAccessProxySqlExceptionCode = "CVD000001";
        public const string DataAccessProxySqlExceptionMessage = "Sql Exception occurrued, Message: {0}, StackTrace:{1}";

        public const string DataAccessProxyUnknownExceptionCode = "CVD000002";
        public const string DataAccessProxyUnknownExceptionMessage = "Unknown Exception occurred, Message: {0}, StackTrace:{1}";

        public const string DataAccessProxyBadDataExceptionCode = "CVD000003";
        public const string DataAccessProxyBadDataExceptionMessage = "Bad Data exists, Message: {0}, StackTrace:{1}";

        public const int SPExecutionSuccess = 0; //db result
        public const string SPReturnParameter = "@ReturnValue";


        public static ReturnResult ConvertSqlExceptionToReturnRet(SqlException sqle) 
        {
            return new ReturnResult()
            {
                Code = DataAccessProxyConstantRepo.DataAccessProxySqlExceptionCode,
                Message = string.Format(DataAccessProxyConstantRepo.DataAccessProxyUnknownExceptionMessage, sqle.Message, sqle.StackTrace),
                Type = ResultType.DATAACCESSProxy
            };
        }

        public static ReturnResult ConvertUnExpectedExceptionToReturnRet(Exception e)
        {
            return new ReturnResult()
            {

                Code = DataAccessProxyConstantRepo.DataAccessProxyUnknownExceptionCode,
                Message = string.Format(DataAccessProxyConstantRepo.DataAccessProxyUnknownExceptionMessage, e.Message, e.StackTrace),
                Type = ResultType.DATAACCESSProxy
            };
        }

        public static T DataAccessExceptionGuard<T>(Func<T> function)
        {
            try
            {
                T r = function();
                return r;
            }
            catch (Exception e)
            {
                if (e is VrentApplicationException)
                {
                    throw e;
                }
                else
                {
                    //LogInfor.WriteError(Constants.DBExceptionTitle, string.Format(Constants.DBExceptionTitle, e.Message, e.StackTrace), string.Empty);
                    ReturnResult ret = DataAccessProxyConstantRepo.ConvertUnExpectedExceptionToReturnRet(e);
                    throw new FaultException<ReturnResult>(ret, ret.Message);
                }
            }
        }

        public static void DataAccessExceptionGuard(Action function)
        {
            try
            {
                function();
            }
            catch (Exception e)
            {
                if (e is VrentApplicationException)
                {
                    throw e;
                }
                else
                {
                    ReturnResult ret = DataAccessProxyConstantRepo.ConvertUnExpectedExceptionToReturnRet(e);
                    throw new FaultException<ReturnResult>(ret, ret.Message);
                }
            }
        }

        public static int ExecuteSPNonQuery(string spName, SqlParameter[] parameters)
        {
            //refactor codes later
            SqlConnection sqlConn = null;
            SqlDataReader sqlReader = null;
            int affected = -1;
            try
            {
                sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings[Constants.SqlConnStrKey].ConnectionString);
                using (SqlCommand cmd = new SqlCommand(spName, sqlConn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parameters);

                    sqlConn.Open();
                    affected = cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException sqle)
            {
                //string msg = string.Format(Constants.SqlExceptionLogPattern, sqle.Message, sqle.StackTrace);
                //LogInfor.WriteError(Constants.DBExceptionTitle,msg , string.Empty);
                ReturnResult ret = DataAccessProxyConstantRepo.ConvertSqlExceptionToReturnRet(sqle);
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
            return affected;
        }

        public static T ExecuteSPReturnReader<T>(string spName, SqlParameter[] parameters, Func<SqlDataReader, T> read)
        {
            //refactor codes later
            SqlConnection sqlConn = null;
            SqlDataReader sqlReader = null;
            T t = default(T);
            try
            {
                sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings[Constants.SqlConnStrKey].ConnectionString);
                using (SqlCommand cmd = new SqlCommand(spName, sqlConn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parameters);


                    sqlConn.Open();
                    sqlReader = cmd.ExecuteReader();

                    int ret = 0;
                    //int returnRet = Convert.ToInt32(cmd.Parameters[SPReturnParameter]);

                    if (ret == SPExecutionSuccess && sqlReader != null)
                    {
                        t = read(sqlReader);
                    }
                }
            }
            catch (SqlException sqle)
            {
                //string msg = string.Format(Constants.SqlExceptionLogPattern, sqle.Message, sqle.StackTrace);
                //LogInfor.WriteError(Constants.DBExceptionTitle,msg , string.Empty);
                ReturnResult ret = DataAccessProxyConstantRepo.ConvertSqlExceptionToReturnRet(sqle);
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
            return t;
        }

        public static T ExecuteSPReturnReaderAdv<T>(string spName, SqlParameter[] parameters, Func<SqlDataReader, T> read, ref int processingResult)
        {
            //refactor codes later
            SqlConnection sqlConn = null;
            SqlDataReader sqlReader = null;
            T t = default(T);
            try
            {
                sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings[Constants.SqlConnStrKey].ConnectionString);
                using (SqlCommand cmd = new SqlCommand(spName, sqlConn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parameters);


                    sqlConn.Open();
                    sqlReader = cmd.ExecuteReader();

                    if (sqlReader != null && !sqlReader.IsClosed)
                    {
                        t = read(sqlReader);
                        sqlReader.Close();
                    }


                    processingResult = Convert.ToInt32(cmd.Parameters[SPReturnParameter].Value);
                }
            }
            catch (SqlException sqle)
            {
                //string msg = string.Format(Constants.SqlExceptionLogPattern, sqle.Message, sqle.StackTrace);
                //LogInfor.WriteError(Constants.DBExceptionTitle,msg , string.Empty);
                ReturnResult ret = DataAccessProxyConstantRepo.ConvertSqlExceptionToReturnRet(sqle);
                throw new FaultException<ReturnResult>(ret, ret.Message);
            }
            finally
            {
                if (sqlReader != null)
                {
                    if (!sqlReader.IsClosed)
                    {
                        sqlReader.Close(); 
                    }
                    sqlReader.Dispose();
                    sqlReader = null;
                }
                if (sqlConn != null)
                {
                    sqlConn.Close();
                    sqlConn.Dispose();
                }
            }
            return t;
        }



        public static T[] RetrieveItemsWithPaging<T>(string tableName, string pkColumn,int itemsPerPage, int pageNumber)
        {
            return null;
        }

        #region DataSet,DataTable

        public static DataSet RetrieveTableSchema(string selectStmt,string tableName)
        {
            //refactor codes later
            SqlConnection sqlConn = null;
            DataSet ds = new DataSet();
            try
            {
                sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings[Constants.SqlConnStrKey].ConnectionString);

                SqlCommand schemaCmd = new SqlCommand(selectStmt,sqlConn);
                SqlDataAdapter sda = new SqlDataAdapter(schemaCmd);
                sda.FillSchema(ds,SchemaType.Source,tableName);

            }
            catch (SqlException sqle)
            {
                //string msg = string.Format(Constants.SqlExceptionLogPattern, sqle.Message, sqle.StackTrace);
                //LogInfor.WriteError(Constants.DBExceptionTitle,msg , string.Empty);
                ReturnResult ret = DataAccessProxyConstantRepo.ConvertSqlExceptionToReturnRet(sqle);
                throw new FaultException<ReturnResult>(ret, ret.Message);
            }
            finally
            {
                if (sqlConn != null)
                {
                    sqlConn.Close();
                    sqlConn.Dispose();
                }
            }

            return ds;

        }
        public static void SaveCompletedBookings(DataTable completedBookingDT, string tableName)
        {
            //refactor codes later
            SqlConnection sqlConn = null;
            SqlBulkCopy copy = null;
            try
            {
                sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings[Constants.SqlConnStrKey].ConnectionString);
                sqlConn.Open();
                copy = new SqlBulkCopy(sqlConn);
                copy.BatchSize = 100;
                copy.DestinationTableName = tableName;
           
                copy.WriteToServer(completedBookingDT);
            }
            catch (SqlException sqle)
            {
                //string msg = string.Format(Constants.SqlExceptionLogPattern, sqle.Message, sqle.StackTrace);
                //LogInfor.WriteError(Constants.DBExceptionTitle,msg , string.Empty);
                ReturnResult ret = DataAccessProxyConstantRepo.ConvertSqlExceptionToReturnRet(sqle);
                throw new FaultException<ReturnResult>(ret, ret.Message);
            }
            finally
            {
                if (sqlConn != null)
                {
                    sqlConn.Close();
                    sqlConn.Dispose();
                }
            }
        }
        #endregion
    }
}
