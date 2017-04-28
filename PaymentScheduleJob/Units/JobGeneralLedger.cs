using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.Job.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using VRentDJ.Utility;
using CF.VRent.Common;
using CF.VRent.Log;
using CF.VRent.Log.ConcreteLog;
using CF.VRent.BLL.BLLFactory;
using CF.VRent.SAPSDK;
using CF.VRent.Entities.DataAccessProxy;
using System.IO;
using CF.VRent.SAPSDK.Interfaces;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace VRentDJ.Units
{
    public class JobGeneralLedger : JobUnit
    {
        public string UserName { get; set; }
        public string UserPwd { get; set; }

        /// <summary>
        /// A flag to control ccb general ledger
        /// </summary>
        public static bool CCBGL_IsAllowed = false;

        private int _type;

        public override void Init()
        {
            if (Parameters != null)
            {
                UserName = Parameters["userName"];
                UserPwd = Parameters["userPwd"];
                _type = Parameters["type"].ToInt();
            }
        }

        public override void Invoke(NameValueCollection containerParameters = null)
        {
            ProxyUserSetting setting = KemasLogin.Login(UserName, UserPwd);
            //Login user
            if (setting == null)
            {
                throw new FaultException<ReturnResult>(new ReturnResult()
                {
                    Code = MessageCode.CVCE000006.ToString(),
                    Message = MessageCode.CVCE000006.GetDescription(),
                    Type = ResultType.VRENT
                }, MessageCode.CVCE000006.GetDescription());
            }

            var utility = new RemoteConnUtility();
            var sapSDK = SAPContext.CreateManager();
            try
            {
                var glBLL = ServiceImpInstanceFactory.CreateGeneralLedgerInstance(setting);
                
                //Connect to remote shared folder
                int status = utility.Connect(sapSDK.Common[SAPSDKConstants.RemoteSharedFolder],
                    sapSDK.Common[SAPSDKConstants.LocalMapDeviceName],
                    sapSDK.Common[SAPSDKConstants.RemoteUserName],
                    sapSDK.Common[SAPSDKConstants.RemoteUserPassword]);

                if (utility.IsConnected)
                {
                    if (_type == 1)
                    {
                        #region DUB
                        //DUB
                        //Append today's ledger to today's temporary cvs file
                        //Send it to romote shared folder
                        var start = DateTime.Now.Date;
                        var end = DateTime.Now;
                        //use the started time of job as end time
                        if (containerParameters != null)
                        {
                            end = DateTime.Now.Date.Add(TimeSpan.Parse(containerParameters["jobTime"]));
                        }


                        var headerId = glBLL.AddGeneralLedgerHeader(new GeneralLedgerHeader()
                        {
                            CreatedBy = glBLL.UserInfo.ID.ToGuidNull(),
                            PostingFrom = start,
                            PostingEnd = end,
                            HeaderType = VRentDataDictionay.GLHeaderType.DUB
                        });

                        var lines = glBLL.GenerateDUBLedger(headerId, start, end);

                        //group by CompanyCode
                        var groupLines = lines.GroupBy(l => l.CompanyCode);

                        //For each group
                        Parallel.ForEach(groupLines, r =>
                        {
                            sapSDK.FileName[SAPSDKConstants.CompanyCode] = r.Key;
                            using (var fs = this._openCSVFile(sapSDK))
                            {
                                //Write header and lines to file
                                using (StreamWriter stream = new StreamWriter(fs))
                                {
                                    if (fs.Length == 0)
                                    {
                                        sapSDK.Header[SAPSDKConstants.PostingDate] = end.Date.ToString(sapSDK.Common[SAPSDKConstants.DateFormat]);
                                        sapSDK.Header[SAPSDKConstants.DocumentDate] = sapSDK.Header[SAPSDKConstants.PostingDate];
                                        sapSDK.Header[SAPSDKConstants.CompanyCode] = r.Key;
                                        sapSDK.Header[SAPSDKConstants.Reference] = "E" + sapSDK.Header[SAPSDKConstants.PostingDate];
                                        stream.WriteLine(sapSDK.Header.FormatEntity);
                                    }
                                    //For each line
                                    Parallel.ForEach(r, s =>
                                    {
                                        if (!String.IsNullOrWhiteSpace(s.PostingBody))
                                        {
                                            stream.WriteLine(s.PostingBody);
                                        }
                                    });

                                    stream.Flush();
                                    stream.Close();
                                }
                            }

                        }); 
                        #endregion
                    }
                    else if (_type == 2 && CCBGL_IsAllowed)
                    {
                        #region CCB
                        //CCB
                        //Send it to remote shared folder directly
                        var now = DateTime.Now;
                        var start = new DateTime(now.AddMonths(-1).Year, now.AddMonths(-1).Month, 1);
                        var end = new DateTime(now.Year, now.Month, 1);

                        var headerId = glBLL.AddGeneralLedgerHeader(new CF.VRent.Entities.DataAccessProxy.GeneralLedgerHeader()
                        {
                            CreatedBy = glBLL.UserInfo.ID.ToGuidNull(),
                            PostingFrom = start,
                            PostingEnd = end,
                            HeaderType = VRentDataDictionay.GLHeaderType.CCB
                        });

                        var lines = glBLL.GenerateCCBLedger(headerId, start, end);

                        //group by CompanyCode
                        var groupLines = lines.GroupBy(l => l.CompanyCode);

                        //For each group
                        Parallel.ForEach(groupLines, r =>
                        {
                            sapSDK.FileName[SAPSDKConstants.CompanyCode] = r.Key;
                            using (var fs = this._openCSVFile(sapSDK))
                            {
                                //Write header and lines to file
                                using (StreamWriter stream = new StreamWriter(fs))
                                {
                                    if (fs.Length == 0)
                                    {
                                        sapSDK.Header[SAPSDKConstants.PostingDate] = now.Date.ToString(sapSDK.Common[SAPSDKConstants.DateFormat]);
                                        sapSDK.Header[SAPSDKConstants.DocumentDate] = sapSDK.Header[SAPSDKConstants.PostingDate];
                                        sapSDK.Header[SAPSDKConstants.CompanyCode] = r.Key;
                                        sapSDK.Header[SAPSDKConstants.Reference] = "E" + sapSDK.Header[SAPSDKConstants.PostingDate];
                                        stream.WriteLine(sapSDK.Header.FormatEntity);
                                    }
                                    //For each line
                                    Parallel.ForEach(r, s =>
                                    {
                                        if (!String.IsNullOrWhiteSpace(s.PostingBody))
                                        {
                                            stream.WriteLine(s.PostingBody);
                                        }
                                    });

                                    stream.Close();
                                }
                            }

                        }); 
                        #endregion
                    }
                }
                else
                {
                    LogInfor.GetLogger<GeneralLedgerLogger>().WriteError("Remote server connection error", "ErrorID:" + status, "Schedule job");
                }
            }
            catch (Exception ex)
            {
                LogInfor.GetLogger<GeneralLedgerLogger>().WriteError("Generate ledger failed", ex.ToStr(), "Schedule job");
            }
            finally
            {
                if (utility.IsConnected)
                {
                    //disconnect remote shared folder
                    utility.Disconnect(sapSDK.Common[SAPSDKConstants.LocalMapDeviceName]);
                }
            }

        }
        /// <summary>
        /// Get a file stream 
        /// </summary>
        /// <param name="sap"></param>
        /// <returns></returns>
        private FileStream _openCSVFile(ISAPManager sap)
        {
            var fileName = sap.Common[SAPSDKConstants.LocalMapDeviceName] + @"\" + sap.FileName.FormatEntity + sap.Common[SAPSDKConstants.FileExtension];

            if (File.Exists(fileName))
            {
                FileStream fs = new FileStream(fileName, FileMode.Append);
                return fs;
            }
            else
            {
                FileStream fs = new FileStream(fileName, FileMode.CreateNew);
                return fs;
            }
        }

        public override void Finish()
        {

        }
    }
}
