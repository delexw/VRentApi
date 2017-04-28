using CF.VRent.Common;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.AccountingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CF.VRent.Entities.DataAccessProxyWrapper
{
    public partial class DataAccessProxyManager: IAccountingService
    {
        #region Debit-Note
        public void GeneateDebitNotes(DebitNotePeriod dnp, ProxyUserSetting operatorInfo)
        {
            AccountingServiceClient accounting = new AccountingServiceClient();
            InnerTryCatchInvoker
                (
                    () => accounting.GeneateDebitNotes(dnp, operatorInfo),
                    accounting,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public DebitNotesSearchConditions RetrieveDebitNotesWithPaging(DebitNotesSearchConditions conditions, ProxyUserSetting operatorInfo)
        {
            AccountingServiceClient accounting = new AccountingServiceClient();
            return InnerTryCatchInvoker
                (
                    () => accounting.RetrieveDebitNotesWithPaging(conditions, operatorInfo),
                    accounting,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public DebitNote RetrieveDebitNotesByID(int debitNoteID, DateTime queryTime, ProxyUserSetting operatorInfo)
        {
            AccountingServiceClient accounting = new AccountingServiceClient();
            return InnerTryCatchInvoker
                (
                    () => accounting.RetrieveDebitNotesByID(debitNoteID,queryTime, operatorInfo),
                    accounting,
                    MethodInfo.GetCurrentMethod().Name
                );
        }


        public DebitNote UpdateDebitNotesByID(DebitNote note, ProxyUserSetting operatorInfo)
        {
            AccountingServiceClient accounting = new AccountingServiceClient();
            return InnerTryCatchInvoker
                (
                    () => accounting.UpdateDebitNotesByID(note,operatorInfo),
                    accounting,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public DebitNotePeriod[] RetrievePeriods(ProxyUserSetting operatorInfo)
        {
            AccountingServiceClient accounting = new AccountingServiceClient();
            return InnerTryCatchInvoker
                (
                    () => accounting.RetrievePeriods(operatorInfo),
                    accounting,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        #endregion

        #region DUB
        public DUBDetailSearchConditions RetrieveDUBDetailsByConditions(DUBDetailSearchConditions conditions, ProxyUserSetting userInfo)
        {
            AccountingServiceClient accounting = new AccountingServiceClient();
            return InnerTryCatchInvoker
                (
                    () => accounting.RetrieveDUBDetailsByConditions(conditions, userInfo),
                    accounting,
                    MethodInfo.GetCurrentMethod().Name
                );
        }
        #endregion

        public void SaveIntoStagingAre(StagedBookings conditions, ProxyUserSetting userInfo)
        {
            AccountingServiceClient accounting = new AccountingServiceClient();
            InnerTryCatchInvoker
                (
                    () => accounting.SaveIntoStagingAre(conditions, userInfo),
                    accounting,
                    MethodInfo.GetCurrentMethod().Name
                );
        }





        public DebitNotePeriod[] RetrieveCompletedPeriods(ProxyUserSetting operatorInfo)
        {
            AccountingServiceClient accounting = new AccountingServiceClient();
            return InnerTryCatchInvoker
                (
                    () => accounting.RetrieveCompletedPeriods(operatorInfo),
                    accounting,
                    MethodInfo.GetCurrentMethod().Name
                );
        }


        public DebitNotePeriod[] RetrievePeriodsByState(SyncedRecordState state, int debitMonth,ProxyUserSetting operatorInfo)
        {
            AccountingServiceClient accounting = new AccountingServiceClient();
            return InnerTryCatchInvoker
                (
                    () => accounting.RetrievePeriodsByState(state,debitMonth, operatorInfo),
                    accounting,
                    MethodInfo.GetCurrentMethod().Name
                );
        }


        public DebitNoteDetailSearchConditions RetrieveDebitNoteDetailsInRange(DebitNoteDetailSearchConditions conditions, ProxyUserSetting userInfo)
        {
            AccountingServiceClient accounting = new AccountingServiceClient();
            return InnerTryCatchInvoker
                (
                    () => accounting.RetrieveDebitNoteDetailsInRange(conditions, userInfo),
                    accounting,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public BookingCompact[] RetrieveID(BookingCompact[] input, ProxyUserSetting operatorInfo)
        {
            AccountingServiceClient accounting = new AccountingServiceClient();
            return InnerTryCatchInvoker
                (
                    () => accounting.RetrieveID(input, operatorInfo),
                    accounting,
                    MethodInfo.GetCurrentMethod().Name
                );
        }


        public PricingItemMonthlysummary[] RetrieveDebitNoteMonthlySummary(DebitNote[] notes, ProxyUserSetting userInfo)
        {
            AccountingServiceClient accounting = new AccountingServiceClient();
            return InnerTryCatchInvoker
                (
                    () => accounting.RetrieveDebitNoteMonthlySummary(notes, userInfo),
                    accounting,
                    MethodInfo.GetCurrentMethod().Name
                );
        }


        public void ClearUpTempDataByPeriod(DebitNotePeriod dnp, ProxyUserSetting operatorInfo)
        {
            AccountingServiceClient accounting = new AccountingServiceClient();
            InnerTryCatchInvoker
                (
                    () => accounting.ClearUpTempDataByPeriod(dnp, operatorInfo),
                    accounting,
                    MethodInfo.GetCurrentMethod().Name
                );
        }
    }
}
