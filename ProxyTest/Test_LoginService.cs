using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.ServiceModel.Web;
using System.IO;

namespace ProxyTest
{
    [TestClass]
    public class Test_LoginService
    {
        /// <summary>
        /// Test the ping method
        /// If the service works fine return 200
        /// </summary>
        [TestMethod]
        public void TestPingMethod()
        {
            WebClient client = new WebClient();
            string actual = string.Empty;
            actual =  client.DownloadString(UnitTestConfiguration.GetHostName()+"/LoginService/Ping");

            string expected = "\"200\""; // TODO: Initialize to an appropriate value
            
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test the Login function 
        /// If the login successfully return true 
        /// </summary>
        [TestMethod]
        public void TestLoginMethod_CorrectUser()
        {
            using (WebClient client = new WebClient())
            {
                string actual = string.Empty;
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                string data = ProxyTest.DatReader.Read(@"C:\svn Repo\VrentGit\ProxyTest\TestData\LoginData_Corret.txt");

                try
                {
                    actual = client.UploadString("http://localhost/Proxy/LoginService/Login", "POST", data);
                    //actual = client.UploadString(UnitTestConfiguration.GetHostName() + "/LoginService/Login", "POST", data); 
                }
                catch (Exception ex)
                {
                    //need to check http error code
 
                }


                string expected = string.Empty; // TODO: Initialize to an appropriate value
                Assert.AreNotSame(expected, actual); 
            }


        }

        /// <summary>
        /// Test the Login function 
        /// Wrong user name and password
        /// </summary>
        [TestMethod]
        public void TestLoginMethod_WrongUser()
        {
            WebClient client = new WebClient();
            
            string actual = string.Empty;

            string data = ProxyTest.DatReader.Read("LoginData_Wrong.txt");
            client.Headers[HttpRequestHeader.ContentType] = "application/json";

            try
            {
                actual = client.UploadString(UnitTestConfiguration.GetHostName() + "/LoginService/Login", "POST", data);
            }
            catch (Exception ex)
            {
                HttpStatusCode stateCode = ((HttpWebResponse)((System.Net.WebException)ex).Response).StatusCode;
                //Check response state code
                Assert.AreEqual(stateCode, HttpStatusCode.Unauthorized);

                Stream dataStream = ((System.Net.WebException)ex).Response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                //check the return content
                Assert.AreEqual(responseFromServer, "\"Access Denied, user certificate not correct.\"");
            }
        }

        /// <summary>
        /// Test the Login function 
        /// Empty user and password
        /// </summary>
        [TestMethod]
        public void TestLoginMethod_EmptyUser()
        {
            WebClient client = new WebClient();

            string actual = string.Empty;

            string data = ProxyTest.DatReader.Read("LoginData_Empty.txt");
            client.Headers[HttpRequestHeader.ContentType] = "application/json";

            try
            {
                actual = client.UploadString(UnitTestConfiguration.GetHostName() + "/LoginService/Login", "POST", data);
            }
            catch (Exception ex)
            {
                HttpStatusCode stateCode = ((HttpWebResponse)((System.Net.WebException)ex).Response).StatusCode;
                //Check response state code
                Assert.AreEqual(stateCode, HttpStatusCode.Unauthorized);

                Stream dataStream = ((System.Net.WebException)ex).Response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                //check the return content
                Assert.AreEqual(responseFromServer, "\"Access Denied, user certificate not correct.\"");
            }
        }




        /// <summary>
        /// Test the Login function 
        /// If the logout successfully return true 
        /// </summary>
        [TestMethod]
        public void TestLogoutMethod()
        {
            WebClient client = new WebClient();
            string actual = string.Empty;
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            actual = client.DownloadString(UnitTestConfiguration.GetHostName() + "/LoginService/Logout");

            string expected = "true"; // TODO: Initialize to an appropriate value

            Assert.AreEqual(expected, actual);
        }
    }
}
