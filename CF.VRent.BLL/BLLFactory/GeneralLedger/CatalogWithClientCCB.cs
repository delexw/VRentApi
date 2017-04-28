using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Log;
using CF.VRent.Log.ConcreteLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CF.VRent.Common;
using CF.VRent.Common.Entities;

namespace CF.VRent.BLL.BLLFactory.GeneralLedger
{
    public class CatalogWithClientCCB : ICatalogWithClient<GeneralLedgerStatisticCCB, string>
    {
        private ProxyUserSetting _loginUser;

        public CatalogWithClientCCB(ProxyUserSetting userInfo)
        {
            _loginUser = userInfo;
        }

        public IEnumerable<IGrouping<string, GeneralLedgerStatisticCCB>> Catalog(IEnumerable<GeneralLedgerStatisticCCB> source)
        {
            var kemasApi = KemasAccessWrapper.CreateKemasUserAPI2Instance();
            var glLogger = LogInfor.GetLogger<GeneralLedgerLogger>();

            var InforMessageFieldNULL = MessageCode.CVB000071.GetDescription() + "No clientId";
            var InforMessageFieldZero = MessageCode.CVB000071.GetDescription() + "Credit price is 0";

            Parallel.ForEach<GeneralLedgerStatisticCCB>(source, r =>
            {
                //ignore without payment
                if (!r.ClientID.HasValue)
                {
                    glLogger.WriteInfo(InforMessageFieldNULL, r.ObjectToJson(), _loginUser.ID);
                }
                //ignore without costs
                else if (r.CCBTotalCredit == 0)
                {
                    glLogger.WriteInfo(InforMessageFieldZero, String.Format("ClientId:{0}", r.ClientID), _loginUser.ID);
                }
                else
                {
                    r.CompanyCode = "4175";
                    r.BusinessArea = "04";
                }
            });

            return source.GroupBy(r => r.CompanyCode);
        }
    }
}
