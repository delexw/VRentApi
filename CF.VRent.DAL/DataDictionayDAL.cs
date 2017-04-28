using CF.VRent.Common.Entities.DBEntity;
using CF.VRent.Common.Entities.DBEntity.Aggregation;
using CF.VRent.Common.Entities.DBEntity.Operator;
using CF.VRent.DataAccessProxy.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CF.VRent.DAL
{
    public class DataDictionayDAL : DBProxyOperator<Country>
    {
        public override void Dispose()
        {
            
        }

        public override DBEntityAggregation<Country, DBConditionObject> Get(IDBConditionAggregationRoot condition)
        {
            List<Country> country = new List<Country>();

            DataTable dt = SQLHelper.ExecuteDataTable(null, "Sp_VrentCountry_Get", CommandType.StoredProcedure);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow r in dt.Rows)
                {
                    Country newCountry = new Country();

                    foreach (DataColumn c in dt.Columns)
                    {
                        var rowValue = r[c.ColumnName];
                        if (r[c.ColumnName] == DBNull.Value)
                        {
                            rowValue = null;
                        }
                        newCountry.EntityType.GetProperty(c.ColumnName).SetValue(newCountry, rowValue, null);
                    }

                    country.Add(newCountry);
                }
                var ret =
                    new DBEntityAggregation<Country, DBConditionObject>();

                ret.SetEntities(country);

                return ret;
            }

            return null;
        }
    }
}
