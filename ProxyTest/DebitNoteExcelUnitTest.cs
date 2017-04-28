using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using CF.VRent.Entities.AccountingService;
using System.Globalization;
using CF.VRent.Common;
using CF.VRent.BLL;
using CF.VRent.Entities.KEMASWSIF_AUTHRef;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.Common.UserContracts;
using System.IO;
using System.Text;
using System.Configuration;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ProxyTest
{

    [TestClass]
    public class DebitNoteExcelUnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            string detail =
            @"<?xml version='1.0' encoding='UTF-8'?>
                <Price pre-auth='4000.25' timestamp='' id='' total='2800.25'>
                    <Rental total='2100.25'>
                        <item type='business_hours' total='100'>
                            <period from='2013-10-10 18:30:00' to='2013-10-10 20:00:00'/>
                            <period from='2013-10-11 08:00:00' to='2013-10-11 08:30:00'/>
                        </item>
                        <item type='night' from='2013-10-10 18:00:00' to='2010-10-10 20:00:00' total='500'/>
                        <item type='holiday' from='2013-10-10 18:00:00' to='2010-10-10 20:00:00' total='1500'/>
                        <item type='weekend' from='2013-10-10 18:00:00' to='2010-10-10 20:00:00' total='0'/>
                    </Rental>
                    <InsuranceFee total='50'/>
                    <Fuel kilometer='20' total='50'/>
                    <Fine total='600'>
                        <item type='cancel' description='' total=''/>
                        <item type='late_return' description='' total='500'/>
                        <item type='shorten' description='' total='100'/>
                        <item type='over_max_kilometer' description='' total='100'/>
                    </Fine>
                </Price>";
            PricingItemMonthlysummary[] items = DebitNotePricingUtility.RetrieveCatalog(detail);

            Assert.IsTrue(items.Count() > 0);

        }

        [TestMethod]
        public void WorkingDayTestMethod()
        {
            DateTime current = DateTime.Now;
            DateTime interalDay = new DateTime(current.Year,current.Month,10).Date;
            
            int finalJobDay = 4;

            DateTime finalDay = DebitNoteUtility.CalculateDateForFinalJob(interalDay, finalJobDay);
        }

        #region Debit-Note Job
        [TestMethod]
        public void GenerateDebitNotesTestMethod()
        {
            WS_Auth_Response auth = KemasAdmin.SignOn();
            IAccountingService ias = new DataAccessProxyManager();

            ProxyUserSetting userInfo = new ProxyUserSetting()
            {
                ID = auth.ID,
                SessionID = auth.SessionID
            };

            IDebitNoteJob job = new DebitNoteJob();
            job.UserInfo = userInfo;
            int jobDay = 15;
            int finalJobDay = 20;
            job.GeneateDebitNotes(jobDay, finalJobDay);

            Dictionary<string, string> excelFiles = job.ExcelFiles;

            Assert.IsTrue(true);
        }
        #endregion

        #region Debit-Note On Demand
        [TestMethod]
        public void GenerateOnDemandTestMethod()
        {
            WS_Auth_Response auth = KemasAdmin.SignOn();
            IAccountingService ias = new DataAccessProxyManager();

            ProxyUserSetting userInfo = new ProxyUserSetting()
            {
                ID = auth.ID,
                SessionID = auth.SessionID
            };

            DebitNotePeriod[] periods = ias.RetrievePeriods(userInfo);

            if (periods != null && periods.Length > 0)
            {
                foreach (var period in periods)
                {
                    IDebitNoteJob job = new DebitNoteJob();
                    job.UserInfo = userInfo;
                    job.GenerateDebitNotesOnDemand(period);

                    Dictionary<string, string> excelFiles = job.ExcelFiles;
                }
            }

            Assert.IsTrue(true);
        }
        #endregion


        [TestMethod]
        public void GenerateExcelUnitTest() 
        {
            WS_Auth_Response auth = KemasAdmin.SignOn();
            ProxyUserSetting userInfo = new ProxyUserSetting()
            {
                ID = auth.ID,
                SessionID = auth.SessionID
            };

            IAccountingService ias = new DataAccessProxyManager();

            DebitNotePeriod[] periods = ias.RetrieveCompletedPeriods(userInfo);

            DebitNotePeriod targetPeriod = null;
            if (periods != null && periods.Length > 0)
            {
                targetPeriod = periods[0]; 
            }

            DebitNotesSearchConditions notesSearch = new DebitNotesSearchConditions();
            notesSearch.PeriodBegin = targetPeriod.PeriodStartDate;
            notesSearch.PeriodEnd = targetPeriod.PeriodEndDate;
            notesSearch.ItemsPerPage = 1000;
            notesSearch.PageNumber = 1;

            DebitNotesSearchConditions notesData = ias.RetrieveDebitNotesWithPaging(notesSearch, userInfo);

            if (notesData.Notes != null && notesData.Notes.Length > 0)
            {
                List<ExcelClient> kemasClients = DebitNoteUtility.LoadClientsFromKemas(userInfo);
                List<ExcelUser> kemasUsers = DebitNoteUtility.LoadUsersFromKemas(userInfo);

                if (ConfigurationManager.AppSettings["TemplateFolder"] != null)
                {
                    string _templateFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,ConfigurationManager.AppSettings["TemplateFolder"].ToString());
                    string _tempDataFolder = DebitNoteLoggingUtility.CreateTempDataFolder(_templateFolder);
                    ExcelDebitNote[] excelData = DebitNoteExcelDataUtility.Load(notesData.Notes, kemasUsers, kemasClients);

                    foreach (var excelNote in excelData)
                    {
                        DebitNoteExcelUtility noteUtility = new DebitNoteExcelUtility(excelNote, _tempDataFolder, userInfo.ID);
                        noteUtility.WriteAll(); 
                    }
                }
            }
        }

        [TestMethod]
        public void WriteExcelFromJsonData() 
        {
            string path = @"C:\CF-repo\vrent802\ProxyTest\bin\Debug\Template\Temp\e1c286c4-ae86-4c7d-810f-1b6357892f9f.json";
            string content = File.ReadAllText(path, Encoding.UTF8);

            ExcelDebitNote note = SerializedHelper.JsonDeserialize<ExcelDebitNote>(content);

            if (ConfigurationManager.AppSettings["TemplateFolder"] != null)
            {
                string _templateFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["TemplateFolder"].ToString());
                string _tempDataFolder = DebitNoteLoggingUtility.CreateTempDataFolder(_templateFolder);

                DebitNoteExcelUtility noteUtility = new DebitNoteExcelUtility(note, _tempDataFolder,null);

                noteUtility.WriteAll();
            }

        }

        [TestMethod]
        public void OpenExcelFile()
        {
            string path = @"C:\svn Repo\10_Documentation\20_Input\60_Payment_Admin_Portal\Debit_Note\VRent_Debit Note_Client Name_Corporate Template_MMM-YYYY.xlsx";

            if (File.Exists(path))
            {
                FileStream file = null;

                try
                {
                    file = new FileStream(path, FileMode.Open, FileAccess.Read);
                    IWorkbook workbook = WorkbookFactory.Create(file);

                    DirectoryInfo dir = new FileInfo(path).Directory;
                    string instanceName = Path.Combine(dir.FullName, "Sample.xlsx");
                    FileStream sw = new FileStream(instanceName, FileMode.Create);
                    workbook.Write(sw);
                    sw.Close();
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                }
                finally
                {
                    if (file != null)
                    {
                        file.Dispose();
                    }
                }
            }


        }

    }
}
