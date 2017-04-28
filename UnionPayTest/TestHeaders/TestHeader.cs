using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.UPSDK.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UnionPayTest.TestHeaders.AggregateEntity;
using CF.VRent.Common;
using System.Collections;
using CF.VRent.Common.UserContracts;

namespace UnionPayTest.TestHeaders
{
    public abstract  class TestHeader
    {
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

        /// <summary>
        /// User 
        /// </summary>
        private static ProxyUserSetting _user;
        public static ProxyUserSetting User
        {
            get
            {
                if (_user == null)
                {
                    _user = GetUserSettings.GetUser("adam", "123456");
                }
                return _user;
            }
            set
            {
                _user = value;
            }
        }

        /// <summary>
        /// Up Customer
        /// </summary>
        private static UnionPayCustomInfo _upCustomer;
        public static UnionPayCustomInfo UpCustomer
        {
            get
            {
                if (_upCustomer == null)
                {
                    _upCustomer = new UnionPayCustomInfo()
                    {
                        CardNo = "6221558812340000",
                        CertifId = "341126197709218366",
                        CertifTp = "01",
                        CustomerNm = "互联网",
                        SmsCode = "111111",
                        PhoneNo = "13552535506",
                        Cvn2 = "123",
                        Expired = "1711"
                    };
                }
                return _upCustomer;
            }
            set
            {
                _upCustomer = value;
            }
        }

        /// <summary>
        /// Union Parameters
        /// </summary>
        public static UnionPayParameters UnionPayParams { get; set; }

        public static UserLogin LoginParameters { get; set; }

        static TestHeader()
        {
            UnionPayParams = new UnionPayParameters();
            LoginParameters = new UserLogin();
        }

        /// <summary>
        /// Reflect properties and show properties' values
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fromLevel"></param>
        /// <param name="toLevel"></param>
        public void OutputMessage(object obj, int fromLevel = 1, int toLevel = 1)
        {
            if (obj != null)
            {
                if (fromLevel <= toLevel)
                {
                    this.TestContext.WriteLine("--------------" + obj.GetType().FullName + "--------------");
                    if (obj.GetType().BaseType == typeof(Array) || obj.GetType().GetInterfaces().Contains(typeof(IEnumerable)))
                    {
                        dynamic arrayObj = null;
                        if (obj.GetType().BaseType == typeof(Array))
                        {
                            arrayObj = obj as Array;
                        }
                        else if (obj.GetType().GetInterfaces().Contains(typeof(IEnumerable)))
                        {
                            arrayObj = obj as IEnumerable;
                        }

                        int i = 1;
                        foreach (object sub in arrayObj)
                        {
                            this.TestContext.WriteLine("    --------------" + obj.GetType().FullName + i + "--------------");
                            _outputOneObjectMessage(sub, fromLevel, toLevel);
                            ++i;
                        }
                    }
                    else
                    {
                        _outputOneObjectMessage(obj, fromLevel, toLevel);
                    }

                }
            }
            else
            {
                TestContext.WriteLine("--------------Object is null--------------");
            }
        }

        private void _outputOneObjectMessage(object obj, int fromLevel = 1, int toLevel = 1)
        {
            _showMessage(obj.GetType().Name, obj.ToStr());
            var pps = obj.GetType().GetProperties();
            foreach (PropertyInfo pi in pps)
            {
                //Exclude indexer
                if (pi.Name != "Item")
                {
                    _showMessage(pi.Name, pi.GetValue(obj).ToStr());
                    OutputMessage(pi.GetValue(obj), ++fromLevel, toLevel);
                    --fromLevel;
                }
            }
        }

        private void _showMessage(string key,string value)
        {
            if (key == "ErrorJSON")
            {
                value = value.JsonDeserialize<KemasError>().ToKeyValueString();
            }
            TestContext.WriteLine(@key + " : " + value);
        }
    }
}
