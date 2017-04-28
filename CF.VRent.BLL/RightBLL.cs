using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Contract;
using CF.VRent.Entities;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Common.UserContracts;

namespace CF.VRent.BLL
{
    public class RightBLL: AbstractBLL
    {
        public RightBLL(ProxyUserSetting userInfo):base(userInfo)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
         public List<ProxyRight> GetAllRights(string uid)
         {
             List<ProxyRight> list = null;

             KemasUserAPI client = new KemasUserAPI();
             var res = client.getRights(uid);

             if (res != null)
             {
                 list = new List<ProxyRight>();
                 foreach (var item in res)
                 {
                     ProxyRight right = new ProxyRight() { RightMember = item.Right1 };
                     list.Add(right);
                 } 
             }
             return list;
         }

         public List<ProxyRole> GetAllRoles(string userId,string sessionId)
         {
             List<ProxyRole> list = null;

             KemasUserAPI client = new KemasUserAPI();
             var res = client.findUser2(userId, sessionId);

             if (res != null && res.UserData != null && res.UserData.Roles != null)
             {
                 list = new List<ProxyRole>();
                 foreach (var item in res.UserData.Roles)
                 {
                     ProxyRole role = new ProxyRole() { RoleMember = item.Name };
                     list.Add(role);
                 }
             }

             return list;
         }
    }
}
