

CREATE PROCEDURE [dbo].[Sp_CreateOrderItems]
	(@OrderItems dbo.OrderItem readonly)

AS

BEGIN

INSERT INTO [VrentOrderItems]
           (
           [OrderID]
           ,[ProductCode]
           ,[ProductName]
           ,[SpecModel]
           ,[Category]
           ,[TypeID]
           ,[Type]
           ,[UnitOfMeasure]
           ,[UnitPrice]
           ,[Quantity]
           ,[NetAmount]
           ,[TaxRate]
           ,[TaxAmount]
           ,[TotalAmount]
           ,[Remark]
           ,[State]
           ,[CreatedOn]
           ,[CreatedBy]
           ,[ModifiedOn]
           ,[ModifiedBy]
           )
	Select 
           orderItem.[OrderID]
           ,orderItem.[ProductCode]
           ,orderItem.[ProductName]
           ,orderItem.[SpecModel]
           ,orderItem.[Category]
           ,orderItem.[TypeID]
           ,orderItem.[Type]
           ,orderItem.[UnitOfMeasure]
           ,orderItem.[UnitPrice]
           ,orderItem.[Quantity]
           ,orderItem.[NetAmount]
           ,orderItem.[TaxRate]
           ,orderItem.[TaxAmount]
           ,orderItem.[TotalAmount]
           ,orderItem.[Remark]
           ,orderItem.[State]
           ,orderItem.[CreatedOn]
           ,orderItem.[CreatedBy]
           ,orderItem.[ModifiedOn]
           ,orderItem.[ModifiedBy]

	from @OrderItems as orderItem
END

