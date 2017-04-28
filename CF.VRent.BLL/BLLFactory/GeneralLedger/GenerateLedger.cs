using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.SAPSDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Common;
using CF.VRent.Entities.DataAccessProxyWrapper;

namespace CF.VRent.BLL.BLLFactory.GeneralLedger
{
    public abstract class GenerateLedger : IGenerateLedger
    {
        protected ProxyUserSetting _sessionUser;

        public GenerateLedger(ProxyUserSetting loginUser)
        {
            _sessionUser = loginUser;
        }

        public abstract List<Entities.GeneralLedgerLine> Generate(long headerId, DateTime from, DateTime end);

        /// <summary>
        /// Deal with dub items
        /// </summary>
        /// <param name="items"></param>
        /// <param name="lines"></param>
        /// <param name="sap"></param>
        /// <param name="dubStatistic"></param>
        /// <param name="headerID"></param>
        protected void _dubItemSets(List<long> items,
            List<GeneralLedgerLine> lines,
            ISAPManager sap,
            GeneralLedgerStatisticDUB dubStatistic,
            long headerID,
            VRentDataDictionay.GLItemType type)
        {
            //add item with amount of money paid to union pay
            items.Add(
                this._addGLItem(new GeneralLedgerItem()
                {
                    HeaderID = headerID,
                    ItemType = type,
                    CreatedBy = _sessionUser.ID.ToGuidNull(),
                    PostingBody = sap.Item.FormatEntity,
                    ClientID = dubStatistic.ClientID.ToGuid(),
                    CompanyCode = dubStatistic.CompanyCode,
                    BusinessArea = dubStatistic.BusinessArea
                })
            );
            lines.Add(new GeneralLedgerLine()
            {
                CompanyCode = dubStatistic.CompanyCode,
                BusinessArea = dubStatistic.BusinessArea,
                PostingBody = sap.Item.FormatEntity
            });
        }

        /// <summary>
        /// Deal with ccb items
        /// </summary>
        /// <param name="items"></param>
        /// <param name="lines"></param>
        /// <param name="sap"></param>
        /// <param name="ccbStatistic"></param>
        /// <param name="headerID"></param>
        /// <param name="type"></param>
        protected void _ccbItemSets(List<long> items,
            List<GeneralLedgerLine> lines,
            ISAPManager sap,
            GeneralLedgerStatisticCCB ccbStatistic,
            long headerID,
            VRentDataDictionay.GLItemType type)
        {
            items.Add(
                this._addGLItem(new GeneralLedgerItem()
                {
                    HeaderID = headerID,
                    ItemType = VRentDataDictionay.GLItemType.Debit,
                    CreatedBy = _sessionUser.ID.ToGuidNull(),
                    PostingBody = sap.Item.FormatEntity,
                    ClientID = ccbStatistic.ClientID.ToGuid(),
                    CompanyCode = ccbStatistic.CompanyCode,
                    BusinessArea = ccbStatistic.BusinessArea
                })
            );

            //debit line
            lines.Add(new GeneralLedgerLine()
            {
                CompanyCode = ccbStatistic.CompanyCode,
                BusinessArea = ccbStatistic.BusinessArea,
                PostingBody = sap.Item.FormatEntity
            });
        }


        /// <summary>
        /// Add header
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        protected long _addGLHeader(GeneralLedgerHeader header)
        {
            var dataManager = new DataAccessProxyManager();

            return dataManager.AddGeneralLedgerHeader(header);
        }

        /// <summary>
        /// Add item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected long _addGLItem(GeneralLedgerItem item)
        {
            var dataManager = new DataAccessProxyManager();

            return dataManager.AddGeneralLedgerItem(item);
        }

        /// <summary>
        /// Add details
        /// </summary>
        /// <param name="detail"></param>
        protected void _addGLItemDetails(GeneralLedgerItemDetail detail)
        {
            var dataManager = new DataAccessProxyManager();

            dataManager.AddGeneralLedgerItemDetails(detail);
        }
    }
}
