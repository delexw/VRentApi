
using CF.VRent.Cache;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities;
using CF.VRent.Entities.KEMASWSIF_CONFIGRef;
using CF.VRent.Log;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.ReservationAOP.Cache
{
    public class ClientCacheBehavior : IInterceptionBehavior
    {
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var ret = getNext()(input, getNext);

            try
            {
                if ((input.MethodBase.Name == "CreateCompany" || input.MethodBase.Name == "UpdateCompany") && 
                    ret.Exception == null)
                {
                    var company = ret.ReturnValue as UserCompanyExtenstion;
                    var userProperty = input.Target.GetType().GetProperty("UserInfo");
                    var userInfo = userProperty.GetValue(input.Target, null) as ProxyUserSetting;

                    if (CacheContext.Context.ShortModel.Exist(userInfo.ID + "_getClients"))
                    {
                        var clients = CacheContext.Context.ShortModel.Get<getClientsResponse>(userInfo.ID + "_getClients");
                        var list = clients.Clients.ToList();
                        var hitClient = list.FirstOrDefault(r => r.ID == company.ID);
                        if (hitClient == null)
                        {
                            list.Insert(0, new Client()
                            {
                                ID = company.ID,
                                Name = company.Name,
                                Status = company.Status
                            });
                            clients.Clients = list.ToArray();
                        }
                        else
                        {
                            hitClient.Name = company.Name;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogInfor.WriteDebug("Cache init failed", ex.ToString(), "System");
            }

            return ret;
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}
