using CF.VRent.SAPSDK.Entities;
using CF.VRent.SAPSDK.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace CF.VRent.SAPSDK
{
    public static class SAPContext
    {
        private static XmlDocument Common { get; set; }
        private static XmlDocument Header { get; set; }
        private static XmlDocument FileName { get; set; }
        private static XmlDocument Item { get; set; }
        internal static List<PostConfigurationEntity> Documents { get; private set; }

        static SAPContext()
        {
            Documents = new List<PostConfigurationEntity>();

            Common = new XmlDocument();
            Header = new XmlDocument();
            FileName = new XmlDocument();
            Item = new XmlDocument();

            Documents.Add(new PostConfigurationEntity()
            {
                Type = EntityType.Common,
                Doc = Common,
                FilePath = AppDomain.CurrentDomain.BaseDirectory + @"\bin\Configuration\SAP_FI_Posting_Common.xml",
                FileResource = Configuration.ConfigureFiles.SAP_FI_Posting_Common
            });
            Documents.Add(new PostConfigurationEntity()
            {
                Type = EntityType.Header,
                Doc = Header,
                FilePath = AppDomain.CurrentDomain.BaseDirectory + @"\bin\Configuration\SAP_FI_Posting_Header.xml",
                FileResource = Configuration.ConfigureFiles.SAP_FI_Posting_Header
            });
            Documents.Add(new PostConfigurationEntity()
            {
                Type = EntityType.FileName,
                Doc = FileName,
                FilePath = AppDomain.CurrentDomain.BaseDirectory + @"\bin\Configuration\SAP_FI_Posting_FileName.xml",
                FileResource = Configuration.ConfigureFiles.SAP_FI_Posting_FileName
            });
            Documents.Add(new PostConfigurationEntity()
            {
                Type = EntityType.Item,
                Doc = Item,
                FilePath = AppDomain.CurrentDomain.BaseDirectory + @"\bin\Configuration\SAP_FI_Posting_Item.xml",
                FileResource = Configuration.ConfigureFiles.SAP_FI_Posting_Item
            });

            foreach (PostConfigurationEntity name in Documents)
            {
                if (File.Exists(name.FilePath))
                {
                    name.Doc.Load(name.FilePath);
                }
                else
                {
                    name.Doc.LoadXml(name.FileResource);
                }
            }
        }

        public static ISAPManager CreateManager()
        {
            return new SAPManager();
        }
    }
}
