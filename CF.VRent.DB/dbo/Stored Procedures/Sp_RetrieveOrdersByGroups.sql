-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Sp_RetrieveOrdersByGroups]
	(
		@ProxyBookingID int,
		@OperatorID uniqueidentifier,
		@Groups dbo.OrderItemGroup readonly,
		@return_value int output
	)

AS
declare @IsBookingOwner int
declare @BookingOrderID int

--@return_result
-- -1:  not booking owner
-- -2:  order is not generated

	BEGIN
		Select @BookingOrderID = vo.ID from VrentOrders as vo 
		where vo.ProxyBookingID = @ProxyBookingID
		and vo.State = 0
		
		if(@BookingOrderID > 0)
			Begin   
				select 
					voi.[ID]
					,[OrderID]
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
					,voi.[State]
					,voi.[CreatedOn]
					,voi.[CreatedBy]
					,voi.[ModifiedOn]
					,voi.[ModifiedBy]
				from VrentOrderItems as voi inner join VrentOrders as vo 
				on voi.OrderID = vo.ID and voi.State = 0 and vo.State = 0
				where vo.ProxyBookingID = @ProxyBookingID 
				and voi.Category in ((select g.OrderItemGroup from @Groups as g))
			Set @return_value = 0
			End
		Else
			Begin
				Set @return_value = -2
			End
	end