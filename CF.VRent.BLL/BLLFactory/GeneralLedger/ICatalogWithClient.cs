using CF.VRent.Entities.DataAccessProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.GeneralLedger
{
    public interface ICatalogWithClient<T,V>
    {
        /// <summary>
        /// Get the clientId from kemas, attached it to datasource and return the sets grouped by clientId
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        IEnumerable<IGrouping<V, T>> Catalog(IEnumerable<T> source);
    }
}
