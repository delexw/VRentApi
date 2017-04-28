
Create PROCEDURE [dbo].[Sp_CreateOrderItemsViaBookingID]
	(
		@ProxyBookingID int,
		@OperatorID uniqueidentifier,
		@OrderItems dbo.OrderItem readonly,
		@Return_Value int output
	)

AS
declare 
@OrderID int = -1,
@IsBookingOnwer int = -1,
@Affected_cnt int  = 0

--@return value
-- 0: success
-- -1: is not ownere or operator
-- -2: order is not generated
BEGIN

	--check
	EXEC @IsBookingOnwer = [dbo].[Sp_IsBookingOwner]
		@ProxyBookingID = @ProxyBookingID,
		@OperatorID = @OperatorID
	
	--bypass role-check
	Set @IsBookingOnwer = 1
		
	if(@IsBookingOnwer = 1)
		begin
			select @OrderID = vo.ID 
			from VrentOrders as vo inner join VrentBookings as vb
			on 
			vo.ProxyBookingID = @ProxyBookingID
			AND vb.ID = @ProxyBookingID
			AND vo.ProxyBookingID = vb.ID 
			AND vo.BookingUserID = vb.UserID
			AND vo.State = 0 and vb.[State] != 'swBookingModel/deleted'
		End
	else
		Begin
			set @Return_Value = -1
		End

	if(@OrderID > 0)
		Begin

			INSERT INTO [IndirectFeeTypes]
			   ([Type]
			   ,[Group]
			   ,[SourceType]
			   ,[Note]
			   ,[State]
			   ,[CreatedOn]
			   ,[CreatedBy]
			   ,[ModifiedOn]
			   ,[ModifiedBy])
	select 
	Source.[Type],
	source.Category,
	source.SourceType,
	source.Note,
	source.[State],
	source.[CreatedOn],
	source.[CreatedBy],
	source.[ModifiedOn],
	source.[ModifiedBy]
	from 
	--should be a unqiue list
	(
		select 
			distinct orderItem.Type as [Type],
			ext.Category as Category,
			1 as SourceType, --0: builtIn, 1:writeIn
			null as Note,
			ext.State as State,
			ext.CreatedOn as CreatedOn,
			ext.CreatedBy as CreatedBy,
			ext.ModifiedOn as ModifiedOn,
			ext.ModifiedBy as ModifiedBy
		from @OrderItems as orderItem cross apply(select top 1 * from @OrderItems where Category='INDIRECTFEE') as ext
		where orderItem.TypeID is null 
		and not exists(select * from IndirectFeeTypes as ift where ift.Type = orderItem.Type)
	) 
	as Source
	where not exists(select * from IndirectFeeTypes as ift where ift.Type = Source.Type)

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
					   @OrderID
					   ,orderItem.[ProductCode]
					   ,orderItem.[ProductName]
					   ,orderItem.[SpecModel]
					   ,orderItem.[Category]
					   ,isnull(orderItem.[TypeID],ift.ID)
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

				from @OrderItems as orderItem inner join IndirectFeeTypes as ift on orderItem.Type = ift.Type

			Set @Affected_cnt = @@ROWCOUNT
			Set @Return_Value = 0					
			
			update vb
			set vb.TotalAmount = 
			(Select SUM(voi.TotalAmount) 
			from VrentOrders as vo inner join VrentOrderItems as voi 
			on vo.ID = voi.OrderID and vo.ProxyBookingID = @ProxyBookingID
			where vo.State = 0 and voi.State = 0)
			from VrentBookings as vb
			where vb.ID = @ProxyBookingID
	
		END
	else
		Begin
			Set @Return_Value = -2
		End
END
return @Affected_cnt
