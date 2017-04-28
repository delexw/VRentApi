using CF.VRent.Common.Entities.DBEntity;
using CF.VRent.Contract;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.Log;
using CF.VRent.Log.ConcreteLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Common;
using CF.VRent.SAPSDK;
using CF.VRent.Common.Entities;
using System.Threading.Tasks;
using CF.VRent.Common.UserContracts;
using System.ServiceModel;
using CF.VRent.BLL.BLLFactory;
using CF.VRent.Entities;
using CF.VRent.SAPSDK.Interfaces;
using CF.VRent.BLL.BLLFactory.GeneralLedger;

namespace CF.VRent.BLL
{
    public class GeneralLedgerBLL : AbstractBLL, IGeneralLedgerBLL
    {
        public GeneralLedgerBLL(ProxyUserSetting userInfo) : base(userInfo) { }

        /// <summary>
        /// Get the statistics of dub from DB and convert records to the format identified by sap
        /// </summary>
        /// <param name="from"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<GeneralLedgerLine> GenerateDUBLedger(long headerId, DateTime from, DateTime end)
        {
            var fundList = ServiceImpInstanceFactory.CreateGenerateLedgerInstance<GenerateLedgerDUBForFund>(this.UserInfo).Generate(headerId, from, end);

            var reFundList = ServiceImpInstanceFactory.CreateGenerateLedgerInstance<GenerateLedgerDUBForRefund>(this.UserInfo).Generate(headerId, from, end);

            return fundList.Union(reFundList).ToList();
        }

        /// <summary>
        /// Get the statistics of debit note from DB and convert records to the format identified by sap
        /// </summary>
        /// <param name="from"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<GeneralLedgerLine> GenerateCCBLedger(long headerId, DateTime from, DateTime end)
        {
            var fundList =  ServiceImpInstanceFactory.CreateGenerateLedgerInstance<GenerateLedgerCCBForFund>(this.UserInfo).Generate(headerId, from, end);
            var reFundList = ServiceImpInstanceFactory.CreateGenerateLedgerInstance<GenerateLedgerCCBForRefund>(this.UserInfo).Generate(headerId, from, end);

            return fundList.Union(reFundList).ToList();
        }

        /// <summary>
        /// Add header
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private long _addGLHeader(GeneralLedgerHeader header)
        {
            var dataManager = new DataAccessProxyManager();

            return dataManager.AddGeneralLedgerHeader(header);
        }

        

        /// <summary>
        /// Add header
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public long AddGeneralLedgerHeader(GeneralLedgerHeader header)
        {
            return _addGLHeader(header);
        }
    }
}
