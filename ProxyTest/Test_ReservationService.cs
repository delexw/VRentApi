using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using CF.VRent.Entities;
using CF.VRent.Common;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.BLL;
using Proxy;
using System.IO;
using System.Configuration;
using CF.VRent.Contract;
using CF.VRent.Entities.KemasWrapper;
using System.ServiceModel.Web;
using CF.VRent.Common.Entities;
using CF.VRent.Entities.FapiaoPreferenceProxy;
using CF.VRent.Entities.KEMASWSIF_AUTHRef;
using CF.VRent.Entities.KEMASWSIF_RESERVATIONRef;
using System.ServiceModel;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.Common.UserContracts;
using System.Web;

namespace ProxyTest
{
    /// <summary>
    /// Summary description for Test_ReservationService
    /// </summary>
    [TestClass]
    public class Test_ReservationService
    {
        private static string basedir = ConfigurationManager.AppSettings["TestDir"];

        #region Helper Method

        public static Guid cfUserID = Guid.Parse("1c9d9c82-d074-45a4-863e-e7eeb2384c64");
        public static Guid SystemID = Guid.Parse("99999999-9999-9999-9999-999999999999");
        public static string Language = "english";
        public static Guid existingBookingID = Guid.Parse("ed4ff64a-3db0-4c28-9059-dcc5d09d2edc");

        #endregion

        #region Create NewReservation

        [TestMethod]
        public void TestRetrieveProxyBookingDetail()
        {
            KemasAuthencationAPI kemasAuth = new KemasAuthencationAPI();
            WS_Auth_Response auth = kemasAuth.authByLogin("jaime@qaz.com", "123456");

            string name1 = "&#38472;";

            string number = string.Join(string.Empty,name1.ToCharArray().Where(m=> char.IsDigit(m)).ToArray());

            string hexValue = Int32.Parse(number).ToString("X");

            name1 = name1.Replace("38472", hexValue);

            // copy the string as UTF-8 bytes.
            byte[] utf8Bytes = new byte[name1.Length];
            for (int i = 0; i < name1.Length; ++i)
            {
                //Debug.Assert( 0 <= utf8String[i] && utf8String[i] <= 255, "the char must be in byte's range");
                utf8Bytes[i] = (byte)name1[i];
            }
            string finalName = Encoding.UTF8.GetString(utf8Bytes, 0, utf8Bytes.Length);

            char[] nameChars = Encoding.Unicode.GetChars(utf8Bytes);

            string s = HttpUtility.HtmlEncode("/u9648");



            string output = HttpUtility.HtmlDecode(name1);

            ProxyUserSetting userinfo = new ProxyUserSetting();
            userinfo.ID = auth.ID.ToString();
            userinfo.SessionID = auth.SessionID;


            IProxyReservation impl = new ProxyReservationImpl(userinfo);
            int existingProxyBookingID = 221;

            KemasReservationEntity proxyResponse = impl.ProxyReservationDetail(auth.SessionID, existingProxyBookingID, "english");

            string response = SerializedHelper.JsonSerialize<KemasReservationEntity>(proxyResponse);

            Assert.IsTrue(proxyResponse != null, "Jason format for Edit an existing Reservation");
        }

        //vrent bug206
        //[TestMethod]
        //public void UpdateRetrieveProxyBookingDetailAfterDeleteFP()
        //{
        //    IProxyReservation impl = new ProxyReservationImpl();
        //    Guid existingProxyBookingID = Guid.Parse("08FA4704-086B-47FC-881E-152455C46635");
        //    //ProxyFindReservationResResponse proxyResponse = impl.ProxyReservationDetail(cfUserID.ToString(), existingProxyBookingID.ToString());
        //    Guid? FPID = proxyResponse.FaPiaoPreferenceID;

        //    string response = SerializedHelper.JsonSerialize<ProxyFindReservationResResponse>(proxyResponse);

        //    //remove the Fapiao Preference
        //    FapiaoPreferenceImpl fpi = new FapiaoPreferenceImpl(cfUserID.ToString());
        //    fpi.DeleteFapiaoPreference(FPID.ToString());

        //    //ProxyFindReservationResResponse proxyResponse1 = impl.ProxyReservationDetail(cfUserID.ToString(), existingProxyBookingID.ToString());

        //    Assert.IsTrue(proxyResponse1.FaPiaoPreferenceID != null && proxyResponse1.FaPiaoPreferenceID.Equals(FPID), "Jason format for Edit an existing Reservation");
        //}

        //vrent bug206
        //[TestMethod]
        //public void RetrieveBookingDetailAfterUpdateFapiaoPreference()
        //{
        //    IProxyReservation impl = new ProxyReservationImpl();
        //    Guid existingProxyBookingID = Guid.Parse("08FA4704-086B-47FC-881E-152455C46635");
        //    //ProxyFindReservationResResponse proxyResponse = impl.ProxyReservationDetail(cfUserID.ToString(), existingProxyBookingID.ToString());
        //    Guid? FPID = proxyResponse.FaPiaoPreferenceID;

        //    string response = SerializedHelper.JsonSerialize<ProxyFindReservationResResponse>(proxyResponse);

        //    //remove the Fapiao Preference
        //    FapiaoPreferenceImpl fpi = new FapiaoPreferenceImpl(cfUserID.ToString());

        //    ProxyFapiaoPreference currentOne = fpi.GetFapiaoPreferenceDetail(FPID.ToString());
        //    currentOne.AddresseeName = "BookingDetail";

        //    ProxyFapiaoPreference updated = fpi.UpdateFapiaoPreference(currentOne);

        //    //ProxyFindReservationResResponse proxyResponse1 = impl.ProxyReservationDetail(cfUserID.ToString(), existingProxyBookingID.ToString());

        //    //Assert.IsTrue(proxyResponse1.FaPiaoPreferenceID != null && proxyResponse1.FaPiaoPreferenceID.Equals(FPID), "FP will not change after is was updated");
        //}

        [TestMethod]
        public void TestCreateBooking()
        {
            string datafile = Path.Combine(@"C:\CF-repo\vrent429\ProxyTest\TestData\Reservation_Create", "CreateReservationRequest.txt");
            string data = ProxyTest.DatReader.Read(datafile);

            KemasReservationEntity pr = SerializedHelper.JsonDeserialize<KemasReservationEntity>(data);
            WS_Auth_Response res = AuthencationUnitTestKemasAPI.SignOn();
            ProxyUserSetting pus = new ProxyUserSetting() { SessionID = res.SessionID };

            ProxyReservationImpl impl = new ProxyReservationImpl(pus);
            KemasReservationEntity proxyResponse = impl.CreateReservation(res.SessionID, pr, "english");

            string response = SerializedHelper.JsonSerialize<KemasReservationEntity>(proxyResponse);

            Assert.IsNotNull(proxyResponse != null, "Jason format for requesting Fapiao for an existing Reservation or creating a new reservation");
        }


        //[TestMethod]
        //public void TestFindExistingReservation()
        //{
        //    IKemasReservation kemas = new KemasReservationAPI();
        //    FindReservationRes fr = KemasHelper.ConvertFromFindReservation_Response(kemas.findReservationKemas("967b965a-8f99-4e93-8e9f-b53b07a06a1c", cfUserID.ToString()));
        //    Assert.IsNotNull(fr);
        //}

        [TestMethod]
        public void TestFindExistingReservationNew()
        {
            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);

            IProxyReservation ri = new ProxyReservationImpl(setting);
            KemasReservationEntity[] bookings = ri.FindReservations(cfUserID, new string[3] { "swBookingModel/completed", "swBookingModel/released", "swBookingModel/canceled" },"english");
            Assert.IsNotNull(bookings.Length > 0,"Retrieve All my bookings");
        }

        [TestMethod]
        public void TestFindExistingReservationNewWithSync()
        {
            WS_Auth_Response auth = AuthencationUnitTestKemasAPI.SignOn();
            ProxyUserSetting pus = new ProxyUserSetting()
            {
                ID = cfUserID.ToString(),
                SessionID = auth.SessionID
            };

            //
            DateTime start = DateTime.Now;

            IProxyReservation ri = new ProxyReservationImpl(pus);
            KemasReservationEntity[] bookings = ri.FindReservations(cfUserID, new string[3] { "swBookingModel/completed", "swBookingModel/released", "swBookingModel/canceled" }, "english");
            Assert.IsNotNull(bookings.Length > 0, "Retrieve All my bookings");
            DateTime end = DateTime.Now;

            long duration = (long)(end - start).TotalSeconds;
        }


        [TestMethod]
        public void RetrieveMyBookingsWithStatesUnitTest()
        {
            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);


            IProxyReservation ri = new ProxyReservationImpl(setting);
            KemasReservationEntity[] bookings = ri.FindReservations(cfUserID, new string[1] { "swBookingModel/canceled" },"english");
            Assert.IsNotNull(bookings.Length > 0, "Retrieve All my bookings");
        }

        [TestMethod]
        public void CompareBookingDetailUnitTest()
        {
            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);
            int proxyBookingID = 15;
            IProxyReservation ir = new ProxyReservationImpl(setting);
            KemasReservationEntity Booking1 = ir.ProxyReservationDetail(setting.SessionID, proxyBookingID, "english");
            IKemasReservation ikemas = new KemasReservationAPI();
            findReservation2_Response booking2 = ikemas.findReservation2Kemas(Booking1.KemasBookingID.ToString(), setting.SessionID, "english");

            findReservation_Response  booking4 = ikemas.findReservationKemas(Booking1.KemasBookingID .ToString(),cfUserID.ToString());
            
            //DataServiceClient dsclcient = new DataServiceClient();
            IDataService dsclcient = new DataAccessProxyManager();
            ProxyReservation booking3 = dsclcient.RetrieveReservationByBookingID(proxyBookingID);

            bool isSame = false;
            if (Booking1.BillingOption == booking2.Reservation.BillingOption.ID
                && Booking1.BillingOption == booking3.BillingOption
                && booking2.Reservation.BillingOption.ID == booking3.BillingOption)
            {
                isSame = true;
            }
            Assert.IsNotNull(Booking1 != null && booking2 != null && booking3 != null && isSame, "Retrieve All my bookings");
        }

        [TestMethod]
        public void BookingDetailAPIUnitTest()
        {
            int proxyBookingID = 34;
            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);

            IProxyReservation ir = new ProxyReservationImpl(setting);
            KemasReservationEntity Booking1 = ir.ProxyReservationDetail(setting.SessionID, proxyBookingID, "english");

            Assert.IsNotNull(true, "Retrieve All my bookings");
        }
        
        #endregion

        #region Data access layer exception handling
        [TestMethod]
        [ExpectedException(typeof(FaultException<ReturnResult>))]
        public void DataAccessLayerUnknownExceptionUnitTest()
        {
            //DataServiceClient dsc = new DataServiceClient();


            IDataService dsc = new DataAccessProxyManager();
            try
            {
                ProxyFapiao[] fapiaos = dsc.RetrieveMyFapiaoData(cfUserID);
            }
            catch (FaultException<ReturnResult> fe)
            {
                throw fe;
            }

            Assert.IsNotNull(true, "Retrieve All my bookings");
        }
        #endregion

        #region Cancel Reservation
        [TestMethod]
        public void CancelProxyBookingTestWithPricingPayment()
        {
            WS_Auth_Response auth = AuthencationUnitTestKemasAPI.SignOn();

            int proxyBookingID = 55;

            ProxyUserSetting pus = new ProxyUserSetting() { SessionID = auth.SessionID };
            ProxyReservationImpl reserve = new ProxyReservationImpl(pus);

            int canceledBooking = -1;

            try
            {
                canceledBooking = reserve.CancelReservation(auth.SessionID, proxyBookingID,"english");
            }
            catch (Exception sqle)
            {
                string message = sqle.Message;
            }
        }
        #endregion

        #region update Reservation
        [TestMethod]
        public void UpdateProxyBookingTestWithPricingPayment()
        {
            WS_Auth_Response auth = AuthencationUnitTestKemasAPI.SignOn();

            ProxyUserSetting pus = new ProxyUserSetting()
            {
                SessionID = auth.SessionID
            };
            ProxyReservationImpl reserve = new ProxyReservationImpl(pus);

            string datafile = Path.Combine(@"C:\CF-repo\vrent429\ProxyTest\TestData\Reservation_Create", "UpdateReservationRequest.txt");
            string data = ProxyTest.DatReader.Read(datafile);

            KemasReservationEntity kre = SerializedHelper.JsonDeserialize<KemasReservationEntity>(data);

            KemasReservationEntity updated = reserve.UpdateReservation(auth.SessionID, kre, "english");

            Assert.IsTrue(updated != null);
        }
        #endregion

        #region Test Paging

        [TestMethod]
        public void WhereConditionsUnitTest()
        {
            List<string> columns = new List<string>();
            //"PROXYBOOKINGID",
            //"KEMASBOOKINGID", // guid
            //"DATEBEGIN",//"yyyy-MM-dd HH:mm";
            //"DATEEND", //"yyyy-MM-dd HH:mm";
            //"NUMBER",

            ////"CreatorID",
            ////"CreatorName",

            //"DRIVERID",
            ////"DriverName",
            //"CORPORATEID",
            //"STATE", //kemas state
            //"BILLINGOPTION",
            //"PRICE",

            //"CREATEDON",
            //"CREATEDBY",
            //"MODIFIEDON",
            //"MODIFIEDBY",
            //"FAPIAOPREFERENCEID",
            columns.Add("ProxyBookingID != 1");
            columns.Add("KemasBookingID != " + Guid.NewGuid().ToString());
            columns.Add("DateBegin >= " + "2015-07-02 12:00:45");
            columns.Add("DateEnd <= " + "2015-07-02 12:00:45");
            columns.Add("Number != 0000000123");
            columns.Add("DriverID != " + Guid.NewGuid().ToString());
            columns.Add("CORPORATEID != " + "ABC");
            columns.Add("State = Created");
            columns.Add("BillingOption = 2");
            columns.Add("Price > 3.45");
            columns.Add("CreatedOn > " + "2015-07-02 12:00:45");
            columns.Add("CreatedBy != " + Guid.NewGuid().ToString());
            columns.Add("ModifiedOn > " + "2015-07-02 12:00:45");
            columns.Add("ModifiedBy != " + Guid.NewGuid().ToString());
            columns.Add("FAPIAOPREFERENCEID != null");

            Dictionary<string, string> whereConditions = ReservationPagingUtility.ParseWhereConditions(columns.Select(m=>m.ToUpper()).ToArray());

        }

        [TestMethod]
        public void OrderConditionUnitTest()
        {
            List<string> columns = new List<string>();

            //"PROXYBOOKINGID",
            //"KEMASBOOKINGID", // guid
            //"DATEBEGIN",//"yyyy-MM-dd HH:mm";
            //"DATEEND", //"yyyy-MM-dd HH:mm";
            //"NUMBER",

            ////"CreatorID",
            ////"CreatorName",

            //"DRIVERID",
            ////"DriverName",
            //"CORPORATEID",
            //"STATE", //kemas state
            //"BILLINGOPTION",
            //"PRICE",

            //"CREATEDON",
            //"CREATEDBY",
            //"MODIFIEDON",
            //"MODIFIEDBY",
            //"FAPIAOPREFERENCEID",
            //"FaPiaoRequestType"
            columns.Add("ProxyBookingID desc");
            columns.Add("KemasBookingID asc");
            columns.Add("DateBegin DESC ");
            columns.Add("DateEnd ASC ");
            columns.Add("Number DESC");
            columns.Add("DriverID DESC");
            columns.Add("CORPORATEID ASC");
            columns.Add("State DESC");
            columns.Add("BillingOption ASC");
            columns.Add("Price ASC");
            columns.Add("CreatedOn DESC");
            columns.Add("CreatedBy ASC");
            columns.Add("ModifiedOn DESC");
            columns.Add("ModifiedBy DESC");
            columns.Add("FAPIAOPREFERENCEID ASC");


            Dictionary<string, string> whereConditions = ReservationPagingUtility.ParseOrderByConditions(columns.Select(m => m.ToUpper()).ToArray());

        }

        [TestMethod]
        public void ReservationwithPagingUnitTest()
        {
            #region Where Conditions
            List<string> columns = new List<string>();
            //columns.Add("DateBegin >= 2014-07-07 03:09:53");
            //columns.Add("DateEnd <= " + "2015-06-18 08:58:53");
            columns.Add("Number = 14");
            //columns.Add("DriverFirstName != Tom");
            columns.Add("StartLocationName = OCons KeyBox I");
            columns.Add("Name = Wang");
            #endregion

            #region Order By Conditions
            List<string> orderByColumns = new List<string>();

            //"PROXYBOOKINGID",
            //"KEMASBOOKINGID", // guid
            //"DATEBEGIN",//"yyyy-MM-dd HH:mm";
            //"DATEEND", //"yyyy-MM-dd HH:mm";
            //"NUMBER",

            ////"CreatorID",
            ////"CreatorName",

            //"DRIVERID",
            ////"DriverName",
            //"CORPORATEID",
            //"STATE", //kemas state
            //"BILLINGOPTION",
            //"PRICE",

            //"CREATEDON",
            //"CREATEDBY",
            //"MODIFIEDON",
            //"MODIFIEDBY",
            //"FAPIAOPREFERENCEID",
            //"FaPiaoRequestType"
            //orderByColumns.Add("ProxyBookingID DESC ");
            //orderByColumns.Add("DateBegin DESC ");
            //orderByColumns.Add("DateEnd ASC ");
            //orderByColumns.Add("Number DESC");
            ////orderByColumns.Add("DriverFirstName DESC");
            //orderByColumns.Add("StartLocationName DESC");
            int itemsPerPage = 10;
            int pageNumber = 1;

            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);

            IProxyReservation reservations = new ProxyReservationImpl(setting);
            ProxyReservationsWithPaging pagedBooking = new ProxyReservationsWithPaging()
            {
                RawWhereConditions = columns.Select(m=> m.ToUpper()).ToArray(),
                RawOrderByConditions = orderByColumns.Select(m => m.ToUpper()).ToArray(),
                ItemsPerPage = itemsPerPage,
                PageNumber = pageNumber,

            };

            string reqJson = SerializedHelper.JsonSerialize<ProxyReservationsWithPaging>(pagedBooking);


            ProxyReservationsWithPagingResponse bookings = reservations.FindReservationsWithPaging(pagedBooking);

            string resJson = SerializedHelper.JsonSerialize<ProxyReservationsWithPagingResponse>(bookings);


            Assert.IsTrue(bookings!= null );

            #endregion
        }

        [TestMethod]
        public void ReservationwithPagingWithoutNothingUnitTest()
        {
            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);


            List<string> columns = new List<string>();
            List<string> orderByColumns = new List<string>();

            int itemsPerPage = 5;
            int pageNumber = 1;

            IProxyReservation reservations = new ProxyReservationImpl(setting);
            ProxyReservationsWithPaging pagedBooking = new ProxyReservationsWithPaging()
            {
                RawWhereConditions = columns.Select(m => m.ToUpper()).ToArray(),
                RawOrderByConditions = orderByColumns.Select(m => m.ToUpper()).ToArray(),
                ItemsPerPage = itemsPerPage,
                PageNumber = pageNumber,

            };

            string reqJson = SerializedHelper.JsonSerialize<ProxyReservationsWithPaging>(pagedBooking);

            ProxyReservationsWithPagingResponse bookings = reservations.FindReservationsWithPaging(pagedBooking);

            string resJson = SerializedHelper.JsonSerialize<ProxyReservationsWithPagingResponse>(bookings);

            Assert.IsTrue(bookings != null);
        }
        #endregion

        [TestMethod]
        public void CheckVMReservationPaging() 
        {
            UserSettingBLL usb = new UserSettingBLL();

            UserExtension ue = usb.Login("operation.assist@abc.com", "123456");

            ProxyUserSetting userInfo = ServiceUtility.ConvertFromUserExtention(ue);

            List<string> columns = new List<string>();
            List<string> orderByColumns = new List<string>();

            int itemsPerPage = 5;
            int pageNumber = 1;

            IProxyReservation reservations = new ProxyReservationImpl(userInfo);
            ProxyReservationsWithPaging pagedBooking = new ProxyReservationsWithPaging()
            {
                RawWhereConditions = columns.Select(m => m.ToUpper()).ToArray(),
                RawOrderByConditions = orderByColumns.Select(m => m.ToUpper()).ToArray(),
                ItemsPerPage = itemsPerPage,
                PageNumber = pageNumber,

            };

            string reqJson = SerializedHelper.JsonSerialize<ProxyReservationsWithPaging>(pagedBooking);

            ProxyReservationsWithPagingResponse bookings = reservations.FindReservationsWithPaging(pagedBooking, userInfo);

            Assert.IsTrue(bookings != null);
        }

        [TestMethod]
        public void CheckSLCReservationPaging()
        {
            UserSettingBLL usb = new UserSettingBLL();

            UserExtension ue = usb.Login("operation.mgr@abc.com", "123456");

            ProxyUserSetting userInfo = ServiceUtility.ConvertFromUserExtention(ue);

            List<string> columns = new List<string>();
            List<string> orderByColumns = new List<string>();

            int itemsPerPage = 5;
            int pageNumber = 1;

            IProxyReservation reservations = new ProxyReservationImpl(userInfo);
            ProxyReservationsWithPaging pagedBooking = new ProxyReservationsWithPaging()
            {
                RawWhereConditions = columns.Select(m => m.ToUpper()).ToArray(),
                RawOrderByConditions = orderByColumns.Select(m => m.ToUpper()).ToArray(),
                ItemsPerPage = itemsPerPage,
                PageNumber = pageNumber,

            };

            string reqJson = SerializedHelper.JsonSerialize<ProxyReservationsWithPaging>(pagedBooking);

            ProxyReservationsWithPagingResponse bookings = reservations.FindReservationsWithPaging(pagedBooking, userInfo);

            Assert.IsTrue(bookings != null);
        }

        [TestMethod]
        public void RetrieveListForUser()
        {

            string[] states = new string[3] { "swBookingModel/completed", "swBookingModel/released", "swBookingModel/canceled" };

            UserSettingBLL usb = new UserSettingBLL();
            UserExtension ue = usb.Login("Jack35@abc.com", "123456");

            ProxyUserSetting userInfo = ServiceUtility.ConvertFromUserExtention(ue);

            IProxyReservation bookingAPI = new ProxyReservationImpl(userInfo);

            KemasReservationEntity[] bookings = bookingAPI.FindReservations(Guid.Parse(userInfo.ID), states, "english");
        }


    }
}
