using CF.VRent.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CF.VRent.BLL.BLLFactory.GeneralLedger
{
    public class LocalCompanyCodeSource : ICompanyCodeSource
    {
        private List<BusinessArea> _localData;

        public LocalCompanyCodeSource()
        {
            //var file = AppDomain.CurrentDomain.BaseDirectory + @"\bin\Configuration\CompanyCodeMapping.xml";
            //var doc = new XmlDocument();
            //doc.Load(file);
            ////Get active rows
            //var rows = doc.SelectNodes("//Clients/Client[State=1]");

            //Parallel.ForEach<XmlNode>(rows.Cast<XmlNode>(), r => {
            //    var clientNodes = r.SelectNodes(".//ClientName/Name");

            //    var clients = new List<UserClient>();

            //    Parallel.ForEach<XmlNode>(clientNodes.Cast<XmlNode>(), c => {
            //        clients.Add(new UserClient() { 
            //            ClientName = c.Value
            //        });
            //    });

            //    _localData.Add(new BusinessArea() {
            //        Branch = r["Branch"].Value,
            //        BusinessAreaCode = r["BusinessAreaCode"].Value,
            //        CompanyCode = r["CompanyCode"].Value,
            //        Clients = clients
            //    });
            //});

            //doc = null;
        }

        public BusinessArea GetDataSource()
        {
            throw new NotImplementedException();
        }
    }
}
