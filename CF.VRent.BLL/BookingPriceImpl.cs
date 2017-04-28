using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.Contract;
using CF.VRent.Entities;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.KEMASWSIF_RESERVATIONRef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace CF.VRent.BLL
{
    public class BookingPriceImpl : AbstractBLL, IBookingPrice
    {
        public BookingPriceImpl(ProxyUserSetting userInfo)
            : base(userInfo) 
        {
        }

        #region Convert a DB princing info to a FE Princing Info
        public static BookingPriceInfo ConvertFromDBPriceInfo(ProxyBookingPrice priceInfo)
        {
            BookingPriceInfo price = null;

            try
            {
                price = new BookingPriceInfo();
                price.TimeStamp = priceInfo.Timestamp;
                price.Total = priceInfo.Total;
                price.ID = priceInfo.TagID;
                price.PreAuth = null;

                List<Item> rentalItems = new List<Item>();

                List<Item> fineItems = new List<Item>();

                foreach (ProxyPrincingItem ppi in priceInfo.PrincingItems)
                {
                    if (ppi.Group == PrincingInfoFactory.RentalFeeCategory)
                    {
                        if (ppi.Category == PrincingInfoFactory.RentalNode)
                        {
                            Item rentalItem = new Item()
                            {
                                Type = ppi.Type,
                                Description = ppi.Description,
                                Total = ppi.Total
                            };
                            if (!string.IsNullOrEmpty(ppi.Description))
                            {
                                List<Period> periodsTemp = new List<Period>();
                                string[] timeParts = ppi.Description.Split(new string[1] { "'" }, StringSplitOptions.None);
                                if (timeParts != null && timeParts.Length > 0)
                                {
                                    List<string> Periods = timeParts.Where((m, n) => n % 2 == 1).ToList();


                                    for (int i = 0; i < Periods.Count / 2; i = i + 1)
                                    {
                                        Period duration = new Period()
                                        {
                                            From = DateTime.Parse(Periods[2 * i]),
                                            To = DateTime.Parse(Periods[2 * i + 1])
                                        };
                                        periodsTemp.Add(duration);
                                    }
                                }

                                rentalItem.Periods = periodsTemp.ToArray();
                            }

                            rentalItems.Add(rentalItem);
                        }
                        else if (ppi.Category == PrincingInfoFactory.FineNode)
                        {
                            Item fineItem = new Item()
                            {
                                Type = ppi.Type,
                                Description = ppi.Description,
                                Total = ppi.Total
                            };
                            fineItems.Add(fineItem);
                        }
                        else if (ppi.Category == PrincingInfoFactory.InsuranceFeeNode)
                        {
                            price.InsuranceFee = new Insurance()
                            {
                                Total = ppi.Total
                            };
                        }
                        else if (ppi.Category == PrincingInfoFactory.FuelNode)
                        {
                            price.Fuel = new FuelFee()
                            {
                                Total = ppi.Total,
                                Kilometer = (decimal)ppi.Quantity
                            };
                        }
                    }
                }

                price.Rental = new RentalFee()
                {
                    Items = rentalItems.ToArray(),
                    Total = rentalItems.Sum(m => m.Total)
                };

                price.Fine = new FineFee()
                {
                    Items = fineItems.ToArray(),
                    Total = fineItems.Sum(m => m.Total)
                };
            }
            catch
            {
                price = null;
            }


            return price;
        }

        #endregion
        #region Convert a FE Princing Info to a DB princing info
        public static ProxyBookingPrice ConvertFromFEPriceInfo(BookingPriceInfo priceInfo)
        {
            ProxyBookingPrice price = null;

            try
            {
                price = new ProxyBookingPrice();


                price.Timestamp = priceInfo.TimeStamp;
                price.Total = priceInfo.Total;
                price.TagID = priceInfo.ID;

                List<ProxyPrincingItem> items = new List<ProxyPrincingItem>();

                items.Add(ConvertFromRental(priceInfo.Rental));
                items.Add(ConvertFromInsuranceFee(priceInfo.InsuranceFee));
                items.Add(ConvertFromFuel(priceInfo.Fuel));
                items.AddRange(ConvertFromFineItems(priceInfo.Fine.Items));

                price.PrincingItems = items.ToArray();
            }
            catch
            {
                price = null;
            }


            return price;
        }

        private static ProxyPrincingItem ConvertFromRental(RentalFee rental)
        {
            return new ProxyPrincingItem()
            {
                Category = PrincingInfoFactory.RentalNode,
                Group = PrincingInfoFactory.RentalFeeCategory,
                Total = rental.Total,
            };
        }

        private static ProxyPrincingItem ConvertFromInsuranceFee(Insurance insurance)
        {
            return new ProxyPrincingItem()
            {
                Category = PrincingInfoFactory.InsuranceFeeNode,
                Group = PrincingInfoFactory.RentalFeeCategory,
                Total = insurance.Total,
            };
        }

        private static ProxyPrincingItem ConvertFromFuel(FuelFee fuel)
        {
            return new ProxyPrincingItem()
            {
                Category = PrincingInfoFactory.FuelNode,
                Group = PrincingInfoFactory.RentalFeeCategory,
                Quantity = fuel.Kilometer,
                Total = fuel.Total,
            };
        }

        private static ProxyPrincingItem[] ConvertFromFineItems(Item[] fineItems)
        {
            List<ProxyPrincingItem> fines = new List<ProxyPrincingItem>();

            foreach (var item in fineItems)
            {
                ProxyPrincingItem ppi = new ProxyPrincingItem()
                {
                    Type = item.Type,
                    Group = PrincingInfoFactory.FineCategory,
                    Description = item.Description,
                    Total = item.Total,
                };

                fines.Add(ppi);
            }

            return fines.ToArray();
        }

        #endregion

        public checkPrice_Response CheckPrice(string uid, BookingSample bookingInfo)
        {
            IKemasReservation kemas = new KemasReservationAPI();
            return kemas.checkPrice(uid, bookingInfo);
        }

        public checkPrice2_Response CheckPriceDetailed(string sessionID, checkPrice2_RequestBookingData bookingInfo)
        {
            IKemasReservation kemas = new KemasReservationAPI();
            return kemas.checkPrice2Advanced(sessionID, bookingInfo);
        }

        public string GetPrice(string BookingID)
        {
            IKemasReservation kemas = new KemasReservationAPI();
            return kemas.getPrice(BookingID);
        }

        public string GetPriceDetailed(string BookingID)
        {
            IKemasReservation kemas = new KemasReservationAPI();
            return kemas.getPriceDetailed(BookingID);
        }

        public getCancelReservationFees_Response getCancelReservationFees(string bookingID, string sessionID)
        {
            IKemasReservation kemas = new KemasReservationAPI();
            return kemas.getCancelReservationFees(bookingID,sessionID);
 
        }

        public ProxyBookingPrice LoadPrincingInfo(int bookingID)
        {
            IDataService client = new DataAccessProxyManager();

            return client.LoadPrincingItems(bookingID);
        }
    }
}
