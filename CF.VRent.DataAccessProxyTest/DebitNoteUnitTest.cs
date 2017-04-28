using CF.VRent.Common;
using CF.VRent.Common.UserContracts;
using CF.VRent.DAL;
using CF.VRent.DataAccessProxy.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF.VRent.DataAccessProxyTest
{
    [TestClass]
    public class DebitNoteUnitTest
    {
        [TestMethod]
        public void RetrieveDebitNotePeriodsTestMethod()
        {
            ProxyUserSetting userinfo = null;

            DebitNotePeriod[] periods = DebitNoteDAL.RetrieveDebitNotePeriods(userinfo);

            Assert.IsTrue(periods != null);
        }

        [TestMethod]
        public void RetrievePeriodsByStateTestMethod()
        {
            ProxyUserSetting userinfo = new ProxyUserSetting();
            userinfo.ID = Guid.NewGuid().ToString();

            DebitNotePeriod[] periods = DebitNoteDAL.RetrievePeriodsByState(SyncedRecordState.NotRun, userinfo);

            Assert.IsTrue(periods != null);
        }

        [TestMethod]
        public void RetrieveCompletedPeriodsTestMethod()
        {
            ProxyUserSetting userinfo = new ProxyUserSetting();
            userinfo.ID = Guid.NewGuid().ToString();

            DebitNotePeriod[] periods = DebitNoteDAL.RetrieveCompletedPeriods(userinfo);

            Assert.IsTrue(periods != null);
        }


        [TestMethod]
        public void GenerateDebitNotesTestMethod()
        {
            ProxyUserSetting userinfo = null;

            DebitNotePeriod dnp = new DebitNotePeriod();
            dnp.ID = 2;
            dnp.CreatedOn = DateTime.Now;
            dnp.CreatedBy = Guid.NewGuid();

            DebitNoteDAL.GeneateDebitNotes(dnp, userinfo);
        }


        [TestMethod]
        public void RetrieveDebitNotesTestMethod()
        {
            ProxyUserSetting userinfo = null;

            DateTime start = new DateTime(2015, 9, 1).Date;
            Calendar c = new GregorianCalendar();
            int days = c.GetDaysInMonth(start.Year, start.Month);
            DateTime end = start.AddMonths(1).AddMilliseconds(-1);

            DebitNotesSearchConditions dnsc = new DebitNotesSearchConditions();
            //dnsc.ClientName = "Tom";
            dnsc.ItemsPerPage = 5;
            dnsc.PageNumber = 1;
            //dnsc.PeriodBegin = start;
            //dnsc.PeriodEnd = end;
            dnsc.TotalPages = -1;
            //dnsc.State = 1;

            DebitNotesSearchConditions output = DebitNoteDAL.RetrieveDebitNotesWithPaging(dnsc,userinfo);

            Assert.IsTrue(output.Notes != null);
        }

        [TestMethod]
        public void RetrieveDetailsTestMethod()
        {
            DebitNoteDetailSearchConditions dndsc = new DebitNoteDetailSearchConditions();
            dndsc.DebitNoteID = 1;
            dndsc.DateBegin = new DateTime(2015, 8, 1);
            dndsc.DateEnd = new DateTime(2015, 11, 1);
            dndsc.KemasBookingNumber = "0";
            //dndsc.UserID = Guid.Parse("05A543A4-86A3-4A2C-AEE4-DF5A01A38711");
            dndsc.UserName = "e";

            dndsc.ItemsPerPage = 5;
            dndsc.PageNumber = 1;

            DebitNoteDetailSearchConditions output = DebitNoteDAL.RetrieveDebitNoteDetailsInRange(dndsc, null);

            Assert.IsTrue(output.Items != null);
        }

        [TestMethod]
        public void RetrieveDebitNoteByIDTestMethod()
        {
            ProxyUserSetting userinfo = null;
            int debitNoteID = 1;

            DebitNote note = DebitNoteDAL.RetrieveDebitNotesByID(debitNoteID, DateTime.Now, userinfo);

            Assert.IsTrue(note != null);
        }

        [TestMethod]
        public void SaveStagedBookingsUnitTest() 
        {
            StagedBookings basket = new StagedBookings();
            basket.BeginDate = new DateTime(2015, 9, 1);
            basket.EndDate = new DateTime(2015, 9, 30);
            basket.Items = new CompletedBooking[1];

            CompletedBooking cb = new CompletedBooking();
            //[ID]
            //,[KemasBookingID]
            //,[BookingID]
            //,[DateBegin]
            //,[DateEnd]
            cb.ID = -1;
            cb.KemasBookingID = Guid.NewGuid();
            cb.BookingID = new Nullable<int>(-1);
            cb.DateBegin = DateTime.Now;
            cb.DateEnd = DateTime.Now;

            //,[StartLocationID]
            //,[StartLocationName]
            //,[BillingOption]
            //,[Category]
            //,[CreatorID]
            cb.StartLocationID = Guid.NewGuid();
            cb.StartLocationName = "location";
            cb.BillingOption = 2;
            cb.Category = "Eco";
            cb.CreatorID = Guid.NewGuid();

            //,[UserID]
            //,[CorporateID]
            //,[CorporateName]
            //,[CarID]
            //,[CarName]
            cb.UserID = Guid.NewGuid();
            cb.CorporateID = Guid.NewGuid().ToString();
            cb.CorporateName = "ECO";
            cb.CarID = Guid.NewGuid();
            cb.CarName = "ABCDEFG";

            //,[KemasBookingNumber]
            //,[PickupBegin]
            //,[PickupEnd]
            //,[KeyOut]
            //,[KeyIn]
            cb.KemasBookingNumber = "45678";
            cb.PickupBegin = DateTime.Now;
            cb.PickupEnd = DateTime.Now;
            cb.KeyIn = DateTime.Now;
            cb.KeyOut = DateTime.Now;

            //,[State]
            //,[Price]
            //,[PricingDetail]
            //,[PaymentStatus]
            //,[SyncState]
            cb.State = "completed";
            cb.Price = 5.55m;
            cb.PricingDetail = "";
            cb.PaymentStatus = "payment";
            cb.SyncState = StagingState.Archived;


            //,[MatchState]
            //,[CreatedOn]
            //,[CreatedBy]
            //,[ModifiedOn]
            //,[ModifiedBy]
            cb.CompareResult = MatchState.Unknown;
            cb.CreatedOn = DateTime.Now;
            cb.CreatedBy = Guid.NewGuid();
            cb.ModifiedBy = null;
            cb.ModifiedOn = null;

            basket.Items[0] = cb;

            ProxyUserSetting userInfo = null;
            CompletedBookingDAL.SaveStagedBookings(basket,userInfo);
        }

        [TestMethod]
        public void RetrieveBookingIDTestMethod()
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

            BookingCompact[] output = DebitNoteDAL.RetrieveID(map.ToArray(), userinfo);

            Assert.IsTrue(output != null);
        }
    }
}
