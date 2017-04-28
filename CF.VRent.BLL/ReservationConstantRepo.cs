using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.KEMASWSIF_RESERVATIONRef;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using CF.VRent.Log;
using CF.VRent.UserStatus;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL
{
    public class ReservationConstantRepo
    {
        public const int DefaultThrowPricingFlag = 0;//0 logging ony, 1: throw

        #region validation

        public static bool CheckValidPricingFields(CF.VRent.Entities.KEMASWSIF_RESERVATIONRef.Reservation kemasRet, int throwPricingException)
        {
            bool isBadPrice = false;
            if (string.IsNullOrEmpty(kemasRet.Price) || string.IsNullOrEmpty(kemasRet.PriceDetail))
            {
                isBadPrice = true;
            }

            string pricingMsg = string.Format(ErrorConstants. BadPricingfieldsMessage,
               kemasRet.ID,
               kemasRet.State,
               kemasRet.Price,
               kemasRet.PriceDetail);

            //0 logging ony, 1: throw
            if (throwPricingException ==  DefaultThrowPricingFlag)
            {
                LogInfor.WriteInfo(ErrorConstants.BadPricingfieldsCode, pricingMsg, string.Empty);
            }
            else
            {
                throw new VrentApplicationException(ErrorConstants.BadPricingfieldsCode, pricingMsg, ResultType.Pricing);
            }

            return isBadPrice;
        }


        public static string[] SplitStatusToArray(string statusStr)
        {
            string[] states = null;
            List<string> kemasStates = new List<string>();

            if (!string.IsNullOrEmpty(statusStr) && !string.IsNullOrEmpty(statusStr.Trim()))
            {
                states = statusStr.Trim().Split(',').ToArray();
            }

            foreach (string state in states)
            {
                if (!BookingUtility.IsValidBookingState(state))
                {
                    throw new VrentApplicationException(ErrorConstants.BadBookingStateCode, string.Format(ErrorConstants.BadBookingStateMessage, state), ResultType.VRENTFE);
                }

                kemasStates.Add(BookingUtility.TransformToProxyBookingState(state));
            }

            return kemasStates.ToArray();
        }


        public static bool IsValidBillingOption(KemasReservationEntity booking, ProxyUserSetting pus)
        {
            bool IsValid = false;
            UserData2 ud2 = UserRegistrationConst.RetrieveKemasUserByID(pus.ID, pus.SessionID);

            //set status
            IStatusHelper statusHelper = new UserStatusHelper(new UserStatusManager(ud2.Status));

            int currentTypeOffJouney = statusHelper.IsValidToDoBooking();

            if (statusHelper.IsProfileFilled())
            {
                //int typeOfJouney = ud2.TypeOfJourney; 

                if (currentTypeOffJouney == 0)
                {
                    IsValid = false;
                }
                else if (currentTypeOffJouney == 1)
                {
                    IsValid = true;
                }
                else if (currentTypeOffJouney == 2)
                {
                    if (booking.BillingOption == 2)
                    {
                        IsValid = true;
                    }
                    else
                    {
                        IsValid = false;
                    }
                }
                else if (currentTypeOffJouney == 3)
                {
                    if (booking.BillingOption == 3)
                    {
                        IsValid = true;
                    }
                    else
                    {
                        IsValid = false;
                    }
                }

                if (!IsValid)
                {
                    throw new VrentApplicationException(
                        ErrorConstants.InvalidBillingOptionCode,
                        string.Format(ErrorConstants.InvalidBillingOptionMessage, currentTypeOffJouney, booking.DriverID),
                        ResultType.VRENT);
                }
            }
            else
            {
                if (!statusHelper.IsProfileFilled())
                {
                    throw new VrentApplicationException(
                       ErrorConstants.UpdateProfileFirstCode,
                       string.Format(ErrorConstants.UpdateProfileFirstMessage, booking.DriverID),
                       ResultType.VRENTFE);
                }
            }

            return IsValid;
        }



        public static bool IsEligiableForFapiao(ProxyReservation booking)
        {
            bool isEligable = true;
            if (booking.BillingOption != 0 && booking.BillingOption == 2)
            {
                return false;
            }

            return isEligable;
        }

        public static bool IsEligiableForFapiaoForKemas(CF.VRent.Entities.KEMASWSIF_RESERVATIONRef.Reservation KemasBooking)
        {
            bool isEligable = true;
            if (KemasBooking.BillingOption.ID == 2)
            {
                return false;
            }

            return isEligable;
        }

        public static bool IsDataOutOfDate(CF.VRent.Entities.KEMASWSIF_RESERVATIONRef.Reservation kemasBooking, ProxyReservation proxy,ProxyUserSetting userInfo)
        {
            bool IsOutOFDate = false;

            ProxyReservation dup = KemasEntityFactory.ConvertFromReservationToProxyBooking(kemasBooking, userInfo);

            if (
                !(proxy.BillingOption .Equals( dup.BillingOption))
                //|| !(proxy.CorporateID.Equals( dup.CorporateID)) //?
                //|| !(proxy.CorporateName.Equals( dup.CorporateName)) //?
                || !(proxy.CreatorFirstName.Equals( dup.CreatorFirstName)) //?
                || !(proxy.CreatorLastName.Equals( dup.CreatorLastName)) //?
                || !(proxy.StartLocationID.Equals( dup.StartLocationID)) //?
                || !(proxy.StartLocationName .Equals( dup.StartLocationName)) //?
                || !(proxy.TotalAmount .Equals( dup.TotalAmount))
                || !(proxy.UserFirstName .Equals( dup.UserFirstName))
                || !(proxy.UserLastName .Equals( dup.UserLastName))
                || !(proxy.UserID.Equals(dup.UserID))
                || !(proxy.DateBegin.Equals(dup.DateBegin))
                || !(proxy.DateEnd.Equals(dup.DateEnd))
                || !(proxy.KemasBookingID.Equals(dup.KemasBookingID))
                || !(proxy.KemasBookingNumber.Equals(dup.KemasBookingNumber))
                || !(proxy.TotalAmount.Equals( dup.TotalAmount)
                //|| !(proxy.State.Equals(dup.State))
                )
              )
            {
                IsOutOFDate = true;
            }

            return IsOutOFDate;
        }

        public static bool IsValidProxyBookingID(int ProxyBookingID)
        {
            bool IsValid = true;

            if (ProxyBookingID < 1)
            {
                IsValid = false;
            }

            return IsValid;
        }

        public static int[] SplitFapiaoSourceToArray(string fapiaoSourcesStr)
        {
            string[] sources = null;
            List<int> sourcesBE = new List<int>();

            if (!string.IsNullOrEmpty(fapiaoSourcesStr) && !string.IsNullOrEmpty(fapiaoSourcesStr.Trim()))
            {
                sources = fapiaoSourcesStr.Trim().Split(',');
            }

            foreach (string source in sources)
            {
                if (Enum.IsDefined(typeof(FapiaoSource), source))
                {
                    throw new VrentApplicationException(ErrorConstants.BadFapiaoSourceCode, string.Format(ErrorConstants.BadFapiaoSourceMessage), ResultType.VRENTFE);
                }

                sourcesBE.Add((int)Enum.Parse(typeof(FapiaoSource), source));
            }

            return sourcesBE.ToArray();
        }


        #endregion

        #region Kemas Pricing

        public static void SyncPricingFromBooking(ProxyReservation booking,ProxyBookingPrice pbp) 
        {
            //set system columns to proper values
            pbp.CreatedOn = booking.CreatedOn;
            pbp.CreatedBy = booking.CreatedBy;

            if (booking.ProxyBookingID > 0)
            {
                // update booking
                pbp.ProxyBookingID = booking.ProxyBookingID;
                pbp.ModifiedOn = booking.ModifiedOn;
                pbp.ModifiedBy = booking.ModifiedBy;
            }
            else
            {
                //create booking
                pbp.ProxyBookingID = -1;
            }


            foreach (ProxyPrincingItem item in pbp.PrincingItems)
            {
                item.CreatedOn = booking.CreatedOn;
                item.CreatedBy = booking.CreatedBy;
            }
        }

        public static bool IsPricingConsistent(Reservation kemasReserve,ref ProxyBookingPrice pbp,int throwPricingException)
        {
            bool isConsistent = false;
            try
            {
                IPricingFactory factory = new PricingProcessor(kemasReserve.PriceDetail);
                factory.Process();
                pbp = BLLPrincingHelper.ConvertFromFEPriceInfo(factory.Price);
            }
            catch(VrentApplicationException vae)
            {
                LogInfor.WriteInfo(vae.ErrorCode,vae.ErrorMessage,string.Empty);
            }

            if (pbp != null && decimal.Parse(kemasReserve.Price) == pbp.Total && pbp.Total == pbp.PrincingItems.Sum(m => m.Total))
            {
                isConsistent = true;
            }
            else
            {
                //0 logging ony, 1: throw

                string pricingMsg = string.Format(ErrorConstants.BadPricingfieldsMessage, kemasReserve.ID, kemasReserve.State, kemasReserve.PriceDetail, kemasReserve.Price);
                if (throwPricingException == DefaultThrowPricingFlag)
                {
                    LogInfor.WriteInfo(ErrorConstants.BadPricingfieldsCode, pricingMsg, kemasReserve.Creator.ID.ToString());
                }
                else
                {
                    throw new VrentApplicationException(ErrorConstants.BadPricingfieldsCode, pricingMsg, ResultType.Pricing);
                }
            }
            return isConsistent;

        }

        public static bool IsPricingConsistent(ProxyReservation booking, ref ProxyBookingPrice pbp, int throwPricingException)
        {
            bool isConsistent = false;
            try
            {
                IPricingFactory factory = new PricingProcessor(booking.PricingDetail);
                factory.Process();
                pbp = BLLPrincingHelper.ConvertFromFEPriceInfo(factory.Price);
            }
            catch (VrentApplicationException vae)
            {
                LogInfor.WriteInfo(vae.ErrorCode, vae.ErrorMessage, string.Empty);
            }

            if (booking.TotalAmount == pbp.Total && pbp.Total == pbp.PrincingItems.Sum(m => m.Total))
            {
                isConsistent = true;
            }
            else
            {
                string pricingMsg = string.Format(ErrorConstants.BadPricingfieldsMessage, booking.KemasBookingID, booking.PricingDetail, booking.TotalAmount);
                if (throwPricingException == DefaultThrowPricingFlag)
                {
                    LogInfor.WriteInfo(ErrorConstants.BadPricingfieldsCode, pricingMsg, booking.UserID.ToString());
                }
                else
                {
                    throw new VrentApplicationException(ErrorConstants.BadPricingfieldsCode, pricingMsg, ResultType.Pricing);
                }
            }

            return isConsistent;

        }


        #endregion

    }
}
