using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace CF.VRent.Common
{
    public interface IHttpClientHelper
    {
        string Url { get; set; }
        bool IsSessionKeeping { get; }

        T SendGetRequest<T>(Dictionary<string, string> headers = null) where T : new();
        V SendPostRequest<T, V>(T postData, Dictionary<string, string> headers = null)
            where T : new()
            where V : new();
        T SendDeleteRequest<T>(Dictionary<string, string> headers = null) where T : new();
        void BeginSession();
        void EndSession();
    }

    public class HttpClientHelper : IHttpClientHelper
    {
        private string _url;
        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                _url = value;
            }
        }

        private WebResponse _resp;


        public HttpClientHelper()
        {
            _sessionCookie = new CookieContainer();
        }

        public T SendGetRequest<T>(Dictionary<string, string> headers = null) where T : new()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Url);

            request.Method = "GET";

            if (headers != null)
            {
                foreach (var kv in headers)
                {
                    request.Headers.Add(kv.Key, kv.Value);
                }
            }

            if (_isSessionKeeping)
            {
                request.CookieContainer = _sessionCookie;
            }

            _resp = request.GetResponse();

            var resStr = _readResponse();

            if (resStr != null)
            {
                return resStr.JsonDeserialize<T>();
            }
            return new T();
        }

        public V SendPostRequest<T, V>(T postData, Dictionary<string, string> headers = null)
            where T : new()
            where V : new()
        {

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Url);

            request.Method = "POST";
            request.ContentType = "application/json";

            if (headers != null)
            {
                foreach (var kv in headers)
                {
                    request.Headers.Add(kv.Key, kv.Value);
                }
            }

            if (_isSessionKeeping)
            {
                request.CookieContainer = _sessionCookie;
            }

            string postValue = postData.ObjectToJson();

            byte[] postBytes = Encoding.UTF8.GetBytes(postValue);

            request.ContentLength = postBytes.Length;

            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(postBytes, 0, postBytes.Length);
            }

            _resp = request.GetResponse();

            var resStr = _readResponse();

            if (resStr != null)
            {
                return resStr.JsonDeserialize<V>();
            }
            return new V();
        }



        private string _readResponse()
        {
            using (_resp)
            {
                using (Stream repStream = _resp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(repStream, Encoding.UTF8);
                    StringBuilder responseData = new StringBuilder();
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        responseData.Append(line);
                    }
                    if (!String.IsNullOrWhiteSpace(responseData.ToString()))
                    {
                        return responseData.ToString();
                    }
                }
            }
            return null;
        }


        public T SendDeleteRequest<T>(Dictionary<string, string> headers = null) where T : new()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Url);
            request.Method = "DELETE";

            if (headers != null)
            {
                foreach (var kv in headers)
                {
                    request.Headers.Add(kv.Key, kv.Value);
                }
            }

            if (_isSessionKeeping)
            {
                request.CookieContainer = _sessionCookie;
            }

            _resp = request.GetResponse();

            var resStr = _readResponse();

            if (resStr != null)
            {
                return resStr.JsonDeserialize<T>();
            }
            return new T();
        }

        private CookieContainer _sessionCookie;

        private bool _isSessionKeeping;
        public bool IsSessionKeeping
        {
            get { return _isSessionKeeping; }
        }

        public void BeginSession()
        {
            _isSessionKeeping = true;
        }

        public void EndSession()
        {
            _isSessionKeeping = false;
        }
    }

    public class HttpClientHelperFactory
    {
        public static IHttpClientHelper GetHelperInstance<T>(string url) where T : IHttpClientHelper
        {
            IHttpClientHelper helper = Activator.CreateInstance<T>();
            helper.Url = url;
            return helper;
        }
    }
}
