using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Net;
using System.Web;
using CF.VRent.Log;
using CF.VRent.Common.Entities;
using CF.VRent.Common;

namespace CF.VRent.WCFExtension
{

    
    /// <summary>
    /// Extension for WCF security
    /// </summary>
    public class MyServiceAuthorizationManager : ServiceAuthorizationManager
    {
        /// <summary>
        /// Override the exist method to check if the session is exist
        /// </summary>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            var ctx = WebOperationContext.Current;
            var auth = ctx.IncomingRequest.Headers[HttpRequestHeader.Authorization];

            var operationUrlName = ctx.IncomingRequest.UriTemplateMatch.Template.ToString();
            var httpMethod = ctx.IncomingRequest.Method.ToUpper();
            var operationUrlNameGroups = new List<string>() { 
                "POST|Login","POST|LoginNew","POST|Register?tcId={tcId}","POST|ForgotPwd","POST|Users?tcId={tcId}&Lang={lang}","GET|Login","GET|Ping","GET|TermsCondition?tcType={type}&isIncludeContent={isIncludeContent}"
            };
            var op = String.Format("{0}|{1}", httpMethod, operationUrlName);
            if (operationUrlNameGroups.Contains(op))
            {
                if (!op.Equals("GET|Login") || 
                    (op.Equals("GET|Login") && HttpContext.Current != null && HttpContext.Current.Session["UserSetting"]!=null))
                return true;
            }

            var webEx = new WebFaultException<ReturnResult>(new ReturnResult()
            {
                Success = 0,
                Code = MessageCode.CVCE000002.ToString(),
                Message = MessageCode.CVCE000002.GetDescription(),
                Type = MessageCode.CVCE000002.GetMessageType()
            }, HttpStatusCode.Unauthorized);

            if (HttpContext.Current == null || HttpContext.Current.Session["UserSetting"] == null)
            {
                ctx.OutgoingResponse.StatusCode = HttpStatusCode.Unauthorized;
                ctx.OutgoingResponse.StatusDescription = MessageCode.CVCE000002.GetDescription();

                
                //LogInfor.WriteError("Access Denied, Please login first.", webEx.Detail, "unknown");
                throw webEx;
            }
            else if (HttpContext.Current != null)
            {
                var userSession = HttpContext.Current.Session["UserSetting"];
                
                //advanced validation
                //fix: 
                //after a user who have lower permission has logined the app, 
                //then he can access apis to do what he doesn't have permission to do
                if (ctx.IncomingRequest.Headers["userid"] != null)
                {
                    var userId = userSession.GetType().GetProperty("ID").GetValue(userSession, null).ToString();
                    if (userId.Trim() != ctx.IncomingRequest.Headers["userid"].Trim())
                    {
                        throw webEx;
                    }
                    operationContext.IncomingMessageProperties.Add("_user", userId);
                }
                else
                {
                    if (operationUrlName == "LoginNew" && httpMethod == "GET")
                    {
                        return true;
                    }
                    else
                    {
                        throw webEx; 
                    }
                }
                
            }

            return true;
        }
    }
}
