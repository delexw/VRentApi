using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.DAL;
using CF.VRent.DataAccessProxy.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CF.VRent.DataAccessProxy
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AccountingService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select AccountingService.svc or AccountingService.svc.cs at the Solution Explorer and start debugging.
    public class AccountingService : IAccountingService
    {
        #region Debit-Note

        public DebitNoteDetailSearchConditions RetrieveDebitNoteDetailsInRange(DebitNoteDetailSearchConditions conditions, ProxyUserSetting userInfo)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<DebitNoteDetailSearchConditions>
            (
                () => DebitNoteDAL.RetrieveDebitNoteDetailsInRange(conditions, userInfo)
            );
        }


        public void GeneateDebitNotes(DebitNotePeriod dnp, ProxyUserSetting operatorInfo)
        {
            DataAccessProxyConstantRepo.DataAccessExceptionGuard
            (
                () => DebitNoteDAL.GeneateDebitNotes(dnp,operatorInfo)
            ); 
        }

        public DebitNotesSearchConditions RetrieveDebitNotesWithPaging(DebitNotesSearchConditions conditions, ProxyUserSetting operatorInfo)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard
            (
                () => DebitNoteDAL.RetrieveDebitNotesWithPaging(conditions, operatorInfo)
            );
        }

        public DebitNote RetrieveDebitNotesByID(int debitNoteID, DateTime queryTime, ProxyUserSetting operatorInfo)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<DebitNote>
            (
                () => DebitNoteDAL.RetrieveDebitNotesByID(debitNoteID, queryTime, operatorInfo)
            );
        }


        public DebitNote UpdateDebitNotesByID(DebitNote note, ProxyUserSetting operatorInfo)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<DebitNote>
            (
                () => DebitNoteDAL.UpdateDebitNote(note,operatorInfo)
            );
        }

        public DebitNotePeriod[] RetrievePeriods(ProxyUserSetting operatorInfo)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<DebitNotePeriod[]>
            (
                () => DebitNoteDAL.RetrieveDebitNotePeriods(operatorInfo)
            );
        }

        public DebitNotePeriod[] RetrievePeriodsByState(SyncedRecordState state,int debitMonth, ProxyUserSetting operatorInfo)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<DebitNotePeriod[]>
            (
                () => DebitNoteDAL.RetrievePeriodsByState(state,debitMonth, operatorInfo)
            );
        }

        #endregion

        #region DUB

        public DUBDetailSearchConditions RetrieveDUBDetailsByConditions(DUBDetailSearchConditions conditions, ProxyUserSetting userInfo)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard
            (
                () => DUBClosingDAL.RetrieveDUBDetailsByConditions(conditions, userInfo)
            );

        }
        #endregion

        public void SaveIntoStagingAre(StagedBookings completedBookings, ProxyUserSetting userInfo)
        {
            DataAccessProxyConstantRepo.DataAccessExceptionGuard
            (
                () => CompletedBookingDAL.SaveStagedBookings(completedBookings, userInfo)
            );
        }


        public DebitNotePeriod[] RetrieveCompletedPeriods(ProxyUserSetting operatorInfo)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<DebitNotePeriod[]>
            (
                () => DebitNoteDAL.RetrieveCompletedPeriods(operatorInfo)
            );
        }


        public BookingCompact[] RetrieveID(BookingCompact[] input, ProxyUserSetting operatorInfo)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<BookingCompact[]>
            (
                () => DebitNoteDAL.RetrieveID(input,operatorInfo)
            );
        }


        public PricingItemMonthlysummary[] RetrieveDebitNoteMonthlySummary(DebitNote[] notes, ProxyUserSetting userInfo)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<PricingItemMonthlysummary[]>
            (
                () => DebitNoteExcelDAL.RetrievePricingCatalog(notes, userInfo)
            );
        }


        public void ClearUpTempDataByPeriod(DebitNotePeriod dnp, ProxyUserSetting operatorInfo)
        {
            DataAccessProxyConstantRepo.DataAccessExceptionGuard
            (
                () => DebitNoteDAL.ClearUpTempDataByPeriod(dnp, operatorInfo)
            );
        }
    }
}
