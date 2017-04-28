using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Common.UserContracts;
using CF.VRent.DataAccessProxy.Entities;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CF.VRent.DAL
{
    public class IndirectFeeDAL
    {
        private const string RetrieveIndirectFeeType = "Sp_RetrieveIndirectFeeTypes";
        private const string CreateIndirectFeeTypesSp = "Sp_IndirectFeeType_Create";

        private const string RetrieveOrderItemsByGroupsSp = "Sp_RetrieveOrdersByGroups";
        private const string CreateOrderItemsSp = "Sp_CreateOrderItems";

        private const string CreateOrderItemsViaBookingIDSp = "Sp_CreateOrderItemsViaBookingID";


        #region Booking Orders
        public static ReturnResultRetrieveOrderItems RetrieveBookingOrders(int proxyBookingID, string[] groups, ProxyUserSetting operationInfo)
        {
            ProxyOrderItem[] orderItems = null;

            List<SqlParameter> parameters = new List<SqlParameter>();

            IEnumerable<SqlDataRecord> orderItemGroups = null;

            if (groups.Length > 0)
            {
                orderItemGroups = CreateOrderItemGroupRecords(groups);
            }

            //	(@ProxyBookingID int,	 @Groups dbo.OrderItemGroup readonly)
            SqlParameter bookingIDParam = new SqlParameter("@ProxyBookingID", proxyBookingID);
            parameters.Add(bookingIDParam);

            //access control
            SqlParameter companyPara = new SqlParameter("@OperatorID", operationInfo.ID);
            parameters.Add(companyPara);

            SqlParameter orderItemGroupsParam = new SqlParameter("@Groups", orderItemGroups);

            orderItemGroupsParam.SqlDbType = SqlDbType.Structured;
            orderItemGroupsParam.TypeName = "dbo.OrderItemGroup";
            parameters.Add(orderItemGroupsParam);

            SqlParameter retPara = new SqlParameter("@return_value", -1);
            retPara.Direction = ParameterDirection.Output;
            parameters.Add(retPara);


            orderItems = DataAccessProxyConstantRepo.ExecuteSPReturnReader<ProxyOrderItem[]>(RetrieveOrderItemsByGroupsSp, parameters.ToArray(), (sqldatareader) => ReadMultipleOrderItemsFromDataReader(sqldatareader));

            int returnValue = Convert.ToInt32(retPara.Value);
            parameters.Clear(); 
            
            ReturnResultRetrieveOrderItems items = new ReturnResultRetrieveOrderItems();
            items.Data = orderItems;
            items.Success = returnValue;
            ProcessingResult(items, proxyBookingID, operationInfo);

            return items;
        }

        public static int AddOrderItems(ProxyOrderItem[] items)
        {
            int affectedCnt = -1;

            List<SqlParameter> parameters = new List<SqlParameter>();

            IEnumerable<SqlDataRecord> orderItems = null;

            if (items.Length > 0)
            {
                orderItems = CreateOrderItemsRecords(items);
            }

            //	(@ProxyBookingID int,	 @Groups dbo.OrderItemGroup readonly)

            SqlParameter orderItemGroupsParam = new SqlParameter("@OrderItems", orderItems);

            orderItemGroupsParam.SqlDbType = SqlDbType.Structured;
            orderItemGroupsParam.TypeName = "dbo.OrderItem";
            parameters.Add(orderItemGroupsParam);

            affectedCnt = DataAccessProxyConstantRepo.ExecuteSPNonQuery(CreateOrderItemsSp, parameters.ToArray());

            parameters.Clear();

            return affectedCnt;
        }

        public static IEnumerable<SqlDataRecord> CreateOrderItemsRecords(ProxyOrderItem[] orderItems)
        {
            //[ID] [int] IDENTITY(1,1) NOT NULL,
            //[OrderID] [int] NOT NULL,
            //[ProductCode] [nvarchar](50) NULL,
            //[ProductName] [nvarchar](50) NULL,
            //[SpecModel] [nvarchar](50) NULL,
            //[Category] [nchar](15) NOT NULL,
            //[TypeID] [int] NULL,
            //[Type] [nvarchar](50) NOT NULL,
            //[UnitOfMeasure] [nvarchar](50) NULL,
            //[UnitPrice] [decimal](18, 0) NULL,
            //[Quantity] [int] NULL,
            //[NetAmount] [decimal](18, 0) NOT NULL,
            //[TaxRate] [decimal](18, 0) NOT NULL,
            //[TaxAmount] [decimal](18, 0) NOT NULL,
            //[TotalAmount] [decimal](18, 0) NOT NULL,
            //[Remark] [nvarchar](50) NULL,
            //[State] [tinyint] NOT NULL,
            //[CreatedOn] [datetime] NOT NULL,
            //[CreatedBy] [uniqueidentifier] NOT NULL,
            //[ModifiedOn] [datetime] NULL,
            //[ModifiedBy] [uniqueidentifier] NULL
            SqlMetaData[] metaData = new SqlMetaData[21]
            {
                new SqlMetaData("ID",SqlDbType.Int),
                new SqlMetaData("OrderID",SqlDbType.Int),
                new SqlMetaData("ProductCode",SqlDbType.NVarChar,50),
                new SqlMetaData("ProductName",SqlDbType.NVarChar,500),
                new SqlMetaData("SpecModel",SqlDbType.NVarChar,50),

                new SqlMetaData("Category",SqlDbType.NVarChar,15),
                new SqlMetaData("TypeID",SqlDbType.Int),
                new SqlMetaData("Type",SqlDbType.NVarChar,100),
                new SqlMetaData("UnitOfMeasure",SqlDbType.NVarChar,50),
                new SqlMetaData("UnitPrice",SqlDbType.Decimal,10,3),

                new SqlMetaData("Quantity",SqlDbType.Decimal,10,3),
                new SqlMetaData("NetAmount",SqlDbType.Decimal,10,3),
                new SqlMetaData("TaxRate",SqlDbType.Decimal,10,3),
                new SqlMetaData("TaxAmount",SqlDbType.Decimal,10,3),
                new SqlMetaData("TotalAmount",SqlDbType.Decimal,10,3),

                new SqlMetaData("Remark",SqlDbType.NVarChar,50),
                new SqlMetaData("State",SqlDbType.SmallInt),
                new SqlMetaData("CreatedOn",SqlDbType.DateTime),
                new SqlMetaData("CreatedBy",SqlDbType.UniqueIdentifier),
                new SqlMetaData("ModifiedOn",SqlDbType.DateTime),
                new SqlMetaData("ModifiedBy",SqlDbType.UniqueIdentifier),
            };

            return orderItems.Select(m => SetOrderItemValues(metaData, m));
        }
        private static SqlDataRecord SetOrderItemValues(SqlMetaData[] metaData, ProxyOrderItem orderItem)
        {

            //[ID] [int] IDENTITY(1,1) NOT NULL,
            //[OrderID] [int] NOT NULL,
            //[ProductCode] [nvarchar](50) NULL,
            //[ProductName] [nvarchar](50) NULL,
            //[SpecModel] [nvarchar](50) NULL,
            //[Category] [nchar](15) NOT NULL,
            
            //[State] [tinyint] NOT NULL,
            //[CreatedOn] [datetime] NOT NULL,
            //[CreatedBy] [uniqueidentifier] NOT NULL,
            //[ModifiedOn] [datetime] NULL,
            //[ModifiedBy] [uniqueidentifier] NULL
            SqlDataRecord orderItemRecord = new SqlDataRecord(metaData);

            orderItemRecord.SetInt32(1, orderItem.OrderID);

            if (string.IsNullOrEmpty(orderItem.ProductCode))
            {orderItemRecord.SetDBNull(2);            }
            else
            {orderItemRecord.SetSqlString(2, orderItem.ProductCode); }
            if (string.IsNullOrEmpty(orderItem.ProductName))
            { orderItemRecord.SetDBNull(3); }
            else
            { orderItemRecord.SetSqlString(3, orderItem.ProductName); }
            if (string.IsNullOrEmpty(orderItem.SpecMode))
            { orderItemRecord.SetDBNull(4); }
            else
            { orderItemRecord.SetSqlString(4, orderItem.SpecMode); }  
            orderItemRecord.SetSqlString(5, orderItem.Category);

            //[TypeID] [int] NULL,
            //[Type] [nvarchar](50) NOT NULL,
            //[UnitOfMeasure] [nvarchar](50) NULL,
            //[UnitPrice] [decimal](18, 0) NULL,
            //[Quantity] [int] NULL,
            if (orderItem.TypeID == null)
            {                orderItemRecord.SetDBNull(6);}
            else
            {                orderItemRecord.SetInt32(6,orderItem.TypeID.Value); }
            orderItemRecord.SetSqlString(7, orderItem.Type);

            if (string.IsNullOrEmpty(orderItem.UnitMeasure))
            {
                orderItemRecord.SetDBNull(8);
            }
            else
            {
                orderItemRecord.SetString(8, orderItem.UnitMeasure);
            }

            if (orderItem.UnitPrice == null)
            {
                orderItemRecord.SetDBNull(9);
            }
            else
            {
                orderItemRecord.SetDecimal(9,orderItem.UnitPrice.Value);
            }

            if (orderItem.SalesQuantity == null)
            {
                orderItemRecord.SetDBNull(10);
            }
            else
            {
                orderItemRecord.SetDecimal(10, orderItem.SalesQuantity.Value);
            }

            //[NetAmount] [decimal](18, 0) NOT NULL,
            //[TaxRate] [decimal](18, 0) NOT NULL,
            //[TaxAmount] [decimal](18, 0) NOT NULL,
            //[TotalAmount] [decimal](18, 0) NOT NULL,
            //[Remark] [nvarchar](50) NULL,

            orderItemRecord.SetDecimal(11, orderItem.AmountExclVAT.Value);
            orderItemRecord.SetDecimal(12, orderItem.TaxRate.Value);
            orderItemRecord.SetDecimal(13, orderItem.Tax.Value);
            orderItemRecord.SetDecimal(14, orderItem.AmountIncVAT.Value);

            if (string.IsNullOrEmpty(orderItem.Remark))
            { orderItemRecord.SetDBNull(15); }
            else
            { orderItemRecord.SetSqlString(15, orderItem.Remark); }

            orderItemRecord.SetInt16(16, orderItem.State);
            orderItemRecord.SetDateTime(17, orderItem.CreatedOn.Value);
            orderItemRecord.SetSqlGuid(18, orderItem.CreatedBy.Value);

            if (orderItem.ModifiedOn == null)
            {
                orderItemRecord.SetDBNull(19);
            }
            else
            {
                orderItemRecord.SetDateTime(19, orderItem.ModifiedOn.Value);
            }

            if (orderItem.ModifiedBy == null)
            {
                orderItemRecord.SetDBNull(20);
            }
            else
            {
                orderItemRecord.SetSqlGuid(20, orderItem.ModifiedBy.Value);
            }

            return orderItemRecord;
        }


        #region Order Items Helper Method
        public static IEnumerable<SqlDataRecord> CreateOrderItemGroupRecords(string[] groups)
        {

            SqlMetaData[] metaData = new SqlMetaData[1]
            {
                new SqlMetaData("OrderItemGroup",SqlDbType.NVarChar,15)
            };


            return groups.Select(m => SetOrderItemGroupValues(metaData, m));
        }
        private static SqlDataRecord SetOrderItemGroupValues(SqlMetaData[] metaData, string group)
        {
            SqlDataRecord OrderItemGroup = new SqlDataRecord(metaData);

            OrderItemGroup.SetSqlString(0, group);

            return OrderItemGroup;
        }

        private static ProxyOrderItem ReadSingleBookingFromDataReader(SqlDataReader sqlReader)
        {
            ProxyOrderItem orderItem = new ProxyOrderItem();

            while (sqlReader.Read())
            {


            //[ID] [int] IDENTITY(1,1) NOT NULL,
            //[OrderID] [int] NOT NULL,
            //[ProductCode] [nvarchar](50) NULL,
            //[ProductName] [nvarchar](50) NULL,
            //[SpecModel] [nvarchar](50) NULL,
            //[Category] [nchar](15) NOT NULL,
            //[TypeID] [int] NULL,
            //[Type] [nvarchar](50) NOT NULL,
            //[UnitOfMeasure] [nvarchar](50) NULL,
            //[UnitPrice] [decimal](18, 0) NULL,
            //[Quantity] [int] NULL,
            //[NetAmount] [decimal](18, 0) NOT NULL,
            //[TaxRate] [decimal](18, 0) NOT NULL,
            //[TaxAmount] [decimal](18, 0) NOT NULL,
            //[TotalAmount] [decimal](18, 0) NOT NULL,
            //[Remark] [nvarchar](50) NULL,
            //[State] [tinyint] NOT NULL,
            //[CreatedOn] [datetime] NOT NULL,
            //[CreatedBy] [uniqueidentifier] NOT NULL,
            //[ModifiedOn] [datetime] NULL,
            //[ModifiedBy] [uniqueidentifier] NULL,

                orderItem.ID = Convert.ToInt32(sqlReader[0].ToString());
                orderItem.OrderID = Convert.ToInt32(sqlReader[1].ToString());

                orderItem.ProductCode = sqlReader[2].ToString().ToString();
                orderItem.ProductName = sqlReader[3].ToString();
                orderItem.SpecMode = sqlReader[4].ToString();
                orderItem.Category = sqlReader[5].ToString();
                orderItem.Type = sqlReader[6].ToString();

                orderItem.UnitMeasure = sqlReader[7].ToString();
                orderItem.UnitPrice = sqlReader[8] == DBNull.Value ? new Nullable<decimal>() : decimal.Parse(sqlReader[8].ToString());
                orderItem.SalesQuantity = sqlReader[9] == DBNull.Value ? new Nullable<decimal>() : decimal.Parse(sqlReader[8].ToString());
                orderItem.AmountExclVAT = decimal.Parse(sqlReader[10].ToString());
                orderItem.TaxRate = decimal.Parse(sqlReader[11].ToString());
                orderItem.Tax = decimal.Parse(sqlReader[12].ToString());
                orderItem.AmountIncVAT = decimal.Parse(sqlReader[13].ToString());
                orderItem.Remark = sqlReader[14].ToString();

                orderItem.State = Convert.ToInt16(sqlReader[15].ToString());
                orderItem.CreatedOn = Convert.ToDateTime(sqlReader[16].ToString());
                orderItem.CreatedBy = Guid.Parse(sqlReader[17].ToString());

                orderItem.ModifiedOn = sqlReader[18].Equals(DBNull.Value) ? new Nullable<DateTime>() : Convert.ToDateTime(sqlReader[18].ToString());
                orderItem.ModifiedBy = sqlReader[19].Equals(DBNull.Value) ? new Nullable<Guid>() : Guid.Parse(sqlReader[19].ToString());
            }

            return orderItem;
        }
        private static ProxyOrderItem[] ReadMultipleOrderItemsFromDataReader(SqlDataReader sqlReader)
        {
            List<ProxyOrderItem> orderItems = new List<ProxyOrderItem>();

            while (sqlReader.Read())
            {
                ProxyOrderItem orderItem = new ProxyOrderItem();

                //[ID] [int] IDENTITY(1,1) NOT NULL,
                //[OrderID] [int] NOT NULL,
                //[ProductCode] [nvarchar](50) NULL,
                //[ProductName] [nvarchar](50) NULL,
                //[SpecModel] [nvarchar](50) NULL,
                //[Category] [nchar](15) NOT NULL,
                //[TypeID] [int] NULL,
                //[Type] [nvarchar](50) NOT NULL,
                //[UnitOfMeasure] [nvarchar](50) NULL,
                //[UnitPrice] [decimal](18, 0) NULL,
                //[Quantity] [int] NULL,
                //[NetAmount] [decimal](18, 0) NOT NULL,
                //[TaxRate] [decimal](18, 0) NOT NULL,
                //[TaxAmount] [decimal](18, 0) NOT NULL,
                //[TotalAmount] [decimal](18, 0) NOT NULL,
                //[Remark] [nvarchar](50) NULL,
                //[State] [tinyint] NOT NULL,
                //[CreatedOn] [datetime] NOT NULL,
                //[CreatedBy] [uniqueidentifier] NOT NULL,
                //[ModifiedOn] [datetime] NULL,
                //[ModifiedBy] [uniqueidentifier] NULL,

                orderItem.ID = Convert.ToInt32(sqlReader[0].ToString());
                orderItem.OrderID = Convert.ToInt32(sqlReader[1].ToString());

                orderItem.ProductCode = sqlReader[2].ToString().ToString();
                orderItem.ProductName = sqlReader[3].ToString();
                orderItem.SpecMode = sqlReader[4].ToString();
                orderItem.Category = sqlReader[5].ToString();

                orderItem.TypeID = sqlReader[6]== DBNull.Value? new Nullable<int>(): int.Parse(sqlReader[6].ToString());
                orderItem.Type = sqlReader[7].ToString();

                orderItem.UnitMeasure = sqlReader[8].ToString();
                orderItem.UnitPrice =  sqlReader[9] == DBNull.Value? new Nullable<decimal>(): decimal.Parse(sqlReader[9].ToString());
                orderItem.SalesQuantity = sqlReader[10] == DBNull.Value ? new Nullable<decimal>() : decimal.Parse(sqlReader[10].ToString());

                orderItem.AmountExclVAT = decimal.Parse(sqlReader[11].ToString());
                orderItem.TaxRate = decimal.Parse(sqlReader[12].ToString());
                orderItem.Tax = decimal.Parse(sqlReader[13].ToString());
                orderItem.AmountIncVAT = decimal.Parse(sqlReader[14].ToString());
                orderItem.Remark = sqlReader[15].ToString();

                orderItem.State = Convert.ToInt16(sqlReader[16].ToString());
                orderItem.CreatedOn = Convert.ToDateTime(sqlReader[17].ToString());
                orderItem.CreatedBy = Guid.Parse(sqlReader[18].ToString());

                orderItem.ModifiedOn = sqlReader[19].Equals(DBNull.Value) ? new Nullable<DateTime>() : Convert.ToDateTime(sqlReader[19].ToString());
                orderItem.ModifiedBy = sqlReader[20].Equals(DBNull.Value) ? new Nullable<Guid>() : Guid.Parse(sqlReader[20].ToString());

                orderItems.Add(orderItem);
            }

            return orderItems.ToArray();
        }
        #endregion
        #endregion

        #region indirect fee types
        public static IndirectFeeType[] RetrieveAllIndirectFeeTypes()
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            return DataAccessProxyConstantRepo.ExecuteSPReturnReader<IndirectFeeType[]>(RetrieveIndirectFeeType, parameters.ToArray(), (sqldatareader) => ReadMultipleIndirectFeeTypeFromDataReader(sqldatareader));
        }
        public static int SaveIndirectFeeType(IndirectFeeType[] inputtypes)
        {
            int affected = -1;
            IEnumerable<SqlDataRecord> indirectFeeType = null;

            if (inputtypes.Length > 0)
            {
                indirectFeeType = CreateIndirectFeeRecords(inputtypes);
            }

            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlParameter indirectfeeTypesParam = new SqlParameter("@IndirectFeeTypesInput", indirectFeeType);

            indirectfeeTypesParam.SqlDbType = SqlDbType.Structured;
            indirectfeeTypesParam.TypeName = "dbo.IndirectFeeType";
            parameters.Add(indirectfeeTypesParam);

            affected = DataAccessProxyConstantRepo.ExecuteSPNonQuery(CreateIndirectFeeTypesSp, parameters.ToArray());
            parameters.Clear();

            return affected;
        }
        #endregion

        #region helper method
        private static IndirectFeeType[] ReadMultipleIndirectFeeTypeFromDataReader(SqlDataReader sqlReader)
        {
            List<IndirectFeeType> bookings = new List<IndirectFeeType>();

            while (sqlReader.Read())
            {
                IndirectFeeType indirectFee = new IndirectFeeType();

                //[ID] [int] NOT NULL,
                //[Type] [nvarchar](50) NOT NULL,
                //[Group] [nvarchar](50) NOT NULL,
                //[Note] [nvarchar](100) NULL,
                //[State] [tinyint] NOT NULL,
                //[CreatedOn] [datetime] NOT NULL,
                //[CreatedBy] [uniqueidentifier] NOT NULL,
                //[ModifiedOn] [datetime] NULL,
                //[ModifiedBy] [uniqueidentifier] NULL,

                indirectFee.ID = Convert.ToInt32(sqlReader[0].ToString());

                indirectFee.Type = sqlReader[1].ToString();

                indirectFee.Group = sqlReader[2].ToString();

                indirectFee.SourceType = (IndirectFeeSourceType)Enum.Parse(typeof(IndirectFeeSourceType), sqlReader[3].ToString()); 
                
                indirectFee.Note = sqlReader[4].ToString();

                indirectFee.State = Convert.ToByte(sqlReader[5].ToString());

                indirectFee.CreatedOn = Convert.ToDateTime(sqlReader[6].ToString());
                indirectFee.CreatedBy = Guid.Parse(sqlReader[7].ToString());

                indirectFee.ModifiedOn = sqlReader[8].Equals(DBNull.Value) ? new Nullable<DateTime>() : Convert.ToDateTime(sqlReader[8].ToString());
                indirectFee.ModifiedBy = sqlReader[9].Equals(DBNull.Value) ? new Nullable<Guid>() : Guid.Parse(sqlReader[9].ToString());

                bookings.Add(indirectFee);
            }

            return bookings.ToArray();
        }

        public static IEnumerable<SqlDataRecord> CreateIndirectFeeRecords(IndirectFeeType[] indirectFeeTypes)
        {
            //[ID] [int] NOT NULL,
            //[Type] [nvarchar](50) NOT NULL,
            //[Group] [nvarchar](50) NOT NULL,
            //[Note] [nvarchar](100) NULL,
            //[State] [tinyint] NOT NULL,
            //[CreatedOn] [datetime] NOT NULL,
            //[CreatedBy] [uniqueidentifier] NOT NULL,
            //[ModifiedOn] [datetime] NULL,
            //[ModifiedBy] [uniqueidentifier] NULL,

            SqlMetaData[] metaData = new SqlMetaData[9]
            {
                new SqlMetaData("Type",SqlDbType.NVarChar,100),
                new SqlMetaData("Group",SqlDbType.NVarChar,50),
                new SqlMetaData("SourceType",SqlDbType.TinyInt),
                new SqlMetaData("Note",SqlDbType.NVarChar,100),
                new SqlMetaData("State",SqlDbType.TinyInt),
                new SqlMetaData("CreatedOn",SqlDbType.DateTime),
                new SqlMetaData("CreatedBy",SqlDbType.UniqueIdentifier),
                new SqlMetaData("ModifiedOn",SqlDbType.DateTime),
                new SqlMetaData("ModifiedBy",SqlDbType.UniqueIdentifier),
            };


            return indirectFeeTypes.Select(m => SetIndirectFeeValues(metaData, m));
        }

        private static SqlDataRecord SetIndirectFeeValues(SqlMetaData[] metaData, IndirectFeeType indirectFeeType)
        {
            SqlDataRecord indirectFeeTypeItem = new SqlDataRecord(metaData);

            indirectFeeTypeItem.SetSqlString(0, indirectFeeType.Type);
            indirectFeeTypeItem.SetSqlString(1, indirectFeeType.Group);

            indirectFeeTypeItem.SetByte(2, (byte)indirectFeeType.SourceType);

            //group is used as note
            if (string.IsNullOrEmpty(indirectFeeType.Group))
            {
                indirectFeeTypeItem.SetDBNull(3);
            }
            else
            {
                indirectFeeTypeItem.SetString(3, indirectFeeType.Group);
            }

            indirectFeeTypeItem.SetByte(4, (byte)indirectFeeType.State);
            indirectFeeTypeItem.SetDateTime(5, indirectFeeType.CreatedOn.Value);
            indirectFeeTypeItem.SetSqlGuid(6, indirectFeeType.CreatedBy.Value);

            if (indirectFeeType.ModifiedOn == null)
            {
                indirectFeeTypeItem.SetDBNull(7);
            }
            else
            {
                indirectFeeTypeItem.SetDateTime(7, indirectFeeType.ModifiedOn.Value);
            }

            if (indirectFeeType.ModifiedBy == null)
            {
                indirectFeeTypeItem.SetDBNull(8);
            }
            else
            {
                indirectFeeTypeItem.SetSqlGuid(8, indirectFeeType.ModifiedBy.Value);
            }

            return indirectFeeTypeItem;
        }

        #endregion

        public static ReturnResultAddIndirectFeeItems AddOrderItemsByBookingID(int proxyBookingID, ProxyOrderItem[] orderItems,ProxyUserSetting profile)
        {
            int affectedCnt = -1;

            List<SqlParameter> parameters = new List<SqlParameter>();

            IEnumerable<SqlDataRecord> items = null;

            if (orderItems.Length > 0)
            {
                items = CreateOrderItemsRecords(orderItems);
            }

            //	(@ProxyBookingID int,	 @Groups dbo.OrderItemGroup readonly)
            SqlParameter bookingIDParam = new SqlParameter("@ProxyBookingID", proxyBookingID);
            parameters.Add(bookingIDParam);

            //access control
            SqlParameter operatorIDParam = new SqlParameter("@OperatorID", Guid.Parse(profile.ID));
            parameters.Add(operatorIDParam);

            SqlParameter orderItemGroupsParam = new SqlParameter("@OrderItems", items);

            orderItemGroupsParam.SqlDbType = SqlDbType.Structured;
            orderItemGroupsParam.TypeName = "dbo.OrderItem";
            parameters.Add(orderItemGroupsParam);

            SqlParameter retPara = new SqlParameter("@return_value", -1);
            retPara.Direction = ParameterDirection.InputOutput;
            parameters.Add(retPara);

            //--@return value
            //-- 0: success
            //-- -1: is not ownere or operator
            //-- -2: order is not generated

            affectedCnt = DataAccessProxyConstantRepo.ExecuteSPNonQuery(CreateOrderItemsViaBookingIDSp, parameters.ToArray());

            int ret = Convert.ToInt32(retPara.Value);
            parameters.Clear();

            ReturnResultAddIndirectFeeItems addedItems = new ReturnResultAddIndirectFeeItems();
            addedItems.Success = ret;
            addedItems.AffectedCnt = affectedCnt;

            ProcessingResult(addedItems,proxyBookingID,profile);

            return addedItems;
        }

        private static void ProcessingResult(ReturnResult operationResult, int proxyBookigID, ProxyUserSetting profile)
        {
            //--@return value
            //-- 0: success
            //-- -1: is not ownere or operator
            //-- -2: order is not generated

            switch (operationResult.Success)
            {
                case (int)AppendIndirectFeeResult.Success:
                    operationResult.Code = FapiaoRequestConst.SuccessCode;
                    operationResult.Message = FapiaoRequestConst.successMessage;
                    break;
                case (int)AppendIndirectFeeResult.InvalidOperator: //
                    operationResult.Code = FapiaoRequestConst.OperatorIsNotBookingOwnerCode;
                    operationResult.Message = string.Format(FapiaoRequestConst.OperatorIsNotBookingOwnerMessage, profile.ID, proxyBookigID);
                    break;
                case (int)AppendIndirectFeeResult.OrderDoesNotExist:
                    operationResult.Code = IndirectFeeItemsConst.OrderDoesNotExistCode;
                    operationResult.Message = string.Format(IndirectFeeItemsConst.OrderDoesNotExistMessage, proxyBookigID);
                    break;
                default:
                    operationResult.Code = FapiaoRequestConst.UnknownResultCode;
                    operationResult.Message = string.Format(IndirectFeeItemsConst.UnknownResultMessage, proxyBookigID, profile.ID);
                    break;
            }
        }
    }
}
