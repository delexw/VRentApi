using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace ProxyTest
{
    public class HttpWebClient : WebClient
    {
        /// <summary>
        /// CookieContainer
        /// </summary>
        //private static CookieContainer _CookieContainer = new CookieContainer();

        //protected override WebRequest GetWebRequest(Uri address)
        //{
        //    WebRequest request = base.GetWebRequest(address);

        //    if (request is HttpWebRequest)
        //    {
        //        HttpWebRequest httpRequest = request as HttpWebRequest;
        //        httpRequest.CookieContainer = _CookieContainer;
        //        _CookieContainer.SetCookies(new Uri(UnitTestConfiguration.GetHostName()), "ASP.NET_SessionId=zev02g55ynp5kp55xm3ihj45");
        //    }
        //    return request;
        //}
    }


}
