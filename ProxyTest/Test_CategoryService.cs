using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using CF.VRent.Common;
using CF.VRent.Entities;
using System.IO;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.KEMASWSIF_AUTHRef;

namespace ProxyTest
{
    public class ClientAdapter
    {
        public static string GetAPiResult(string baseAddress, string apiURl) 
        {
            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/json";

            return client.DownloadString(new Uri( baseAddress + apiURl));
        }

        public static string GetAPiResult(string JsonPath, string JsonFile, string baseAddress, string apiURl,string method)
        {
            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/json";

            string fullPath = Path.Combine(JsonPath, JsonFile);
            string data = ProxyTest.DatReader.Read(fullPath);

            return client.UploadString(new Uri(baseAddress + apiURl),method,data);
        }
    }


    /// <summary>
    /// Summary description for CategoryService
    /// </summary>
    [TestClass]
    public class CategoryService
    {

        private WebClient client;
        private static string basePath = @"C:\Users\dl250349\Desktop\VRentServiceProxy\Proxy\ProxyTest\TestData\";

        public CategoryService()
        {
            //
            // TODO: Add constructor logic here
            //
        }


        [TestMethod]
        public void TestAllCategories()
        {
            //
            // TODO: Add test logic here
            //
            string actual = string.Empty;
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            actual = client.DownloadString(UnitTestConfiguration.GetHostName() + "/DataService/Categories");
            string expected = string.Empty;
            //check return is not empty
            Assert.AreNotEqual(expected, actual);

            //check return data is the correct fromat
            List<ProxyCategory> list = SerializedHelper.JsonDeserialize<List<ProxyCategory>>(actual);
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void TestTypeOfJourney()
        {
            //
            // TODO: Add test logic here
            //
            string actual = string.Empty;
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            actual = client.DownloadString(UnitTestConfiguration.GetHostName() + "/DataService/TypeOfJourney?Uid=0de53307-00b3-47d9-9744-4916637246d4&lang=english");
            string expected = string.Empty;
            //check return is not empty
            Assert.AreNotEqual(expected, actual);

            //check return data is the correct fromat
            List<ProxyJourneyType> list = SerializedHelper.JsonDeserialize<List<ProxyJourneyType>>(actual);
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void GetAllCategories()
        {
            IKemasOptionsAPI kemasOption = new KemasOptionsAPI();
            var categories = kemasOption.getCategories().Select(m => new ProxyCategory { key = m.key, value = m.value }).ToList();

            Assert.IsTrue(categories != null);
        }

        [TestMethod]
        public void GetAllJourneyTypes()
        {
            IKemasOptionsAPI kemasOption = new KemasOptionsAPI();
            var billingOptions = kemasOption.getUserTypeOfJourney(Test_ReservationService.cfUserID.ToString(), "english").Select(m => new ProxyJourneyType { Key = m.key, Value = m.value }).ToList();
            Assert.IsTrue(billingOptions != null);
        }

        [TestMethod]
        public void GetAllLocation()
        {

            WS_Auth_Response auth = AuthencationUnitTestKemasAPI.SignOn();
            IKemasReservation kemasOption = new KemasReservationAPI();
            var locations = kemasOption.getLocations(auth.ID).Select(m => new ProxyLocation { ID = m.id, Text = m.text }).ToList();
            Assert.IsTrue(locations != null);
        }
    }
}
