using CF.VRent.Common.Entities.DBEntity.Operator;
using CF.VRent.DataAccessProxy.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using CF.VRent.Common;
using CF.VRent.Common.Entities.DBEntity;
using CF.VRent.Common.Entities.DBEntity.Aggregation;
using System.Web;

namespace CF.VRent.DAL
{
    public class TermsConditionDAL : DBProxyOperator<TermsCondition>
    {      
        public override long Add(TermsCondition entity)
        {
            var output = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue };
            SqlParameter[] paras = new SqlParameter[]
                {
                   new SqlParameter("@Title", entity.Title.ToStr()),                
                   new SqlParameter("@DisplayTitle",entity.DisplayTitle.ToStr()),
                   new SqlParameter("@Content",HttpUtility.HtmlDecode(entity.Content.ToStr())),
                   new SqlParameter("@Key", entity.Key),
                   new SqlParameter("@Type", entity.Type),
                   new SqlParameter("@Category", entity.Category),
                   new SqlParameter("@Version", entity.Version.ToStr()),
                   new SqlParameter("@IsNewVersion", entity.IsNewVersion),
                   new SqlParameter("@IsActive", entity.IsActive),
                   new SqlParameter("@CreatedBy", entity.CreatedBy),
                   output
                };

            //TODO:no sp name
            int res = SQLHelper.ExecuteNonQuery(null, "Sp_TermsCondition_Create", CommandType.StoredProcedure, paras);

            if (output.Value != null)
            {
                return output.Value.ToInt();
            }

            return 0;
        }

        /// <summary>
        /// Get tc
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public override DBEntityAggregation<TermsCondition, DBConditionObject> Get(IDBConditionAggregationRoot condition)
        {
            List<TermsCondition> tc = new List<TermsCondition>();

            SqlParameter[] paras = new SqlParameter[]
                {
                    new SqlParameter("@Type", condition.RawWhereConditions["type"]),
                    new SqlParameter("@isIncludeContent",condition.RawWhereConditions["isIncludeContent"])
                };

            DataTable dt = SQLHelper.ExecuteDataTable(null, "Sp_TermsCondition_Latest_Get", CommandType.StoredProcedure, paras);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow r in dt.Rows)
                {
                    TermsCondition newTc = new TermsCondition();

                    foreach (DataColumn c in dt.Columns)
                    {
                        var rowValue = r[c.ColumnName];
                        if (r[c.ColumnName] == DBNull.Value)
                        {
                            rowValue = null;
                        }
                        if (c.ColumnName == "Content")
                        {
                            rowValue = HttpUtility.HtmlEncode(rowValue.ToStr());
                        }
                        newTc.EntityType.GetProperty(c.ColumnName).SetValue(newTc, rowValue, null);
                    }

                    tc.Add(newTc);
                }
                var ret =
                    new DBEntityAggregation<TermsCondition, DBConditionObject>();

                ret.SetEntities(tc);

                return ret;
            }

            return null;
        }

        
        public override void Dispose()
        {
            
        }
    }
}
