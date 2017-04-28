using System.Web.Configuration;
using System.Configuration;
using System.Collections.Specialized;

namespace CF.VRent.UPSDK.SDK
{
    /// <summary>
    /// Internal config class to get the configuration values from file
    /// </summary>
    internal class UPConfig
    {
        private NameValueCollection config = ConfigurationManager.AppSettings;

        public string CardRequestUrl
        {
            get { return config["sdk.cardRequestUrl"]; }
        }
        public string AppRequestUrl
        {
            get { return config["sdk.appRequestUrl"]; }
        }

        public string FrontTransUrl
        {
            get { return config["sdk.frontTransUrl"]; }
        }
        public string EncryptCert
        {
            get { return config["sdk.encryptCert.path"]; }
        }

        public string BackTransUrl
        {
            get { return config["sdk.backTransUrl"]; }
        }

        public string SingleQueryUrl
        {
            get { return config["sdk.singleQueryUrl"]; }
        }

        public string FileTransUrl
        {
            get { return config["sdk.fileTransUrl"]; }
        }

        public string SignCertPath
        {
            get { return config["sdk.signCert.path"]; }
        }

        public string SignCertPwd
        {
            get { return config["sdk.signCert.pwd"]; }
        }

        public string ValidateCertDir
        {
            get { return config["sdk.validateCert.dir"]; }
        }
        public string BatTransUrl
        {
            get { return config["sdk.batTransUrl"]; }
        }

        public string ProxyAddress
        {
            get { return config["proxyAddress"]; }
        }

        public string ProxyCredentialUserName
        {
            get { return config["proxyCredentialUserName"]; }
        }

        public string ProxyCredentialPassword
        {
            get { return config["proxyCredentialPassword"]; }
        }
    }

    /// <summary>
    /// Extenal class to expose the configuration values
    /// </summary>
    public class SDKConfig
    {
        public static string CardRequestUrl
        {
            get { return new UPConfig().CardRequestUrl; }
        }
        public static string AppRequestUrl
        {
            get { return new UPConfig().AppRequestUrl; }
        }

        public static string FrontTransUrl
        {
            get { return new UPConfig().FrontTransUrl; }
        }
        public static string EncryptCert
        {
            get { return new UPConfig().EncryptCert; }
        }

        public static string BackTransUrl
        {
            get { return new UPConfig().BackTransUrl; }
        }

        public static string SingleQueryUrl
        {
            get { return new UPConfig().SingleQueryUrl; }
        }

        public static string FileTransUrl
        {
            get { return new UPConfig().FileTransUrl; }
        }

        public static string SignCertPath
        {
            get { return new UPConfig().SignCertPath; }
        }

        public static string SignCertPwd
        {
            get { return new UPConfig().SignCertPwd; }
        }

        public static string ValidateCertDir
        {
            get { return new UPConfig().ValidateCertDir; }
        }
        public static string BatTransUrl
        {
            get { return new UPConfig().BatTransUrl; }
        }

        public static string ProxyAddress
        {
            get { return new UPConfig().ProxyAddress; }
        }

        public static string ProxyCredentialUserName
        {
            get { return new UPConfig().ProxyCredentialUserName; }
        }

        public static string ProxyCredentialPassword
        {
            get { return new UPConfig().ProxyCredentialPassword; }
        }
    }
}