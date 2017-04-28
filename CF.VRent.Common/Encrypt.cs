using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace CF.VRent.Common
{
    /// <summary>
    /// Encrypt for the string
    /// </summary>
    public class Encrypt
    {
        /// <summary>
        /// Get the md5 32bit Encrypt
        /// </summary>
        /// <param name="value">orginal value</param>
        /// <returns>value after encrypt</returns>
        public static string EncryptMD5(string value)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
            bytes = md5.ComputeHash(bytes);
            md5.Clear();

            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                ret += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
            }

            return ret.PadLeft(32, '0');
        }

        public static string GetPasswordFormat(string pwd)
        {
            if (!String.IsNullOrWhiteSpace(pwd))
            {
                string newPwd = ":md5:" + Encrypt.EncryptMD5(pwd);
                return newPwd;
            }
            return pwd;
        }
    }
}
