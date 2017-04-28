using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CF.VRent.Common
{
    /// <summary>
    /// description: Object To Json
    /// author: liuyang
    /// date: 2014-03-27
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// DataTable To Json
        /// Extend DataTable Obejct
        /// </summary>
        /// <param name="p_SourceDB">Source DataTable Instance</param>
        /// <returns>Json</returns>
        public static string DataTableToJson(this DataTable p_SourceDB)
        {
           return JsonConvert.SerializeObject(p_SourceDB,new DataTableConverter());
        }

        public static string ObjectToJson(this Object p_SourceObject)
        {
            return JsonConvert.SerializeObject(p_SourceObject, new JavaScriptDateTimeConverter());
        }

        public static T JsonDeserialize<T>(this string p_JsonStr)
        {
            return JsonConvert.DeserializeObject<T>(p_JsonStr, new JavaScriptDateTimeConverter());
        }

        public static T JsonDeserializeAnony<T>(this string p_JsonStr,T p_anonymousType)
        {
            return JsonConvert.DeserializeAnonymousType(p_JsonStr, p_anonymousType);
        }

        public static object JsonDeserialize(this string p_JsonStr)
        {
            return JsonConvert.DeserializeObject(p_JsonStr);
        }

        public static string orderJson(this List<Hashtable> p_source)
        {
            return p_source.ObjectToJson();
        }

        /// <summary>
        /// Query in Requset to Json
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static string QueryToJson(this string query)
        {
            if (query != "")
            {
                var paras = new Hashtable();
                var start = query.Substring(0, 1) == "?" ? query.Substring(1) : query;
                var parasSplit = start.Split('&');

                foreach (string s in parasSplit)
                {
                    var paraSplit = s.Split('=');
                    paras.Add(paraSplit[0], paraSplit[1]);
                }

                return paras.ObjectToJson();
            }
            return "";
        }

        public static T ReadFromFile<T>(string filePath) where T : new()
        {
            if (File.Exists(filePath))
            {
                byte[] conByte = new byte[] { };
                using (FileStream fs = File.OpenRead(filePath))
                {
                    fs.Read(conByte, 0, Convert.ToInt32(fs.Length));
                }
               return Encoding.UTF8.GetString(conByte).JsonDeserialize<T>();
            }
            else
            {
                return new T();
            }
        }
    }
}
