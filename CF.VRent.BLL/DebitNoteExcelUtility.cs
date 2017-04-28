using CF.VRent.Common;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.AccountingService;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.Entities.KEMASWSIF_AUTHRef;
using CF.VRent.Entities.KEMASWSIF_RESERVATIONRef;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF.VRent.BLL
{
    public class DebitNoteExcelDataUtility
    {

        private static FeeType Evaluate(IGrouping<string, PricingItemMonthlysummary> pricingCatalog)
        {
            FeeType type = FeeType.Unknown;

            int rentalCnt = pricingCatalog.Where(m => m.Group.Equals(DebitNoteExcelConstants.RentalFeeGroup)).Count();
            int indrectCnt = pricingCatalog.Where(m => m.Group.Equals(DebitNoteExcelConstants.IndirectFeeGroup)).Count();

            if (rentalCnt > 0 && indrectCnt > 0)
            {
                type = FeeType.Full;
            }
            else if (rentalCnt == 0 && indrectCnt > 0)
            {
                type = FeeType.Indirect;
            }
            else if (rentalCnt > 0 && indrectCnt == 0)
            {
                type = FeeType.Rental;
            }
            return type;
        }

        private static Dictionary<string, ExcelBookingPricingCatalog> ConvertToPricingCatalog(IEnumerable<PricingItemMonthlysummary> items, List<ExcelUser> kemasUsers)
        {
            Dictionary<string, ExcelBookingPricingCatalog> bookings = new Dictionary<string, ExcelBookingPricingCatalog>();

            IEnumerable<IGrouping<string, PricingItemMonthlysummary>> bookingCatalog = items.GroupBy(m => m.kemasBookingID, n => n);

            foreach (var princingCatalog in bookingCatalog)
            {
                ExcelBookingPricingCatalog excelBoooking = new ExcelBookingPricingCatalog();

                PricingItemMonthlysummary unique = princingCatalog.FirstOrDefault();

                //find userInfo
                ExcelUser eu = kemasUsers.FirstOrDefault(m => m.ID.Equals(unique.UserID));
                if (eu != null)
                {
                    excelBoooking.UserFirstName = eu.UserFirstName;
                    excelBoooking.UserLastName = eu.UserLastName;
                    excelBoooking.EmployeeNumber = eu.EmplyoeeNumber;
                }

                excelBoooking.KemasBookingID = unique.kemasBookingID;
                excelBoooking.KemasBookingNumber = unique.kemasBookingNumber;
                excelBoooking.UserID = unique.UserID;
                excelBoooking.PricingCatalogs = princingCatalog.ToArray();
                excelBoooking.CarID = unique.CarID;
                excelBoooking.CarCategory = unique.CarCategory;
                excelBoooking.CarModel = unique.CarModel;
                excelBoooking.EndTime = unique.EndTime;
                excelBoooking.StartTime = unique.StartTime;
                excelBoooking.KeyIn = unique.KeyIn;
                excelBoooking.KeyOut = unique.KeyOut;
                excelBoooking.StationID = unique.StationID;
                excelBoooking.StationName = unique.StationName;
                excelBoooking.BookingState = unique.BookingState;
                excelBoooking.FeeComposition = Evaluate(princingCatalog);

                //flat booking/pricing relationship for writing value into excel eaysier
                //Added by Adam
                foreach (var pricingCatalogItem in princingCatalog)
                {
                    pricingCatalogItem.Booking = excelBoooking;
                }

                bookings.Add(excelBoooking.KemasBookingID, excelBoooking);
            };

            return bookings;
        }

        /// <summary>
        /// Aggregate objects for excel
        /// </summary>
        /// <param name="notes"></param>
        /// <param name="kemasUsers"></param>
        /// <param name="kemasClients"></param>
        /// <param name="innerDelegate">a delegate function doing logic in this method, default is null. Added by Adam for performance improvement</param>
        /// <returns></returns>
        public static ExcelDebitNote[] Load(DebitNote[] notes, List<ExcelUser> kemasUsers, List<ExcelClient> kemasClients, Action<ExcelDebitNote> innerDelegate = null)
        {
            ProxyUserSetting pus = new ProxyUserSetting();
            pus.ID = string.Empty;
            pus.SessionID = string.Empty;

            List<ExcelDebitNote> excelData = new List<ExcelDebitNote>();
            IAccountingService ias = new DataAccessProxyManager();

            PricingItemMonthlysummary[] princingCatalog = ias.RetrieveDebitNoteMonthlySummary(notes, pus);

            Dictionary<string, ExcelBookingPricingCatalog[]> bookingCatelog = princingCatalog.GroupBy(m => m.DebitNoteID).ToDictionary(n => n.Key, s => ConvertToPricingCatalog(s, kemasUsers).Values.ToArray());

            //no order, changed foreach to Parallel.ForEach
            //Modified by Adam
            Parallel.ForEach(notes, note =>
            {
                string temp = note.ID.ToString();
                if (bookingCatelog.ContainsKey(temp))
                {
                    ExcelClient client = kemasClients.FirstOrDefault(m => m.ID.Equals(note.ClientID.ToString()));


                    ExcelDebitNote edn = new ExcelDebitNote()
                    {
                        BillingDate = note.BillingDate.ToString(),

                        ClientID = note.ClientID,
                        ClientName = client == null ? note.ClientName : client.ClientName,
                        DueDate = note.DueDate.ToString(),
                        ID = note.ID,
                        ContractNumber = client == null ? note.ContactPerson : client.ContractPerson,
                        From = note.PeriodStartDate.ToString(DebitNoteExcelConstants.DebitNoteSummaryDateFormat),
                        To = note.PeriodEndDate.AddDays(-1).ToString(DebitNoteExcelConstants.DebitNoteSummaryDateFormat),
                        Bookings = bookingCatelog[temp].OrderBy(m => Convert.ToInt32(m.KemasBookingNumber.TrimStart('0'))).ToArray(),
                        Total = bookingCatelog[temp].Sum(m => m.PricingCatalogs.Sum(n => n.Total)),
                    };

                    //no order, changed foreach to Parallel.ForEach
                    //Modified by Adam
                    Parallel.ForEach(edn.Bookings, booking =>
                    {
                        ExcelUser kemasUser = kemasUsers.FirstOrDefault(m => m.ID.Equals(booking.UserID));

                        if (kemasUser != null)
                        {
                            booking.UserFirstName = kemasUser.UserFirstName;
                            booking.UserLastName = kemasUser.UserLastName;
                            booking.EmployeeNumber = kemasUser.EmplyoeeNumber;
                        }
                    });

                    //Inovke delegate method and dont need do loop from external again
                    //Added by Adam
                    if (innerDelegate != null)
                    {
                        innerDelegate(edn);
                    }

                    excelData.Add(edn);
                }
            });

            return excelData.ToArray();
        }
    }
}
