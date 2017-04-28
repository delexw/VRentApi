using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.Contract;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.UserRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL
{
    public class IndirectFeeBLL : AbstractBLL, IIndirectFeeOperation
    {
        public IndirectFeeBLL(ProxyUserSetting userInfo)
            : base(userInfo) 
        {
        }

        public static string[] SplitGroupStrToArray(string groupStr)
        {
            string[] groups = null;
            List<string> groupLists = new List<string>();

            if (!string.IsNullOrEmpty(groupStr) && !string.IsNullOrEmpty(groupStr.Trim()))
            {
                groups = groupStr.Trim().Split(',').ToArray();
            }

            OrderItemConstant orC = new OrderItemConstant();

            return groups.Where(m => orC.Groups.Contains(m)).ToArray();

        }



        public IndirectFeeType[] GetIndirectFeeTypes()
        {
            IDataService ds = new DataAccessProxyManager();
            return ds.RetrieveIndirectFeeTypes();
        }

        public int AddIndirectFeeTypes(IndirectFeeType[] types)
        {
            foreach (var type in types)
            {
                type.State = 0;
                type.CreatedOn = DateTime.Now;
                type.CreatedBy = Guid.Parse(UserInfo.ID);
                type.ModifiedOn = null;
                type.ModifiedBy = null;
            }
            IDataService ds = new DataAccessProxyManager();
            return ds.AddIndirectFeeTypes(types);
        }

        public ProxyOrderItem[] RetrieveOrderItems(int proxyBookingID,string[] groups)
        {
            ProxyOrderItem[] items = null;

            IDataService ds = new DataAccessProxyManager();
            ReturnResultRetrieveOrderItems orderItems = ds.FindBookingOrders(proxyBookingID, groups, UserInfo);

            if (orderItems.Success == 0)
            {
                items = orderItems.Data;
            }
            else
            {
                throw new VrentApplicationException(orderItems);
            }
            return items;
        }


        public int AddOrderItems(int proxyBookingID,ProxyOrderItem[] orderItems)
        {
            int affectedCount = -1;
            if (RoleUtility.IsAdministrator(UserInfo) || RoleUtility.IsOperationManager(UserInfo))
            {
                if (orderItems.Length > 0)
                {
                    string category = orderItems[0].Category;
                    int orderID = orderItems[0].OrderID;

                    foreach (var orderItem in orderItems)
                    {
                        if (orderItem.OrderID == orderID)
                        {
                            //OrderID = 3,
                            //Category = orderItemCon.Groups[0],
                            //Type = "A",
                            //orderItem.UnitPrice = 0;
                            //orderItem.SalesQuantity = 0;
                            //AmountIncVAT = 200,
                            //Remark = "Some thing",

                            //append these value

                            orderItem.AmountExclVAT = orderItem.AmountIncVAT;
                            orderItem.TaxRate = 0;
                            orderItem.Tax = 0;

                            orderItem.State = 0;
                            orderItem.CreatedOn = DateTime.Now;
                            orderItem.CreatedBy = Guid.Parse(UserInfo.ID);
                            orderItem.ModifiedOn = null;
                            orderItem.ModifiedBy = null;
                        }
                        else
                        {
                            throw new VrentApplicationException(ErrorConstants.OrderIDNotConsistentErrorCode, ErrorConstants.OrderIDNotConsistentErrorMessage, ResultType.VRENTFE);
                        }
                    }
                    IDataService ds = new DataAccessProxyManager();
                    ReturnResultAddIndirectFeeItems addOrderITems = ds.AddOrderItemsByProxyBookingID(proxyBookingID, orderItems, UserInfo);

                    if (addOrderITems.Success == 0)
                    {
                        affectedCount = addOrderITems.AffectedCnt;
                    }
                    else
                    {
                        throw new VrentApplicationException(addOrderITems);
                    }

                }
                else
                {
                    throw new VrentApplicationException(ErrorConstants.NotOrderItemsCode, ErrorConstants.NotOrderItemsMessage, ResultType.VRENTFE);
                }
            }
            else
            {
                throw new VrentApplicationException(ErrorConstants.NoPrivilegeCode, string.Format(ErrorConstants.NoPrivilegeMessage, UserInfo.ID), ResultType.VRENT);
            }

            return affectedCount;
        }
    }
}
