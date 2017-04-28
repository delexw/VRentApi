using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities;
using CF.VRent.Log;
using CF.VRent.Log.ConcreteLog;
using CF.VRent.SAPSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using CF.VRent.Common;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.Common.Entities.DBEntity;
using System.Threading.Tasks;
using CF.VRent.Entities.DataAccessProxy;

namespace CF.VRent.BLL.BLLFactory.GeneralLedger
{
    public class GenerateLedgerCCBForFund : GenerateLedger
    {
        public GenerateLedgerCCBForFund(ProxyUserSetting loginUser) : base(loginUser) { }

        public override List<GeneralLedgerLine> Generate(long headerId, DateTime from, DateTime end)
        {
            var ErrorMessageDAE = MessageCode.CVB000071.GetDescription() + "DataAccess error";
            var ErrorMessageUEE = MessageCode.CVB000071.GetDescription() + "Unexpected error";
            var InfoMessageFieldNULL = MessageCode.CVB000071.GetDescription() + "ClientId or CompanyCode is null";
            var InfoMessageObjNull = MessageCode.CVB000071.GetDescription() + "No ledger data";

            try
            {
                var dataManager = new DataAccessProxyManager();

                var condi = new DBConditionObject();
                condi.RawWhereConditions.Add("DateFrom", from.ToString());
                condi.RawWhereConditions.Add("DateEnd", end.ToString());

                var statisticData = dataManager.GetCCBStatistic(condi);

                List<GeneralLedgerLine> generalLine = new List<GeneralLedgerLine>();

                if (statisticData.Entities.Count() > 0)
                {
                    var postData = DateTime.Now.Date;

                    var groupedData = ServiceImpInstanceFactory.CreateCCBGeneralLedgerCatalogInstance(this._sessionUser).Catalog(statisticData.Entities);

                    Parallel.ForEach(groupedData, r =>
                    {
                        //Has client info, include id and company code
                        if (!String.IsNullOrWhiteSpace(r.Key))
                        {
                            var groupedByArea = r.GroupBy(i => i.BusinessArea);
                            Parallel.ForEach(groupedByArea, a =>
                            {
                                #region One client
                                var sapSDK = SAPContext.CreateManager();
                                sapSDK.Item[SAPSDKConstants.BaselineDate] = postData.ToString(sapSDK.Common[SAPSDKConstants.DateFormat]);
                                sapSDK.Item[SAPSDKConstants.Reference1] = sapSDK.Item[SAPSDKConstants.BaselineDate];

                                var items = new List<long>();

                                #region Add item with amount of money paid to union pay
                                sapSDK.Item[SAPSDKConstants.DebitOrCredit] = "D";
                                sapSDK.Item[SAPSDKConstants.Amount] = a.Sum(i => i.CCBTotalDebit).ToString();
                                this._ccbItemSets(items, generalLine, sapSDK, a.FirstOrDefault(), headerId, VRentDataDictionay.GLItemType.Debit);
                                #endregion

                                #region Add item with tax
                                //Add item with tax
                                sapSDK.Item[SAPSDKConstants.DebitOrCredit] = "C";
                                sapSDK.Item[SAPSDKConstants.Amount] = (a.Sum(i => i.CCBTotalDebit) * sapSDK.Common[SAPSDKConstants.Tax].ToDouble()).ToStr();
                                this._ccbItemSets(items, generalLine, sapSDK, a.FirstOrDefault(), headerId, VRentDataDictionay.GLItemType.Credit);
                                #endregion

                                #region Add item with profits
                                sapSDK.Item[SAPSDKConstants.DebitOrCredit] = "C";
                                sapSDK.Item[SAPSDKConstants.Amount] = (a.Sum(i => i.CCBTotalDebit) * (1 - sapSDK.Common[SAPSDKConstants.Tax].ToDouble())).ToStr();
                                this._ccbItemSets(items, generalLine, sapSDK, a.FirstOrDefault(), headerId, VRentDataDictionay.GLItemType.Credit);
                                #endregion

                                //Add details
                                Parallel.ForEach<GeneralLedgerStatisticCCB>(a, s =>
                                {
                                    Parallel.ForEach<long>(items, i =>
                                    {
                                        try
                                        {
                                            this._addGLItemDetails(new GeneralLedgerItemDetail()
                                            {
                                                DebitNoteID = s.DebitNoteID,
                                                HeaderID = headerId,
                                                ItemID = i,
                                                CreatedBy = this._sessionUser.ID.ToGuidNull(),
                                                DetailType = VRentDataDictionay.GLItemDetailType.DebitNote
                                            });
                                        }
                                        catch (Exception ex)
                                        {
                                            LogInfor.WriteError(ErrorMessageDAE, ex.ToStr(), this._sessionUser.ID);
                                        }
                                    });
                                });
                                #endregion
                            });
                        }
                        else
                        {
                            LogInfor.GetLogger<GeneralLedgerLogger>().WriteInfo(InfoMessageFieldNULL,
                                                   r.ObjectToJson(), this._sessionUser.ID);
                        }
                    });
                }
                else
                {
                    LogInfor.GetLogger<GeneralLedgerLogger>().WriteInfo(InfoMessageObjNull,
                                           String.Format("Time:{0}-{1}", from.ToString(), end.ToString()), this._sessionUser.ID);
                }

                return generalLine;
            }
            catch (FaultException<ReturnResult> ex)
            {
                LogInfor.WriteError(ErrorMessageDAE, ex.Detail.ObjectToJson(), this._sessionUser.ID);
            }
            catch (Exception ex)
            {
                LogInfor.WriteError(ErrorMessageUEE, ex.ToStr(), this._sessionUser.ID);
            }

            return null;
        }
    }
}
