using CF.VRent.Log;
using CF.VRent.Log.ConcreteLog;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.OpenXml4Net.OPC;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CF.VRent.Common
{
    [DataContract]
    public class ExcelDebitNote
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public Guid ClientID { get; set; }

        [DataMember]
        public string ClientName { get; set; }

        [DataMember]
        public string ContractNumber { get; set; }

        [DataMember]
        public string From { get; set; }

        [DataMember]
        public string To { get; set; }

        [DataMember]
        public string BillingDate { get; set; }

        [DataMember]
        public string DueDate { get; set; }

        [DataMember]
        public decimal Total { get; set; }

        [DataMember]
        public ExcelBookingPricingCatalog[] Bookings { get; set; }
    }

    [DataContract]
    public class ExcelUser
    {
        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string ClientID { get; set; }

        [DataMember]
        public string UserFirstName { get; set; }

        [DataMember]
        public string UserLastName { get; set; }

        [DataMember]
        public string EmplyoeeNumber { get; set; }
    }

    [DataContract]
    public class ExcelClient
    {
        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string ClientName { get; set; }

        [DataMember]
        public string ContractPerson { get; set; }

        [DataMember]
        public string ContractInfo { get; set; }

    }


    [DataContract]
    public class ExcelDebitNoteSummary
    {
        [DataMember]
        public string ExcelGroup { get; set; }

        [DataMember]
        public string ExcelCategory { get; set; }

        [DataMember]
        public string From { get; set; }

        [DataMember]
        public string Until { get; set; }

        [DataMember]
        public decimal Amount { get; set; }
    }

    public enum FeeType { Rental = 0, Indirect = 1, Full = 2, Unknown = 3 };

    [DataContract]
    public class ExcelBookingPricingCatalog
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string KemasBookingID { get; set; }

        [DataMember]
        public string KemasBookingNumber { get; set; }

        [DataMember]
        public string UserID { get; set; }

        [DataMember]
        public string EmployeeNumber { get; set; }
        //ReservationNumber,
        //Employee No.
        //User Name	
        //Pick-up Station	
        //Reservation Start time	
        //Reservation End time	

        [DataMember]
        public string UserFirstName { get; set; }

        [DataMember]
        public string UserLastName { get; set; }

        [DataMember]
        public string StationID { get; set; }

        [DataMember]
        public string StationName { get; set; }

        [DataMember]
        public string StartTime { get; set; }

        [DataMember]
        public string EndTime { get; set; }

        [DataMember]
        public string KeyOut { get; set; }

        [DataMember]
        public string KeyIn { get; set; }

        [DataMember]
        public string CarID { get; set; }

        [DataMember]
        public string CarCategory { get; set; }

        [DataMember]
        public string CarModel { get; set; }

        [DataMember]
        public string PriceDetail { get; set; }

        [DataMember]
        public string BookingState { get; set; }

        [DataMember]
        public string RentalAmount { get; set; }

        [DataMember]
        public string IndirectAmount { get; set; }

        [DataMember]
        public FeeType FeeComposition { get; set; }

        [DataMember]
        public PricingItemMonthlysummary[] PricingCatalogs { get; set; }
    }

    //external pricing
    [DataContract]
    public class PricingItemMonthlysummary
    {
        [DataMember]
        public string DebitNoteID { get; set; }

        [DataMember]
        public string PeriodID { get; set; }

        [DataMember]
        public string ClientID { get; set; }

        [DataMember]
        public string kemasBookingID { get; set; }

        [DataMember]
        public string BookingID { get; set; }

        [DataMember]
        public string kemasBookingNumber { get; set; }

        [DataMember]
        public string UserID { get; set; }

        #region duplicate info with Booking
        [DataMember]
        public string StationID { get; set; }

        [DataMember]
        public string StationName { get; set; }

        [DataMember]
        public string StartTime { get; set; }

        [DataMember]
        public string EndTime { get; set; }

        [DataMember]
        public string KeyOut { get; set; }

        [DataMember]
        public string KeyIn { get; set; }

        [DataMember]
        public string CarID { get; set; }

        [DataMember]
        public string CarCategory { get; set; }

        [DataMember]
        public string CarModel { get; set; }

        #endregion

        [DataMember]
        public string OrderMonth { get; set; }

        [DataMember]
        public string OrderID { get; set; }

        [DataMember]
        public string BookingState { get; set; }

        [DataMember]
        public string Group { get; set; }

        [DataMember]
        public string Category { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public decimal Total { get; set; }

        /// <summary>
        /// Navigate to associated booking
        /// Added by Adam
        /// </summary>
        public ExcelBookingPricingCatalog Booking { get; set; }

    }

    public class DebitNotePricingUtility
    {
        public static PricingItemMonthlysummary[] RetrieveCatalog(string xml)
        {
            return XDocument.Parse(xml).Root.Descendants("item")
            .Select(
                    m => new PricingItemMonthlysummary()
                    {
                        Type = m.Attribute("type").Value,
                        Total = m.Attribute("total") == null ? 0 : (string.IsNullOrEmpty(m.Attribute("total").Value) ? 0 : decimal.Parse(m.Attribute("total").Value)),
                        Category = m.Parent.Name.LocalName,
                        Group = "RENTAL"
                    }
            ).ToArray();
        }
    }

    public interface IDebitNoteExcel
    {
        ExcelDebitNote Note { get; set; }
        string TemplateFolder { get; set; }
        string ExcelFileName();
        string ExcelFullPath { get; }

        void DumpData();

        IWorkbook WorkBook { get; }

        void Open();
        void WriteSummary(); //sheet 1
        void WriteRentalFeeSummary(); //sheet 2
        void WriteIndirectfeeSummary(); // sheet 3
        void Close();
    }

    public class DebitNoteExcelConstants
    {
        public const string RentalFeeGroup = "RENTALFEE";
        public const string IndirectFeeGroup = "INDIRECTFEE";

        public const string DebitNoteSummaryDateFormat = "yyyy.MM.dd";

        public const string DebitNoteExcel = "DebitNoteExcel";
        public const string TempFolder = "Temp";
        public const string TemplateKey = "TemplateFolder";

        public const string ExcelNameDateFormat = "yyyyMM";

        public const string PayByCompanyChargeType = "pay by company";

        public const string BusinessHourType = "business_hours";
        public const string NightType = "night";
        public const string WeekendType = "weekend";
        //Checked "Interface with KEMAS.doc"
        //There is a item named "weekend_lt_24h"
        //Added by Adam
        public const string WeekendLT24Type = "weekend_lt_24h";
        public const string HolidayType = "holiday";
        public const string LateReturnType = "late_return";
        public const string ShortenType = "shorten";
        public const string CancelType = "cancel";

        public const string ZeroAmount = "0";
    }

    public class DebitNoteExcelUtility : IDebitNoteExcel
    {
        private string _templateFolder;
        private string _tempDataFolder;
        private string _excelFullPath;
        private string _userID = null;

        public const int StartingRow = 11;
        public const int StartingColumn = 1;

        public const int EndingRow = 20;
        public const int BottomRow = 30;
        public const int EndingColumn = 6;

        /// <summary>
        /// a object used in lock keyword
        /// Added by Adam
        /// </summary>
        private static readonly object _locker = new Object();

        private ExcelDebitNote _note;

        private IWorkbook _workBook = null;

        private static IVRentLog _debitNoteLog = LogInfor.DebitNoteLogWriter;

        #region Properties
        public ExcelDebitNote Note
        {
            get
            {
                return _note;
            }
            set
            {
                _note = value;
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

        public IWorkbook WorkBook
        {
            get
            {
                return _workBook;
            }
        }

        public double TotalPrice
        {
            get;
            private set;
        }


        #endregion

        public DebitNoteExcelUtility(ExcelDebitNote note, string tempDataFolder, string userID)
        {
            _note = note;

            _tempDataFolder = tempDataFolder;

            _userID = userID;

            if (!Directory.Exists(_tempDataFolder))
            {
                Directory.CreateDirectory(_tempDataFolder);
            }

            _templateFolder = new DirectoryInfo(_tempDataFolder).Parent.Parent.FullName;
        }

        public void WriteAll()
        {
            _debitNoteLog.WriteInfo(DebitNoteExcelConstants.DebitNoteExcel, "Start Writing Debit Note Excel", string.Empty);
            if (_note != null && (_note.Bookings != null && _note.Bookings.Length > 0))
            {
                DumpData();
                Open();

                if (_workBook != null)
                {
                    //better performace
                    //Modified by Adam
                    Parallel.Invoke(() => WriteSummary(), () => WriteRentalFeeSummary(), () => WriteIndirectfeeSummary());

                    Close();
                }
            }
            _debitNoteLog.WriteInfo(DebitNoteExcelConstants.DebitNoteExcel, "End Writing Debit Note Excel", _userID);
        }

        public void Open()
        {
            _debitNoteLog.WriteInfo(DebitNoteExcelConstants.DebitNoteExcel, string.Format("Template:{0}", _templateFolder), _userID);

            if (Directory.Exists(_templateFolder))
            {
                string path = Path.Combine(_templateFolder, "DebitNote_Template.xlsx");

                _debitNoteLog.WriteInfo(DebitNoteExcelConstants.DebitNoteExcel, string.Format("Template Path:{0}", path), _userID);

                if (File.Exists(path))
                {
                    FileStream file = null;
                    //Lock excel tempalte for reading
                    //Added by Adam
                    lock (_locker)
                    {
                        try
                        {
                            using (file = new FileStream(path, FileMode.Open, FileAccess.Read))
                            {
                                _workBook = WorkbookFactory.Create(file);
                            }
                        }
                        catch (Exception ex)
                        {
                            _debitNoteLog.WriteInfo(DebitNoteExcelConstants.DebitNoteExcel, string.Format("{0}-{1}", ex.Message, ex.StackTrace), _userID);
                        }
                    }
                }
            }
        }

        public void WriteSummary()
        {
            try
            {
                //start with line C12
                List<PricingItemMonthlysummary> items = new List<PricingItemMonthlysummary>();
                items = _note.Bookings.Aggregate<ExcelBookingPricingCatalog, List<PricingItemMonthlysummary>>(items, (m, n) =>
                {
                    items.AddRange(n.PricingCatalogs);
                    return items;
                });

                var summary =
                    from item in items
                    group item by new
                    {
                        Group = item.Group,
                        Category = item.Group.Equals(DebitNoteExcelConstants.RentalFeeGroup) ? (item.Category.Equals("Rental") ? "Primary" : "Additional") : item.Category
                    } into g

                    select
                        new ExcelDebitNoteSummary()
                        {
                            ExcelGroup = g.Key.Group,
                            ExcelCategory = g.Key.Category,
                            From = _note.From,
                            Until = _note.To,
                            Amount = g.Sum(d => d.Total)
                        };

                ISheet sheet1 = _workBook.GetSheetAt(0);

                //sort into right order
                summary = summary.OrderByDescending(m => m.ExcelGroup).ThenByDescending(m => m.ExcelCategory);

                ExcelDebitNoteSummary rentalFeeSummary = summary.FirstOrDefault(m => m.ExcelGroup.Equals(DebitNoteExcelConstants.RentalFeeGroup) && m.ExcelCategory.Equals("Primary"));

                ExcelDebitNoteSummary AdditionalUsageFeeSummary = summary.FirstOrDefault(m => m.ExcelGroup.Equals(DebitNoteExcelConstants.RentalFeeGroup) && m.ExcelCategory.Equals("Additional"));

                ExcelDebitNoteSummary LatePaymentFeeSummary = summary.FirstOrDefault(m => m.ExcelGroup.Equals(DebitNoteExcelConstants.IndirectFeeGroup) && m.ExcelCategory.Equals("Primary"));

                ExcelDebitNoteSummary OtherAdditionalFeeSummary = summary.FirstOrDefault(m => m.ExcelGroup.Equals(DebitNoteExcelConstants.IndirectFeeGroup) && m.ExcelCategory.Equals("Additional"));

                int startRowIndex = 11; //determined by index
                int startColumnIndex = 2;

                #region Write client Section
                int clientNameRowIndex = 6;
                int billingDateRowIndex = 8;
                int dateRowIndex = 9;
                IRow clientNameRow = sheet1.GetRow(clientNameRowIndex);
                ICell clientNameCell = clientNameRow.GetCell(1);
                clientNameCell.SetCellValue(string.Format("Client name: {0}", _note.ClientName));

                IRow billingDateRow = sheet1.GetRow(billingDateRowIndex);
                ICell billingDateCell = billingDateRow.GetCell(1);
                billingDateCell.SetCellValue(string.Format("Billing Date: {0}", DateTime.Parse(_note.BillingDate).ToString("yyyy.MM.dd")));

                IRow dateRow = sheet1.GetRow(dateRowIndex);
                ICell dateCell = dateRow.GetCell(1);
                dateCell.SetCellValue(string.Format("Date: {0}", DateTime.Now.ToString("yyyy.MM.dd")));

                #endregion

                #region rentalFeeSummary
                IRow startRow = sheet1.GetRow(startRowIndex);

                ICell startCell11 = startRow.GetCell(startColumnIndex);
                startCell11.SetCellValue("Rental Fee");

                ICell startCell12 = startRow.GetCell(startColumnIndex + 1);
                startCell12.SetCellValue(_note.From);

                ICell startCell13 = startRow.GetCell(startColumnIndex + 2);
                startCell13.SetCellValue(_note.To);

                ICell startCell14 = startRow.GetCell(startColumnIndex + 3);
                startCell14.SetCellValue(rentalFeeSummary == null ? 0 : rentalFeeSummary.Amount.ToDouble());

                #endregion

                #region Additional Usage Fee Summary
                IRow startRow2 = sheet1.GetRow(startRowIndex + 1);

                ICell startCell21 = startRow2.GetCell(startColumnIndex);
                startCell21.SetCellValue("Additional Usage Fee");

                ICell startCell22 = startRow2.GetCell(startColumnIndex + 1);
                startCell22.SetCellValue(_note.From);

                ICell startCell23 = startRow2.GetCell(startColumnIndex + 2);
                startCell23.SetCellValue(_note.To);

                ICell startCell24 = startRow2.GetCell(startColumnIndex + 3);
                startCell24.SetCellValue(AdditionalUsageFeeSummary == null ? 0 : AdditionalUsageFeeSummary.Amount.ToDouble());

                #endregion

                #region Additional Usage Fee Summary
                IRow startRow3 = sheet1.GetRow(startRowIndex + 2);

                ICell startCell31 = startRow3.GetCell(startColumnIndex);
                startCell31.SetCellValue("Late Payment Fee");

                ICell startCell32 = startRow3.GetCell(startColumnIndex + 1);
                startCell32.SetCellValue(_note.From);

                ICell startCell33 = startRow3.GetCell(startColumnIndex + 2);
                startCell33.SetCellValue(_note.To);

                ICell startCell34 = startRow3.GetCell(startColumnIndex + 3);
                startCell34.SetCellValue(LatePaymentFeeSummary == null ? 0 : LatePaymentFeeSummary.Amount.ToDouble());

                #endregion

                #region OtherAdditionalFeeSummary
                IRow startRow4 = sheet1.GetRow(startRowIndex + 3);

                ICell startCell41 = startRow4.GetCell(startColumnIndex);
                startCell41.SetCellValue("Other Additional Fee");

                ICell startCell42 = startRow4.GetCell(startColumnIndex + 1);
                startCell42.SetCellValue(_note.From);

                ICell startCell43 = startRow4.GetCell(startColumnIndex + 2);
                startCell43.SetCellValue(_note.To);

                ICell startCell44 = startRow4.GetCell(startColumnIndex + 3);
                startCell44.SetCellValue(OtherAdditionalFeeSummary == null ? 0 : OtherAdditionalFeeSummary.Amount.ToDouble());

                #endregion

                IRow totalRow = sheet1.GetRow(15);
                ICell totalCell = totalRow.GetCell(5);
                TotalPrice = summary.Sum(m => m.Amount).ToDouble();
                totalCell.SetCellValue(TotalPrice);

            }
            catch (Exception ex)
            {
                _debitNoteLog.WriteInfo(DebitNoteExcelConstants.DebitNoteExcel, string.Format("{0}-{1}", ex.Message, ex.StackTrace), _userID);
            }
        }

        public string ExcelFileName()
        {
            //VRent_Debit Note_Client Name_Corporate Template_MMM-YYYY
            DateTime period = DateTime.ParseExact(_note.From, DebitNoteExcelConstants.DebitNoteSummaryDateFormat, null);

            return string.Format("VRent_Debit Note_{0}_Corporate Template_{1}", _note.ClientName, period.ToString("MMM-yyyy"));
        }


        public void WriteRentalFeeSummary()
        {
            try
            {
                ISheet sheet1 = _workBook.GetSheetAt(1);

                //determined by template
                int startRowIndex = 8;
                int startColumnIndex = 1;

                //if there is only one empty row in template page2-booking details, only append new row after the empty row.
                //time compliexty of row creating is O(n*m) and the amount of code line is less than before
                //Modified by Adam 2015/11/24
                CreateNewRows(sheet1, startRowIndex, _note.Bookings.Count(), startColumnIndex, (bookingIndex, cell) =>
                {
                    _setCellValueForBookingDetails(_note.Bookings[bookingIndex], cell);
                });
            }
            catch (Exception ex)
            {
                _debitNoteLog.WriteInfo(DebitNoteExcelConstants.DebitNoteExcel, string.Format("{0}-{1}", ex.Message, ex.StackTrace), _userID);
            }
        }

        /// <summary>
        /// Copy a row and override the exist one, but the old one still can be refered to
        /// Modified by Adam
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="modelRow"></param>
        /// <param name="startCol"></param>
        /// <param name="targetRowIndex"></param>
        /// <param name="innerDelegations"></param>
        private void CopyRow(ISheet sheet, IRow modelRow, int startCol, int targetRowIndex, params Action<ICell>[] innerDelegations)
        {
            IRow target;

            //if try to create model row, get it
            if (modelRow.RowNum == targetRowIndex)
            {
                target = sheet.GetRow(targetRowIndex);
            }
            else
            {
                target = sheet.CreateRow(targetRowIndex);
            }

            for (int i = 0; i < modelRow.Cells.Count; i++)
            {
                ICell targetCell;

                //if try to create cell of modal row, get it
                if (modelRow.RowNum == targetRowIndex)
                {
                    targetCell = target.GetCell(i + startCol);
                }
                else
                {
                    targetCell = target.CreateCell(i + startCol, modelRow.Cells[i].CellType);

                    ICellStyle style = modelRow.Cells[i].CellStyle;

                    if (style != null)
                    {
                        targetCell.CellStyle = style;
                    }

                    //Set value from model row
                    switch (modelRow.Cells[i].CellType)
                    {
                        case CellType.String:
                            targetCell.SetCellValue(modelRow.Cells[i].StringCellValue);
                            break;
                        case CellType.Numeric:
                            targetCell.SetCellValue(modelRow.Cells[i].NumericCellValue);
                            break;
                        case CellType.Boolean:
                            targetCell.SetCellValue(modelRow.Cells[i].BooleanCellValue);
                            break;
                        case CellType.Formula:
                            targetCell.SetCellValue(modelRow.Cells[i].RichStringCellValue);
                            break;
                        case CellType.Error:
                            targetCell.SetCellValue(modelRow.Cells[i].ErrorCellValue);
                            break;
                    }

                }

                //Invoke delegation method
                foreach (var delegation in innerDelegations)
                {
                    delegation(targetCell);
                }

            }

            if (modelRow.RowStyle != null && modelRow.RowNum != targetRowIndex)
            {
                target.RowStyle = modelRow.RowStyle;
            }
        }

        /// <summary>
        /// Create new rows, the start row as the model must be a empty row in template
        /// Added by Adam
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="startRowIndex">the start index of rows</param>
        /// <param name="footerRowIndex"></param>
        /// <param name="startColumnIndex"></param>
        /// <param name="rowNums"></param>
        private void CreateNewRows(ISheet sheet, int startRowIndex, int rowNums, int startColumnIndex,
            params Action<int, ICell>[] setCellValueDelegations)
        {
            //Copy rows in tempalte
            List<IRow> temp = new List<IRow>();
            var firstRow = sheet.GetRow(startRowIndex);
            
            //iterate rows down-top
            var lastRowIndex = sheet.LastRowNum + rowNums - 1;
            for (int i = lastRowIndex; i >= startRowIndex; i--)
            {
                var currentRow = sheet.GetRow(i);
                //rows in this case are already defined in template
                if (currentRow != null && i != startRowIndex)
                {
                    //So, move these rows down giving space for records
                    CopyRow(sheet, currentRow, startColumnIndex, currentRow.RowNum + rowNums - 1);
                }

                //In the space of records
                if (i >= startRowIndex && i < startRowIndex + rowNums)
                {
                    //Create new one with start row as modal row, meanwhile set cells' value with records
                    CopyRow(sheet, firstRow, startColumnIndex, i, cell =>
                    {
                        foreach (var d in setCellValueDelegations)
                        {
                            d(i - startRowIndex, cell);
                        }
                    });
                }
            }

            List<CellRangeAddress> mergedRegionsIndex = new List<CellRangeAddress>();
            //Remove merge ranges and recreate merge ranges
            for (int j = 0; j < sheet.NumMergedRegions; j++)
            {
                //Copy range obj
                var range = sheet.GetMergedRegion(j).Copy();
                if (range.FirstRow > startRowIndex)
                {
                    mergedRegionsIndex.Add(range);
                    //remove range stayed between start row and end row
                    sheet.RemoveMergedRegion(j);
                }
            }

            mergedRegionsIndex.ForEach(range =>
            {
                //Create merge range 
                sheet.AddMergedRegion(new CellRangeAddress(range.FirstRow + rowNums - 1, range.LastRow + rowNums - 1, range.FirstColumn, range.LastColumn));
            });

            mergedRegionsIndex = null;
        }

        public void WriteIndirectfeeSummary()
        {
            try
            {
                ISheet sheet1 = _workBook.GetSheetAt(2);

                //data rows
                int startRowIndex = 11; //determined by template
                int startColumnIndex = 1;

                IEnumerable<ExcelBookingPricingCatalog> indirectFeeBookingOnly = _note.Bookings
                    .Where
                    (
                        m => (m.FeeComposition == FeeType.Full || m.FeeComposition == FeeType.Indirect)
                    //m => m.PricingCatalogs.Where(n => (n.Group.Equals(DebitNoteExcelConstants.IndirectFeeGroup) && n.Category.Equals("Additional"))).Count() > 0
                    );

                if (indirectFeeBookingOnly.Count() > 0)
                {
                    //int totalCnt = 0;

                    indirectFeeBookingOnly = indirectFeeBookingOnly
                        .Select
                        (
                            m => new ExcelBookingPricingCatalog()
                            {
                                BookingState = m.BookingState,
                                CarCategory = m.CarCategory,
                                CarID = m.CarID,
                                CarModel = m.CarModel,
                                EmployeeNumber = m.EmployeeNumber,
                                EndTime = m.EndTime,
                                FeeComposition = m.FeeComposition,
                                ID = m.ID,
                                KemasBookingID = m.KemasBookingID,
                                KemasBookingNumber = m.KemasBookingNumber,
                                KeyIn = m.KeyIn,
                                KeyOut = m.KeyOut,
                                PricingCatalogs = m.PricingCatalogs.Where(n => (n.Group.Equals(DebitNoteExcelConstants.IndirectFeeGroup) && n.Total > 0)).ToArray(),
                                StartTime = m.StartTime,
                                StationID = m.StationID,
                                StationName = m.StationName,
                                UserFirstName = m.UserFirstName,
                                UserID = m.UserID,
                                UserLastName = m.UserLastName
                            }
                        );

                    //Aggregate all bookings' additional fee records
                    //Added by Adam
                    List<PricingItemMonthlysummary> additionalFee = new List<PricingItemMonthlysummary>();
                    indirectFeeBookingOnly.Aggregate(additionalFee, (m, n) =>
                    {
                        additionalFee.AddRange(n.PricingCatalogs);
                        return additionalFee;
                    });

                    //Add rows and set value
                    //Added by Adam
                    CreateNewRows(sheet1, startRowIndex, additionalFee.Count, startColumnIndex, (addtionalIndex, cell) =>
                    {
                        _setCellValueForAdditionalFee(additionalFee[addtionalIndex], cell);
                    });

                    //Merge line header
                    sheet1.AddMergedRegion(new CellRangeAddress(startRowIndex - 1, startRowIndex + additionalFee.Count - 1, startColumnIndex, startColumnIndex));

                    //Sum total with formula
                    sheet1.GetRow(startRowIndex + additionalFee.Count).GetCell(10).SetCellFormula("SUM(K" + (startRowIndex + 1) + ":K" + (startRowIndex + additionalFee.Count) + ")");
                }
            }
            catch (Exception ex)
            {
                _debitNoteLog.WriteInfo(DebitNoteExcelConstants.DebitNoteExcel, string.Format("{0}-{1}", ex.Message, ex.StackTrace), _userID);
            }
        }


        public void Close()
        {
            if (_workBook != null)
            {
                _excelFullPath = Path.Combine(_templateFolder, ExcelFileName()) + ".xlsx";
                _debitNoteLog.WriteInfo(DebitNoteExcelConstants.DebitNoteExcel, _excelFullPath, _userID);

                if (File.Exists(_excelFullPath))
                {
                    File.Delete(_excelFullPath);
                }
                try
                {
                    FileStream sw = new FileStream(_excelFullPath, FileMode.Create);
                    _workBook.Write(sw);
                    sw.Close();
                }
                catch (Exception ex)
                {
                    _debitNoteLog.WriteInfo(DebitNoteExcelConstants.DebitNoteExcel, string.Format("{0}-{1}", ex.Message, ex.StackTrace), _userID);
                }
            }
        }


        public void DumpData()
        {
            //Additional thread
            //Modified by Adam
            Task.Factory.StartNew(() => {
                try
                {
                    string debitNoteDump = SerializedHelper.JsonSerialize<ExcelDebitNote>(_note);
                    string finalPath = Path.Combine(_tempDataFolder, ExcelFileName()) + ".json";

                    if (File.Exists(finalPath))
                    {
                        File.Delete(finalPath);
                    }

                    File.WriteAllText(finalPath, debitNoteDump, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    _debitNoteLog.WriteInfo(DebitNoteExcelConstants.DebitNoteExcel, string.Format("{0}-{1}", ex.Message, ex.StackTrace), _userID);
                }
            });
        }


        public string ExcelFullPath
        {
            get { return _excelFullPath; }
        }

        /// <summary>
        /// Set column's value with booing entity
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="columnIndex"></param>
        private void _setCellValueForBookingDetails(ExcelBookingPricingCatalog booking, ICell targetCell)
        {
            try
            {
                //According to the index of columns in excel template
                switch (targetCell.ColumnIndex)
                {
                    case 1:
                        targetCell.SetCellValue(booking.KemasBookingNumber.TrimStart('0'));
                        break;
                    case 2:
                        targetCell.SetCellValue(booking.EmployeeNumber);
                        break;
                    case 3:
                        targetCell.SetCellValue(string.Format("{0}-{1}", booking.UserFirstName, booking.UserLastName));
                        break;
                    case 4:
                        targetCell.SetCellValue(booking.StationName);
                        break;
                    case 5:
                        //because in dataaccess layer, start time is explicitly converted to string type with yyyyMMdd HH:mm:ss as format
                        //here for avoiding problems due to the difference between cell type in template and string type of start time
                        //Hence, has to cast it to datetime and pass it into SetCellValue(DateTime d) which is one of overload method of SetCellValue
                        //in this way the conversion from DateTime to excel cell type is controlled by NPOI interface
                        //the same as EndTime,KeyIn and KeyOut
                        //Added by Adam
                        DateTime startTime;
                        if (DateTime.TryParseExact(booking.StartTime,
                                "yyyyMMdd HH:mm:ss",
                                System.Globalization.CultureInfo.CurrentCulture,
                                System.Globalization.DateTimeStyles.None, out startTime))
                        {
                            targetCell.SetCellValue(startTime);
                        }
                        else
                        {
                            targetCell.SetCellValue("");
                        }
                        break;
                    case 6:
                        DateTime endTime;
                        if (DateTime.TryParseExact(booking.EndTime,
                                "yyyyMMdd HH:mm:ss",
                                System.Globalization.CultureInfo.CurrentCulture,
                                System.Globalization.DateTimeStyles.None, out endTime))
                        {
                            targetCell.SetCellValue(endTime);
                        }
                        else
                        {
                            targetCell.SetCellValue("");
                        }
                        break;
                    case 7:
                        DateTime keyOut;
                        if (DateTime.TryParseExact(booking.KeyOut,
                                "yyyyMMdd HH:mm:ss",
                                System.Globalization.CultureInfo.CurrentCulture,
                                System.Globalization.DateTimeStyles.None, out keyOut))
                        {
                            targetCell.SetCellValue(keyOut);
                        }
                        else
                        {
                            targetCell.SetCellValue("");
                        }
                        break;
                    case 8:
                        DateTime keyIn;
                        if (DateTime.TryParseExact(booking.KeyIn,
                                "yyyyMMdd HH:mm:ss",
                                System.Globalization.CultureInfo.CurrentCulture,
                                System.Globalization.DateTimeStyles.None, out keyIn))
                        {
                            targetCell.SetCellValue(keyIn);
                        }
                        else
                        {
                            targetCell.SetCellValue("");
                        }
                        break;
                    case 9:
                        targetCell.SetCellValue(booking.CarCategory);
                        break;
                    case 10:
                        targetCell.SetCellValue(booking.CarModel);
                        break;
                    case 11:
                        //Numerical cell need number not string
                        //Get the sum value
                        //Modified by Adam
                        var businessHour = booking.PricingCatalogs.Where(m => m.Type.Equals(DebitNoteExcelConstants.BusinessHourType));
                        targetCell.SetCellValue(businessHour.Count() == 0 ? 0 : businessHour.Sum(r => r.Total.ToDouble()));
                        break;
                    case 12:
                        //Numerical cell need number not string
                        //Get the sum value
                        //Modified by Adam
                        var nightHour = booking.PricingCatalogs.Where(m => m.Type.Equals(DebitNoteExcelConstants.NightType));
                        targetCell.SetCellValue(nightHour.Count() == 0 ? 0 : nightHour.Sum(r => r.Total.ToDouble()));
                        break;
                    case 13:
                        //Numerical cell need number not string
                        //Get the sum value
                        //Modified by Adam
                        var weekendHour = booking.PricingCatalogs.Where(m => m.Type.Equals(DebitNoteExcelConstants.WeekendType) || m.Type.Equals(DebitNoteExcelConstants.WeekendLT24Type));
                        targetCell.SetCellValue(weekendHour.Count() == 0 ? 0 : weekendHour.Sum(r => r.Total.ToDouble()));
                        break;
                    case 14:
                        //Numerical cell need number not string
                        //Get the sum value
                        //Modified by Adam
                        var holidayHour = booking.PricingCatalogs.Where(m => m.Type.Equals(DebitNoteExcelConstants.HolidayType));
                        targetCell.SetCellValue(holidayHour.Count() == 0 ? 0 : holidayHour.Sum(r => r.Total.ToDouble()));
                        break;
                    case 15:
                        //Numerical cell need number not string
                        //Get the sum value
                        //Modified by Adam
                        var laterReturn = booking.PricingCatalogs.Where(m => m.Type.Equals(DebitNoteExcelConstants.LateReturnType));
                        targetCell.SetCellValue(laterReturn.Count() == 0 ? 0 : laterReturn.Sum(r => r.Total.ToDouble()));
                        break;
                    case 16:
                        //Numerical cell need number not string
                        //Get the sum value
                        //Modified by Adam
                        var shorten = booking.PricingCatalogs.Where(m => m.Type.Equals(DebitNoteExcelConstants.ShortenType));
                        targetCell.SetCellValue(shorten.Count() == 0 ? 0 : shorten.Sum(r => r.Total.ToDouble()));
                        break;
                    case 17:
                        //Numerical cell need number not string
                        //Get the sum value
                        //Modified by Adam
                        var cancel = booking.PricingCatalogs.Where(m => m.Type.Equals(DebitNoteExcelConstants.CancelType));
                        targetCell.SetCellValue(cancel.Count() == 0 ? 0 : cancel.Sum(r => r.Total.ToDouble()));
                        break;
                    case 18:
                        //Numerical cell need number not string
                        //Get the sum value
                        //Modified by Adam
                        var rental = booking.PricingCatalogs.Where(r => r.Group == DebitNoteExcelConstants.RentalFeeGroup);
                        targetCell.SetCellValue(rental.Count() == 0 ? 0 : rental.Sum(r => r.Total.ToDouble()));
                        break;
                }
            }
            catch (Exception ex)
            {
                _debitNoteLog.WriteInfo(DebitNoteExcelConstants.DebitNoteExcel, booking.ObjectToJson(), _userID);
                _debitNoteLog.WriteInfo(DebitNoteExcelConstants.DebitNoteExcel, ex.ToString(), _userID);
            }
        }

        /// <summary>
        /// Set additianal fee column's value with booking
        /// Added by Adam
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="targetCell"></param>
        private void _setCellValueForAdditionalFee(PricingItemMonthlysummary additionalFee, ICell targetCell)
        {
            try
            {
                //According to column order in template
                switch (targetCell.ColumnIndex)
                {
                    case 1:
                        targetCell.SetCellValue("");
                        break;
                    case 2:
                        targetCell.SetCellValue(additionalFee.Booking.KemasBookingNumber.TrimStart('0'));
                        break;
                    case 3:
                        targetCell.SetCellValue(additionalFee.Booking.EmployeeNumber);
                        break;
                    case 4:
                        targetCell.SetCellValue(string.Format("{0}-{1}", additionalFee.Booking.UserFirstName, additionalFee.Booking.UserLastName));
                        break;
                    case 5:
                        DateTime startTime;
                        if (DateTime.TryParseExact(additionalFee.Booking.StartTime,
                                "yyyyMMdd HH:mm:ss",
                                System.Globalization.CultureInfo.CurrentCulture,
                                System.Globalization.DateTimeStyles.None, out startTime))
                        {
                            targetCell.SetCellValue(startTime);
                        }
                        else
                        {
                            targetCell.SetCellValue("");
                        }
                        break;
                    case 6:
                        DateTime endTime;
                        if (DateTime.TryParseExact(additionalFee.Booking.EndTime,
                                "yyyyMMdd HH:mm:ss",
                                System.Globalization.CultureInfo.CurrentCulture,
                                System.Globalization.DateTimeStyles.None, out endTime))
                        {
                            targetCell.SetCellValue(endTime);
                        }
                        else
                        {
                            targetCell.SetCellValue("");
                        }
                        break;
                    case 7:
                        targetCell.SetCellValue(additionalFee.Booking.CarModel);
                        break;
                    case 8:
                        targetCell.SetCellValue(additionalFee.Type);
                        break;
                    case 9:
                        targetCell.SetCellValue(additionalFee.Description);
                        break;
                    case 10:
                        targetCell.SetCellValue(additionalFee.Total.ToDouble());
                        break;
                    case 11:
                        targetCell.SetCellValue("");
                        break;
                }
            }
            catch (Exception ex)
            {
                _debitNoteLog.WriteInfo(DebitNoteExcelConstants.DebitNoteExcel, additionalFee.ObjectToJson(), _userID);
                _debitNoteLog.WriteInfo(DebitNoteExcelConstants.DebitNoteExcel, ex.ToString(), _userID);
            }
        }
    }
}
