using CF.VRent.Common;
using CF.VRent.DAL;
using CF.VRent.DataAccessProxy.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CF.VRent.DataAccessProxyTest
{
    [TestClass]
    public class ReservationDALUnitTest
    {
        public const string SqlDevConnstr = "server=172.21.216.21;database=VRentBooking_Dev;uid=sa;pwd=H2yAts77";

        [TestMethod]
        public void UpdateBookingPaymentUnitTest()
        {
            ProxyBookingPayment pbp = new ProxyBookingPayment();
            pbp.ProxyBookingID = 91;
            pbp.UPPaymentID = 300;
            pbp.State = 1;
            pbp.CreatedOn = DateTime.Now;
            pbp.CreatedBy = Guid.NewGuid();
            try
            {
                ReservationDAL.UpdateUpPaymentForBooking(pbp);
            }
            catch (SqlException sqle)
            {
                string message = sqle.Message;
            }

            Assert.IsTrue(true);
        }


        [TestMethod]
        public void CreateProxyBookingTestWithPricingPayment()
        {
            //SQLHelper.ConnectionStringDefault = SqlDevConnstr;

            ProxyReservation reservation = new ProxyReservation();
            ProxyReservation reservationDB = new ProxyReservation();


            reservation.BillingOption = 1; 
            reservation.KemasBookingID = Guid.NewGuid();
            reservation.KemasBookingNumber = "0001111";

            reservation.DateBegin = DateTime.Now;
            reservation.DateEnd = DateTime.Now;
            reservation.TotalAmount = Convert.ToDecimal(5.7);

            reservation.UserID = Guid.Parse(testUserID);
            reservation.UserFirstName = "Tom";
            reservation.UserLastName = "Mike";

            reservation.CorporateID = "12345";
            reservation.CorporateName = "54321";

            reservation.CreatorID = Guid.Parse(testUserID);
            reservation.CreatorFirstName = "Make";
            reservation.CreatorLastName = "Tom";

            reservation.StartLocationID = Guid.NewGuid();
            reservation.StartLocationName = "KeyBox 1";

            
            reservation.State = "swBookingModel/released";
            reservation.CreatedOn = DateTime.Now;
            reservation.CreatedBy = Guid.NewGuid();
            reservation.ModifiedOn = null;
            reservation.ModifiedBy = null;

            reservation.PricingDetail = "<Price total=\"800.75\" id=\"\" timestamp=\"1401989684\"><Rental total=\"0\" /><InsuranceFee total=\"0\" /><Fuel kilometer=\"0\" total=\"0\" /><Fine total=\"80.69\"><item type=\"cancel\" description=\"cancel_book\" total=\"5.78\" /></Fine></Price>";

            ProxyBookingPayment payment = new ProxyBookingPayment()
            {
                UPPaymentID = 0,
                ProxyBookingID = -1,
                State = 0,
                CreatedOn = reservation.CreatedOn,
                CreatedBy = reservation.CreatedBy,
                ModifiedOn = reservation.ModifiedOn,
                ModifiedBy = reservation.ModifiedBy
            };
            try
            {
                //reservationDB = ReservationDAL.CreateProxyReservationWithPaymentInfo(reservation, payment);
            }
            catch (SqlException sqle)
            {
                string message = sqle.Message;
            }
        }

        public string testUserID = "1c9d9c82-d074-45a4-863e-e7eeb2384c64";

        [TestMethod]
        public void UpdateProxyBookingTestWithPricingPayment()
        {
            //SQLHelper.ConnectionStringDefault = SqlDevConnstr;

            ProxyReservation reservation = new ProxyReservation();
            ProxyReservation reservationDB = new ProxyReservation();

            reservation.ProxyBookingID = 10;
            reservation.BillingOption = 1;
            reservation.KemasBookingID = Guid.NewGuid();
            reservation.KemasBookingNumber = "0001111";

            reservation.DateBegin = DateTime.Now;
            reservation.DateEnd = DateTime.Now;
            reservation.TotalAmount = Convert.ToDecimal(5.7);

            reservation.UserID = Guid.Parse(testUserID);
            reservation.UserFirstName = "Tom";
            reservation.UserLastName = "Mike";

            reservation.CorporateID = "12345";
            reservation.CorporateName = "54321";

            reservation.CreatorID = Guid.Parse(testUserID);
            reservation.CreatorFirstName = "Make";
            reservation.CreatorLastName = "Tom";

            reservation.StartLocationID = Guid.NewGuid();
            reservation.StartLocationName = "KeyBox 1";


            reservation.State = "swBookingModel/released";
            reservation.CreatedOn = DateTime.Now;
            reservation.CreatedBy = Guid.NewGuid();
            reservation.ModifiedOn = DateTime.Now;
            reservation.ModifiedBy = Guid.NewGuid();

            reservation.PricingDetail = "<Price total=\"800.75\" id=\"\" timestamp=\"1401989684\"><Rental total=\"0\" /><InsuranceFee total=\"0\" /><Fuel kilometer=\"0\" total=\"0\" /><Fine total=\"80.69\"><item type=\"cancel\" description=\"cancel_book\" total=\"5.78\" /></Fine></Price>";
            try
            {
                //reservationDB = ReservationDAL.UpdateProxyBookingWithPricingInfo(reservation);
            }
            catch (SqlException sqle)
            {
                string message = sqle.Message;
            }

            Assert.IsTrue(reservationDB != null);
        }

        [TestMethod]
        public void CancelProxyBookingTestWithNewPricing()
        {
            ProxyReservation reservation = new ProxyReservation();
            ProxyReservation reservationDB = new ProxyReservation();


            reservation.ProxyBookingID = 9;
            reservation.TotalAmount = 10.56m;

            reservation.State = "swBookingModel/canceled";
            reservation.CreatedOn = DateTime.Now;
            reservation.CreatedBy = Guid.NewGuid();
            reservation.ModifiedOn = DateTime.Now;
            reservation.ModifiedBy = Guid.NewGuid();

            reservation.PricingDetail = "<Price total=\"1000.78\" id=\"\" timestamp=\"1401989684\"><Rental total=\"0\" /><InsuranceFee total=\"0\" /><Fuel kilometer=\"0\" total=\"0\" /><Fine total=\"10.50\"><item type=\"cancel\" description=\"cancel_book\" total=\"10.45\" /></Fine></Price>";
            try
            {
                //reservationDB = ReservationDAL.CancelProxyReservation(reservation);
            }
            catch (SqlException sqle)
            {
                string message = sqle.Message;
            }
        }

        [TestMethod]
        public void CancelProxyBookingTestWithExistingPricing()
        {
            ProxyReservation reservation = new ProxyReservation();
            ProxyReservation reservationDB = new ProxyReservation();

            reservation.ProxyBookingID = 3;
            reservation.TotalAmount = Convert.ToDecimal(800);

            reservation.State = "swBookingModel/canceled";
            reservation.CreatedOn = DateTime.Now;
            reservation.CreatedBy = Guid.NewGuid();
            reservation.ModifiedOn = DateTime.Now;
            reservation.ModifiedBy = Guid.NewGuid();

            reservation.PricingDetail = "<Price total=\"800\" id=\"\" timestamp=\"1401989684\"><Rental total=\"0\" /><InsuranceFee total=\"0\" /><Fuel kilometer=\"0\" total=\"0\" /><Fine total=\"800\"><item type=\"cancel\" description=\"cancel_book\" total=\"800\" /></Fine></Price>";
            try
            {
                //reservationDB = ReservationDAL.CancelProxyReservation(reservation);
            }
            catch (SqlException sqle)
            {
                string message = sqle.Message;
            }
        }

        [TestMethod]
        public void RetrieveMyBookingsUnitTest()
        {
            ProxyReservation[] bookings = null;
            try
            {
                bookings = ReservationDAL.RetrieveReservations(Guid.Parse(testUserID.ToString()), new string[3] { "swBookingModel/created", "swBookingModel/released", "swBookingModel/canceled" });
            }
            catch (SqlException sqle)
            {
                string message = sqle.Message;
            }

            Assert.IsTrue(bookings.Length > 0, "");


        }

        #region Paging Test
        [TestMethod]
        public void RetrieveMyBookingsWithPagingUnitTest()
        {
            ProxyReservationsWithPaging bookings = null;

            List<string> whereConditions = new List<string>();
            string fuzzyNameCondition = "Name = Wang";
            string kemasbookingIDCondition = "State = released";
            whereConditions.Add(fuzzyNameCondition);
            whereConditions.Add(kemasbookingIDCondition);

            List<string> orderByConditions = new List<string>();
            string idCondition = "PROXYBOOKINGID DESC";
            string dateBeginCondition = "DateBegin asc";
            orderByConditions.Add(idCondition);
            orderByConditions.Add(dateBeginCondition);

            int itemsPerPage = 5;
            int pageNumber = 1;
            ProxyReservationsWithPaging pp = new ProxyReservationsWithPaging()
            {
                RawWhereConditions = whereConditions.Select(m => m.ToUpper()).ToArray(),
                RawOrderByConditions = orderByConditions.Select(m => m.ToUpper()).ToArray(),
                ItemsPerPage = itemsPerPage,
                PageNumber = pageNumber
            };
            try
            {
                bookings = ReservationDAL.RetrieveReservationsWithPaging(pp);
            }
            catch (SqlException sqle)
            {
                string message = sqle.Message;
            }
            Assert.IsTrue(bookings.Reservations != null && bookings.Reservations.Length > 0, "");
        }

        [TestMethod]
        public void RetrieveMyBookingsWithNoWherePagingUnitTest()
        {
            ProxyReservationsWithPaging bookings = null;

            List<string> whereConditions = new List<string>();
            //string bookingTypeCondition = "BillingOption = 2";
            //string kemasbookingIDCondition = "KemasBookingID != "+Guid.NewGuid();
            //whereConditions.Add(bookingTypeCondition);
            //whereConditions.Add(kemasbookingIDCondition);

            List<string> orderByConditions = new List<string>();
            string idCondition = "PROXYBOOKINGID DESC";
            string dateBeginCondition = "DateBegin asc";
            orderByConditions.Add(idCondition);
            orderByConditions.Add(dateBeginCondition);

            int itemsPerPage = 5;
            int pageNumber = 500;
            ProxyReservationsWithPaging pp = new ProxyReservationsWithPaging()
            {
                RawWhereConditions = whereConditions.Select(m => m.ToUpper()).ToArray(),
                RawOrderByConditions = orderByConditions.Select(m => m.ToUpper()).ToArray(),
                ItemsPerPage = itemsPerPage,
                PageNumber = pageNumber
            };
            try
            {
                bookings = ReservationDAL.RetrieveReservationsWithPaging(pp);
            }
            catch (SqlException sqle)
            {
                string message = sqle.Message;
            }
            Assert.IsTrue(bookings.Reservations != null, "");
        }

        #endregion

        [TestMethod]
        public void RetrieveMyBookingsWithStatesUnitTest()
        {
            ProxyReservation[] bookings = null;
            try
            {
                bookings = ReservationDAL.RetrieveReservations(Guid.Parse(testUserID.ToString()), new string[1] { "swBookingModel/released" });
            }
            catch (SqlException sqle)
            {
                string message = sqle.Message;
            }

            Assert.IsTrue(bookings.Length > 0, "");
        }

        [TestMethod]
        public void RetrieveBookingByBookingIDWithStatesUnitTest()
        {
            ProxyReservation booking = null;
            try
            {
                booking = ReservationDAL.RetrieveReservationDetailByID(12);
            }
            catch (SqlException sqle)
            {
                string message = sqle.Message;
            }

            Assert.IsTrue(booking != null, "");
        }

        #region Create Data File

        [TestMethod]
        public void WriteDataFile() 
        {
            DataWriter dw = new DataWriter(1000000);
            dw.WriteFile();

            DateTime importStart = DateTime.Now;
            DataImport.Import(dw.DataFilePath);
            DateTime importEnd = DateTime.Now;
            TimeSpan elapsed = importEnd - importStart;
        }


        #endregion
    }

    #region helper class
    public class DataWriter
    {
        private int _count;
        private string _dataFilePath;

        public string DataFilePath
        {
            get { return _dataFilePath; }
        }

        public DataWriter(int count)
        {
            _count = count;
        }

        private void CreateDatafile()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            _dataFilePath = Path.Combine(path, "DataFile.txt");

            if (File.Exists(_dataFilePath))
            {
                File.Delete(_dataFilePath);
            }
            using (FileStream fs = File.Create(_dataFilePath))
            {

            }
        }



        public void WriteFile()
        {
            CreateDatafile();



            List<Guid> users = new List<Guid>();
            for (int i = 0; i < 1000; i++)
            {
                users.Add(Guid.NewGuid());
            }

            List<Guid> companies = new List<Guid>();
            for (int i = 0; i < 100; i++)
            {
                companies.Add(Guid.NewGuid());
            }

            List<int> billingOptions = new List<int>();
            for (int i = 0; i < 2; i++)
            {
                billingOptions.Add(i + 2);
            }

            DateTime begin = DateTime.Now.AddYears(-1);
            DateTime end = DateTime.Now;
            double mGap = (end-begin).TotalMinutes;

            List<int> durations = new List<int>();
            for (int i = 1; i <= 12; i++)
            {
                durations.Add(i);
            }


            string BillingOption;
            string KemasBookingID;
            string KemasBookingNumber;
            string DateBegin;
            string DateEnd;
            string TotalAmount;
            string UserID;
            string CorporateID = null;
            string FaPiaoPreferenceID = null;
            string FaPiaoRequestType;
            string State = null;
            string CreatedOn;
            string CreatedBy;

            DateTime current = DateTime.Now;
            int batchSize = 100;
            string[] contents = new string[batchSize];

            int count = 1;

            for (int i = 0; i < _count; i++)
            {
                Random r = new Random(Environment.TickCount);
                //[ID] [int] IDENTITY(1,1) NOT NULL,
                //[BookingType] [tinyint] NOT NULL,
                //[KemasBookingID] [uniqueidentifier] NOT NULL,
                //[KemasBookingNumber] [nvarchar](20) NOT NULL,
                //[DateBegin] [datetime] NOT NULL,
                //[DateEnd] [datetime] NOT NULL,
                //[TotalAmount] [decimal](10, 3) NOT NULL,
                //[UserID] [uniqueidentifier] NOT NULL,
                //[CorporateID] [nvarchar](50) NULL,
                //[FapiaoPreferenceID] [uniqueidentifier] NULL,
                //[IsFapiaoRequested] [tinyint] NULL,
                //[State] [nvarchar](50) NOT NULL,
                //[CreatedOn] [datetime] NOT NULL,
                //[CreatedBy] [uniqueidentifier] NOT NULL,
                //[ModifiedOn] [datetime] NULL,
                //[ModifiedBy] [uniqueidentifier] NULL,
                BillingOption = billingOptions[r.Next(0, 1)].ToString();
                KemasBookingID = Guid.NewGuid().ToString();
                KemasBookingNumber = string.Format("{0}", i).PadLeft(10,'0');
                DateTime DateBeginStr = begin.AddMinutes(r.Next(0, (int)mGap));
                DateBegin = DateBeginStr.ToString();
                DateEnd = DateBeginStr.AddHours(durations[r.Next(0, durations.Count - 1)]).ToString();

                TotalAmount = r.Next(1, 10000).ToString();
                UserID = users[r.Next(0, users.Count-1)].ToString();
                CorporateID = companies[r.Next(0, companies.Count -1)].ToString();
                FaPiaoPreferenceID = null;
                FaPiaoRequestType = (r.Next(0, 100000) % 2 == 0 ? 1 : 0).ToString();

                State = BookingUtility.TransformToProxyBookingState("Created");
                DateTime createdOnStr = DateBeginStr.AddHours(durations[r.Next(0, durations.Count - 1)] * (-1));
                CreatedOn = createdOnStr.ToString();
                CreatedBy = UserID;

                string temp = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
                    BillingOption, KemasBookingID, KemasBookingNumber, DateBegin, DateEnd,
                    TotalAmount, UserID, CorporateID, FaPiaoPreferenceID,FaPiaoRequestType, 
                    State, CreatedOn, CreatedBy);

                count = i % batchSize + 1;
                contents[count - 1] = temp;

                if (count == batchSize)
                {
                    File.AppendAllLines(_dataFilePath, contents);
                    Thread.Sleep(10);
                }

            }
        }
    }

    public class DataImport
    {
        public static void Import(string datafile) 
        {
            using (SqlConnection conn = new SqlConnection("Data Source=.;Initial Catalog=Vrent;Integrated Security=True"))
            {
                conn.Open();
                using (TextDataReader reader = new TextDataReader(datafile))
                {
                    SqlBulkCopy bulk = new SqlBulkCopy(conn);

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        SqlBulkCopyColumnMapping item = new SqlBulkCopyColumnMapping(i, i + 1);
                        bulk.ColumnMappings.Add(item);
                    }
                    bulk.EnableStreaming = true;
                    bulk.DestinationTableName = "VrentBookings";
                    bulk.BatchSize = 1000;

                    bulk.WriteToServer(reader);

                    bulk.Close();
                }

                conn.Close();
            }
        }
    }

    public class TextDataReader : IDataReader
    {
        protected StreamReader Stream { get; set; }
        protected object[] Values;
        protected bool EOF { get; set; }
        protected string CurrentRecord { get; set; }
        protected int CurrentIndex { get; set; }

        public TextDataReader(string dataFile)
        {
            Stream = new StreamReader(dataFile);
            Values = new object[FieldCount];
        }

        public void Close()
        {
            Array.Clear(Values, 0, Values.Length);
            Stream.Close();
            Stream.Dispose();
        }

        public int Depth
        {
            get { return 0; }
        }

        public DataTable GetSchemaTable()
        {
            return null;
        }

        public bool IsClosed
        {
            get { return EOF; }
        }

        public bool NextResult()
        {
            return false;
        }

        public bool Read()
        {
            CurrentRecord = Stream.ReadLine();
            EOF = CurrentRecord == null;

            if (!EOF)
            {
                Values = CurrentRecord.Split(',');
                CurrentIndex++;
            }

            return !EOF;
        }

        public int RecordsAffected
        {
            get { return -1; }
        }

        public void Dispose()
        {
            
        }

        public int FieldCount
        {
            get { return 13; }
        }

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            if (i == 0)
            {
                return this;
            }
            else
            {
                return null;
            }
        }

        public string GetDataTypeName(int i)
        {
            return "string";
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            return Convert.ToInt16(Values[0]);
        }

        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public string GetName(int i)
        {
            return Values[i].ToString();
        }

        public int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            return Values[i].ToString();
        }

        public object GetValue(int i)
        {
            object var = null;
            switch (i)
            {
                //BillingOption = r.Next(2, 3).ToString();
                //KemasBookingID = Guid.NewGuid().ToString();
                //KemasBookingNumber = string.Format("0{0}", i);
                //DateBegin = current.AddMilliseconds((double)i * 2).ToString();
                //DateEnd = current.AddMilliseconds((double)i * 1).ToString();
                case 0:
                    var = Convert.ToInt16(Values[i]);
                    break;
                case 1:
                    var = Guid.Parse(Values[i].ToString());
                    break;
                case 2:
                    var = Values[i].ToString();
                    break;
                case 3:
                    var = DateTime.Parse(Values[i].ToString());
                    break;
                case 4:
                    var = DateTime.Parse(Values[i].ToString());
                    break;

                //TotalAmount = r.Next(100, 10000).ToString();
                //UserID = users[r.Next(0, 99)].ToString();
                //CorporateID = companies[r.Next(0, 9)].ToString();
                //FaPiaoPreferenceID = null;
                //FaPiaoRequestType = (r.Next(0, 10) % 2 == 0 ? 1 : 0).ToString();
                case 5:
                    var = Decimal.Parse(Values[i].ToString());
                    break;
                case 6:
                    var = Guid.Parse(Values[i].ToString());
                    break;
                case 7:
                    var = Values[i].ToString();
                    break;
                case 8:
                    var = null;
                    break;
                case 9:
                    var = Int16.Parse(Values[i].ToString());
                    break;

                //State = "Created";
                //CreatedOn = DateBegin;
                //CreatedBy = UserID;
                case 10:
                    var = Values[i].ToString();
                    break;
                case 11:
                    var = DateTime.Parse(Values[i].ToString());
                    break;
                case 12:
                    var = Guid.Parse(Values[i].ToString());
                    break;
                default:
                    break;

            }

            return var;
        }

        public int GetValues(object[] values)
        {
            Array.Copy(values, Values, this.FieldCount);
            return this.FieldCount;
        }

        public bool IsDBNull(int i)
        {
            return false;
        }

        public object this[string name]
        {
            get { throw new NotImplementedException(); }
        }

        public object this[int i]
        {
            get { return Values[i]; }
        }
    }
    #endregion

}
