using CF.VRent.BLL;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities;
using CF.VRent.Entities.AccountingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Web;

namespace Proxy
{
    public partial class DataService
    {
        #region Debit-note
        [WebGet(UriTemplate = "DebitNoteDetails?DebitNoteID={debitNoteID}&bookingNumber={bookingNumber}&userID={userID}&userName={userName}&beginDate={beginDate}&endDate={endDate}&itemsPerPage={itemsPerPage}&pageNumber={pageNumber}", ResponseFormat = WebMessageFormat.Json)]
        public DebitNoteDetailSearchConditions RetrieveDebitNotesDetailsByID(string debitNoteID, string bookingNumber,string userID,string userName,string beginDate,string endDate, string itemsPerPage, string pageNumber)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();

            DebitNoteDetailSearchConditions conditions = DUBUtility.GenerateDebitNoteDetailSearchConditions(debitNoteID, bookingNumber, userID,userName, beginDate, endDate, itemsPerPage, pageNumber);

            IDebitNote dnb = new DebitNoteBLL(setting);
            return dnb.RetrieveDebitNoteDetailsByID(conditions);
        }

        [WebGet(UriTemplate = "DebitNotePeriods", ResponseFormat = WebMessageFormat.Json)]
        public DebitNotePeriod[] RetrieveDebitNotesPeriods()
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();

            DebitNoteBLL dnb = new DebitNoteBLL(setting);
            return dnb.RetrieveDebitNotePeriods();
        }


        [WebGet(UriTemplate = "DebitNotes/{debitNoteID}", ResponseFormat = WebMessageFormat.Json)]
        public DebitNote RetrieveDebitNotesByID(string debitNoteID)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();

            DebitNoteBLL dnb = new DebitNoteBLL(setting);
            DebitNote output = dnb.RetrieveDebitNotesByID(debitNoteID);

            return output;
        }

        [WebGet(UriTemplate = "DebitNotes?clientID={clientID}&state={state}&beginDate={beginDate}&endDate={endDate}&itemsPerPage={itemsPerPage}&pageNumber={pageNumber}", ResponseFormat = WebMessageFormat.Json)]
        public DebitNotesSearchConditions RetrieveDebitNotesWithPaging(string clientID, string state, string beginDate, string endDate, string itemsPerPage, string pageNumber)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();

            DebitNotesSearchConditions input = DebitNoteUtility.GenerateDebitNotesSearchConditions(clientID, state, beginDate, endDate, itemsPerPage, pageNumber);

            DebitNoteBLL dnb = new DebitNoteBLL(setting);

            return dnb.RetrieveDebitNotes(input);
        }

        [WebInvoke(UriTemplate = "DebitNotes/{debitNoteID}", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public DebitNote UpdateDebitNoteByID(string debitNoteID,DebitNotePaymentState state)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();

            DebitNoteBLL dnb = new DebitNoteBLL(setting);
            return dnb.UpdateDebitNotesByID(debitNoteID,state);
        }
        #endregion

        #region DUB
        [WebGet(UriTemplate = "DUBDetails?bookingNumber={bookingNumber}&userID={userID}&userName={userName}&beginDate={beginDate}&endDate={endDate}&status={status}&itemsPerPage={itemsPerPage}&pageNumber={pageNumber}", ResponseFormat = WebMessageFormat.Json)]
        public DUBDetailSearchConditions RetrieveDUBDetailsWithPaging(string bookingNumber, string userID, string userName,string beginDate, string endDate, string status, string itemsPerPage, string pageNumber)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();
            DUBDetailSearchConditions input = DUBUtility.GenerateDUBSearchConditions(bookingNumber,userID,userName, beginDate, endDate, status,itemsPerPage, pageNumber);

            DUBBLL dnb = new DUBBLL(setting);
            return dnb.RetrieveDUBDetails(input);
        }
        #endregion
    }
}