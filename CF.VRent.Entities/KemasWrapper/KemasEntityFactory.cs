using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.KEMASWSIF_RESERVATIONRef;
using CF.VRent.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace CF.VRent.Entities.KemasWrapper
{
    //the Entity class representing a kemas reservation(super-set) 
    public class KemasReservationEntity
    {

        //      {
        //   "ProxyBookingID":

        //   "KemasBookingID":null,
        //   "DateBegin":null,
        //   "DateEnd":null,
        //   "CarID":null,
        //   "CarName":null,

        //   "Number":null,
        //   "CreatorID":null,
        //   "CreatorName":null,
        //   "Creator":null,
        //   "DriverID":null,
        //   "DriverName":null,
        //   "Driver":null,

        //   "CorporateID":null,

        //   "PickupBegin":null,
        //   "PickupEnd":null,
        //   "StartLocationID":null,
        //   "StartLocationName":null,

        //   "KeyOut":null,
        //   "keyIn":null
        //   "State":null,

        //   "BillingOption":0,
        //   "BillingOptionName":null,

        //   "Category":null,
        //   "Price":0,
        //   "PriceDetail":null,
        //   "PaymentStatus":null,

        //   "UPPaymentID":

        //   "CreatedBy":null,
        //   "CreatedOn":null,
        //   "ModifiedBy":null,
        //   "ModifiedOn":null,

        //"FaPiaoPreferenceID":"3178afae-4678-4707-bb54-5ae18997ffe1",
        //"FaPiaoRequestType":"1"
        //}

        public int ProxyBookingID { get; set; }

        public string KemasBookingID { get; set; } // guid
        public string DateBegin { get; set; } //"yyyy-MM-dd HH:mm";
        public string DateEnd { get; set; }//"yyyy-MM-dd HH:mm";
        public string CarID { get; set; } //guid
        public string CarName { get; set; }
        public string Number { get; set; }

        //public ProxyUserSetting Creator { get; set; }
        public string CreatorID { get; set; }
        public string CreatorFirstName { get; set; }
        public string CreatorLastName { get; set; }


        //public ProxyUserSetting Driver { get; set; }
        public string DriverID { get; set; }
        public string DriverFirstName { get; set; }
        public string DriverLastName { get; set; }


        public string CorporateID { get; set; }
        public string CorporateName { get; set; }

        public string PickupBegin { get; set; } //"yyyy-MM-dd HH:mm";
        public string PickupEnd { get; set; } //"yyyy-MM-dd HH:mm";

        public string StartLocationID { get; set; } // guid
        public string StartLocationName { get; set; } //

        public string KeyOut { get; set; }
        public string keyIn { get; set; }
        public string State { get; set; } //kemas state

        public int BillingOption { get; set; }
        public string BillingOptionName { get; set; }

        public string Category { get; set; }

        public decimal Price { get; set; }
        public string PriceDetail { get; set; }
        public PaymentStatus PaymentStatus { get; set; }

        public int UPPaymentID { get; set; }

        //system columns
        public DateTime? CreatedOn { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid? ModifiedBy { get; set; }

        /// <summary>
        /// Represent current price of booking
        /// Added by Adam
        /// </summary>
        public BookingPriceInfo PriceDetailEntity { get; set; }

        //Constructor
        public KemasReservationEntity()
        {
            PaymentStatus = new PaymentStatus();
            PriceDetailEntity = new BookingPriceInfo();
        }
    }

    public class KemasEntityFactory
    {
        public const string BookingUserName = "{0} {1}";
        public static Dictionary<int, string> BillingOptions = new Dictionary<int, string>();
        static KemasEntityFactory()
        {
            BillingOptions.Add(1, "business & private");
            BillingOptions.Add(2, "business");
            BillingOptions.Add(3, "private");
        }

        #region helper method
        private static void SetBillingOption(KemasReservationEntity kre, int billingOption)
        {
            kre.BillingOption = billingOption;
            kre.BillingOptionName = BillingOptions[billingOption];
        }

        private static void SetBillingOptionKemas(KemasReservationEntity kre, KEMASWSIF_RESERVATIONRef.Reservation kemasFind)
        {
            kre.BillingOption = kemasFind.BillingOption.ID;
            kre.BillingOptionName = kemasFind.BillingOption.Name;
        }

        private static void SetUsersInfoFromKemas(KemasReservationEntity kre, KEMASWSIF_RESERVATIONRef.Reservation kemasFind, ProxyUserSetting userInfo)
        {
            if (kemasFind.Driver != null)
            {
                kre.DriverID = kemasFind.Driver.ID;

                kre.DriverFirstName = HttpUtility.HtmlDecode(kemasFind.Driver.Name);
                kre.DriverLastName = HttpUtility.HtmlDecode(kemasFind.Driver.VName);

                Client[] userClients = kemasFind.Driver.Clients;

                if (userClients != null && userClients.Length > 1)
                {
                    kre.CorporateID = userClients[0].ID;
                    kre.CorporateName = kemasFind.Driver.Company;
                }
                else
                {
                    kre.CorporateID = userInfo.ClientID;
                    kre.CorporateName = userInfo.Company;
                }
            }

            if (kemasFind.Creator != null)
            {
                kre.CreatorID = kemasFind.Creator.ID;
                kre.CreatorFirstName = HttpUtility.HtmlDecode(kemasFind.Creator.Name);
                kre.CreatorLastName = HttpUtility.HtmlDecode(kemasFind.Creator.VName);
            }
        }

        private static void SetUsersInfoFromKemasToProxy(ProxyReservation booking, KEMASWSIF_RESERVATIONRef.Reservation kemasFind)
        {
            if (kemasFind.Driver != null)
            {
                Guid driverID;
                if (!Guid.TryParse(kemasFind.Driver.ID, out driverID))
                {
                    LogInfor.WriteError("Error occured during casting from kemasFind.Driver.ID to booking.UserID at SetUsersInfoFromKemasToProxy",
                    String.Format("kemasFind.Driver.ID:{0}", kemasFind.Driver.ID), booking.UserID.ToString());
                }
                else
                {
                    booking.UserID = driverID;
                }

                booking.UserFirstName = HttpUtility.HtmlDecode(kemasFind.Driver.Name);
                booking.UserLastName = HttpUtility.HtmlDecode(kemasFind.Driver.VName);
            }

            if (kemasFind.Creator != null)
            {
                Guid creatorID;
                if (!Guid.TryParse(kemasFind.Creator.ID, out creatorID))
                {
                    LogInfor.WriteError("Error occured during casting from kemasFind.Creator.ID to booking.CreatorID at SetUsersInfoFromKemasToProxy",
                                       String.Format("kemasFind.Creator.ID:{0}", kemasFind.Creator.ID), booking.UserID.ToString());
                }
                else
                {
                    booking.CreatorID = creatorID;
                }
                //booking.CreatorID = Guid.Parse(kemasFind.Creator.ID);
                booking.CreatorFirstName = HttpUtility.HtmlDecode(kemasFind.Creator.Name);
                booking.CreatorLastName = HttpUtility.HtmlDecode(kemasFind.Creator.VName);
            }
        }

        private static void SetUsersInfoFromProxy(KemasReservationEntity kre, ProxyReservation proxyBooking)
        {
            kre.DriverID = proxyBooking.UserID.ToString();
            kre.DriverFirstName = proxyBooking.UserFirstName;
            kre.DriverLastName = proxyBooking.UserLastName;

            kre.CorporateID = proxyBooking.CorporateID;
            kre.CorporateName = proxyBooking.CorporateName;

            kre.CreatorID = proxyBooking.CreatorID.ToString();
            kre.CreatorFirstName = proxyBooking.CreatorFirstName;
            kre.CreatorLastName = proxyBooking.CreatorLastName;
        }

        private static void SetLocationInfoFromProxy(KemasReservationEntity kre, ProxyReservation booking)
        {
            kre.StartLocationID = booking.StartLocationID.ToString();
            kre.StartLocationName = booking.StartLocationName;
        }

        private static void SetLocationInfoFromKemas(ProxyReservation booking, KEMASWSIF_RESERVATIONRef.Reservation kemasFind)
        {
            //booking.StartLocationID = Guid.Parse(kemasFind.StartLocation.ID);
            Guid startlocID;
            if (!Guid.TryParse(kemasFind.StartLocation.ID, out startlocID))
            {
                LogInfor.WriteError("Error occured during casting from kemasFind.StartLocation.ID to booking.StartLocationID at SetLocationInfoFromKemas",
                    String.Format("kemasFind.StartLocation.ID:{0}", kemasFind.StartLocation.ID), booking.UserID.ToString());
            }
            else
            {
                booking.StartLocationID = startlocID;
            }
            //Decode name to aviod html contents
            booking.StartLocationName = HttpUtility.HtmlDecode(kemasFind.StartLocation.Name);
        }

        private static void SetLocationInfo(KemasReservationEntity kre, KEMASWSIF_RESERVATIONRef.Reservation kemasFind)
        {
            kre.StartLocationID = kemasFind.StartLocation.ID;
            //Decode name to aviod html contents
            kre.StartLocationName = HttpUtility.HtmlDecode(kemasFind.StartLocation.Name);
        }

        private static void SetSystemColumns(KemasReservationEntity kre, ProxyReservation proxyBooking)
        {
            kre.CreatedBy = proxyBooking.CreatedBy;
            kre.CreatedOn = proxyBooking.CreatedOn;
            kre.ModifiedBy = proxyBooking.ModifiedBy;
            kre.ModifiedOn = proxyBooking.ModifiedOn;
            //kre.State = proxyBooking.State;
        }

        #endregion

        private const string FEDateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public static KemasReservationEntity ConvertFromProxyReservation(ProxyReservation proxyBooking)
        {
            KemasReservationEntity kre = new KemasReservationEntity();

            kre.ProxyBookingID = proxyBooking.ProxyBookingID;


            SetBillingOption(kre, proxyBooking.BillingOption);
            SetUsersInfoFromProxy(kre, proxyBooking);
            SetLocationInfoFromProxy(kre, proxyBooking);

            kre.DateBegin = proxyBooking.DateBegin == null ? null : proxyBooking.DateBegin.Value.ToString(FEDateTimeFormat);
            kre.DateEnd = proxyBooking.DateEnd == null ? null : proxyBooking.DateEnd.Value.ToString(FEDateTimeFormat);

            kre.KemasBookingID = proxyBooking.KemasBookingID.ToString();
            kre.Number = proxyBooking.KemasBookingNumber;
            kre.Price = proxyBooking.TotalAmount.Value;

            kre.State = proxyBooking.State;

            SetSystemColumns(kre, proxyBooking);

            return kre;
        }

        public static void AppendProxyReservationInfo(KemasReservationEntity kre, ProxyReservation proxyBooking)
        {
            kre.ProxyBookingID = proxyBooking.ProxyBookingID;

            SetSystemColumns(kre, proxyBooking);
        }

        public static void AppendUPPaymentInfo(KemasReservationEntity kre, KemasReservationEntity inputFromFE)
        {
            kre.UPPaymentID = inputFromFE.UPPaymentID;
        }

        public static KemasReservationEntity ConvertFromReservation(KEMASWSIF_RESERVATIONRef.Reservation kemasFind, ProxyUserSetting userInfo)
        {
            KemasReservationEntity kre = new KemasReservationEntity();

            SetBillingOptionKemas(kre, kemasFind);

            kre.CarID = kemasFind.CarID;
            kre.CarName = kemasFind.Car;
            kre.Category = kemasFind.Category;

            SetUsersInfoFromKemas(kre, kemasFind, userInfo);

            kre.DateBegin = kemasFind.DateBegin;
            kre.DateEnd = kemasFind.DateEnd;
            kre.KemasBookingID = kemasFind.ID;
            kre.keyIn = kemasFind.KeyIn;
            kre.KeyOut = kemasFind.KeyOut;
            kre.Number = kemasFind.Number;
            kre.PickupBegin = kemasFind.PickupBegin;
            kre.PickupEnd = kemasFind.PickupEnd;


            kre.Price = decimal.Parse(kemasFind.Price);

            //kre.PriceDetail = kemasFind.PriceDetail;
            kre.PriceDetail = null;

            SetLocationInfo(kre, kemasFind);

            kre.State = kemasFind.State;

            return kre;

        }

        public static ReservationData ConvertUniformBookingDataToReservationData(KemasReservationEntity bookingData)
        {
            return new ReservationData()
            {
                DateBegin = bookingData.DateBegin,
                DateEnd = bookingData.DateEnd,
                StartLocation = bookingData.StartLocationID,
                BillingOption = bookingData.BillingOption,
                Category = bookingData.Category,
                Driver = bookingData.DriverID,
                //PaymentStatus = null
            };
        }

        public static ProxyReservation ConvertFromReservationToProxyBooking(KEMASWSIF_RESERVATIONRef.Reservation kemasCreate, ProxyUserSetting userInfo)
        {
            ProxyReservation proxyReserveInput = new ProxyReservation();

            Guid kemasBookingID = Guid.Parse(kemasCreate.ID);
            //Guid bookingUserID = Guid.Parse(kemasCreate.Driver.ID);

            //append corporate info
            if (kemasCreate.Driver.Clients != null && kemasCreate.Driver.Clients.Length > 0)
            {
                proxyReserveInput.CorporateID = kemasCreate.Driver.Clients[0].ID;
                proxyReserveInput.CorporateName = kemasCreate.Driver.Clients[0].Name;
            }
            else
            {
                proxyReserveInput.CorporateID = userInfo.ClientID;
                proxyReserveInput.CorporateName = userInfo.Company;
            }

            proxyReserveInput.BillingOption = kemasCreate.BillingOption.ID;
            proxyReserveInput.KemasBookingID = kemasBookingID;
            proxyReserveInput.KemasBookingNumber = kemasCreate.Number;
            proxyReserveInput.DateBegin = DateTime.Parse(kemasCreate.DateBegin);
            proxyReserveInput.DateEnd = DateTime.Parse(kemasCreate.DateEnd);

            proxyReserveInput.TotalAmount = decimal.Parse(kemasCreate.Price);

            proxyReserveInput.PricingDetail = kemasCreate.PriceDetail;

            SetUsersInfoFromKemasToProxy(proxyReserveInput, kemasCreate);

            SetLocationInfoFromKemas(proxyReserveInput, kemasCreate);

            proxyReserveInput.State = kemasCreate.State;

            return proxyReserveInput;
        }
    }
}
