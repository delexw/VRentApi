using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Common;
using CF.VRent.Entities.EntityFactory;
using CF.VRent.Entities;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Common.UserContracts;
using CF.VRent.Log;
using CF.VRent.UserRole;
using CF.VRent.Cache;

namespace CF.VRent.BLL.ReservationAOP.Cache
{
    public class UserMgmtCacheBehavior:IInterceptionBehavior
    {
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            try
            {
                var userProperty = input.Target.GetType().GetProperty("UserInfo");
                var userInfo = userProperty.GetValue(input.Target, null) as ProxyUserSetting;

                var userFactory = new UserFactory();

                if (input.MethodBase.Name == "CreateCorpUser" && input.Arguments[input.Arguments.Count - 1] != null)
                {
                    //var ret = getNext()(input, getNext);
                    //var returnValue = ret.ReturnValue as UserExtension;
                    //var listCachekey = CacheContext.Context.ShortModel.GetKeyStartWith(String.Format("{0}_{1}", userInfo.ID, "GetUserList"));
                    //if (listCachekey.Count() > 0)
                    //{
                        //var list = CacheContext.Context.ShortModel.Get<EntityPager<UserExtensionHeader>>(listCachekey);
                        //var entities = list.Enitites.ToList();
                        //entities.Insert(0, userFactory.CreateHeaderEntity(returnValue));
                        //list.Enitites = entities.AsEnumerable();
                    //}
                    //return ret;
                }

                if (input.MethodBase.Name == "GetUserList" && input.Arguments[input.Arguments.Count - 1] != null)
                {
                    var permissionWhere = new UserPermissionFactory().CreateUserConditionEntity(input.Arguments["where"] as UserExtension,
                        input.Arguments[input.Arguments.Count - 1] as UserRoleEntityCollection);

                    var cacheKey = String.Format("{0}_{1}_{2}",
                        userInfo.ID,//User id
                        input.MethodBase.Name, //Method name
                        String.Format("{0}_{1}_{2}", 
                            input.Arguments["pageNumber"].ToStr(), 
                            input.Arguments["itemsPerPage"].ToStr(), 
                            permissionWhere.ObjectToJson()).GetHashCode());

                    if (CacheContext.Context.ShortModel.Exist(cacheKey))
                    {
                        return input.CreateMethodReturn(
                            CacheContext.Context.ShortModel.Get<EntityPager<UserExtensionHeader>>(cacheKey));
                    }
                    else
                    {
                        var ret = getNext()(input, getNext);
                        if (ret.Exception == null)
                        {
                            CacheContext.Context.ShortModel.Set(cacheKey, ret.ReturnValue);
                        }
                        return ret;
                    }
                }

                if (input.MethodBase.Name == "UpdateUser" && input.Arguments[input.Arguments.Count - 1] != null)
                {
                    var ret = getNext()(input, getNext);
                    var returnValue = ret.ReturnValue as UserExtension;
                    var listCache = CacheContext.Context.ShortModel
                        .GetObjectKeyStartWith<EntityPager<UserExtensionHeader>>(String.Format("{0}_{1}", userInfo.ID, "GetUserList"));

                    foreach (var list in listCache)
                    {
                        var hitUser = list.Enitites.FirstOrDefault(r => r.ID == returnValue.ID);
                        if (hitUser != null)
                        {
                            foreach (var property in hitUser.GetType().GetProperties())
                            {
                                property.SetValue(hitUser, returnValue.GetType().GetProperty(property.Name).GetValue(returnValue, null), null);
                            }
                        }
                    }

                    return ret;
                }
            }
            catch (Exception ex)
            {
                LogInfor.WriteDebug("Cache init failed", ex.ToString(), "System");
            }

            return getNext()(input, getNext);
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}
