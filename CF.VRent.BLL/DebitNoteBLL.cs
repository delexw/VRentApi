using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities;
using CF.VRent.Entities.AccountingService;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.KEMASWSIF_AUTHRef;
using CF.VRent.Entities.KEMASWSIF_CONFIGRef;
using CF.VRent.Entities.KEMASWSIF_RESERVATIONRef;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using CF.VRent.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CF.VRent.BLL
{
    public class DebitNoteUtility
    {
        public const int DefaultItemsPerPage = 100;
        public const int DefaultPageNumber = 1;

        public const string DebitNoteDateTime = "yyyy-MM-dd";

        public static DateTime CalculateDateForFinalJob(DateTime interalDay, int nthWorkingDay)
        {
            //calculate 5th working day
            DateTime finalDay = interalDay;

            int workingDays = 0;

            while (workingDays < nthWorkingDay)
            {
                finalDay = finalDay.AddDays(1);

                if (finalDay.DayOfWeek == DayOfWeek.Saturday)
                {
                    continue;
                }
                else if (finalDay.DayOfWeek == DayOfWeek.Sunday)
                {
                    continue;
                }
                else
                {
                    workingDays++;
                }
            }
            return finalDay;
        }

        public static List<ExcelClient> LoadClientsFromKemas(ProxyUserSetting UserInfo)
        {
            List<ExcelClient> kemasClients = null;

            try
            {
                KemasConfigsAPI kemasConfig = new KemasConfigsAPI();

                getClientsResponse gcr = kemasConfig.getClients(UserInfo.SessionID);

                if (gcr.Error.ErrorCode.Equals("E0000") && gcr.Clients != null && gcr.Clients.Length > 0)
                {
                    kemasClients = gcr.Clients.Select(m =>
                        new ExcelClient()
                        {
                            ID = m.ID,
                            ClientName = m.Name,
                            ContractPerson = m.ContactPerson,
                            ContractInfo = m.ContactInfo
                        }
                        ).ToList();
                }
            }
            catch
            {

            }
            return kemasClients;
        }

        public static List<ExcelUser> LoadUsersFromKemas(ProxyUserSetting UserInfo)
        {
            //load ccb bookings occurred in that time range for all clients.
            List<ExcelUser> excelUsers = null;

            //load all users
            KemasUserAPI kemasUsers = new KemasUserAPI();
            getUsers2Request gu2r = new getUsers2Request();
            gu2r.SessionID = UserInfo.SessionID;
            getUsers2Response gu2res = kemasUsers.getUsers2(gu2r);

            if (gu2res.Error.ErrorCode.Equals("E0000") && gu2res.Users != null && gu2res.Users.Length > 0)
            {
                excelUsers = gu2res.Users.Select(m =>
                    new ExcelUser()
                    {
                        ID = m.ID,
                        ClientID = (m.Clients == null) ? null : m.Clients[0].ID,
                        UserFirstName = m.Name,
                        UserLastName = m.VName,
                        EmplyoeeNumber = m.PNo
                    }
                    )
                    .ToList();
            }

            return excelUsers;
        }

        public static CF.VRent.Entities.KEMASWSIF_RESERVATIONRef.Reservation[] LoadBookingsFromKemas(DateTime? beginDate, DateTime? endDate, Guid? userID, string userName, ProxyUserSetting userInfo)
        {
            findReservations2_Request fr2Req = new findReservations2_Request();
            if (userID.HasValue)
            {
                fr2Req.Driver = userID.Value.ToString();
            }
            //below is not implemented on kemas side
            //if (!string.IsNullOrEmpty(userName))
            //{
            //    fr2Req.Driver = userName;
            //}

            //if (beginDate.HasValue)
            //{
            //    fr2Req.BeginDate = beginDate.Value;
            //}

            //if (endDate.HasValue)
            //{
            //    fr2Req.EndDate = endDate.Value;
            //}

            fr2Req.States = ProxyReservationUtility.DoNotSyncStates();
            fr2Req.Language = "english";
            fr2Req.SessionID = userInfo.SessionID;

            KemasReservationAPI kemasReserve = new KemasReservationAPI();
            findReservations2_Response fr2Res = kemasReserve.findReservations2Kemas(fr2Req);

            if (fr2Res.Error.ErrorCode.Equals("E0000"))
            {
                return fr2Res.Reservations;
            }
            else
            {
                throw new VrentApplicationException(fr2Res.Error.ErrorCode, fr2Res.Error.ErrorMessage, ResultType.KEMAS);
            }
        }


        public static PaymentState TransformToDebitNoteState(string debitNoteState)
        {
            PaymentState state = (PaymentState)Enum.Parse(typeof(PaymentState), debitNoteState);
            return state;
        }

        public static DebitNotesSearchConditions GenerateDebitNotesSearchConditions(string clientID, string state, string beginDate, string endDate, string itemsPerPage, string pageNumber)
        {
            DebitNotesSearchConditions dnsc = new DebitNotesSearchConditions();
            if (!string.IsNullOrEmpty(clientID))
            {
                dnsc.ClientID = Guid.Parse(clientID);
            }

            if (!string.IsNullOrEmpty(state))
            {
                dnsc.Status = TransformToDebitNoteState(state);
            }
            else
            {
                dnsc.Status = new Nullable<PaymentState>();
            }

            if (!string.IsNullOrEmpty(beginDate))
            {
                dnsc.PeriodBegin = DateTime.ParseExact(beginDate, DebitNoteDateTime, null);
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                dnsc.PeriodEnd = DateTime.ParseExact(endDate, DebitNoteDateTime, null);
            }

            if (!string.IsNullOrEmpty(itemsPerPage))
            {
                dnsc.ItemsPerPage = Convert.ToInt32(itemsPerPage);
            }
            else
            {
                dnsc.ItemsPerPage = DefaultItemsPerPage;
            }

            if (!string.IsNullOrEmpty(pageNumber))
            {
                dnsc.PageNumber = Convert.ToInt32(pageNumber);
            }
            else
            {
                dnsc.PageNumber = DefaultPageNumber;
            }

            dnsc.QueryTime = DateTime.Now;

            dnsc.TotalPages = -1;

            return dnsc;
        }

        public static void SetPaymentDate(DebitNote note, ProxyUserSetting userInfo)
        {
            note.PaymentStatus = PaymentState.Paid;
            note.PaymentDate = DateTime.Now.Date;
        }

        private static DateTime ConvertDateTimeToYYYYMMDD(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day).Date;
        }

        public static void ConsolidateDateRange(DebitNoteDetailSearchConditions dndsc, DebitNotePeriod[] periods)
        {
            //periods must has at least one
            DateTime firstPeriod = ConvertDateTimeToYYYYMMDD(periods[0].PeriodStartDate);
            DateTime latestPeriod = ConvertDateTimeToYYYYMMDD(periods[periods.Length - 1].PeriodEndDate);
            if (dndsc.DateBegin.HasValue)
            {
                DateTime searchBegin = ConvertDateTimeToYYYYMMDD(dndsc.DateBegin.Value);

                if (searchBegin <= firstPeriod)
                {
                    dndsc.DateBegin = firstPeriod;
                }
                else
                {
                    dndsc.DateBegin = searchBegin;
                }
            }
            else
            {
                dndsc.DateBegin = firstPeriod;
            }

            if (dndsc.DateEnd.HasValue)
            {
                DateTime searchEnd = ConvertDateTimeToYYYYMMDD(dndsc.DateEnd.Value);

                if (searchEnd >= latestPeriod)
                {
                    dndsc.DateEnd = latestPeriod;
                }
                else
                {
                    dndsc.DateEnd = searchEnd;
                }
            }
            else
            {
                dndsc.DateEnd = latestPeriod;
            }

        }

    }

    public class DebitNoteLoggingUtility
    {
        public const string DebitNoteTitle = "Debit Note Job";
        public static string DebitNotePeriodFormat(DebitNotePeriod period)
        {
            return string.Format("{0}-{1}", period.Period, period.ID);
        }

        /// <summary>
        /// Dump data into log file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="name"></param>
        /// <param name="parameter"></param>
        /// <param name="tempFolder"></param>
        public static void DumpData<T>(T data, string name, string parameter, string tempFolder)
        {
            //IO operation could hit performance a little bit, in order to improve performace use task to do IO operation in additional thread
            //Modified by Adam
            Task.Factory.StartNew(() => {
                string fileName = null;
                if (string.IsNullOrEmpty(parameter))
                {
                    fileName = Path.Combine(tempFolder, name) + ".log";
                }
                else
                {
                    fileName = Path.Combine(tempFolder, name + "_" + parameter) + ".log";
                }
                string content = SerializedHelper.JsonSerialize<T>(data);

                File.WriteAllText(fileName, content);
            });
        }

        public static string CreateTempDataFolder(string templatefolder)
        {
            string _tempDataFolder = Path.Combine(templatefolder, "Temp");

            //folder used to keep temp data
            if (!Directory.Exists(_tempDataFolder))
            {
                Directory.CreateDirectory(_tempDataFolder);
            }
            DateTime current = DateTime.Now;
            string tempFolderName = string.Format("{0}{1}{2}_{3}{4}{5}", current.Year, current.Month, current.Day, current.Hour, current.Minute, current.Second);

            _tempDataFolder = Path.Combine(_tempDataFolder, tempFolderName);
            if (!Directory.Exists(_tempDataFolder))
            {
                Directory.CreateDirectory(_tempDataFolder);
            }

            return _tempDataFolder;

        }
    }

    public class PricingUtility
    {
        public const string SchemaRelativePath = "SchemaRelativePath";
        public static string RetrieveSchemaPath()
        {
            string folder = null;
            if (OperationContext.Current != null)
            {
                folder = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                //retrieve hosting environment
            }
            else
            {
                folder = AppDomain.CurrentDomain.BaseDirectory;
            }

            string schemaPath = ConfigurationManager.AppSettings[SchemaRelativePath];

            return Path.Combine(folder, schemaPath);
        }

        public static Price ProcessWithValidation(string schemaFullPath, string pricingXml)
        {
            Price rawPrice = null;

            FileStream schemaStream = null;
            XmlReader schemareader = null;

            XmlSerializer xs = null;

            XmlReaderSettings xmlSettings = null;

            MemoryStream ms = null;
            XmlReader reader = null;
            if (File.Exists(schemaFullPath))
            {
                try
                {
                    schemaStream = new FileStream(schemaFullPath, FileMode.Open);
                    schemareader = XmlReader.Create(schemaStream);
                    XmlSchemaSet schemas = new XmlSchemaSet();
                    schemas.Add(string.Empty, schemareader);

                    xmlSettings = new XmlReaderSettings();
                    xmlSettings.CloseInput = true;

                    xmlSettings.Schemas = schemas;
                    xmlSettings.ValidationType = ValidationType.Schema;

                    xmlSettings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings;

                    xs = new XmlSerializer(typeof(Price));
                    byte[] pricingStream = Encoding.UTF8.GetBytes(pricingXml);

                    ms = new MemoryStream(pricingStream);
                    reader = XmlReader.Create(ms, xmlSettings);

                    rawPrice = xs.Deserialize(reader) as Price;
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                        reader = null;
                    }

                    if (ms != null)
                    {
                        ms.Close();
                        ms.Dispose();
                        ms = null;
                    }

                    if (xmlSettings != null)
                    {
                        xmlSettings = null;
                    }

                    if (schemareader != null)
                    {
                        schemareader.Close();
                        schemareader = null;
                    }

                    if (schemaStream != null)
                    {
                        schemaStream.Close();
                        schemaStream.Dispose();
                        schemaStream = null;
                    }

                }
            }
            else
            {
                return null;
                //                throw new VrentApplicationException(ScheamFileNotExistCode, string.Format(ScheamFileNotExistMessage, _scheamFullPath), ResultType.VRENT);
            }
            return rawPrice;
        }

        public static DebitNoteDetail[] ConvertPriceToDebitNoteDetail(CompletedBooking rawBooking, Price rawPrice)
        {
            List<DebitNoteDetail> details = new List<DebitNoteDetail>();

            if (rawPrice.Fine != null && rawPrice.Fine.item != null)
            {
                foreach (var fineItem in rawPrice.Fine.item)
                {
                    DebitNoteDetail dnd = new DebitNoteDetail()
                    {
                        ClientID = Guid.Parse(rawBooking.CorporateID),
                        KemasBookingID = rawBooking.KemasBookingID,
                        KemasBookingNumber = rawBooking.KemasBookingNumber,
                        ItemCategory = "RENTALFEE",
                        TotalAmount = (decimal)fineItem.total
                    };

                    details.Add(dnd);
                }
            }

            return details.ToArray();
        }
    }

    public interface IDebitNote
    {
        DebitNotesSearchConditions RetrieveDebitNotes(DebitNotesSearchConditions conditions);
        DebitNote RetrieveDebitNotesByID(string debitNoteID);
        DebitNote UpdateDebitNotesByID(string debitNoteId, DebitNotePaymentState state);
        DebitNotePeriod[] RetrieveDebitNotePeriods();

        DebitNoteDetailSearchConditions RetrieveDebitNoteDetailsByID(DebitNoteDetailSearchConditions conditions);
    }

    public interface IDebitNoteJob
    {
        ProxyUserSetting UserInfo { get; set; }
        DebitNotePeriod TargetPeriod { get; set; }

        Dictionary<string, string> ExcelFiles { get; }

        bool Mode { get; set; }
        string TemplateFolder { get; set; }

        DebitNotePeriod RetrievePendingPeriods(int PreviweJobDay, int FinalJobDay, int DebitMonth);
        List<ExcelUser> LoadBookingsFromKemas(StagedBookings bookings);
        void SavingIntoStagingArea(StagedBookings bookings);
        void GeneateDebitNotes(int PreviweJobDay, int FinalJobDay, int DebitMonth);
        void GenerateDebitNotesOnDemand(DebitNotePeriod dnp);
    }

    public class DebitNoteJob : IDebitNoteJob
    {
        public ProxyUserSetting UserInfo { get; set; }
        public DebitNotePeriod TargetPeriod { get; set; }
        private static IVRentLog _debitNoteLog = LogInfor.DebitNoteLogWriter;

        private Dictionary<string, string> _excelFiles = null;

        private bool _isDebug = false;
        private string _templateFolder = null;
        private string _tempDataFolder = null;

        public DebitNoteJob()
        {
            _excelFiles = new Dictionary<string, string>();
            DebitNoteTotalPrice = new Dictionary<string, double>();
            if (ConfigurationManager.AppSettings["Mode"] != null)
            {
                _isDebug = ConfigurationManager.AppSettings["Mode"].ToString().Equals("0") ? false : true;
            }

            if (ConfigurationManager.AppSettings["TemplateFolder"] != null)
            {
                _templateFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["TemplateFolder"].ToString());
                _tempDataFolder = DebitNoteLoggingUtility.CreateTempDataFolder(_templateFolder);
            }
        }

        #region Run on Demand, lack of trigger
        public void GenerateDebitNotesOnDemand(DebitNotePeriod target)
        {
            if (target != null && (target.State == SyncedRecordState.NotRun || target.State == SyncedRecordState.Preview))
            {
                _debitNoteLog.WriteInfo(DebitNoteLoggingUtility.DebitNoteTitle, DebitNoteLoggingUtility.DebitNotePeriodFormat(target), UserInfo.ID);

                if (_isDebug)
                {
                    DebitNoteLoggingUtility.DumpData<DebitNotePeriod>(target, "JobPeriod", string.Empty, _tempDataFolder);
                }

                //clearing up temp data
                IAccountingService ias = new DataAccessProxyManager();
                ias.ClearUpTempDataByPeriod(target, UserInfo);


                StagedBookings completedBookings = new StagedBookings();
                completedBookings.BeginDate = target.PeriodStartDate;
                completedBookings.EndDate = target.PeriodEndDate;

                List<ExcelUser> kemasUsers = LoadBookingsFromKemas(completedBookings);

                if (completedBookings.Items != null && completedBookings.Items.Length > 0)
                {
                    SavingIntoStagingArea(completedBookings);
                }

                List<ExcelClient> kemasClients = DebitNoteUtility.LoadClientsFromKemas(UserInfo);

                ias.GeneateDebitNotes(target, UserInfo);

                DebitNotesSearchConditions notesSearch = new DebitNotesSearchConditions();
                notesSearch.PeriodBegin = target.PeriodStartDate;
                notesSearch.PeriodEnd = target.PeriodEndDate;
                notesSearch.ItemsPerPage = 1000;
                notesSearch.PageNumber = 1;

                _debitNoteLog.WriteInfo(DebitNoteLoggingUtility.DebitNoteTitle, "Begin Retrieve Pricing Catalog", UserInfo.ID);
                DebitNotesSearchConditions notesData = ias.RetrieveDebitNotesWithPaging(notesSearch, UserInfo);
                _debitNoteLog.WriteInfo(DebitNoteLoggingUtility.DebitNoteTitle, "End Retrieve Pricing Catalog", UserInfo.ID);

                if (notesData.Notes != null && notesData.Notes.Length > 0)
                {
                    _debitNoteLog.WriteInfo(DebitNoteLoggingUtility.DebitNoteTitle, "Begin Prepare Excel Data", UserInfo.ID);

                    //pass a delegate method for making excel
                    //time complexity is O(n) not O(2n)
                    //Modified by Adam
                    ExcelDebitNote[] excelData = DebitNoteExcelDataUtility.Load(notesData.Notes, kemasUsers, kemasClients, r => {

                        _debitNoteLog.WriteInfo(DebitNoteLoggingUtility.DebitNoteTitle, "Begin Write Excel", UserInfo.ID);
                        DebitNoteExcelUtility noteUtility = new DebitNoteExcelUtility(r, _tempDataFolder, UserInfo.ID);
                        noteUtility.WriteAll();
                        _excelFiles.Add(r.ClientID.ToString(), noteUtility.ExcelFullPath);
                        DebitNoteTotalPrice.Add(r.ClientID.ToString(), noteUtility.TotalPrice);

                        _debitNoteLog.WriteInfo(DebitNoteLoggingUtility.DebitNoteTitle, "End Write Excel", UserInfo.ID);

                    });
                    _debitNoteLog.WriteInfo(DebitNoteLoggingUtility.DebitNoteTitle, "End Prepare Excel Data", UserInfo.ID);

                    //foreach (var noteDetail in excelData)
                    //{
                    //    _debitNoteLog.WriteInfo(DebitNoteLoggingUtility.DebitNoteTitle, "Begin Write Excel", UserInfo.ID);
                    //    DebitNoteExcelUtility noteUtility = new DebitNoteExcelUtility(noteDetail, _tempDataFolder, UserInfo.ID);
                    //    noteUtility.WriteAll();

                    //    _excelFiles.Add(noteDetail.ClientID.ToString(), noteUtility.ExcelFullPath);

                    //    _debitNoteLog.WriteInfo(DebitNoteLoggingUtility.DebitNoteTitle, "End Write Excel", UserInfo.ID);
                    //}
                }
            }
        }
        #endregion

        #region monthly job
        /// <summary>
        /// Generate notes
        /// </summary>
        /// <param name="PreviweJobDay">the first date</param>
        /// <param name="FinalJobDay">the final date</param>
        /// <param name="DebitMonth">customized month</param>
        public void GeneateDebitNotes(int PreviweJobDay, int FinalJobDay, int DebitMonth)
        {
            string method = MethodInfo.GetCurrentMethod().Name;
            _debitNoteLog.WriteInfo(DebitNoteLoggingUtility.DebitNoteTitle, method, UserInfo.ID);

            if (UserInfo != null)
            {
                TargetPeriod = RetrievePendingPeriods(PreviweJobDay, FinalJobDay, DebitMonth);

                GenerateDebitNotesOnDemand(TargetPeriod);
            }

            _debitNoteLog.WriteInfo(DebitNoteLoggingUtility.DebitNoteTitle, method, UserInfo.ID);
        }
        #endregion

        #region Loading from Kemas
        public List<ExcelUser> LoadBookingsFromKemas(StagedBookings bookings)
        {
            string method = MethodInfo.GetCurrentMethod().Name;
            _debitNoteLog.WriteInfo(DebitNoteLoggingUtility.DebitNoteTitle, method, UserInfo.ID);

            List<ExcelUser> excelUsers = null;
            try
            {
                excelUsers = DebitNoteUtility.LoadUsersFromKemas(UserInfo);
            }
            catch (Exception ex)
            {
                //Added log by Adam
                _debitNoteLog.WriteError(DebitNoteLoggingUtility.DebitNoteTitle, ex.ToString(), UserInfo.ID);
            }

            if (_isDebug)
            {
                DebitNoteLoggingUtility.DumpData<List<ExcelUser>>(excelUsers, "Users", string.Empty, _tempDataFolder);
            }

            List<CompletedBooking> container = new List<CompletedBooking>(10000);
            try
            {
                //Use task.waitall doesn't make any sense for performance, use Parellel.ForEach instead
                //Modified by Adam 2015/11/24

                //Task<List<CompletedBooking>>[] tasks = new Task<List<CompletedBooking>>[excelUsers.Count];

                //for (int i = 0; i < tasks.Length; i++)
                //{
                //    string temp = excelUsers[i].ID;
                //    tasks[i] = Task.Factory.StartNew<List<CompletedBooking>>(() => LoadAllCCBBookingByUser(temp, bookings.BeginDate, bookings.EndDate));
                //}

                //Task.WaitAll(tasks);

                //for (int i = 0; i < tasks.Length; i++)
                //{
                //    if (tasks[i].IsCompleted && tasks[i].Result != null && tasks[i].Result.Count > 0)
                //    {
                //        container.AddRange(tasks[i].Result);
                //    }
                //}
                

                //debug code only
                //var bookingsByClient = bookings.Items.GroupBy(m => m.CorporateID);

                //Using parallel.forEach is better performance 
                Parallel.ForEach(excelUsers, u => {
                    var userCCBBookings = LoadAllCCBBookingByUser(u.ID, bookings.BeginDate, bookings.EndDate);
                    if (userCCBBookings != null && userCCBBookings.Count > 0)
                    {
                        container.AddRange(userCCBBookings);
                    }
                });
                bookings.Items = container.OrderByDescending(b => b.KemasBookingNumber).ToArray();
            }
            catch (AggregateException ae)
            {
                _debitNoteLog.WriteInfo(DebitNoteLoggingUtility.DebitNoteTitle, string.Format("{0}-{1}", ae.Message, ae.StackTrace), UserInfo.ID);
            }

            _debitNoteLog.WriteInfo(DebitNoteLoggingUtility.DebitNoteTitle, method, UserInfo.ID);

            return excelUsers;
        }

        public List<CompletedBooking> LoadAllCCBBookingByUser(string userID, DateTime beginDate, DateTime endTime)
        {
            List<CompletedBooking> stagedBookings = new List<CompletedBooking>();
            try
            {
                findReservations2_Request fr2Req = new findReservations2_Request();
                fr2Req.Driver = userID;
                fr2Req.States = ProxyReservationUtility.DoNotSyncStates();
                fr2Req.Language = "english";
                fr2Req.SessionID = UserInfo.SessionID;

                KemasReservationAPI kemasReserve = new KemasReservationAPI();
                findReservations2_Response fr2Res = kemasReserve.findReservations2Kemas(fr2Req);

                if (fr2Res.Error.ErrorCode.Equals("E0000") && fr2Res.Reservations != null && fr2Res.Reservations.Length > 0)
                {
                    if (_isDebug)
                    {
                        DebitNoteLoggingUtility.DumpData<CF.VRent.Entities.KEMASWSIF_RESERVATIONRef.Reservation[]>(fr2Res.Reservations, "KemasBookings", userID, _tempDataFolder);
                    }

                    //Loop all user's booking once
                    //time complexity is O(n)
                    //Modified by Adam
                    Parallel.ForEach(fr2Res.Reservations.OrderByDescending(r => r.DateBegin.ToDate()), kemasBooking =>
                    {
                        if (IsCandidate(kemasBooking, beginDate, endTime))
                        {
                            CompletedBooking comBooking = ConvertFromReservationToStagedBooking(kemasBooking, UserInfo);
                            if (comBooking != null)
                            {
                                stagedBookings.Add(comBooking);
                            }
                        }
                    });

                    if (_isDebug)
                    {
                        DebitNoteLoggingUtility.DumpData<List<CompletedBooking>>(stagedBookings, "ComBookings", userID, _tempDataFolder);
                    }

                    //IEnumerable<CF.VRent.Entities.KEMASWSIF_RESERVATIONRef.Reservation> qualifiedBookings
                    //    = fr2Res.Reservations
                    //    .Where(m =>
                    //    {
                    //        return IsCandidate(m, beginDate, endTime);
                    //    });

                    //if (qualifiedBookings != null && qualifiedBookings.Count() > 0)
                    //{
                    //    foreach (var kemasBooking in qualifiedBookings)
                    //    {
                    //        CompletedBooking comBooking = ConvertFromReservationToStagedBooking(kemasBooking, UserInfo);
                    //        if (comBooking != null)
                    //        {
                    //            stagedBookings.Add(comBooking);
                    //        }
                    //    }

                    //    if (_isDebug)
                    //    {
                    //        DebitNoteLoggingUtility.DumpData<List<CompletedBooking>>(stagedBookings, "ComBookings", userID, _tempDataFolder);
                    //    }
                    //}
                }

            }
            catch (AggregateException ae)
            {
                string message = ae.Message;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }

            return stagedBookings;
        }

        public bool IsCandidate(CF.VRent.Entities.KEMASWSIF_RESERVATIONRef.Reservation m, DateTime beginDate, DateTime endDate)
        {
            bool candidate = false;
            try
            {
                if (m.BillingOption.ID == 2 && decimal.Parse(m.Price) > 0)
                {
                    if (string.IsNullOrEmpty(m.KeyIn) || string.IsNullOrEmpty(m.KeyOut))
                    {
                        try
                        {
                            //bypass bad data
                            candidate = DateTime.ParseExact(m.DateBegin, DateTimeFormat, null) >= beginDate
                                && DateTime.ParseExact(m.DateEnd, DateTimeFormat, null) < endDate;
                        }
                        catch
                        {
                            candidate = DateTime.Parse(m.DateBegin) >= beginDate && DateTime.Parse(m.DateEnd) < endDate;
                        }
                    }
                    else
                    {
                        candidate = DateTime.ParseExact(m.KeyOut, DateTimeFormat, null) >= beginDate
                            && DateTime.ParseExact(m.KeyIn, DateTimeFormat, null) < endDate;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return candidate;
        }

        public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public DateTime? ConvertToDatetime(string datetime)
        {
            return string.IsNullOrEmpty(datetime) ? new Nullable<DateTime>() : DateTime.ParseExact(datetime, DateTimeFormat, CultureInfo.CurrentCulture); ;
        }

        public Decimal? ConvertToDecimal(string price)
        {
            return string.IsNullOrEmpty(price) ? new Nullable<Decimal>() : Decimal.Parse(price);
        }

        private CompletedBooking ConvertFromReservationToStagedBooking(CF.VRent.Entities.KEMASWSIF_RESERVATIONRef.Reservation kemasBooking, ProxyUserSetting userInfo)
        {
            CompletedBooking proxyBooking = null;
            try
            {
                proxyBooking = new CompletedBooking();
                proxyBooking.BillingOption = kemasBooking.BillingOption.ID;

                proxyBooking.CarID = string.IsNullOrEmpty(kemasBooking.CarID) ? new Nullable<Guid>() : Guid.Parse(kemasBooking.CarID);
                proxyBooking.CarName = kemasBooking.Car;
                proxyBooking.Category = kemasBooking.Category;
                proxyBooking.CompareResult = MatchState.Unknown;
                proxyBooking.CorporateID = kemasBooking.Driver.Clients[0].ID;
                proxyBooking.CorporateName = kemasBooking.Driver.Clients[0].Name;

                proxyBooking.CreatedOn = DateTime.Now;
                proxyBooking.CreatedBy = Guid.Parse(userInfo.ID);
                proxyBooking.CreatorID = Guid.Parse(kemasBooking.Creator.ID);
                proxyBooking.DateBegin = DateTime.Parse(kemasBooking.DateBegin);
                proxyBooking.DateEnd = DateTime.Parse(kemasBooking.DateEnd);
                proxyBooking.KemasBookingID = Guid.Parse(kemasBooking.ID);
                proxyBooking.KemasBookingNumber = kemasBooking.Number;
                proxyBooking.KeyIn = ConvertToDatetime(kemasBooking.KeyIn);
                proxyBooking.KeyOut = ConvertToDatetime(kemasBooking.KeyOut);
                proxyBooking.ModifiedBy = null;
                proxyBooking.ModifiedOn = null;

                proxyBooking.PaymentStatus = kemasBooking.PaymentStatus;
                proxyBooking.PickupBegin = ConvertToDatetime(kemasBooking.PickupBegin);
                proxyBooking.PickupEnd = ConvertToDatetime(kemasBooking.PickupEnd);
                proxyBooking.Price = ConvertToDecimal(kemasBooking.Price);
                proxyBooking.PricingDetail = kemasBooking.PriceDetail;
                proxyBooking.StartLocationID = Guid.Parse(kemasBooking.StartLocation.ID);
                proxyBooking.StartLocationName = kemasBooking.StartLocation.Name;
                proxyBooking.State = kemasBooking.State;
                proxyBooking.SyncState = StagingState.Created;
                proxyBooking.UserID = Guid.Parse(kemasBooking.Driver.ID);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                proxyBooking = null; // bad data
            }


            return proxyBooking;

        }

        #endregion

        public void SavingIntoStagingArea(StagedBookings bookings)
        {
            string method = MethodInfo.GetCurrentMethod().Name;
            _debitNoteLog.WriteInfo(DebitNoteLoggingUtility.DebitNoteTitle, method, UserInfo.ID);
            IAccountingService ias = new DataAccessProxyManager();
            ias.SaveIntoStagingAre(bookings, UserInfo);
            _debitNoteLog.WriteInfo(DebitNoteLoggingUtility.DebitNoteTitle, method, UserInfo.ID);
        }

        public const string PreviewJobDay = "PreviewJobDay";
        public const string FinalJobDay = "FinalJobDay";

        //the job only run for last month based on current date
        public SyncedRecordState DetermineJobDay(int PreviweJobDay, int FinalJobDay)
        {

            DateTime current = DateTime.Now;

            int previewDay = PreviweJobDay;
            int finalJobDay = FinalJobDay;

            //Change the two parameters value according to Schedule job running time

            //if (ConfigurationManager.AppSettings[PreviewJobDay] != null)
            //{
            //    previewDay = Convert.ToInt32(ConfigurationManager.AppSettings[PreviewJobDay].ToString());
            //}

            //if (ConfigurationManager.AppSettings[FinalJobDay] != null)
            //{
            //    //always 5th
            //    finalJobDay = Convert.ToInt32(ConfigurationManager.AppSettings[FinalJobDay].ToString());
            //}

            //alwasy run the job on the first day.
            DateTime interalDay = new DateTime(current.Year, current.Month, previewDay).Date;

            DateTime finalDay = DebitNoteUtility.CalculateDateForFinalJob(interalDay, finalJobDay);

            SyncedRecordState state = SyncedRecordState.UnKnown;
            if (current.Day == previewDay)
            {
                state = SyncedRecordState.NotRun;
            }
            else if (current.Day == finalDay.Day)
            {
                state = SyncedRecordState.Preview;
            }

            return state;
        }

        public DebitNotePeriod RetrievePendingPeriods(int PreviweJobDay, int FinalJobDay, int DebitMonth)
        {
            DebitNotePeriod dnp = null;

            string method = MethodInfo.GetCurrentMethod().Name;
            _debitNoteLog.WriteInfo(DebitNoteLoggingUtility.DebitNoteTitle, method, UserInfo.ID);

            SyncedRecordState state = DetermineJobDay(PreviweJobDay, FinalJobDay);
            if (state == SyncedRecordState.NotRun || state == SyncedRecordState.Preview)
            {
                IAccountingService ias = new DataAccessProxyManager();
                DebitNotePeriod[] pendingPeriods = ias.RetrievePeriodsByState(state, DebitMonth, UserInfo);

                //should be zero or one
                if (pendingPeriods != null && pendingPeriods.Length > 0)
                {
                    dnp = pendingPeriods[0];
                }
            }

            _debitNoteLog.WriteInfo(DebitNoteLoggingUtility.DebitNoteTitle, method, UserInfo.ID);

            return dnp;
        }

        #region Properties
        public bool Mode
        {
            get
            {
                return _isDebug;
            }
            set
            {
                _isDebug = value;
            }
        }
        public string TemplateFolder
        {
            get
            {
                return _templateFolder;
            }
            set
            {
                _templateFolder = value;
            }
        }
        #endregion


        public Dictionary<string, string> ExcelFiles
        {
            get
            {
                return _excelFiles;
            }
        }

        /// <summary>
        /// Get the set of total price for every debit note
        /// </summary>
        public Dictionary<string, double> DebitNoteTotalPrice
        {
            get;
            private set;
        }
    }

    public class DebitNoteBLL : AbstractBLL, IDebitNote
    {
        public DebitNoteBLL(ProxyUserSetting userInfo)
            : base(userInfo)
        {
        }

        public DebitNotesSearchConditions RetrieveDebitNotes(DebitNotesSearchConditions conditions)
        {
            IAccountingService ias = new DataAccessProxyManager();

            DebitNotesSearchConditions notesOutput = ias.RetrieveDebitNotesWithPaging(conditions, UserInfo);

            if (notesOutput.Notes != null && notesOutput.Notes.Length > 0)
            {
                WS_Auth_Response admin = KemasAdmin.SignOn();
                KemasConfigsAPI config = new KemasConfigsAPI();
                getClientsResponse configRes = config.getClients(admin.SessionID);

                if (configRes.Clients == null || configRes.Clients.Length == 0)
                {
                    throw new VrentApplicationException(
                        ErrorConstants.AdminRetrieveClientsFailCode,
                        string.Format(ErrorConstants.AdminRetrieveClientsFailMessage, admin.ID)
                        , ResultType.VRENT
                        );
                }
                else
                {
                    if (notesOutput.Notes != null)
                    {
                        for (int i = 0; i < notesOutput.Notes.Length; i++)
                        {
                            CF.VRent.Entities.KEMASWSIF_CONFIGRef.Client existing = configRes.Clients.FirstOrDefault(client => client.ID.Equals(notesOutput.Notes[i].ClientID.ToString()));
                            if (existing != null)
                            {
                                notesOutput.Notes[i].ClientName = existing.Name;
                                notesOutput.Notes[i].ContactPerson = existing.ContactPerson;
                            }
                        }
                    }
                }
            }

            return notesOutput;
        }

        public DebitNote RetrieveDebitNotesByID(string debitNoteID)
        {
            IAccountingService ias = new DataAccessProxyManager();

            int debitNote = Convert.ToInt32(debitNoteID);
            return ias.RetrieveDebitNotesByID(debitNote, DateTime.Now, UserInfo);
        }

        public DebitNote UpdateDebitNotesByID(string debitNoteID, DebitNotePaymentState state)
        {
            DebitNote updated = null;

            if (string.IsNullOrEmpty(debitNoteID) || !ErrorConstants.IsNumber(debitNoteID))
            {
                throw new VrentApplicationException(
                    ErrorConstants.BadDebitNoteDataCode,
                    string.Format(ErrorConstants.BadDebitNoteDataMessage, debitNoteID, state), ResultType.VRENTFE);
            }
            else
            {
                IAccountingService ias = new DataAccessProxyManager();

                DebitNote existing = ias.RetrieveDebitNotesByID(Convert.ToInt32(debitNoteID), DateTime.Now, UserInfo);

                if (existing != null)
                {
                    if (state.State == PaymentState.Paid)
                    {
                        DebitNoteUtility.SetPaymentDate(existing, UserInfo);
                    }
                    else
                    {
                        existing.PaymentStatus = state.State;
                    }

                    existing.ModifiedOn = DateTime.Now.Date;
                    existing.ModifiedBy = Guid.Parse(UserInfo.ID);
                    updated = ias.UpdateDebitNotesByID(existing, UserInfo);
                }
                else
                {
                    throw new VrentApplicationException(
                        ErrorConstants.BadDebitNoteDataCode,
                        string.Format(ErrorConstants.BadDebitNoteDataMessage, debitNoteID, state),
                        ResultType.VRENTFE);
                }
            }

            return updated;
        }

        public DebitNotePeriod[] RetrieveDebitNotePeriods()
        {
            IAccountingService ias = new DataAccessProxyManager();
            return ias.RetrievePeriods(UserInfo);
        }

        public DebitNoteDetailSearchConditions RetrieveDebitNoteDetailsByID(DebitNoteDetailSearchConditions conditions)
        {
            DebitNoteDetailSearchConditions output = null;

            IAccountingService ias = new DataAccessProxyManager();

            DebitNote note = ias.RetrieveDebitNotesByID(Convert.ToInt32(conditions.DebitNoteID), DateTime.Now, UserInfo);

            if (note != null)
            {
                DebitNotePeriod[] periods = ias.RetrieveCompletedPeriods(UserInfo);

                if (periods.Length > 0)
                {
                    DebitNoteUtility.ConsolidateDateRange(conditions, periods);

                    //consolidate into a full list
                    List<DebitNoteDetail> details = new List<DebitNoteDetail>();

                    #region client=side filtering
                    IEnumerable<CF.VRent.Entities.KEMASWSIF_RESERVATIONRef.Reservation> rawDetails = DebitNoteUtility.LoadBookingsFromKemas(conditions.DateBegin, conditions.DateEnd, conditions.UserID, conditions.UserName, UserInfo);
                    if (rawDetails != null && rawDetails.Count() > 0)
                    {
                        //debug only
                        //var allBookings = rawDetails.GroupBy(m=> m.Driver.ID);

                        rawDetails = rawDetails.Where(m => m.Driver.Clients[0].ID.Equals(note.ClientID.ToString()));

                        if (!string.IsNullOrEmpty(conditions.UserName))
                        {
                            rawDetails = rawDetails.Where(m => (m.Driver.Name.Contains(conditions.UserName) || m.Driver.VName.Contains(conditions.UserName)));
                        }

                        if (conditions.DateBegin.HasValue)
                        {
                            rawDetails = rawDetails.Where(m =>
                            {
                                if (!String.IsNullOrEmpty(m.KeyIn))
                                {
                                    return DateTime.ParseExact(m.KeyIn, "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture) >= conditions.DateBegin.Value ? true : false;
                                }
                                else
                                {
                                    return DateTime.ParseExact(m.DateEnd, "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture) >= conditions.DateBegin.Value ? true : false;
                                }
                            });
                        }

                        if (conditions.DateEnd.HasValue)
                        {
                            rawDetails = rawDetails.Where(m =>
                            {
                                if (!String.IsNullOrEmpty(m.KeyIn))
                                {
                                    return DateTime.ParseExact(m.KeyIn, "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture) <= conditions.DateEnd.Value ? true : false;
                                }
                                else
                                {
                                    return DateTime.ParseExact(m.DateEnd, "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture) <= conditions.DateEnd.Value ? true : false;
                                }
                            });
                        }

                        if (!string.IsNullOrEmpty(conditions.KemasBookingNumber))
                        {
                            rawDetails = rawDetails.Where(m => m.Number.Contains(conditions.KemasBookingNumber));
                        }

                        details = rawDetails.Select(m =>
                                    new DebitNoteDetail()
                                    {
                                        ClientID = Guid.Parse(m.Driver.Clients[0].ID),
                                        DebitNoteID = note.ID,
                                        KemasBookingID = Guid.Parse(m.ID),
                                        KemasBookingNumber = m.Number,
                                        ItemCategory = "RENTALFEE",
                                        OrderDate = string.IsNullOrEmpty(m.KeyIn) ?
                                            DateTime.ParseExact(m.DateEnd, "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture) :
                                            DateTime.ParseExact(m.KeyIn, "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture),
                                        PaymentStatus = note.PaymentStatus,
                                        TotalAmount = decimal.Parse(m.Price),
                                        UserID = Guid.Parse(m.Driver.ID),
                                        UserName = string.Format("{0} {1}", m.Driver.Name, m.Driver.VName)
                                    }
                            ).ToList();

                    }
                    #endregion

                    //retrieve all debit-note details that are of indirect-fee type.
                    DebitNoteDetailSearchConditions indirectFeeparts = ias.RetrieveDebitNoteDetailsInRange(conditions, UserInfo);

                    details.AddRange(indirectFeeparts.Items);

                    //sort all data by order date
                    DebitNoteDetail[] itemsInRange = details.OrderByDescending(m => m.KemasBookingNumber).ThenByDescending(m => m.OrderDate)
                        .Skip(conditions.ItemsPerPage * (conditions.PageNumber - 1)).Take(conditions.ItemsPerPage)
                        .Select
                        (m => new DebitNoteDetail()
                            {
                                ClientID = m.ClientID,
                                DebitNoteID = note.ID,
                                KemasBookingID = m.KemasBookingID,
                                KemasBookingNumber = m.KemasBookingNumber.TrimStart('0'),
                                ItemCategory = m.ItemCategory,
                                OrderDate = m.OrderDate,
                                PaymentStatus = m.PaymentStatus,
                                TotalAmount = m.TotalAmount,
                                UserID = m.UserID,
                                UserName = m.UserName,
                                BookingID = m.BookingID,
                                OrderID = m.OrderID,
                            }
                        ).ToArray();


                    BookingCompact[] kemasIDs = itemsInRange.Where(m => m.BookingID <= 0)
                        .GroupBy(m => m.KemasBookingID).
                        Select(m => new BookingCompact()
                        {
                            KemasBookingID = m.Key
                        }).ToArray();

                    if (kemasIDs.Length > 0)
                    {
                        BookingCompact[] bcOutput = ias.RetrieveID(kemasIDs, UserInfo);
                        foreach (var item in itemsInRange)
                        {
                            BookingCompact exist = bcOutput.FirstOrDefault(m => m.KemasBookingID.Equals(item.KemasBookingID));
                            if (exist != null && exist.BookingID.HasValue)
                            {
                                item.BookingID = exist.BookingID.Value;
                                if (exist.OrderID.HasValue)
                                {
                                    item.OrderID = exist.OrderID;
                                }
                            }
                        }
                    }


                    output = new DebitNoteDetailSearchConditions()
                    {
                        DateBegin = conditions.DateBegin,
                        DateEnd = conditions.DateEnd,
                        DebitNoteID = conditions.DebitNoteID,
                        Items = itemsInRange,
                        ItemsPerPage = conditions.ItemsPerPage,
                        PageNumber = conditions.PageNumber,
                        KemasBookingNumber = conditions.KemasBookingNumber,
                        UserID = conditions.UserID,
                        UserName = conditions.UserName,
                        TotalPage = (details.Count % conditions.ItemsPerPage == 0) ? details.Count / conditions.ItemsPerPage : details.Count / conditions.ItemsPerPage + 1
                    };
                }
                else
                {
                    throw new VrentApplicationException(ErrorConstants.NoDebitNoteDataCode, ErrorConstants.NoDebitNoteDataMessage, ResultType.VRENT);
                }
            }
            else
            {
                throw new VrentApplicationException(ErrorConstants.DebitNoteNotExistCode, string.Format(ErrorConstants.DebitNoteNotExistMessage, conditions.DebitNoteID), ResultType.VRENT);
            }

            return output;
        }
    }
}