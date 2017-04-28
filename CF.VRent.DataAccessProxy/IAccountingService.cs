using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.DataAccessProxy.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CF.VRent.DataAccessProxy
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAccountingService" in both code and config file together.
    [ServiceContract]
    public interface IAccountingService
    {
        #region Debit-Note operations

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        BookingCompact[] RetrieveID(BookingCompact[] input, ProxyUserSetting operatorInfo);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        DebitNotePeriod[] RetrievePeriods(ProxyUserSetting operatorInfo);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        DebitNotePeriod[] RetrievePeriodsByState(SyncedRecordState state,int debitMonth, ProxyUserSetting operatorInfo);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        DebitNotePeriod[] RetrieveCompletedPeriods(ProxyUserSetting operatorInfo);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        void ClearUpTempDataByPeriod(DebitNotePeriod dnp, ProxyUserSetting operatorInfo);

        
        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        void GeneateDebitNotes(DebitNotePeriod dnp, ProxyUserSetting operatorInfo);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        DebitNotesSearchConditions RetrieveDebitNotesWithPaging(DebitNotesSearchConditions conditions, ProxyUserSetting operatorInfo);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        DebitNoteDetailSearchConditions RetrieveDebitNoteDetailsInRange(DebitNoteDetailSearchConditions conditions, ProxyUserSetting userInfo);


        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        DebitNote RetrieveDebitNotesByID(int debitNoteID, DateTime queryTime, ProxyUserSetting operatorInfo);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        DebitNote UpdateDebitNotesByID(DebitNote note, ProxyUserSetting operatorInfo);

        #endregion

        #region Debit=Note Job
        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        void SaveIntoStagingAre(StagedBookings conditions, ProxyUserSetting userInfo);

        #endregion

        #region DUB
        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        DUBDetailSearchConditions RetrieveDUBDetailsByConditions(DUBDetailSearchConditions conditions, ProxyUserSetting userInfo);


        #endregion

        #region DebitNoteMonthlySummary
        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        PricingItemMonthlysummary[] RetrieveDebitNoteMonthlySummary(DebitNote[] notes, ProxyUserSetting userInfo);

        #endregion

    }
}
