using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.Common;
using CF.VRent.Job.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using VRentDJ.Utility;
using System.Collections.Specialized;
using CF.VRent.BLL;
using CF.VRent.Log;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.Email;
using CF.VRent.Email.EmailSender;
using System.IO;
using System.Net.Mime;
using CF.VRent.Entities.BigFileService;
using CF.VRent.Email.EmailSender.Entity;
using CF.VRent.Entities.EntityFactory;
using CF.VRent.Entities;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using System.Threading.Tasks;
using CF.VRent.Job;
using System.Configuration;

namespace VRentDJ.Units
{
    public class JobDebitNote : JobUnit
    {
        public string UserName { get; set; }
        public string UserPwd { get; set; }

        private static int _previewDay;
        private static bool _previewIsRunning = false;

        private int _time;

        public override void Init()
        {
            if (Parameters != null)
            {
                UserName = Parameters["userName"];
                UserPwd = Parameters["userPwd"];
                _time = Parameters["time"].ToInt();
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

            //Get debit note

            var debitManager = new DebitNoteJob();

            try
            {
                debitManager.UserInfo = setting;

                int previewDay = 1;
                //the interval days between the preview and the final debit note
                int finalDayInterval = this.Parameters["debitDayInterval"].ToInt();
                //the month in which system aggregates all bookings to report debit note 
                int debitMonth = this.Parameters["debitMonth"].ToInt();

                if (containerParameters.AllKeys.Contains(JobDictionary.JOB_INTERNAL_TIMER_PATTERN))
                {
                    if (containerParameters.AllKeys.Contains(JobDictionary.JOB_INTERNAL_TIMER_TIME))
                    {
                        if (_time == 1)
                        {
                            //the occasion in which the preview debit note is created
                            previewDay = DateTime.Now.Day;
                            _previewDay = previewDay;
                            _previewIsRunning = true;
                            debitManager.GeneateDebitNotes(previewDay, finalDayInterval, debitMonth);
                            _previewIsRunning = false;
                        }
                        else if (_time == 2)
                        {
                            //the occasion in which the final debit note is created
                            if (_previewDay > 0 && !_previewIsRunning &&
                                DateTime.Now.Day == DebitNoteUtility.CalculateDateForFinalJob(new DateTime(DateTime.Now.Year, DateTime.Now.Month, _previewDay).Date, finalDayInterval).Day)
                            {
                                previewDay = _previewDay;
                                debitManager.GeneateDebitNotes(previewDay, finalDayInterval, debitMonth);
                                _previewDay = 0;
                                //Couple with GL
                                JobGeneralLedger.CCBGL_IsAllowed = true;
                            }
                            else
                            {
                                //exit job
                                return;
                            }
                        }
                    }
                }

                var dataManager = new BigFileServerManager();
                var dic = debitManager.ExcelFiles;

                //Loop each of client with its excel
                Parallel.ForEach(dic, r =>
                {
                    //excel file - r.Value
                    if (File.Exists(r.Value))
                    {
                        using (var testStream = File.OpenRead(r.Value))
                        {
                            var emailSender = EmailSenderFactory.CreateDebitNoteCreatedSender();

                            //send email to internal user
                            if (_time == 1)
                            {
                                //Send to vw internal user
                                emailSender.onSendEvent += (arg1, arg2, arg3) =>
                                {
                                    CF.VRent.Entities.BigFileService.EmailProxyParameter proxyPara = new CF.VRent.Entities.BigFileService.EmailProxyParameter()
                                    {
                                        Attachment = new EmailAttachmentEntity()
                                        {
                                            CreatedDate = DateTime.Now,
                                            FileName = r.Value.Substring(r.Value.LastIndexOf('\\') + 1),
                                            MimeType = MediaTypeNames.Application.Octet
                                        },
                                        ContentParameter = arg1,
                                        EmailAddresses = arg3,
                                        EmailType = arg2.ToStr(),
                                        FileStream = testStream,
                                        GroupType = EmailConstants.DebitNoteInternalUserKey
                                    };
                                    dataManager.SendEmailWithAttachments(proxyPara);
                                };
                                emailSender.Send(new EmailParameterEntity() {
                                    //Parameters are used to replace varaibles in tempalte
                                    FirstName = "", // no way to get the internal user name
                                    LastName = "",
                                    DebitNoteDate = DateTime.Now.ToString("MM/yyyy"),
                                    Price = debitManager.DebitNoteTotalPrice[r.Key].ToStr()
                                });
                            }
                            else if (_time == 2)
                            {
                                //Send to VM
                                var api = KemasAccessWrapper.CreateKemasUserAPI2Instance();

                                var page = 0;
                                var pageSize = 10;
                                var userFactory = new UserFactory();
                                //For finding the vm user according to the clientId
                                while (true)
                                {
                                    IEnumerable<UserExtension> userExtension = userFactory.CreateEntity(api.getUsers2(new getUsers2Request()
                                    {
                                        ItemsPerPage = pageSize,
                                        ItemsPerPageSpecified = true,
                                        Page = page,
                                        PageSpecified = true,
                                        SessionID = setting.SessionID,
                                        SearchCondition = new getUsers2RequestSearchCondition()
                                        {
                                            ClientID = r.Key, //r.Key is clientID
                                            Status = "7"
                                        }
                                    }).Users);
                                    if (userExtension == null || userExtension.Count() == 0)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        var vm = userExtension.Where(u => u.RoleEntities.IsVRentManagerUser()).FirstOrDefault();
                                        //found the vm user
                                        if (vm != null)
                                        {
                                            //Send email to VM
                                            emailSender.onSendEvent += (arg1, arg2, arg3) =>
                                            {
                                                CF.VRent.Entities.BigFileService.EmailProxyParameter proxyPara = new CF.VRent.Entities.BigFileService.EmailProxyParameter()
                                                {
                                                    Attachment = new EmailAttachmentEntity()
                                                    {
                                                        CreatedDate = DateTime.Now,
                                                        FileName = r.Value.Substring(r.Value.LastIndexOf('\\') + 1),
                                                        MimeType = MediaTypeNames.Application.Octet
                                                    },
                                                    ContentParameter = arg1,
                                                    EmailAddresses = arg3,
                                                    EmailType = arg2.ToStr(),
                                                    FileStream = testStream
                                                };
                                                dataManager.SendEmailWithAttachments(proxyPara);
                                            };
                                            emailSender.Send(new EmailParameterEntity() {
                                                //Parameters are used to replace varaibles in tempalte
                                                FirstName = vm.Name,
                                                LastName = vm.VName,
                                                DebitNoteDate = DateTime.Now.ToString("MM/yyyy"),
                                                Price = debitManager.DebitNoteTotalPrice[r.Key].ToStr()
                                            }, vm.Mail);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        //Delete excel
                        File.Delete(r.Value);
                    }
                    else
                    {
                        //File is not exited
                        LogInfor.WriteError("Debit note job error", "Can't find file " + r.Value, "Schedule Job");
                    }
                });

                GC.Collect();
                GC.WaitForFullGCComplete();

            }
            catch (FaultException<ReturnResult> ex)
            {
                LogInfor.WriteError("Debit note job error", ex.Detail.ObjectToJson(), "Schedule Job");
            }
            catch (Exception ex)
            {
                LogInfor.WriteError("Debit note job error", ex.ToStr(), "Schedule Job");
            }
        }

        public override void Finish()
        {
        }
    }
}
