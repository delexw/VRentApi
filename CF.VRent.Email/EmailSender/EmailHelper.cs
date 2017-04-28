using CF.VRent.Common.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Configuration;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Specialized;
using CF.VRent.Log;
using CF.VRent.Email.EmailSender.Entity;
using CF.VRent.Common;

namespace CF.VRent.Email.EmailSender
{
    /// <summary>
    /// Email operator wrapper
    /// </summary>
    public class EmailHelper:IDisposable
    {
        private MailMessage mailMessage;  
        private SmtpClient smtpClient;
        private string password;
        private string smtpHost;
        public event SendCompletedEventHandler sendComplete;
        
        /// <summary>
        /// Constructor(Multiple toecipients)
        /// </summary>
        /// <param name="From"></param>
        /// <param name="Body"></param>
        /// <param name="Title"></param>
        /// <param name="Password"></param>
        /// <param name="Host"></param>
        /// <param name="To"></param>
        public EmailHelper(string From, string Body, string Title, string Password, string Host, string[] Cc, params string[] To)
        {
            mailMessage = new MailMessage();
            if (To != null)
            {
                foreach (string t in To)
                {
                    if (!String.IsNullOrEmpty(t.Trim()))
                    {
                        mailMessage.To.Add(t);
                    }
                }
            }
            if (mailMessage.To.Count == 0)
            {
                LogInfor.EmailLogWriter.WriteError(MessageCode.CVCE000001.ToStr(), MessageCode.CVCE000001.GetDescription(), "System");
            }
            if (Cc != null)
            {
                foreach (string c in Cc)
                {
                    if (!String.IsNullOrEmpty(c.Trim()))
                    {
                        mailMessage.CC.Add(c);
                    }
                }
            }
            mailMessage.From = new System.Net.Mail.MailAddress(From);
            mailMessage.Subject = Title;
            mailMessage.Body = Body;
            mailMessage.IsBodyHtml = true;
            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            mailMessage.Priority = System.Net.Mail.MailPriority.High;
            this.password = Password;
            this.smtpHost = Host;
        }

        /// <summary>  
        /// Add attatchments  
        /// </summary>    
        public void Attachments(params EmailAttachmentEntity[] attachments)
        {
            System.Net.Mail.Attachment data = null;
            ContentDisposition disposition;
            foreach (EmailAttachmentEntity s in attachments)
            {
                if (s.FileStream != null)
                {
                    data = new Attachment(s.FileStream, s.MimeType);
                }
                else if (!string.IsNullOrWhiteSpace(s.FilePath))
                {
                    data = new Attachment(s.FilePath, s.MimeType);
                }
                else
                {
                    LogInfor.EmailLogWriter.WriteError("Attachment stream or filepath is null", s.ObjectToJson(), "System");
                }
                if (data != null)
                {
                    disposition = data.ContentDisposition;
                    disposition.CreationDate = s.CreatedDate;
                    disposition.ModificationDate = DateTime.Now;
                    disposition.ReadDate = DateTime.Now;
                    disposition.FileName = s.FileName;
                    mailMessage.Attachments.Add(data);
                }
            }
        }

        /// <summary>  
        /// Send the email asynchronously  
        /// </summary>  
        /// <param name="CompletedMethod"></param>  
        public void SendSMTPAsync()  
        {  
            if (mailMessage != null)  
            {
                try
                {
                    smtpClient = new SmtpClient();
                    if (password.ToStr().Trim() != "")
                    {
                        smtpClient.Credentials = new System.Net.NetworkCredential(mailMessage.From.Address, password);//设置发件人身份的票据  
                    }
                    smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtpClient.Host = smtpHost;
                    smtpClient.SendCompleted += smtpClient_SendCompleted;
                    smtpClient.SendAsync(mailMessage, mailMessage.Body);
                }
                catch (Exception ex)
                {
                    new LogHelper().WriteLog(LogType.EXCEPTION, ex.ToString());
                    this.Dispose();
                    if (sendComplete != null)
                    {
                        sendComplete.Invoke(null, null);
                    }
                }
            }  
        }

        void smtpClient_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            this.Dispose();

            if (sendComplete != null)
            {
                sendComplete.Invoke(sender, e);
            }
        }  
        /// <summary>  
        /// Send email synchronously  
        /// </summary>  
        public void SendSMTP()  
        {
            try
            {
                smtpClient = new SmtpClient();
                if (password.ToStr().Trim() != "")
                {
                    smtpClient.Credentials = new System.Net.NetworkCredential(mailMessage.From.Address, password);//Certification
                }
                smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtpClient.Host = smtpHost;
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                new LogHelper().WriteLog(LogType.EXCEPTION, ex.ToString());
            }
            finally
            {
                this.Dispose();

                if (sendComplete != null)
                {
                    sendComplete.Invoke(null, null);
                }
            }
        }

        /// <summary>
        /// Using template
        /// </summary>
        /// <param name="parasMap"></param>
        public void GetEmailTemplate(string templatePath, 
            string parasNodeXPath,
            EmailParameterEntity parasMap, 
            Func<Match, string> imgParasFormat,
            Func<string,AlternateView[]> imgMap)
        {
            HtmlDocument template = new HtmlDocument();
            if (File.Exists(templatePath))
            {
                template.Load(templatePath, Encoding.UTF8, true);
                HtmlNode rootNode = template.DocumentNode;

                var tempHtml = HttpUtility.HtmlDecode(rootNode.InnerHtml);

                HtmlNodeCollection nodes = rootNode.SelectNodes(parasNodeXPath);

                if (parasMap != null)
                {
                    var emailParameterType = parasMap.GetType();
                    foreach (HtmlNode node in nodes)
                    {
                        var match = _GetInnerParasString(HttpUtility.HtmlDecode(node.InnerText));

                        foreach (Match m in match)
                        {
                            var paraName = m.Value.Substring(1, m.Value.Length - 2);
                            var emailParameterProperty = emailParameterType.GetProperty(paraName);
                            if (emailParameterType != null)
                            {
                                tempHtml = tempHtml.Replace(m.Value, emailParameterProperty.GetValue(parasMap, null).ToStr());
                            }
                        }
                    }
                }
                if (imgParasFormat != null)
                {
                    var matchImg = _GetInnerImgString(tempHtml);

                    foreach (Match m in matchImg)
                    {
                        tempHtml = tempHtml.Replace(m.Value, imgParasFormat(m));
                    }
                }

                var views = new AlternateView[] { };

                if (imgMap != null)
                {
                    views = imgMap(tempHtml);
                    foreach (AlternateView view in views)
                    {
                        mailMessage.AlternateViews.Add(view);
                    }
                }
                else
                {
                    mailMessage.Body = tempHtml;
                }
            }
            else
            {
                LogInfor.EmailLogWriter.WriteError(MessageCode.CVCE000010.ToStr(), String.Format(MessageCode.CVCE000010.GetDescription(), templatePath), "System");
            }
            
        }

        private MatchCollection _GetInnerParasString(string sourceStr)
        {
            string pattern = @"<[^<>]*(((?'Open'<)[^<>]*)+((?'-Open'>)[^<>]+))*(?(Open)(?!))>";
            Regex regx = new Regex(pattern);
            MatchCollection mc = regx.Matches(sourceStr);
            return mc;
        }

        private MatchCollection _GetInnerImgString(string sourceStr)
        {
            string pattern = @"\{((?<Open>\{)|(?<-Open>\})|[^{}]+)*(?(Open)(?!))\}";
            Regex regx = new Regex(pattern);
            MatchCollection mc = regx.Matches(sourceStr);
            return mc;
        }

        /// <summary>
        /// Send Email
        /// </summary>
        public void Send(bool isAsync)
        {
            try
            {
                if (mailMessage.To.Count > 0)
                {
                    LogInfor.EmailLogWriter.WriteInfo("Email client is initialized", "", "System");
                    if (!isAsync)
                    {
                        this.SendSMTP();
                    }
                    else
                    {
                        this.SendSMTPAsync();
                    }
                }
            }
            catch (TargetInvocationException e)
            {
                this.Dispose();
                throw e.InnerException;
            }
        }

        public void Dispose()
        {
            if (smtpClient != null)
            {
                smtpClient.Dispose();
                GC.SuppressFinalize(smtpClient);
                smtpClient = null;
                LogInfor.EmailLogWriter.WriteInfo("Email client is disposed", "", "System");
            }
            _clearAttachmentsStream();
        }

        private void _clearAttachmentsStream()
        {
            if (mailMessage != null)
            {
                foreach (Attachment at in mailMessage.Attachments)
                {
                    if (at.ContentStream != null)
                    {
                        at.ContentStream.Close();

                        LogInfor.EmailLogWriter.WriteInfo("Email attachment is disposed", "AttachmentName[" + at.ContentDisposition.FileName + "]", "System");
                    }
                }
            }
        }
    }
}
