using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.KEMASWSIF_RESERVATIONRef;
using CF.VRent.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;

namespace CF.VRent.Entities.KemasWrapper
{
    public class ProxyBillingOption
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class BookingOptions
    {
        public List<ProxyLocation> Locations { get; set; }
        public List<ProxyDriver> Drivers { get; set; }
        public List<ProxyBillingOption> BillingOptions { get; set; }
    }

    public class KemasHelper
    {
        public static string CommonKemasErrorCode = "10000";
        public static string CommonKemasErrorMsg = "Kemas returns a null object.";
        public static string UnexpectedKemasErrorCode = "10001";
        public static string UnexpectedKemasErrorMsg = "Kemas throws an unexpected exception.";
        public static string KemasTimeoutErrorCode = "10002";
        public static string KemasTimeoutErrorMsg = "Kemas Timeout Exception.";
        public static string KemasCommunicationErrorCode = "10003";
        public static string KemasCommunicationErrorMsg = "Kemas Communication Exception.";

        public static string KemasLogPattern = "ErrorCode:{0},ErrorMessage:{1},Json:{2}";
        public static string ExceptionLogPattern = "Guid:{0},Msg:{1},StackTrace:{2}";

        private static Dictionary<string, string> KemasErrorMessages = new Dictionary<string, string>();

        //WSIF OK 0
        //WSIF ERR GENERAL 1
        //NO VALID MAIL 9101
        //EQUAL PASSWORDS 9102, if user must change password and new password is same as old password
        //NO CURRENT PASSWORD 9103, if user must change password and doesn't inputed the old password
        //NOT CURRENT PASSWORD 9104, if user must change password and inputed a wrong current password
        //NO VALID NUMBER 9105
        //PERSON DEACTIVATED 9106, if person is deactivated
        //ACCOUNT BLOCKED 9107, if users login account is blocked
        //NoError E0000 ()
        //SessionNotFound E0001 (session not found)
        //UserNotFound E1001 (user not found)
        //UserDisabled E1002 (user is disabled)
        //UserBlocked E1003 (user is blocked)
        //UserNotPermitted E1004 (user is not permitted)
        //ReservationNotFound E2001 (reservation not found)
        //ReservationIncorrectStatus E2002 (e.g.: incorrect status : fswBookingModel/canceledg (swBookingMod-
        //el/completed))
        //ReservationAlreadyCanceled E2003 (reservation already canceled)
        //ReservationCategoryEmpty E2004 (category can't be empty)
        //ReservationStatusChange E2011 (no status change possible)
        //ReservationBeginChange E2012 (not possible to change datebegin)
        //ReservationLocationChange E2013 (startlocation change not possible)
        //ReservationBeginInPast E2014 (datebegin is in the past)
        //ReservationDateEnd E2015 (invalid dateend)
        //ReservationMinAheadTime E2016 (not enough minimum time to reservation begin)
        //ReservationCarRequired E2017 (car selection required)
        //ReservationCollision E2018 (Your reservation: Datebegin: ... can not be created because there is
        //already a reservation for these period: DateBegin: ...)
        //ReservationIncompatiblePurpose E2019 (incompatible purpose to cargroup)
        //ReservationNoCarAvailable E2020 (no car available)
        //ReservationNotInWorkingTime E2021 (car providing not during working time possible)
        //ReservationCarCollision E2022 (Datebegin - Dateend (blockingbegin - blockingend) collision with
        //booking datebegin - dateend (blockingbegin - blockingend))
        //PricingConnection E3001 (error while connecting pricing system

        public static void KemasNullGuard<T>(T kemasObj, string methodName)
        {

            string jsonObj = SerializedHelper.JsonSerialize<T>(kemasObj);
            LogInfor.WriteInfo(Constants.KemasDebuggingInfoTitle + "--" + methodName, jsonObj, methodName);

            if (kemasObj == null)
            {
                string errorMsg = string.Format(KemasLogPattern, CommonKemasErrorCode, CommonKemasErrorMsg, jsonObj);
                //LogInfor.WriteError(Constants.KemasDebuggingInfoTitle, errorMsg, methodName);
                throw new VrentApplicationException(CommonKemasErrorCode, errorMsg, ResultType.KEMAS, new Exception(methodName));
            }
        }

        public static void LogKemasException(VrentApplicationException vae, string user)
        {
            #region Move to global error
            //string errorMsg = string.Format(KemasLogPattern, vae.ErrorCode, vae.ErrorMessage, user);

            //string innerExceMsg = string.Empty;
            //if (vae.InnerException != null)
            //{
            //    innerExceMsg = string.Format(ExceptionLogPattern, vae.ID, vae.InnerException.Message, vae.InnerException.StackTrace); 
            //}

            //LogInfor.WriteError(Constants.KemasDebuggingInfoTitle, errorMsg + Environment.NewLine + innerExceMsg,string.Empty); 
            #endregion

        }

        //public static FindReservationRes ConvertFromFindReservation_Response(findReservation_Response response)
        //{
        //    //Kemas
        //    //private string idField;
        //    //private string numberField;        
        //    //private string creatorField;
        //    //private string driverField;  

        //    //private string dateBeginField;        
        //    //private string dateEndField;        
        //    //private string pickupBeginField;        
        //    //private string pickupEndField;
        //    //private string dateHumanField;        
        //    //private string startLocationField;        
        //    //private string carField;
        //    //private string keyOutField;        
        //    //private string keyInField;
        //    //private string stateField;
        //    //private string typeOfJourneyField;        
        //    //private Condition[] conditionsField;

        //    //vrent
        //    //public string ID { get; set; }
        //    //public string Number { get; set; }
        //    //public string Creator { get; set; }
        //    //public string Driver { get; set; }

        //    //public string DateBegin { get; set; }
        //    //public string DateEnd { get; set; }
        //    //public string PickupBegin { get; set; }
        //    //public string PickupEnd { get; set; }
        //    //public string DateHuman { get; set; }


        //    //public string StartLocation { get; set; }
        //    //public string CarID { get; set; }
        //    //public string CarName { get; set; }

        //    //public string KeyOut { get; set; }
        //    //public string KeyIn { get; set; }

        //    //public string State { get; set; }
        //    //public BookingType TypeOfJourney { get; set; }
        //    //public List<ProxyCondition> Conditions { get; set; }
        //    //public string QRCode { get; set; }
        //    //public ProxyUser CreatorInfo { get; set; }
        //    //public ProxyUser DriverInfo { get; set; }

        //    FindReservationRes vrentRes = new FindReservationRes()
        //    {
        //        ID = response.ID,
        //        Number = response.Number,
        //        Creator = response.Creator,
        //        Driver = response.Driver,

        //        DateBegin = response.DateBegin,
        //        DateEnd = response.DateEnd,
        //        PickupBegin = response.PickupBegin,
        //        PickupEnd = response.PickupEnd,
        //        DateHuman = response.DateHuman,

        //        StartLocation = response.StartLocation,
        //        CarID = response.Car,

        //        KeyOut = response.KeyOut,
        //        KeyIn = response.KeyIn,

        //        State = response.State,
        //        TypeOfJourney = (int)BookingUtility.TransformToProxyBookingType(response.TypeOfJourney),
        //        Conditions =
        //        response.Conditions == null ? new List<ProxyCondition>() : response.Conditions.Select(m => new ProxyCondition() { key = m.key, value = m.value }).ToList(),

        //        //unused fields
        //        CarName = null,
        //        QRCode = string.Empty,
        //        CreatorInfo = null,
        //        DriverInfo = null
        //    };

        //    return vrentRes;
        //}

        //public static FindMyReservationRes ConvertFromBooking(Booking kemasBooking) 
        //{
        //    return new FindMyReservationRes()
        //    {
        //        CarID = kemasBooking.CarID,
        //        CarName = kemasBooking.Car,
        //        DateBegin = kemasBooking.DateBegin,
        //        DateEnd = kemasBooking.DateEnd,
        //        DateHuman = kemasBooking.DateHuman,
        //        ID = kemasBooking.ID,
        //        State = kemasBooking.State
        //    };
        //}

        //public static ProxyUser ConvertFromUser(Person bookingUser)
        //{
        //    return new ProxyUser()
        //    {

        //        //kemas user
        //        //private string enabledField;
        //        //private string nameField;
        //        //private string vNameField;
        //        //private string departmentField;
        //        //private string phoneField;
        //        //private string mailField;        
        //        //private string idField;        
        //        //private string pNoField; 

        //        //private string createDateField;
        //        //private string companyField;
        //        //private string personInChargeField;
        //        //private string privateMobileNumberField;
        //        //private string privateBankAccountField;
        //        //private string privateEmailField;
        //        //private string privateAddressField;
        //        //private string businessAddressField;

        //        //private string valid_toField;        
        //        //private string userNameField;
        //        //private int licField;
        //        //private string lic_ExpireDateField;
        //        //private int lic_ExpiredField;
        //        //private bool lic_ExpiredFieldSpecified;
        //        //private string lic_LicenseNumberField;

        //        //proxyUser

        //        //unused fileds
        //        //public bool IsWebUser { get; set; }
        //        //public string Password { get; set; }
        //        //public string ChangePwd { get; set; }
        //        //public ProxyUserSetting UserSetting { get; set; }
        //        //public string CurrentPassword { get; set; }
        //        //public string UserJobTitle { get; set; }
        //        //public string UserOfficeLocation { get; set; }
        //        //public string UserLeadSource { get; set; }
        //        //public string UserIsAcceptAppointment { get; set; }
        //        //public string UserIsShareInfo { get; set; }
        //        //public string UserIsNoImmediateNeed { get; set; }

        //        //used fields
        //        //public string Enabled { get; set; }
        //        //public string Name { get; set; }
        //        //public string VName { get; set; }
        //        //public string Department { get; set; }
        //        //public string Phone { get; set; }
        //        //public string Mail { get; set; }
        //        //public string ID { get; set; }
        //        //public string PNo { get; set; }

        //        //public string CreateDate { get; set; }
        //        //public string Company { get; set; }
        //        //public string PersonInCharge { get; set; }
        //        //public string PrivateMobileNumber { get; set; }
        //        //public string PrivateBankAccount { get; set; }
        //        //public string PrivateEmail { get; set; }
        //        //public string PrivateAddress { get; set; }
        //        //public string BusinessAddress { get; set; }

        //        //public string ValidTo { get; set; }
        //        //public string UserName { get; set; }
        //        //public string Lic { get; set; }
        //        //public string LicExpireDate { get; set; }
        //        //public string LicExpired { get; set; }
        //        //public string LicExpiredSpecified { get; set; }
        //        //public string LicenseNumber { get; set; }



        //        Enabled = bookingUser.Enabled,

        //        Name = bookingUser.Name,
        //        VName = bookingUser.VName,
        //        Department = bookingUser.Department,
        //        Phone = bookingUser.Phone,
        //        Mail = bookingUser.Mail,

        //        ID = bookingUser.ID,
        //        PNo = bookingUser.PNo,
        //        CreateDate = bookingUser.CreateDate,
        //        Company = bookingUser.Company,
        //        PersonInCharge = bookingUser.PersonInCharge,

        //        PrivateMobileNumber = bookingUser.PrivateMobileNumber,
        //        PrivateBankAccount = bookingUser.PrivateBankAccount,
        //        PrivateEmail = bookingUser.PrivateEmail,
        //        PrivateAddress = bookingUser.PrivateAddress,
        //        BusinessAddress = bookingUser.BusinessAddress,

        //        ValidTo = bookingUser.Valid_to,
        //        UserName = bookingUser.UserName,
        //        Lic = bookingUser.Lic.ToString(),
        //        LicExpireDate = bookingUser.Lic_ExpireDate,
        //        LicExpired = bookingUser.Lic_Expired.ToString(),
        //        LicExpiredSpecified = bookingUser.Lic_ExpiredSpecified.ToString(),
        //        LicenseNumber = bookingUser.Lic_LicenseNumber
        //    };
        //}

        //public static FindMyReservationRes ConvertFromBookingDetail(BookingDetail kemasBookingDetail)
        //{
        //    //private string idField;
        //    //private string dateBeginField;
        //    //private string dateEndField;        
        //    //private string carIDField;
        //    //private string carField;       
        //    //private string numberField;   
        //    //private Person creatorField;
        //    //private Person driverField;   

        //    //private string pickupBeginField;      
        //    //private string pickupEndField;      
        //    //private string startLocationField;      
        //    //private string startLocationNameField;     
        //    //private string keyOutField;    
        //    //private string keyInField;      
        //    //private string stateField;     
        //    //private string usePurposeField;      
        //    //private string categoryField;        
        //    //private string qRCodeField;
        //    return new FindMyReservationRes()
        //    {
        //        ID = kemasBookingDetail.ID,
        //        DateBegin = kemasBookingDetail.DateBegin,
        //        DateEnd = kemasBookingDetail.DateEnd,
        //        CarID = kemasBookingDetail.CarID,
        //        Car = kemasBookingDetail.Car,
        //        Number = kemasBookingDetail.Number,
        //        Creator = KemasHelper.ConvertFromUser(kemasBookingDetail.Creator),
        //        Driver = KemasHelper.ConvertFromUser(kemasBookingDetail.Driver),

        //        PickUpBegin = kemasBookingDetail.PickupBegin,
        //        PickUpend = kemasBookingDetail.PickupEnd,
        //        StartLocation = kemasBookingDetail.StartLocation,
        //        StartLocationName = kemasBookingDetail.StartLocationName,
        //        Keyout = kemasBookingDetail.KeyOut,
        //        Keyin = kemasBookingDetail.KeyIn,
        //        State = kemasBookingDetail.State,
        //        UsePurpose = kemasBookingDetail.UsePurpose,
        //        Category = kemasBookingDetail.Category,
        //        QRCode = kemasBookingDetail.QRCode
        //    };
        //}

        public static BookingData ConvertToKemasBookingData(Reservation frontendBooking)
        {
            BookingData data = new BookingData();

            Condition[] arr = new Condition[]
            {   
                new Condition()
                {
                    key="CSCarModel.vehicle_category", value=frontendBooking.VehicleCategory
                },
                new Condition()
                { 
                    key="CarGroupModel.TypeOfJourney", value=frontendBooking.BillingOption.ToString()
                }
            };

            data.Driver = frontendBooking.Driver;
            data.Creator = frontendBooking.Creator;
            data.DateBegin = frontendBooking.DateBegin;
            data.DateEnd = frontendBooking.DateEnd;
            data.StartLocation = frontendBooking.StartLocation;
            data.TypeOfJourney = frontendBooking.BillingOption.ToString();
            data.ID = string.Empty;
            data.Conditions = arr;

            return data;
        }

        public static ReservationData ConvertBookingDataToReservationData(Reservation bookingData)
        {
            return new ReservationData()
            {
                ID = bookingData.ID,
                DateBegin = bookingData.DateBegin,
                DateEnd = bookingData.DateEnd,
                StartLocation = bookingData.StartLocation,
                BillingOption = Convert.ToInt32(bookingData.BillingOption),
                Category = bookingData.VehicleCategory,
                Driver = bookingData.Driver,
                //PaymentStatus = null
            };
        }


        public static BookingOptions ConvertFromKemasBookingOptions(getBookingOptions_Response options)
        {
            return new BookingOptions
            {
                Locations = options.Locations.Select(m => new ProxyLocation() { ID = m.ID, Text = m.Name }).ToList(),
                Drivers = options.Drivers.Select(m => new ProxyDriver() { ID = m.ID, Name = m.Name }).ToList(),
                BillingOptions = options.BillingOptions.Select(m => new ProxyBillingOption() { ID = m.ID, Name = m.Name }).ToList()
            };
        }

        #region Validate Booking from front-end

        public const string KemasBookingDateFormat = "yyyy-MM-dd HH:mm";
        public const string BookingTimeInThePast = "VB000001";
        public const string BookingTimeInThePastMsg = "One of the booking time is in the past";

        public const string LargerThanMaxBookingLeadTime = "VB000002";
        public const string LargerThanMaxBookingLeadTimeMsg = "The maximum booking aheadTime is {0} days, you can't booking a car earlier then this.";

        public const string ExceedMaxBookingDuration = "VB000003";
        public const string ExceedMaxBookingDurationMsg = "The maximum booking duration is {0} hours, you can't booking a car longer then this time";

        public const string StartTimeEndTimeAreEqual = "VB000004";
        public const string StartTimeEndTimeAreEqualMsg = "The begin time and the end time can't be the same, please check.";

        public const string UnknownValidationException = "VB000005";
        public const string UnknownValidationExceptionMsg = "Unexpected Exception occured";

        public const string UnknownLanguageException = "VB000006";
        public const string UnknownLanguageExceptionMsg = "Language {0} is unknown language";

        public const string NotBookingUserCode = "VB000007";
        public const string NotBookingUserCodeMsg = "Permission denied! This booking does not belong to you.";

        public static bool ValidateBookingInput(Reservation feBooking, SystemConfig systemConfig, ref ReturnResult validationRet)
        {
            bool isValid = true;

            try
            {
                long bookingMaxDuration = systemConfig.BookingMaxDuration;
                long bookingAheadTime = systemConfig.BookingAheadTime;

                //check format 
                DateTime bookingBegin = DateTime.ParseExact(feBooking.DateBegin, KemasBookingDateFormat, null);
                DateTime bookingEnd = DateTime.ParseExact(feBooking.DateEnd, KemasBookingDateFormat, null);

                DateTime currentTime = DateTime.Now;

                //if (bookingBegin <= currentTime || bookingEnd <= currentTime)
                //{
                //    validationRet.Code = BookingTimeInThePast;
                //    validationRet.Message = BookingTimeInThePastMsg;
                //    isValid = false;
                //}

                //check max a head time
                TimeSpan ts = currentTime.Subtract(bookingBegin);
                if (bookingAheadTime < Math.Abs(ts.Days))
                {
                    validationRet.Code = LargerThanMaxBookingLeadTime;
                    validationRet.Message = string.Format(LargerThanMaxBookingLeadTimeMsg, bookingAheadTime);
                    isValid = false;
                }

                //validation check max duration
                ts = bookingEnd.Subtract(bookingBegin);
                int hours = ts.Hours;
                if (Math.Abs(hours) > bookingMaxDuration)
                {
                    validationRet.Code = ExceedMaxBookingDuration;
                    validationRet.Message = string.Format(ExceedMaxBookingDurationMsg, bookingAheadTime);
                    isValid = false;
                }

                //check begin time and end time are not the same
                if (bookingBegin == bookingEnd)
                {
                    validationRet.Code = StartTimeEndTimeAreEqual;
                    validationRet.Message = StartTimeEndTimeAreEqualMsg;
                    isValid = false;
                }

            }
            catch (Exception ex)
            {
                LogInfor.WriteError(Constants.ProxyDebuggingInfo, string.Format("Msg:{0},Stacktrace:{1}", ex.Message, ex.StackTrace), string.Empty);

                validationRet.Code = UnknownValidationException;
                validationRet.Message = UnknownValidationExceptionMsg;
                isValid = false;
            }

            return isValid;
        }

        public static bool IsValidLanguage(string language, ref ReturnResult validationRet)
        {
            bool isValid = true;
            if (!Enum.IsDefined(typeof(Lang), language))
            {
                validationRet.Code = UnknownLanguageException;
                validationRet.Message = string.Format(UnknownLanguageExceptionMsg, language);
                isValid = false;
            }
            return isValid;
        }

        #endregion
    }

    public interface IKemasReservation
    {
        getCancelReservationFees_Response getCancelReservationFees(string bookingID, string sessionID);

        string getPrice(string bookingID);

        string getPriceDetailed(string bookingID);

        checkPrice_Response checkPrice(string userID, BookingSample bsample);

        checkPrice2_Response checkPrice2(checkPrice2_Request request);

        checkPrice2_Response checkPrice2Advanced(string sessionID, checkPrice2_RequestBookingData crertia);


        #region CRUD Operations
        //ReservationRes CreateOrUpdateReservationKemas(string uid, BookingData kemasBookingDate, string lang);

        updateReservation2_Response UpdateReservationKemas2(updateReservation2_Request request);

        updateReservation2_Response UpdateReservationKemas2(string sessionID, ReservationData bookingDetails, string lang);

        int CancelReservationKemas(string bookingID, string userID);

        Error CancelReservation2Kemas(string bookingID, string sessionID);

        #endregion

        #region Reservation Options

        getBookingOptions_Response getBookingOptions(string sessionID, string lang);
        Location[] getLocations(string userID);
        string getLocations(string bookingID, string userID, Data bookingData);
        string[] getAvailableCategories(string bookingID, string userID, BookingData bookingData);

        #endregion
        #region Find operation
        findReservation_Response findReservationKemas(string BookingID, string UserID);

        Bookings findMyReservationsKemas(string userID);

        BookingDetail[] findMyReservations2Kemas(string UserID, string[] States, string Lang);

        findReservation2_Response findReservation2Kemas(string BookingID, string SessionID, string Language);

        findReservations2_Response findReservations2Kemas(findReservations2_Request request);

        findReservations2_Response findReservations2Kemas(string sessionID, string[] states, string language, string driver, int itemsPerPage, bool itemsPerPageSpecified, int pageNumber, bool pageSpecified);
        #endregion
    }

    public class KemasReservationAPI : IKemasReservation, IDisposable
    {

        #region Find APIs Wrapper

        public Bookings findMyReservationsKemas(string userID)
        {
            WSKemasPortTypeClient client = new WSKemasPortTypeClient();

            return KemasAccessWrapper.InnerTryCatchInvoker
                (
                    () => client.findMyReservations(userID),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public findReservation_Response findReservationKemas(string bookingID, string userID)
        {
            WSKemasPortTypeClient client = new WSKemasPortTypeClient(); ;

            return KemasAccessWrapper.InnerTryCatchInvoker
            (
                () => client.findReservation(bookingID, userID),
                client,
                MethodInfo.GetCurrentMethod().Name
            );
        }

        public BookingDetail[] findMyReservations2Kemas(string userID, string[] States, string Lang)
        {
            WSKemasPortTypeClient client = new WSKemasPortTypeClient();

            return KemasAccessWrapper.InnerTryCatchInvoker
            (
                () => client.findMyReservations2(userID, States, Lang),
                client,
                MethodInfo.GetCurrentMethod().Name
            );
        }

        public findReservation2_Response findReservation2Kemas(string bookingID, string sessionID, string language)
        {
            WSKemasPortType2Client client = new WSKemasPortType2Client();

            return KemasAccessWrapper.InnerTryCatchInvoker
            (
                () => client.findReservation2(bookingID, sessionID, language),
                client,
                MethodInfo.GetCurrentMethod().Name
            );
        }

        /// <summary>
        /// Call findReservations2 api, support to be overrode
        /// Modified by Adam
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual findReservations2_Response findReservations2Kemas(findReservations2_Request request)
        {
            WSKemasPortType2Client client = new WSKemasPortType2Client();

            return KemasAccessWrapper.InnerTryCatchInvoker
            (
                () => client.findReservations2(request),
                client,
                MethodInfo.GetCurrentMethod().Name
            );
        }

        #endregion

        #region Kemas API extension
        public findReservations2_Response findReservations2Kemas(string sessionID, string[] states, string language, string driver, int itemsPerPage, bool itemsPerPageSpecified, int pageNumber, bool pageSpecified)
        {
            findReservations2_Request request = new findReservations2_Request()
            {
                SessionID = sessionID,
                States = states,
                Language = language,
                Driver = driver,
                ItemsPerPage = itemsPerPage,
                ItemsPerPageSpecified = itemsPerPageSpecified,
                Page = pageNumber,
                PageSpecified = pageSpecified
            };

            return findReservations2Kemas(request);
        }

        #endregion

        #region CRUD Operations

        public updateReservation2_Response UpdateReservationKemas2(updateReservation2_Request request)
        {
            WSKemasPortType2Client client = new WSKemasPortType2Client();

            return KemasAccessWrapper.InnerTryCatchInvoker
            (
                () => client.updateReservation2(request),
                client,
                MethodInfo.GetCurrentMethod().Name
            );
        }

        public updateReservation2_Response UpdateReservationKemas2(string sessionID, ReservationData bookingDetails, string lang)
        {
            updateReservation2_Request request = new updateReservation2_Request()
            {
                SessionID = sessionID,
                ReservationData = bookingDetails,
                Language = lang
            };
            return UpdateReservationKemas2(request);
        }

        public int CancelReservationKemas(string bookingID, string userID)
        {
            WSKemasPortTypeClient client = new WSKemasPortTypeClient();

            return KemasAccessWrapper.InnerTryCatchInvoker
            (
                () => client.cancelReservation(bookingID, userID),
                client,
                MethodInfo.GetCurrentMethod().Name
            );
        }

        public virtual Error CancelReservation2Kemas(string bookingID, string sessionID)
        {
            WSKemasPortType2Client client = new WSKemasPortType2Client();

            return KemasAccessWrapper.InnerTryCatchInvoker
            (
                () => client.cancelReservation2(bookingID, sessionID),
                client,
                MethodInfo.GetCurrentMethod().Name
            );
        }

        #endregion

        #region
        public getCancelReservationFees_Response getCancelReservationFees(string bookingID, string sessionID)
        {
            WSKemasPortType2Client client = new WSKemasPortType2Client();

            return KemasAccessWrapper.InnerTryCatchInvoker
            (
                () => client.getCancelReservationFees(bookingID, sessionID),
                client,
                MethodInfo.GetCurrentMethod().Name
            );
        }

        public string getPrice(string bookingID)
        {
            WSKemasPortTypeClient client = new WSKemasPortTypeClient();
            return KemasAccessWrapper.InnerTryCatchInvoker
            (
                () => client.getPrice(bookingID),
                client,
                MethodInfo.GetCurrentMethod().Name
            );
        }

        public string getPriceDetailed(string bookingID)
        {
            WSKemasPortTypeClient client = new WSKemasPortTypeClient();

            return KemasAccessWrapper.InnerTryCatchInvoker
            (
                () => client.getPriceDetailed(bookingID),
                client,
                MethodInfo.GetCurrentMethod().Name
            );
        }

        public checkPrice_Response checkPrice(string userID, BookingSample bsample)
        {
            WSKemasPortTypeClient client = new WSKemasPortTypeClient();

            return KemasAccessWrapper.InnerTryCatchInvoker
            (
                () => client.checkPrice(userID, bsample),
                client,
                MethodInfo.GetCurrentMethod().Name
            );
        }

        public checkPrice2_Response checkPrice2(checkPrice2_Request request)
        {
            WSKemasPortType2Client client = new WSKemasPortType2Client();

            return KemasAccessWrapper.InnerTryCatchInvoker
            (
                () => client.checkPrice2(request),
                client,
                MethodInfo.GetCurrentMethod().Name
            );
        }

        /// <summary>
        /// Call checkPrice2 api, support to be overrode
        /// Modified by Adam
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="crertia"></param>
        /// <returns></returns>
        public virtual checkPrice2_Response checkPrice2Advanced(string sessionID, checkPrice2_RequestBookingData crertia)
        {
            string currentMethodName = MethodInfo.GetCurrentMethod().Name;

            checkPrice2_Request request = new checkPrice2_Request()
            {
                SessionID = sessionID,
                BookingData = crertia
            };

            return checkPrice2(request);
        }
        #endregion

        #region GetBooking Options

        public getBookingOptions_Response getBookingOptions(string sessionID, string lang)
        {
            WSKemasPortType2Client client = new WSKemasPortType2Client();
            return KemasAccessWrapper.InnerTryCatchInvoker
                (
                    () => client.getBookingOptions(sessionID, lang),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public Location[] getLocations(string userID)
        {
            WSKemasPortTypeClient client = new WSKemasPortTypeClient();
            return KemasAccessWrapper.InnerTryCatchInvoker
                (
                    () => client.getLocations(userID),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public string getLocations(string bookingID, string userID, Data bookingData)
        {
            WSKemasPortTypeClient client = new WSKemasPortTypeClient();
            return KemasAccessWrapper.InnerTryCatchInvoker
                (
                    () => client.getCountAvailableCars(bookingID, userID, bookingData),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public string[] getAvailableCategories(string bookingID, string userID, BookingData bookingData)
        {
            WSKemasPortTypeClient client = new WSKemasPortTypeClient();
            return KemasAccessWrapper.InnerTryCatchInvoker
                (
                    () => client.getAvailableCategories(bookingID, userID, bookingData),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        #endregion

        public void Dispose()
        {
            //Nothing
        }
    }
}