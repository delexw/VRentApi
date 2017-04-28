using CF.VRent.Common.Entities;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using CF.VRent.Common;
using CF.VRent.Common.UserContracts;
using CF.VRent.Log.ConcreteLog;
using CF.VRent.UPSDK;

namespace CF.VRent.BLL.BLLFactory.GeneralLedger
{
    public class CatalogWithClient : ICatalogWithClient<GeneralLedgerStatisticDUB, string>
    {

        private ProxyUserSetting _loginUser;

        public CatalogWithClient(ProxyUserSetting userInfo)
        {
            _loginUser = userInfo;
        }

        public IEnumerable<IGrouping<string, GeneralLedgerStatisticDUB>> Catalog(IEnumerable<GeneralLedgerStatisticDUB> source)
        {
            var kemasApi = KemasAccessWrapper.CreateKemasUserAPI2Instance();
            var glLogger = LogInfor.GetLogger<GeneralLedgerLogger>();

            var InforMessageFieldNULL = MessageCode.CVB000070.GetDescription() + "No payment";
            var InforMessageFieldZero = MessageCode.CVB000070.GetDescription() + "Credit price is 0";
            var InforMessageFieldInvalid = MessageCode.CVB000070.GetDescription() + "Failed payment status";
            var ErrorMessage = MessageCode.CVB000070.GetDescription() + "CatalogWithClient error";

            Parallel.ForEach<GeneralLedgerStatisticDUB>(source, r =>
            {
                try
                {
                    //ignore without payment
                    if (r.RentPaymentID == 0)
                    {
                        glLogger.WriteInfo(InforMessageFieldNULL, String.Format("BookingId:{0}, PaymentId:{1}", r.ID, r.RentPaymentID), _loginUser.ID);
                    }
                    else if (!r.RentalTime.HasValue)
                    {
                        //Igonre it as it is out of the specified time range
                    }
                    //ignore without costs
                    else if (r.RentCreditPrice == 0)
                    {
                        glLogger.WriteInfo(InforMessageFieldZero, String.Format("BookingId:{0}, PaymentId:{1}", r.ID, r.RentPaymentID), _loginUser.ID);
                    }
                    //ignore with failed payment
                    else if (!r.RentalPaymentStatus.IsSuccessStatus())
                    {
                        glLogger.WriteInfo(InforMessageFieldInvalid, String.Format("BookingId:{0}, PaymentId:{1}, PaymentStatus:{2}", r.ID, r.RentPaymentID, r.RentalPaymentStatus.ToStr()), _loginUser.ID);
                    }
                    else
                    {
                        var user = kemasApi.findUser2(r.UserID.ToString(), _loginUser.SessionID);
                        r.ClientID = user.UserData.Clients[0].ID.ToGuidNull();
                        r.CompanyCode = "4073";
                        r.BusinessArea = "00";
                    }
                }
                catch (WebFaultException<ReturnResult> ex)
                {
                    LogInfor.WriteError(ErrorMessage, ex.Detail.ObjectToJson(), _loginUser.ID);
                }
                catch (Exception ex)
                {
                    LogInfor.WriteError(ErrorMessage, ex.ToStr(), _loginUser.ID);
                }
            });

            return source.GroupBy(r => r.CompanyCode);
        }
    }
}
