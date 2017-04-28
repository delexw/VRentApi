using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.KEMASWSIF_RESERVATIONRef;
using CF.VRent.Entities.KEMASWSIF_AUTHRef;
using CF.VRent.Common;
using System.ServiceModel.Web;
using CF.VRent.Common.Entities;
using CF.VRent.Entities;
using System.Collections.Generic;

namespace ProxyTest
{
    [TestClass]
    public class ReservationUnitTestKemasAPI
    {
        public static Guid cfUserID = Guid.Parse("1c9d9c82-d074-45a4-863e-e7eeb2384c64");
        public Guid existingBookingID = Guid.Parse("84693E53-3270-49D2-853F-F7F47B5A3601");

        #region Find
        [TestMethod]
        public void findMyReservationsTestMethod()
        {
            IKemasReservation kemas = new KemasReservationAPI();
            Bookings kemasbooking = kemas.findMyReservationsKemas(cfUserID.ToString());

            Assert.IsTrue(kemasbooking != null,"should retrieve a valid list of bookings.");
        }

        [TestMethod]
        public void findMyReservationByBookingIDandUserIdTestMethod()
        {
            //public findReservation_Response findReservation(string bookingID, string userID)

            
            IKemasReservation kemas = new KemasReservationAPI();
            findReservation_Response kemasbooking = kemas.findReservationKemas(existingBookingID.ToString(),cfUserID.ToString());

            Assert.IsTrue(kemasbooking != null, "should retrieve a valid list of bookings.");
        }

        [TestMethod]
        public void findMyReservationByUserIdAndStatesTestMethod()
        {
            //public BookingDetail[] findMyReservations2(string UserID, string[] States, string Lang)
            IKemasReservation kemas = new KemasReservationAPI();
            string[] states = new string[1]
            {
                //"swBookingModel/created",
                "swBookingModel/released",
                //"swBookingModel/taken",
                //"swBookingModel/interrupted",
                //"swBookingModel/latereturn",
                //"swBookingModel/lostitem",
                //"swBookingModel/lostitem_latereturn",
                //"swBookingModel/completed",
                //"swBookingModel/canceled",
                //"swBookingModel/autocanceled",
                //"swBookingModel/deleted"
            };

            BookingDetail[] kemasbooking = kemas.findMyReservations2Kemas(cfUserID.ToString(), states, "english");

            List<BookingDetail> bookings = new List<BookingDetail>(kemasbooking);
            BookingDetail detail = bookings.Find (m => m.ID.Equals(existingBookingID.ToString()));

            Assert.IsTrue(kemasbooking != null, "should retrieve a valid list of bookings.");
        }

        [TestMethod]
        public void FindReservationAPI() 
        {
            WS_Auth_Response auth = AuthencationUnitTestKemasAPI.SignOn();

            string driver = cfUserID.ToString();
            int itemsPerpage = 10;
            bool itemsPerPageSpecified = true;
            //int pageNumber = 1;
            bool pageSpecified = true;

            string[] states =
                new string[1] 
                { 
                    //"swBookingModel/completed", 
                    "swBookingModel/released", 
                    //"swBookingModel/canceled",
                    //"swBookingModel/autocanceled",

                };

            IKemasReservation kemasReserver = new KemasReservationAPI();
            for (int i = 0; i < 5; i++)
            {
                findReservations2_Response kemasbookings = kemasReserver.findReservations2Kemas(auth.SessionID, states, "english", driver, itemsPerpage, itemsPerPageSpecified, i, pageSpecified);

                findReservations2_Request request = new findReservations2_Request()
                {
                    SessionID = auth.SessionID,
                    States = states,
                    Language = "english",
                    Driver = driver,
                    ItemsPerPage = itemsPerpage,
                    ItemsPerPageSpecified = itemsPerPageSpecified,
                    Page = i,
                    PageSpecified = pageSpecified
                };

                string jsonRequest = SerializedHelper.JsonSerialize<findReservations2_Request>(request);
                //there are only 2 records

                if (kemasbookings.Reservations != null)
                {
                    List<CF.VRent.Entities.KEMASWSIF_RESERVATIONRef.Reservation> oldBookings = new List<CF.VRent.Entities.KEMASWSIF_RESERVATIONRef.Reservation>(kemasbookings.Reservations);
                    CF.VRent.Entities.KEMASWSIF_RESERVATIONRef.Reservation odd = oldBookings.Find(m => !m.State.Equals(states[0]));

                    if (odd != null)
                    {
                        string jsonResposne = SerializedHelper.JsonSerialize<CF.VRent.Entities.KEMASWSIF_RESERVATIONRef.Reservation>(odd); 
                    }
                }
            }

        }



        [TestMethod]
        public void findMyReservationBySessionIdTestMethod()
        {
            //public BookingDetail[] findMyReservations2(string UserID, string[] States, string Lang)
            IKemasReservation kemas = new KemasReservationAPI();

            string userName = "cf";
            string password = "123456";
            IKemasAuthencation kemasAuth = new KemasAuthencationAPI();
            WS_Auth_Response res = kemasAuth.authByLogin(userName, ":md5:" + Encrypt.EncryptMD5(password));

            if(!string.IsNullOrEmpty( res.ID) && !Guid.Parse(res.ID).Equals(Guid.Empty))
            {
                string sessionID = res.SessionID;

                findReservation2_Response kemasbooking = kemas.findReservation2Kemas(existingBookingID.ToString(), sessionID, "english");

                Assert.IsTrue(kemasbooking != null, "should retrieve a valid list of bookings.");
            }
        }

        [TestMethod]
        public void findMyReservationBySessionIdAndPagingTestMethod()
        {
            //public findReservations2_Response findReservations2(string sessionID, string[] states, string language, string driver, int itemsPerPage,bool itemsPerPageSpecified, int pageNumber, bool pageSpecified)
            IKemasReservation kemas = new KemasReservationAPI();

            string userName = "cf";
            string password = "123456";
            IKemasAuthencation kemasAuth = new KemasAuthencationAPI();
            WS_Auth_Response res = kemasAuth.authByLogin(userName, ":md5:" + Encrypt.EncryptMD5(password));

            if (!string.IsNullOrEmpty(res.ID) && !Guid.Parse(res.ID).Equals(Guid.Empty))
            {
                string sessionID = res.SessionID;
                string[] states = new string[11]
                {
                    "swBookingModel/created",
                    "swBookingModel/released",
                    "swBookingModel/taken",
                    "swBookingModel/interrupted",
                    "swBookingModel/latereturn",
                    "swBookingModel/lostitem",
                    "swBookingModel/lostitem_latereturn",
                    "swBookingModel/completed",
                    "swBookingModel/canceled",
                    "swBookingModel/autocanceled",
                    "swBookingModel/deleted"
                };
                string driver = cfUserID.ToString();
                int itemsPerpage = 50;
                bool itemsPerPageSpecified = true;
                //int pageNumber = 1;
                bool pageSpecified = true;

                //there are only 2 records
                for (int i = 0; i < 2; i++)
                {
                    findReservations2_Response kemasbooking = kemas.findReservations2Kemas(sessionID, states, "english", driver, itemsPerpage, itemsPerPageSpecified, i, pageSpecified);

                    CF.VRent.Entities.KEMASWSIF_RESERVATIONRef.Reservation[] bookingcopy = new CF.VRent.Entities.KEMASWSIF_RESERVATIONRef.Reservation[50];
                    Array.Copy(kemasbooking.Reservations, bookingcopy,50);

                    Array.Sort(bookingcopy);
                    Assert.IsTrue(kemasbooking != null, "should retrieve a valid list of bookings."); 
                }
            }
        }

        #endregion

        #region Kemas Create


        [TestMethod]
        public void CreateKemasReservations2TestMethod()
        {
            string userName = "cf";
            string password = "123456";
            IKemasAuthencation kemasAuth = new KemasAuthencationAPI();
            WS_Auth_Response res = kemasAuth.authByLogin(userName, ":md5:" + Encrypt.EncryptMD5(password));

            IKemasReservation kemas = new KemasReservationAPI();
            ReservationData rd = new ReservationData
            {
                Driver = cfUserID.ToString(),
                DateBegin = "2015-05-29 18:15",
                DateEnd = "2015-05-29 18:45",
                StartLocation = "3178afae-4678-4707-bb54-5ae18997ffe2",
                BillingOption = 3,
                Category = "1",
                PaymentStatus = ""
            };

            updateReservation2_Response kemasbooking = kemas.UpdateReservationKemas2(res.SessionID,rd,"english");

            Assert.IsTrue(kemasbooking != null, "should create a valid Kemas booking.");
        }

        #endregion

        #region Update Kemas

        //[TestMethod]
        //public void UpdateKemasReservationsTestMethod()
        //{
        //    IKemasReservation kemas = new KemasReservationAPI();
        //    string[] states = new string[11]
        //    {
        //        "swBookingModel/created",
        //        "swBookingModel/released",
        //        "swBookingModel/taken",
        //        "swBookingModel/interrupted",
        //        "swBookingModel/latereturn",
        //        "swBookingModel/lostitem",
        //        "swBookingModel/lostitem_latereturn",
        //        "swBookingModel/completed",
        //        "swBookingModel/canceled",
        //        "swBookingModel/autocanceled",
        //        "swBookingModel/deleted"
        //    };
        //    BookingDetail[] bds = kemas.findMyReservations2Kemas(cfUserID.ToString(),states,"english");

        //    BookingDetail toUpdate = bds[0];

        //    BookingData rd = new BookingData
        //    {
        //        ID = toUpdate.ID,
        //        Creator = toUpdate.Driver.ID,
        //        Driver = toUpdate.Driver.ID,
        //        DateBegin = toUpdate.DateBegin,
        //        DateEnd = toUpdate.DateBegin,
        //        StartLocation= toUpdate.StartLocation,
        //        Conditions = new Condition[2]
        //        {
        //            new Condition()
        //            {
        //                key="CSCarModel.vehicle_category", value= "1"
        //            },
        //            new Condition()
        //            { 
        //                key="CarGroupModel.TypeOfJourney", value=2.ToString()
        //            }
        //        }
        //    };

        //    ReservationRes updatedBooking = kemas.CreateOrUpdateReservationKemas(cfUserID.ToString(), rd, "english");

        //    Assert.IsTrue(updatedBooking != null, "should create a valid Kemas booking.");
        //}

        [TestMethod]
        public void UpdateKemasReservations2TestMethod()
        {
            string userName = "cf";
            string password = "123456";
            IKemasAuthencation kemasAuth = new KemasAuthencationAPI();
            WS_Auth_Response res = kemasAuth.authByLogin(userName, ":md5:" + Encrypt.EncryptMD5(password)); 
            
            IKemasReservation kemas = new KemasReservationAPI();
            string[] states = new string[11]
            {
                "swBookingModel/created",
                "swBookingModel/released",
                "swBookingModel/taken",
                "swBookingModel/interrupted",
                "swBookingModel/latereturn",
                "swBookingModel/lostitem",
                "swBookingModel/lostitem_latereturn",
                "swBookingModel/completed",
                "swBookingModel/canceled",
                "swBookingModel/autocanceled",
                "swBookingModel/deleted"
            };
            BookingDetail[] bds = kemas.findMyReservations2Kemas(cfUserID.ToString(), states, "english");

            List<BookingDetail> bookings = new List<BookingDetail>(bds);


            BookingDetail toUpdate = bookings.FindLast(m => m.State != states[8]);
            //BookingDetail toUpdate = bookings[0];


            if (toUpdate != null)
            {
                ReservationData rd = new ReservationData
                {
                    ID = toUpdate.ID,
                    Driver = cfUserID.ToString(),
                    DateBegin = toUpdate.DateBegin,
                    DateEnd = toUpdate.DateEnd,
                    StartLocation = toUpdate.StartLocation,
                    BillingOption = 2,
                    Category = toUpdate.Category,
                    PaymentStatus = ""
                };

                updateReservation2_Response kemasbooking = kemas.UpdateReservationKemas2(res.SessionID, rd, "english");
                findReservation_Response findResponse = kemas.findReservationKemas(toUpdate.ID, cfUserID.ToString());

                Assert.IsTrue(kemasbooking != null 
                    && kemasbooking.Reservation.BillingOption == new BillingOption() { ID = 2,Name = "Pay By My company" },
                    "should create a valid Kemas booking.");
 
            }
        }
        #endregion

        #region Cancel Kemas

        [TestMethod]
        public void CancelKemasReservationsTestMethod()
        {
            IKemasReservation kemas = new KemasReservationAPI();
            string[] states = new string[11]
            {
                "swBookingModel/created",
                "swBookingModel/released",
                "swBookingModel/taken",
                "swBookingModel/interrupted",
                "swBookingModel/latereturn",
                "swBookingModel/lostitem",
                "swBookingModel/lostitem_latereturn",
                "swBookingModel/completed",
                "swBookingModel/canceled",
                "swBookingModel/autocanceled",
                "swBookingModel/deleted"
            };
            BookingDetail[] bds = kemas.findMyReservations2Kemas(cfUserID.ToString(), states, "english");

            List<BookingDetail> allBookings = new List<BookingDetail>(bds);
            BookingDetail toUpdate = allBookings.FindLast(m => m.State != states[8]);

            if (toUpdate != null)
            {
                int cancelBooking = kemas.CancelReservationKemas(toUpdate.ID, cfUserID.ToString());

                findReservation_Response findResponse = kemas.findReservationKemas(toUpdate.ID, cfUserID.ToString());

                Assert.IsTrue(cancelBooking != -1 && toUpdate.State != findResponse.State, "should create a valid Kemas booking."); 
            }
        }

        [TestMethod]
        public void CancelKemasReservations2TestMethod()
        {
            string userName = "cf";
            string password = "123456";
            IKemasAuthencation kemasAuth = new KemasAuthencationAPI();
            WS_Auth_Response res = kemasAuth.authByLogin(userName, ":md5:" + Encrypt.EncryptMD5(password));

            IKemasReservation kemas = new KemasReservationAPI();
            string[] states = new string[11]
            {
                "swBookingModel/created",
                "swBookingModel/released",
                "swBookingModel/taken",
                "swBookingModel/interrupted",
                "swBookingModel/latereturn",
                "swBookingModel/lostitem",
                "swBookingModel/lostitem_latereturn",
                "swBookingModel/completed",
                "swBookingModel/canceled",
                "swBookingModel/autocanceled",
                "swBookingModel/deleted"
            };
            BookingDetail[] bds = kemas.findMyReservations2Kemas(cfUserID.ToString(), states, "english");

            List<BookingDetail> allBookings = new List<BookingDetail>(bds);
            BookingDetail toUpdate = allBookings.FindLast(m => m.State != states[8]);

            if (toUpdate != null)
            {
                CF.VRent.Entities.KEMASWSIF_RESERVATIONRef.Error cancelBooking = kemas.CancelReservation2Kemas(toUpdate.ID, res.SessionID);

                findReservation_Response findResponse = kemas.findReservationKemas(toUpdate.ID, cfUserID.ToString());

                Assert.IsTrue(cancelBooking != null && toUpdate.State != findResponse.State, "should create a valid Kemas booking.");
            }
        }

        #endregion
    }
}
