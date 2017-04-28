using CF.VRent.Common.UserContracts;
using CF.VRent.Contract;
using CF.VRent.Entities;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.DataAccessProxyWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL
{
    public class BookingFapiaoImpl : AbstractBLL, IBookingFapiao
    {
        public BookingFapiaoImpl(ProxyUserSetting userInfo):base(userInfo) 
        {
        }

        public ProxyFapiao[] RetrieveFapiaos(Guid uid)
        {
            IDataService dsc = new DataAccessProxyManager();
            return dsc.RetrieveMyFapiaoData(uid);
        }


        public ProxyFapiao RetrieveFapiaoDetail(int faPiaoID)
        {
            IDataService dsc = new DataAccessProxyManager();
            return dsc.RetrieveFapiaoDataDetail(faPiaoID);
        }
    }
}
