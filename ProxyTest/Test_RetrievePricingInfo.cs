using CF.VRent.BLL;
using CF.VRent.Common;
using CF.VRent.Common.UserContracts;
using CF.VRent.Contract;
using CF.VRent.Entities;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.KEMASWSIF_AUTHRef;
using CF.VRent.Entities.KEMASWSIF_RESERVATIONRef;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Proxy;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml;

namespace ProxyTest
{
    [TestClass]
    public class Test_RetrievePricingInfo
    {
        private static string basedir = ConfigurationManager.AppSettings["TestDir"];

        private string validBooking = "8c12eb68-f4e7-4003-a114-c73fa930bdff";

        #region helper Method
        #endregion

        #region Booking Prince

        [TestMethod]
        public void GetCancelReservationFees()
        {
            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);

            Guid cancelBooking = Guid.Parse("87A74C7F-B934-49C3-A2EF-2D8F49DCAF6E");

            if (setting != null)
            {
                IBookingPrice bmil = new BookingPriceImpl(setting);
                getCancelReservationFees_Response price = bmil.getCancelReservationFees(cancelBooking.ToString(), setting.SessionID);

                if (price != null && !string.IsNullOrEmpty(price.PriceDetails))
                {
                    PrincingInfoFactory factory = new PrincingInfoFactory(price.PriceDetails);
                    factory.Process();
                    BookingPriceInfo bpf = factory.Price;

                    string response = SerializedHelper.JsonSerialize<BookingPriceInfo>(bpf);
                    if (!string.IsNullOrEmpty(price.PriceDetails))
                    {
                        Assert.IsNotNull(price.PriceDetails, "should not be empty");
                    }
                }

            }
        }

        [TestMethod]
        public void CheckPriceUnitTest()
        {
            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);

            BookingSample bs = new BookingSample()
            {
                Driver = Test_ReservationService.cfUserID.ToString(),
                DateBegin = DateTime.Now.AddHours(3).ToString(),
                DateEnd =  DateTime.Now.AddDays(4).ToString(),
                Conditions = new Condition[]
                { 
                    new Condition(){ key="CSCarModel.vehicle_category", value="Eco"},
                    new Condition(){ key="CarGroupModel.TypeOfJourney", value="1"}
                }
            };

            IBookingPrice bmil = new BookingPriceImpl(setting);
            checkPrice_Response price = bmil.CheckPrice(Test_ReservationService.cfUserID.ToString(), bs);

            string response = SerializedHelper.JsonSerialize<checkPrice_Response>(price);

            Assert.IsTrue(price != null, "should return an valid price for the booking");
        }

        [TestMethod]
        public void CheckPriceDetailedUnitTest()
        {
            checkPrice2_Response price = null;

            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);

            OptionsBLL lb = new OptionsBLL(setting);
            List<ProxyLocation> locations = lb.GetAllLocation(setting.ID).ToList();

            if (setting != null && locations != null && locations.Count > 0)
            {
                IBookingPrice bmil = new BookingPriceImpl(setting);
                checkPrice2_RequestBookingData vrentbooking = new checkPrice2_RequestBookingData()
                {
                    Driver = Test_ReservationService.cfUserID.ToString(),
                    DateBegin = DateTime.Now.AddHours(3).ToString(),
                    DateEnd =  DateTime.Now.AddDays(4).ToString(),
                    StartLocation = locations[0].ID,
                    BillingOption = 3,
                    Category = "Eco"
                };
                price = bmil.CheckPriceDetailed(setting.SessionID, vrentbooking);
                if (price != null && !string.IsNullOrEmpty(price.PriceDetails))
                {
                    price.PriceDetails = ConfigurationManager.AppSettings["SampleCreatePricingXml"];
                    IPricingFactory factory1 = new PricingProcessor(price.PriceDetails);
                    factory1.Process();
                    BookingPriceInfo bpf = factory1.Price;

                    string response = SerializedHelper.JsonSerialize<BookingPriceInfo>(bpf);
                    if (!string.IsNullOrEmpty(price.PriceDetails))
                    {
                        Assert.IsNotNull(price.PriceDetails, "should not be empty");
                    }
                }
            }

            Assert.IsTrue(price != null, "should return an valid price for the booking");
        }

        [TestMethod]
        public void GetPrice()
        {

            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);

            IBookingPrice bmil = new BookingPriceImpl(setting);
            string price = bmil.GetPrice(validBooking);

            int priceValue = int.Parse(price);

            string jsonResponse = SerializedHelper.JsonSerialize<int>(priceValue);

            Assert.IsTrue(priceValue > 0,"the bookign value should be greater than 0");
        }

        [TestMethod]
        public void GetPriceDetailed()
        {

            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);

            IBookingPrice bmil = new BookingPriceImpl(setting);
            string priceDetail = bmil.GetPriceDetailed(validBooking);

            if (!string.IsNullOrEmpty(priceDetail))
            {
                PrincingInfoFactory factory = new PrincingInfoFactory(priceDetail);
                factory.Process();
                BookingPriceInfo bpf = factory.Price;

                ProxyBookingPrice pbp = BookingPriceImpl.ConvertFromFEPriceInfo(bpf);

                //ProxyBookingPrice pbp = PrincingHelper.ConvertFromFEPriceInfo(bpf);

                //string response = SerializedHelper.JsonSerialize<BookingPriceInfo>(bpf);
                //if (!string.IsNullOrEmpty(priceDetail))
                //{
                //    Assert.IsNotNull(priceDetail, "should not be empty");
                //} 
            }

        }

        [TestMethod]
        public void ConvertFEPriceInfoToDBPriceInfo()
        {

            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);

            IBookingPrice bmil = new BookingPriceImpl(setting);
            string priceDetail = bmil.GetPriceDetailed(validBooking);

            if (!string.IsNullOrEmpty(priceDetail))
            {
                PrincingInfoFactory factory = new PrincingInfoFactory(priceDetail);
                factory.Process();
                BookingPriceInfo bpf = factory.Price;

            //    ProxyBookingPrice pbp = PrincingHelper.ConvertFromFEPriceInfo(bpf);

            //    Assert.IsNotNull(pbp, "should not be empty");
            //
            }

        }

        [TestMethod]
        public void GetPriceDetailUnitTest()
        {
            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);


            BookingPriceInfo bpi = null;

            IBookingPrice ibp = new BookingPriceImpl(setting);
            bpi = BookingPriceImpl.ConvertFromDBPriceInfo(ibp.LoadPrincingInfo(1));

            string resposne = SerializedHelper.JsonSerialize<BookingPriceInfo>(bpi);
            Assert.IsTrue(bpi != null,"should not null");

        }

        [TestMethod]
        public void ConvertDBPriceInfoToFEPriceInfo()
        {

            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);

            IBookingPrice bmil = new BookingPriceImpl(setting);
            string priceDetail = bmil.GetPriceDetailed(validBooking);

            if (!string.IsNullOrEmpty(priceDetail))
            {
                PrincingInfoFactory factory = new PrincingInfoFactory(priceDetail);
                factory.Process();
                BookingPriceInfo bpf = factory.Price;

                //ProxyBookingPrice pbp = PrincingHelper.ConvertFromFEPriceInfo(bpf);

                //BookingPriceInfo bpi = BookingPriceImpl.ConvertFromDBPriceInfo(pbp);

                //Assert.IsNotNull(pbp, "should not be empty");
            }

        }


        #endregion

        [TestMethod]
        public void LoadProxyBookingPriceTestMethod()
        {

            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);

            IBookingPrice bmil = new BookingPriceImpl(setting);
            ProxyBookingPrice fromDB = bmil.LoadPrincingInfo(2);

            Assert.IsTrue(fromDB != null, "should load a new set of princing");

        }

        [TestMethod]
        public void LoadProxyBookingPriceFromFETestMethod()
        {
            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);

            IBookingPrice ibp = new BookingPriceImpl(setting);
            BookingPriceInfo bpi = BookingPriceImpl.ConvertFromDBPriceInfo(ibp.LoadPrincingInfo(9));

            Assert.IsTrue(bpi != null, "should load a new set of princing");

        }

        [TestMethod]
        public void GetCancelFees()
        {
            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);

            Guid BookingID = Guid.Parse("2978F4F2-00B7-44CD-9BA2-88AC4F6A1D17");

            BookingPriceInfo cancelFee = null;

            if (setting != null)
            {
                IBookingPrice ibp = new BookingPriceImpl(setting);
                getCancelReservationFees_Response price = ibp.getCancelReservationFees(BookingID.ToString(), setting.SessionID);

                if (!string.IsNullOrEmpty(price.PriceDetails))
                {
                    PrincingInfoFactory pif = new PrincingInfoFactory(price.PriceDetails);
                    pif.Process();
                    cancelFee = pif.Price;
                }

            }
        }

        #region Retrieve Pricing via ProxyBookingID

        [TestMethod]
        public void GetPriceUnitTestViaProxyBookingID()
        {

            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);
            int BookingID = 544;
            IProxyReservation proxy = new ProxyReservationImpl(setting);

            string priceStr = proxy.GetPrice(BookingID);

            Assert.IsTrue(priceStr != null, "should retrieve rough price");
        }

        [TestMethod]
        public void GetPriceDetailUnitTestViaProxyBookingID()
        {
            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);
            int BookingID = 36;
            IProxyReservation proxy = new ProxyReservationImpl(setting);

            BookingPriceInfo priceDetail = proxy.GetPriceDetail(BookingID);

            string jsonRes = SerializedHelper.JsonSerialize<BookingPriceInfo>(priceDetail);

            Assert.IsTrue(priceDetail != null, "should retrieve rough price");
        }

        [TestMethod]
        public void GetCancelReservationFeesUnitTestViaProxyBookingID()
        {
            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);
            int BookingID = 544;
            IProxyReservation proxy = new ProxyReservationImpl(setting);

            BookingPriceInfo priceDetail = proxy.GetCancelReservationFees(BookingID, setting.SessionID);

            Assert.IsTrue(priceDetail != null, "should retrieve rough price");
        }
        #endregion

        [TestMethod]
        public void ValidateRealPriceDetailUnitTest()
        {
            string pricingXml = @"<Price total='300' id='' timestamp='1435858188'><Rental total='300'><item type='business_hours' total='150'><item from='2015-07-02 17:00:00' to='2015-07-02 20:00:00' /></item><item type='night' from='2015-07-02 20:00:00' to='2015-07-02 20:45:00' total='150' /></Rental><InsuranceFee total='0' /><Fuel kilometer='0' total='0' /><Fine total='0' /></Price>";

            //string schemaPath = @"C:\svn Repo\10_Documentation\20_Input\10_Booking_Tool\Pricing_Response_XML_Definition.xsd";
            string schemaPath = @"C:\svn Repo\10_Documentation\20_Input\10_Booking_Tool\Pricing_Response_XML_Definition.xsd";

            PrincingInfoFactory factory = new PrincingInfoFactory(pricingXml);
            factory.Process();

            BookingPriceInfo bpi = factory.Price;
            string response = SerializedHelper.JsonSerialize<BookingPriceInfo>(bpi);

            Assert.IsTrue(response != null);

        }

        [TestMethod]
        public void ValidatePriceDetailUnitTest()
        {
            string priceDetail = @"<Price total='800' id='' timestamp='1401989684'><Rental total='0'></Rental><InsuranceFee total='0'/><Fuel kilometer='0' total='0'/><Fine total='800'><item type='cancel' description='cancel_book' total='800' /><item type='lost' description='lost_book' total='700' /></Fine></Price>";

            //string schemaPath = @"C:\svn Repo\10_Documentation\20_Input\10_Booking_Tool\Pricing_Response_XML_Definition.xsd";
            string schemaPath = @"C:\svn Repo\10_Documentation\20_Input\10_Booking_Tool\Pricing_Response_XML_Definition.xsd";

            PrincingInfoFactory factory = new PrincingInfoFactory(priceDetail);
            factory.Process();
        }

        [TestMethod]
        public void PriceDetailProductUnitTest()
        {
            //debug only

            string realPriceDetails = "<?xml version='1.0' encoding='UTF-8'?><Price pre-auth='4000.25' timestamp='' id='' total='2800.25'><Rental total='2100.25'><item type='business_hours' total='100'><period from='2013-10-10 18:30:00' to='2013-10-10 20:00:00'/><period from='2013-10-11 08:00:00' to='2013-10-11 08:30:00'/></item><item type='night' from='2013-10-10 18:00:00' to='2010-10-10 20:00:00' total='500'/><item type='holiday' from='2013-10-10 18:00:00' to='2010-10-10 20:00:00' total='1500'/><item type='weekend' from='2013-10-10 18:00:00' to='2010-10-10 20:00:00' total='0'/></Rental><InsuranceFee total='50'/><Fuel kilometer='20' total='50'/><Fine total='600'><item type='cancel' description='' total='0'/><item type='late_return' description='' total='500'/><item type='shorten' description='' total='100'/><item type='over_max_kilometer' description='' total='100'/></Fine></Price>";
            //string realPriceDetails = "&lt;?xml version='1.0' encoding='UTF-8'?&gt;&lt;Price pre-auth='4000.25' timestamp='' id='' total='2800.25'&gt;&lt;Rental total='2100.25'&gt;&lt;item type='business_hours' total='100'&gt;&lt;period from='2013-10-10 18:30:00' to='2013-10-10 20:00:00'/&gt;&lt;period from='2013-10-11 08:00:00' to='2013-10-11 08:30:00'/&gt;&lt;/item&gt;&lt;item type='night' from='2013-10-10 18:00:00' to='2010-10-10 20:00:00' total='500'/&gt;&lt;item type='holiday' from='2013-10-10 18:00:00' to='2010-10-10 20:00:00' total='1500'/&gt;&lt;item type='weekend' from='2013-10-10 18:00:00' to='2010-10-10 20:00:00' total='0'/&gt;&lt;/Rental&gt;&lt;InsuranceFee total='50'/&gt;&lt;Fuel kilometer='20' total='50'/&gt;&lt;Fine total='600'&gt;&lt;item type='cancel' description='' total=''/&gt;&lt;item type='late_return' description='' total='500'/&gt;&lt;item type='shorten' description='' total='100'/&gt;&lt;item type='over_max_kilometer' description='' total='100'/&gt;&lt;/Fine&gt;&lt;/Price&gt;";
            IPricingFactory factory = new PricingProcessor(realPriceDetails);
            factory.Process();

            BookingPriceInfo bpi = factory.Price;

            string json = SerializedHelper.JsonSerialize<BookingPriceInfo>(bpi);

        }


        [TestMethod]
        public void SeriazerPriceDetailIntoObjectsUnitTest1()
        {

            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);

            int BookingID = 572;
            IProxyReservation proxy = new ProxyReservationImpl(setting);
            string jsonRes = null;

            KemasReservationEntity bookingDetail = proxy.ProxyReservationDetail(setting.SessionID, BookingID, "english");

            IBookingPrice bmil = new BookingPriceImpl(setting);
            string priceDetail = bmil.GetPriceDetailed(bookingDetail.KemasBookingID);

            if (!string.IsNullOrEmpty(priceDetail))
            {
                PrincingInfoFactory factory = new PrincingInfoFactory(priceDetail);
                factory.Process();

                jsonRes = SerializedHelper.JsonSerialize<BookingPriceInfo>(factory.Price);
            }

            Assert.IsTrue(priceDetail != null, "should retrieve rough price");
        }

        #region checkPrice 2 API

        [TestMethod]
        public void CheckPrice2ForUpdate() 
        {
            KemasAuthencationAPI kemasAuth = new KemasAuthencationAPI();
            WS_Auth_Response auth = kemasAuth.authByLogin("jack70@abc.com", "123456");

            ProxyUserSetting userinfo = new ProxyUserSetting();
            userinfo.ID = auth.ID.ToString();
            userinfo.SessionID = auth.SessionID;


            Guid kemasBookingID = Guid.Parse("E0E79383-6DBD-4184-847D-CEF03CD77E68");

            IBookingPrice ibp = new BookingPriceImpl(userinfo);

            checkPrice2_RequestBookingData checkPriceInfo = new checkPrice2_RequestBookingData()
            {
                ID = "E0E79383-6DBD-4184-847D-CEF03CD77E68",
                DateBegin = "2015-10-13 11:30:00.00",
                DateEnd = "2015-10-13 11:45:00.00",
                Driver ="BEB892AB-0B13-4805-9B0B-38AE5DE22C09",
                StartLocation = "3178AFAE-4678-4707-BB54-5AE18997FFE2",
                BillingOption = 2,
                Category = "Eco"
            };

            checkPrice2_Response price = ibp.CheckPriceDetailed(auth.SessionID, checkPriceInfo);

            if (!string.IsNullOrEmpty(price.PriceDetails))
            {
                IPricingFactory factory = new PricingProcessor(price.PriceDetails);
                factory.Process();

                BookingPriceInfo bpi = factory.Price;
            }


        }
        #endregion

    }
}
