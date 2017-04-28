using CF.VRent.Common.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CF.VRent.Common
{

    //<?xml version="1.0" encoding="UTF-8"?>
    //<Price pre-auth="4000.25" timestamp="" id="" total="2800.25">
    //    <Rental total="2100.25">
    //        <item type="business_hours" total="100">
    //            <period from="2013-10-10 18:30:00" to="2013-10-10 20:00:00"/>
    //            <period from="2013-10-11 08:00:00" to="2013-10-11 08:30:00"/>
    //        </item>
    //        <item type="night" from="2013-10-10 18:00:00" to="2010-10-10 20:00:00" total="500"/>
    //        <item type="holiday" from="2013-10-10 18:00:00" to="2010-10-10 20:00:00" total="1500"/>
    //        <item type="weekend" from="2013-10-10 18:00:00" to="2010-10-10 20:00:00" total="0"/>

    //    </Rental>
    //        <InsuranceFee total="50"/>
    //        <Fuel kilometer="20" total="50"/>
    //        <Fine total="600">
    //            <item type="cancel" description="" total=""/>
    //            <item type="late_return" description="" total="500"/>
    //            <item type="shorten" description="" total="100"/>
    //            <item type="over_max_kilometer" description="" total="100"/>
    //        </Fine>
    //</Price>
    public class BookingPriceInfo
    {
        public decimal Total { get; set; }
        public string ID { get; set; }
        public string TimeStamp { get; set; }
        public string PreAuth { get; set; }

        public RentalFee Rental { get; set; }
        public Insurance InsuranceFee { get; set; }
        public FuelFee Fuel { get; set; }

        public FineFee Fine { get; set; }
    }

    public class RentalFee
    {
        public decimal Total { get; set; }
        public Item[] Items { get; set; }
    }

    public class Insurance
    {
        public decimal Total { get; set; }
    }

    public class FuelFee
    {
        public decimal Kilometer { get; set; }
        public decimal Total { get; set; }
    }


    public class FineFee
    {
        public decimal Total { get; set; }

        public Item[] Items { get; set; }
    }

    //public enum Type{Cancel};

    public class Item
    {
        public string Type { get; set; }

        public string Description { get; set; }

        [XmlIgnore]
        public string RawDescription { get; set; }

        public Period[] Periods { get; set; }

        public decimal Total { get; set; }
    }

    public class Period
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }

    public interface IPricingFactory
    {
        BookingPriceInfo Price { get; }
        void Process();
        string PricingXml { get; }
    }

    public class PrincingInfoFactory : IPricingFactory
    {

        public const string RootNodeName = "Price";
        public const string PriceTotal = "TOTAL";
        public const string PriceID = "ID";
        public const string PriceTIMESTAMP = "TIMESTAMP";

        public const string RentalNode = "RENTAL";
        public const string InsuranceFeeNode = "INSURANCEFEE";
        public const string FuelNode = "FUEL";

        public const string FineNode = "FINE";

        public const string ChildNodeTotal = "total";
        public const string ChildNodeType = "type";
        public const string ChildNodeDESCR = "description";

        public const string RentalFeeCategory = "RENTALFEE";
        public const string FineCategory = "FINEFEE";

        private string _xmlStr = null;
        private XmlDocument pricingDoc = null;
        private BookingPriceInfo _bpi = null;



        public BookingPriceInfo Price
        {
            get { return _bpi; }
        }

        public string PricingXml
        {
            get { return _xmlStr; }
        }

        public PrincingInfoFactory(string xmlStr)
        {
            _xmlStr = xmlStr;
        }


        #region dev environemnt
        public void Process()
        {
            pricingDoc = new XmlDocument();
            try
            {
                ReadPrincingString(_xmlStr);
                XmlElement root = pricingDoc.DocumentElement;
                if (root != null)
                {
                    _bpi = new BookingPriceInfo();
                    ProcessPriceNodeAttributes(root, _bpi);


                    ProcessPriceChildNodes(root, _bpi);

                }
            }
            catch (Exception ex)
            {
                throw new VrentApplicationException(PricingProcessor.UnknownErroProcesingPricingCode, string.Format(PricingProcessor.UnknownErroProcesingPricingMessage, ex.Message, ex.StackTrace, _xmlStr), ResultType.VRENT);
            }
        }

        private void ReadPrincingString(string pricingStr)
        {
            pricingDoc.LoadXml(pricingStr);
        }
        private void ProcessPriceNodeAttributes(XmlElement priceNode, BookingPriceInfo bpi)
        {
            foreach (XmlAttribute xa in priceNode.Attributes)
            {
                string attrName = xa.Name.ToUpper();
                string attValue = xa.Value;
                switch (attrName)
                {
                    case PriceTotal:
                        bpi.Total = decimal.Parse(attValue);
                        break;
                    case PriceID:
                        bpi.ID = attValue;
                        break;
                    case PriceTIMESTAMP:
                        bpi.TimeStamp = attValue;
                        break;
                    default:
                        break;
                }
            }
        }

        private void ProcessPriceChildNodes(XmlElement priceNode, BookingPriceInfo bpi)
        {
            foreach (XmlElement xe in priceNode.ChildNodes)
            {
                string attrName = xe.Name.ToUpper();

                switch (attrName)
                {
                    case RentalNode:
                        bpi.Rental = new RentalFee { Total = decimal.Parse(xe.Attributes[ChildNodeTotal].Value) };
                        break;
                    case InsuranceFeeNode:
                        bpi.InsuranceFee = new Insurance { Total = decimal.Parse(xe.Attributes[ChildNodeTotal].Value) };
                        break;
                    case FuelNode:
                        bpi.Fuel = new FuelFee { Total = decimal.Parse(xe.Attributes[ChildNodeTotal].Value) };
                        break;
                    case FineNode:
                        FineFee fineFee = new FineFee()
                        {
                            Total = decimal.Parse(xe.Attributes[ChildNodeTotal].Value)
                        };

                        List<Item> fineItems = new List<Item>();
                        foreach (XmlElement xeChild in xe.ChildNodes)
                        {
                            Item f = new Item()
                            {
                                Total = xeChild.Attributes[ChildNodeTotal] == null ? 0 : decimal.Parse(xeChild.Attributes[ChildNodeTotal].Value),
                                Type = xeChild.Attributes[ChildNodeType] == null ? string.Empty : xeChild.Attributes[ChildNodeType].Value,
                                Description = (xeChild.Attributes[ChildNodeDESCR] == null ? string.Empty : xeChild.Attributes[ChildNodeDESCR].Value)
                            };

                            fineItems.Add(f);
                        }
                        fineFee.Items = fineItems.ToArray();

                        bpi.Fine = fineFee;
                        break;
                }
            }
        }


        #endregion
    }

    public interface IPricingProcessor
    {
        string PricingXml { get; }
        string SchemaFullPath { get; }
        void Process();
        Price PricingData { get; }
    }

    public class PricingProcessor : IPricingFactory
    {
        public const string SchemaRelativePath = "SchemaRelativePath";

        private Price _bookingPrice = null;
        private string _schemaPath = null;
        private string _xmlStr = null;
        private string _scheamFullPath = null;
        private List<string> _validationMsgs = new List<string>();

        private BookingPriceInfo _FEPrice = null;

        public PricingProcessor(string xmlStr)
        {
            _xmlStr = xmlStr;

            if (ConfigurationManager.AppSettings.Count > 0 && ConfigurationManager.AppSettings[SchemaRelativePath] != null)
            {
                _schemaPath = ConfigurationManager.AppSettings[SchemaRelativePath];
            }

            _scheamFullPath = RetrieveSchemaPath();
        }

        private string RetrieveSchemaPath()
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

            return Path.Combine(folder, _schemaPath);
        }

        public const string UnknownErroProcesingPricingCode = "CVC000101";
        public const string UnknownErroProcesingPricingMessage = "Error {0}-{1} occurred when processing the pricing xml{2}.";
        public const string ScheamFileNotExistCode = "CVC000102";
        public const string ScheamFileNotExistMessage = "The pricing Schema {0} is missing.";
        public const string ValidationAgainstScheamFailedCode = "CVC000103";
        public const string ValidationAgainstScheamFailedMessage = "Validate Xml {0} aginst scheam Failed {1}";

        #region Product Environment

        public void ProcessWithValidation()
        {
            FileStream schemaStream = null;
            XmlReader schemareader = null;

            XmlSerializer xs = null;

            XmlReaderSettings xmlSettings = null;

            MemoryStream ms = null;
            XmlReader reader = null;
            if (File.Exists(_scheamFullPath))
            {
                try
                {
                    schemaStream = new FileStream(_scheamFullPath, FileMode.Open);
                    schemareader = XmlReader.Create(schemaStream);
                    XmlSchemaSet schemas = new XmlSchemaSet();
                    schemas.Add(string.Empty, schemareader);

                    xmlSettings = new XmlReaderSettings();
                    xmlSettings.CloseInput = true;

                    xmlSettings.Schemas = schemas;
                    xmlSettings.ValidationType = ValidationType.Schema;
                    xmlSettings.ValidationEventHandler += xmlSettings_ValidationEventHandler;

                    xmlSettings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings;

                    xs = new XmlSerializer(typeof(Price));
                    byte[] pricingStream = Encoding.UTF8.GetBytes(_xmlStr);

                    ms = new MemoryStream(pricingStream);
                    reader = XmlReader.Create(ms, xmlSettings);

                    _bookingPrice = xs.Deserialize(reader) as Price;


                }
                catch (Exception ex)
                {
                    throw new VrentApplicationException(UnknownErroProcesingPricingCode, string.Format(UnknownErroProcesingPricingMessage, ex.Message, ex.StackTrace, _xmlStr), ResultType.VRENT);
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
                        xmlSettings.ValidationEventHandler -= xmlSettings_ValidationEventHandler;
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
                throw new VrentApplicationException(ScheamFileNotExistCode, string.Format(ScheamFileNotExistMessage, _scheamFullPath), ResultType.VRENT);
            }

        }



        private void xmlSettings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Warning:
                    _validationMsgs.Add(e.Message);
                    break;
                case XmlSeverityType.Error:
                    _validationMsgs.Add(e.Message);
                    break;
                default:
                    break;
            }
        }

        private BookingPriceInfo ConvertPriceToBookingPriceInfo()
        {
            BookingPriceInfo bpi = new BookingPriceInfo();

            bpi.ID = _bookingPrice.id;
            bpi.TimeStamp = _bookingPrice.timestamp;
            bpi.Total = (decimal)_bookingPrice.total;
            bpi.PreAuth = _bookingPrice.preauth;

            if (_bookingPrice.Fine != null)
            {
                bpi.Fine = ConvertFromFineToFineFee(_bookingPrice.Fine);
            }

            if (_bookingPrice.Fuel != null)
            {
                bpi.Fuel = ConvertFromFuelTofuelFee(_bookingPrice.Fuel);
            }

            if (_bookingPrice.InsuranceFee != null)
            {
                bpi.InsuranceFee = ConvertFromInsuranceFeeToInsuranceFee(_bookingPrice.InsuranceFee);
            }

            if (_bookingPrice.Rental != null)
            {
                bpi.Rental = ConvertFromRentalFeeToRental(_bookingPrice.Rental);
            }

            return bpi;
        }

        private FineFee ConvertFromFineToFineFee(PriceFine pf)
        {
            FineFee ff = null;
            if (pf != null)
            {
                ff = new FineFee();
                ff.Total = (decimal)pf.total;

                List<Item> fineItems = new List<Item>();
                if (pf.item != null)
                {
                    foreach (PriceFineItem item in pf.item)
                    {
                        Item fineItem = new Item()
                        {
                            Type = item.type,
                            Description = item.description,
                            Total = (decimal)item.total
                        };
                        fineItems.Add(fineItem);
                    }
                    ff.Items = fineItems.ToArray();
                }
            }

            return ff;
        }

        private FuelFee ConvertFromFuelTofuelFee(PriceFuel pf)
        {
            FuelFee ff = null;
            if (pf != null)
            {
                ff = new FuelFee()
                {
                    Total = (decimal)pf.total,
                    Kilometer = pf.kilometer
                };
            }

            return ff;
        }

        private Insurance ConvertFromInsuranceFeeToInsuranceFee(PriceInsuranceFee pif)
        {
            Insurance i = null;
            if (pif != null)
            {
                i = new Insurance()
                {
                    Total = (decimal)pif.total
                };
            }

            return i;
        }

        private Period ConvertFromRawPeriodTo(PriceRentalItemPeriod prip)
        {
            return new Period()
            {
                From = DateTime.Parse(prip.from),
                To = DateTime.Parse(prip.to)
            };
        }

        private const string RawPeriodPattern = "<period from='{0}' to='{1}'/>";
        private RentalFee ConvertFromRentalFeeToRental(PriceRental pr)
        {
            RentalFee rf = null;
            if (pr != null)
            {
                rf = new RentalFee();
                rf.Total = (decimal)pr.total;


                if (pr.item != null && pr.item.Length > 0)
                {
                    List<Item> rentalItems = new List<Item>();

                    foreach (var prItem in pr.item)
                    {
                        Item rentalItem = new Item();
                        rentalItem.Type = prItem.type;


                        StringBuilder sb = new StringBuilder();
                        List<Period> periods = new List<Period>();
                        //handling periods subnode
                        if (prItem.period != null && prItem.period.Length > 0)
                        {
                            foreach (var period in prItem.period)
                            {
                                periods.Add(ConvertFromRawPeriodTo(period));
                                sb.Append(string.Format(RawPeriodPattern, period.from, period.to));
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(prItem.from) || string.IsNullOrEmpty(prItem.to))
                            {
                                rentalItem.Description = null;
                            }
                            else
                            {
                                //temp format
                                string periodStr = string.Format(RawPeriodPattern, prItem.from, prItem.to);

                                PriceRentalItemPeriod prip = new PriceRentalItemPeriod() { from = prItem.from, to = prItem.to };
                                periods.Add(ConvertFromRawPeriodTo(prip));
                                sb.Append(periodStr);
                            }
                        }
                        rentalItem.Periods = periods.ToArray();

                        rentalItem.RawDescription = sb.ToString();

                        rentalItem.Total = (decimal)prItem.total;
                        rentalItems.Add(rentalItem);
                    }

                    rf.Items = rentalItems.ToArray();
                }
            }

            return rf;
        }


        #endregion

        public string PricingXml
        {
            get { return _xmlStr; }
        }

        public string SchemaFullPath
        {
            get { return _scheamFullPath; }
        }

        public void Process()
        {
            ProcessWithValidation();
            if (_bookingPrice != null)
            {
                //if (_validationMsgs.Count > 0)
                //{
                //    string msg = string.Join("|||",_validationMsgs);
                //    throw new VrentApplicationException(ValidationAgainstScheamFailedCode, string.Format(ValidationAgainstScheamFailedMessage, _xmlStr, msg), ResultType.VRENT);
                //}
                //else
                //{
                _FEPrice = ConvertPriceToBookingPriceInfo();
                //}
            }
        }

        public Price PricingData
        {
            get { return _bookingPrice; }
        }

        public BookingPriceInfo Price
        {
            get { return _FEPrice; }
        }
    }


}
