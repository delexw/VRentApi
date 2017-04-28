using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using CF.VRent.Common;

namespace ProxyTest
{
    /// <summary>
    /// Summary description for PricingUnitTest
    /// </summary>
    [TestClass]
    public class PricingUnitTest
    {
        public PricingUnitTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
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
        public void TestMethod()
        {
            //string pricingXml = @"<Price total='800' id='' timestamp='1401989684'><Rental total='0'></Rental><InsuranceFee total='0'/><Fuel kilometer='0' total='0'/><Fine total='800'><item type='cancel' description='cancel_book' total='800' /><item type='lost' description='lost_book' total='700' /></Fine></Price>";

            string pricingXml = @"<Price total='300' id='0012345' timestamp='1435858188'><Rental total='300'><item type='business_hours' total='150'><item from='2015-07-02 17:00:00' to='2015-07-02 20:00:00' /></item><item type='night' from='2015-07-02 20:00:00' to='2015-07-02 20:45:00' total='150' /></Rental><InsuranceFee total='100' /><Fuel kilometer='10' total='200' /><Fine total='20' /></Price>";
            string scheamPath = @"C:\CF-repo\vrent382\Proxy\Schema\Pricing_Response_XML_Definition.xsd";

            FileStream schemaStream = new FileStream(scheamPath,FileMode.Open);
            XmlReader schemareader = XmlReader.Create(schemaStream);
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(string.Empty,schemareader);
            
            XmlReaderSettings xmlSettings = new XmlReaderSettings();
            xmlSettings.Schemas = schemas;
            xmlSettings.ValidationType = ValidationType.Schema;
            xmlSettings.ValidationEventHandler += xmlSettings_ValidationEventHandler;
            xmlSettings.ValidationFlags =  
                XmlSchemaValidationFlags.ReportValidationWarnings;
            XmlSerializer xs = new XmlSerializer(typeof(Price));

            byte[] pricingStream = Encoding.UTF8.GetBytes(pricingXml);

            MemoryStream ms = new MemoryStream(pricingStream);
            XmlReader reader = XmlReader.Create(ms,xmlSettings);

            Price bookingPricing = xs.Deserialize(reader) as Price;

            Assert.IsTrue(bookingPricing!= null,"should be a valid pricing object");
        }

        private List<string> validationMsgs = new List<string>();

        void xmlSettings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Warning:
                    validationMsgs.Add(e.Message);
                    break;
                case XmlSeverityType.Error:
                    validationMsgs.Add(e.Message);
                    break;
                default:
                    break;
            }
        }

        [TestMethod]
        public void TestCurrentImplMethod()
        {
            //string pricingXml = @"<Price total='800' id='' timestamp='1401989684'><Rental total='0'></Rental><InsuranceFee total='0'/><Fuel kilometer='0' total='0'/><Fine total='800'><item type='cancel' description='cancel_book' total='800' /><item type='lost' description='lost_book' total='700' /></Fine></Price>";

            string pricingXml = @"<Price total='300' id='0012345' timestamp='1435858188'><Rental total='300'><item type='business_hours' total='150'><item from='2015-07-02 17:00:00' to='2015-07-02 20:00:00' /></item><item type='night' from='2015-07-02 20:00:00' to='2015-07-02 20:45:00' total='150' /></Rental><InsuranceFee total='100' /><Fuel kilometer='10' total='200' /><Fine total='20' /></Price>";
            //string scheamPath = @"C:\CF-repo\vrent382\Proxy\Schema\Pricing_Response_XML_Definition.xsd";

            //FileStream schemaStream = new FileStream(scheamPath, FileMode.Open);
            //XmlReader schemareader = XmlReader.Create(schemaStream);
            //XmlSchemaSet schemas = new XmlSchemaSet();
            //schemas.Add(string.Empty, schemareader);

            //XmlReaderSettings xmlSettings = new XmlReaderSettings();
            //xmlSettings.Schemas = schemas;
            //xmlSettings.ValidationType = ValidationType.Schema;
            //xmlSettings.ValidationEventHandler += xmlSettings_ValidationEventHandler;
            //xmlSettings.ValidationFlags =
            //    XmlSchemaValidationFlags.ReportValidationWarnings;
            //XmlSerializer xs = new XmlSerializer(typeof(Price));

            //byte[] pricingStream = Encoding.UTF8.GetBytes(pricingXml);

            //MemoryStream ms = new MemoryStream(pricingStream);
            //XmlReader reader = XmlReader.Create(ms, xmlSettings);

            //Price bookingPricing = xs.Deserialize(reader) as Price;

            IPricingFactory oldAPI = new PrincingInfoFactory(pricingXml);
            oldAPI.Process();
            BookingPriceInfo bpi = oldAPI.Price;

            string jsonResponse = SerializedHelper.JsonSerialize<BookingPriceInfo>(bpi);

            Assert.IsTrue(jsonResponse != null, "should be a valid pricing object");
        }

    }
}
