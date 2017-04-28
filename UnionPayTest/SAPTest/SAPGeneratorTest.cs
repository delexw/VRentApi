using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CF.VRent.Common;
using System.IO;
using CF.VRent.Entities.DataAccessProxyWrapper;
using System.Linq;
using UnionPayTest.TestHeaders;
using CF.VRent.BLL;
using CF.VRent.Email.EmailSender;
using CF.VRent.Email.EmailSender.Entity;
using System.Net.Mime;
using CF.VRent.Email;
using System.Threading;

namespace UnionPayTest.SAPTest
{
    [TestClass]
    public class SAPGeneratorTest
    {
        [TestMethod]
        public void ConnectToRemoteSharedFolder()
        {
            var utility = new RemoteConnUtility();
            string localpath = "X:";
            string remotepath = "\\\\172.21.216.21\\Test2";
            //int status = NetworkConnection.Connect(@"\\192.168.0.2\test", localpath, @"test", "test");
            int status = utility.Connect(remotepath, localpath, @"condali\al250545", "2jSpIoYV");
            if (status == (int)RemoteConnUtility.ERROR_ID.ERROR_SUCCESS)
            {
                FileStream fs = new FileStream(localpath + @"\\123.txt", FileMode.OpenOrCreate);
                using (StreamWriter stream = new StreamWriter(fs))
                {
                    stream.WriteLine("a testing content");
                    stream.Flush();
                    stream.Close();
                }
                fs.Close();
            }
            else
            {
                Console.WriteLine(status);
            }
            utility.Disconnect(localpath);

            Assert.AreEqual(status, RemoteConnUtility.ERROR_ID.ERROR_SUCCESS.GetValue());
        }

        [TestMethod]
        public void GetDUBStatistic()
        {
            var dataManager = new DataAccessProxyManager();

            var condi = new CF.VRent.Common.Entities.DBEntity.DBConditionObject();

            var start = new DateTime(2015, 9, 16);
            var end = start.AddDays(1).AddSeconds(-1);

            condi.RawWhereConditions.Add("DateFrom", start.ToString());
            condi.RawWhereConditions.Add("DateEnd", end.ToString());

            var data =  dataManager.GetDUBStatistic(condi);

            Assert.AreNotEqual(data.Entities.Count(), 0);
        }

        [TestMethod]
        public void GetCCBStatistic()
        {
            var dataManager = new DataAccessProxyManager();

            var condi = new CF.VRent.Common.Entities.DBEntity.DBConditionObject();

            var start = new DateTime(2015, 9, 1);
            var end = new DateTime(2015, 10, 1).AddMilliseconds(-1);

            condi.RawWhereConditions.Add("DateFrom", start.ToString());
            condi.RawWhereConditions.Add("DateEnd", end.ToString());

            var data = dataManager.GetCCBStatistic(condi);

            Assert.AreNotEqual(data.Entities.Count(), 0);
        }

        [TestMethod]
        public void AddGLHeader()
        {
            var dataManager = new DataAccessProxyManager();

            var id = dataManager.AddGeneralLedgerHeader(new CF.VRent.Entities.DataAccessProxy.GeneralLedgerHeader()
            {
                CreatedBy = TestHeader.User.ID.ToGuidNull(),
                PostingFrom = DateTime.Now.Date,
                PostingEnd = DateTime.Now
            });


            Assert.AreNotEqual(id, 0);
        }

        [TestMethod]
        public void AddGLItem()
        {
            var dataManager = new DataAccessProxyManager();

            var id = dataManager.AddGeneralLedgerItem(new CF.VRent.Entities.DataAccessProxy.GeneralLedgerItem() {
                CreatedBy = TestHeader.User.ID.ToGuidNull(),
                HeaderID = 100000000006,
                ItemType = CF.VRent.Common.Entities.VRentDataDictionay.GLItemType.Credit,
                PostingBody = "Test"
            });

            Assert.AreNotEqual(id, 0);
        }

        [TestMethod]
        public void AddGLItemDetail()
        {
            var dataManager = new DataAccessProxyManager();


            dataManager.AddGeneralLedgerItemDetails(new CF.VRent.Entities.DataAccessProxy.GeneralLedgerItemDetail() {
                CreatedBy = TestHeader.User.ID.ToGuidNull(),
                DetailType = CF.VRent.Common.Entities.VRentDataDictionay.GLItemDetailType.RentalFee,
                HeaderID = 100000000006,
                ItemID = 1,
                PaymentID = 1
            });

        }

        [TestMethod]
        public void StartDUBGeneralLedger()
        {
            var gl = new GeneralLedgerBLL(TestHeader.User);

            var start = new DateTime(2015, 9, 16);
            var end = start.AddDays(1).AddSeconds(-1);

            var headerId = gl.AddGeneralLedgerHeader(new CF.VRent.Entities.DataAccessProxy.GeneralLedgerHeader() { 
                CreatedBy = gl.UserInfo.ID.ToGuidNull(),
                PostingFrom = start,
                PostingEnd = end,
                HeaderType = CF.VRent.Common.Entities.VRentDataDictionay.GLHeaderType.DUB
            });

            var lines = gl.GenerateDUBLedger(headerId, start, end);

            Assert.AreNotEqual(lines.Count, 0);
        }

        [TestMethod]
        public void StartCCBGeneralLedger()
        {
            var gl = new GeneralLedgerBLL(TestHeader.User);

            var start = new DateTime(2015, 9, 1);
            var end = new DateTime(2015, 10, 1).AddSeconds(-1);

            var headerId = gl.AddGeneralLedgerHeader(new CF.VRent.Entities.DataAccessProxy.GeneralLedgerHeader()
            {
                CreatedBy = gl.UserInfo.ID.ToGuidNull(),
                PostingFrom = start,
                PostingEnd = end,
                HeaderType = CF.VRent.Common.Entities.VRentDataDictionay.GLHeaderType.CCB
            });

            var lines = gl.GenerateCCBLedger(headerId, start, end);

            Assert.AreNotEqual(lines.Count, 0);
        }

        [TestMethod]
        public void SendEmailWithAttach()
        {
            var dataManager = new BigFileServerManager();

            var emailSender = EmailSenderFactory.CreateDebitNoteCreatedSender();

            //Empty excel file
            using (var testStream = File.OpenRead(@"Temp\Test.xlsx"))
            {
                MemoryStream ms = new MemoryStream();

                //Send to vw internal user
                emailSender.onSendEvent += (arg1, arg2, arg3) =>
                {
                    CF.VRent.Entities.BigFileService.EmailProxyParameter proxyPara = new CF.VRent.Entities.BigFileService.EmailProxyParameter()
                    {
                        Attachment = new EmailAttachmentEntity()
                        {
                            CreatedDate = DateTime.Now,
                            FileName = "123.xlsx",
                            MimeType = MediaTypeNames.Application.Octet
                        },
                        ContentParameter = arg1,
                        EmailAddresses = arg3,
                        EmailType = arg2.ToStr(),
                        FileStream = testStream
                    };
                    dataManager.SendEmailWithAttachments(proxyPara);
                };
                emailSender.Send(new EmailParameterEntity(), "adam.liu@mcon-group.com");
            }

            Thread.Sleep(10000000);
        }
    }
}
