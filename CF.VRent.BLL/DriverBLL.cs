using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Contract;
using CF.VRent.Entities;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Common.UserContracts;

namespace CF.VRent.BLL
{
    public class DriverBLL:AbstractBLL
    {
        public DriverBLL(ProxyUserSetting userInfo)
            : base(userInfo) 
        {
        }
        public List<ProxyDriver> FindAllDrivers(string uid)
        {
            //return iDriver.FindAllDrivers(uid);
            List<ProxyDriver> listDrivers = new List<ProxyDriver>();

            KemasUserAPI client = new KemasUserAPI();
            var res = client.findAllDrivers(uid);

            foreach (var item in res)
            {
                ProxyDriver drive = new ProxyDriver() { ID = item.ID, Name = item.Name };
                listDrivers.Add(drive);
            }

            return listDrivers;
        }
    }
}
