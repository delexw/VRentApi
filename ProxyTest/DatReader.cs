using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ProxyTest
{
    public class UnitTestConfiguration
    {
        public static string GetHostName()
        {
            // return "http://172.21.216.21:8080";

            return "http://booking-vrent-dev.mcon.net/api";
        }
    }
    public class DatReader
    {
        public static string Read(string name) 
        {
            StreamReader sr = new StreamReader(name);
            try
            {
                string data = sr.ReadToEnd();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sr.Close();
            }

        }
    }
}
