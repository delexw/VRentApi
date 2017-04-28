using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using CF.VRent.Entities.AccountingService;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.BLL;
using CF.VRent.Common;
using CF.VRent.Entities;
using CF.VRent.Entities.KEMASWSIF_AUTHRef;
using System.Xml.Linq;

using System.Linq;
using System.Collections.Generic;

namespace ProxyTest
{
    [TestClass]
    public class DebitNoteUnitTest
    {
        #region Debit-Note Dataaccess proxy test
        [TestMethod]
        public void GenerateMonthlyDebitNotesTestMethod()
        {
            ProxyUserSetting userinfo = null;

            DebitNoteBLL dnp = new DebitNoteBLL(userinfo);

            DebitNotePeriod[] periods = dnp.RetrieveDebitNotePeriods();

            if (periods != null && periods.Length > 0)
            {
                IAccountingService ias = new DataAccessProxyManager();
                ias.GeneateDebitNotes(periods[0], userinfo); 
            }
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void RetrieveDebitNotesTestMethod()
        {
            ProxyUserSetting userinfo = null;

            DebitNotesSearchConditions dnsc = DebitNoteUtility.GenerateDebitNotesSearchConditions(null,null,"20150801","20151010","5","1");

            IAccountingService ias = new DataAccessProxyManager();

            DebitNotesSearchConditions output = ias.RetrieveDebitNotesWithPaging(dnsc, userinfo);

            Assert.IsTrue(output.Notes != null);
        }

        [TestMethod]
        public void RetrieveDebitNoteByIDTestMethod()
        {
            ProxyUserSetting userinfo = null;

            int debitNoteID = 1;
            IAccountingService ias = new DataAccessProxyManager();

            DebitNote output = ias.RetrieveDebitNotesByID(debitNoteID, DateTime.Now, userinfo);

            Assert.IsTrue(output != null);
        }

        [TestMethod]
        public void UpdateDebitNoteByIDTestMethod()
        {
            ProxyUserSetting userinfo = new ProxyUserSetting();
            userinfo.ID = Guid.NewGuid().ToString();

            int debitNoteID = 1;
            IAccountingService ias = new DataAccessProxyManager();

            DebitNote note = ias.RetrieveDebitNotesByID(debitNoteID, DateTime.Now, userinfo);

            DebitNoteUtility.SetPaymentDate(note, userinfo);
            DebitNote output = ias.UpdateDebitNotesByID(note, userinfo);

            Assert.IsTrue(output != null);
        }

        [TestMethod]
        public void RetrieveDebitNoteDetailsBLLTestMethod()
        {
            ProxyUserSetting userinfo = null;
            DebitNoteDetailSearchConditions detailSearch = new DebitNoteDetailSearchConditions();
            detailSearch.DebitNoteID = 1;
            detailSearch.DateBegin = new DateTime(2015, 9, 1);
            detailSearch.DateEnd = new DateTime(2015, 10, 1);
            //detailSearch.UserID = Guid.Parse("05A543A4-86A3-4A2C-AEE4-DF5A01A38711");

            detailSearch.UserName = "e";
            detailSearch.KemasBookingNumber = "0";

            detailSearch.ItemsPerPage = 5;
            detailSearch.PageNumber = 1;
            detailSearch.TotalPage = -1;

            IAccountingService ias = new DataAccessProxyManager();
            DebitNoteDetailSearchConditions output = ias.RetrieveDebitNoteDetailsInRange(detailSearch, userinfo);

            string jsonRes = SerializedHelper.JsonSerialize<DebitNoteDetailSearchConditions>(output);

            Assert.IsTrue(output != null);
        }

        #endregion

        #region BLL Test
        [TestMethod]
        public void RetrieveDebitNotePeriodsBLLTestMethod()
        {
            ProxyUserSetting userinfo = null;
            IDebitNote period = new DebitNoteBLL(userinfo);
            DebitNotePeriod[] periods = period.RetrieveDebitNotePeriods();

            try
            {
                string jsonRes = SerializedHelper.JsonSerialize<DebitNotePeriod[]>(periods);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }

            Assert.IsTrue(periods != null);
        }

        [TestMethod]
        public void RetrieveDebitNotesBLLTestMethod()
        {
            ProxyUserSetting userinfo = null;

            DebitNoteBLL dnb = new DebitNoteBLL(userinfo);

            DebitNotesSearchConditions dnsc = DebitNoteUtility.GenerateDebitNotesSearchConditions(null, null, "2015-09-01", "2015-09-30", "5", "1");

            DebitNotesSearchConditions output = dnb.RetrieveDebitNotes(dnsc);

            string jsonRes = SerializedHelper.JsonSerialize<DebitNotesSearchConditions>(output);

            Assert.IsTrue(output.Notes != null);
        }

        [TestMethod]
        public void RetrieveDebitNoteByIDBLLTestMethod()
        {
            ProxyUserSetting userinfo = null;

            int debitNoteID = 1;
            DebitNoteBLL dnb = new DebitNoteBLL(userinfo);

            DebitNote output = dnb.RetrieveDebitNotesByID(debitNoteID.ToString());

            string jsonRes = SerializedHelper.JsonSerialize<DebitNote>(output);

            Assert.IsTrue(output != null);
        }

        [TestMethod]
        public void UpdateDebitNoteByIDBLLTestMethod()
        {
            ProxyUserSetting userinfo = new ProxyUserSetting();
            userinfo.ID = Guid.NewGuid().ToString();

            int debitNoteID = 1;
            DebitNoteBLL dnb = new DebitNoteBLL(userinfo);

            DebitNotePaymentState state = new DebitNotePaymentState();
            state.State = PaymentState.Paid;
            DebitNote output = dnb.UpdateDebitNotesByID(debitNoteID.ToString(), state);

            Assert.IsTrue(output != null);
        }

        #endregion

        #region Sync Test

        [TestMethod]
        public void SyncBookingDataTestMethod()
        {
            WS_Auth_Response auth = KemasAdmin.SignOn();
            IAccountingService ias = new DataAccessProxyManager();

            ProxyUserSetting userInfo = new ProxyUserSetting()
            {
                ID = auth.ID,
                SessionID = auth.SessionID
            };
            DebitNotePeriod[] output = ias.RetrievePeriods(userInfo);

            DebitNotePeriod period = output[0];

            IDebitNoteJob job = new DebitNoteJob();
            job.UserInfo = userInfo;
            job.TargetPeriod = period;

            StagedBookings staged = new StagedBookings();
            staged.BeginDate = period.PeriodStartDate;
            staged.EndDate = period.PeriodEndDate;

            job.LoadBookingsFromKemas(staged);

            if (staged.Items != null && staged.Items.Length > 0)
            {
                job.SavingIntoStagingArea(staged); 
            }


            Assert.IsTrue(output != null);
        }

        #endregion

        #region handling Raw
        [TestMethod]
        public void DebitNoteDetailUnitTest() 
        {
            WS_Auth_Response auth = KemasAdmin.SignOn();
            IAccountingService ias = new DataAccessProxyManager();

            ProxyUserSetting userInfo = new ProxyUserSetting()
            {
                ID = auth.ID,
                SessionID = auth.SessionID
            };

            DebitNoteBLL dnb = new DebitNoteBLL(userInfo);

            DebitNoteDetailSearchConditions dndsc = new DebitNoteDetailSearchConditions();
            dndsc.DebitNoteID = 1;
            dndsc.DateBegin = new DateTime(2015, 8, 1);
            dndsc.DateEnd = new DateTime(2015, 11, 1);
            dndsc.KemasBookingNumber = "0";
            //dndsc.UserID = Guid.Parse("05A543A4-86A3-4A2C-AEE4-DF5A01A38711");
            dndsc.UserName = "a";


            dndsc.ItemsPerPage = 5;
            dndsc.PageNumber = 1;

            DebitNoteDetailSearchConditions output = dnb.RetrieveDebitNoteDetailsByID(dndsc);

            Assert.IsTrue(output != null);
        }
        #endregion



        #region retrieve ids
        [TestMethod]
        public void RetrieveIDsTestMethod()
        {
            ProxyUserSetting userinfo = null;

            BookingCompact bc1 = new BookingCompact();
            bc1.KemasBookingID = Guid.Parse("B59A4D72-5472-4516-A074-9CF3F8EEF9E8");

            BookingCompact bc2 = new BookingCompact();
            bc2.KemasBookingID = Guid.Parse("50233C54-6263-4CB3-BAE9-100F1AC59EB6");

            BookingCompact bc3 = new BookingCompact();
            bc3.KemasBookingID = Guid.Parse("664E6956-6605-4B88-9676-D3A2ED1CD139");

            BookingCompact bc4 = new BookingCompact();
            bc4.KemasBookingID = Guid.Parse("E7883D12-27AC-44D8-8FF0-30DAB26F671C");

            List<BookingCompact> map = new List<BookingCompact>();
            map.Add(bc1);
            map.Add(bc2);
            map.Add(bc3);
            map.Add(bc4);

            IAccountingService ias = new DataAccessProxyManager();

            BookingCompact[] output = ias.RetrieveID(map.ToArray(), userinfo);

            Assert.IsTrue(output != null);
        }
        #endregion

    }
}
