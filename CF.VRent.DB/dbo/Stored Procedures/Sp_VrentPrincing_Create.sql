

CREATE PROCEDURE [dbo].[Sp_VrentPrincing_Create] 
	(@BookingID int
,
	@PrincingInfo dbo.BookingPrice readonly,
	@PrincingDetailInfo dbo.BookingPriceItem readonly
)

AS

BEGIN
   DECLARE @newPriceID      TABLE (InsertedID   INT)

	--update an existing one to delete state
	update dbo.BookingPrice 
	set state = 1
	where BookingID = @BookingID

 	--update an existing one to delete state
	update dbo.BookingPriceItem  
	set state = 1 
	from BookingPriceItem as bpi inner join BookingPrice as bp on bpi.PrincingID = bp.ID
	where bp.BookingID = @BookingID


	INSERT INTO [dbo].[BookingPrice]
			   ([BookingID]
			   ,[Total]
			   ,[TimeStamp]
			   ,[TagID]
			   ,[state]
			   ,[CreatedOn]
			   ,[CreatedBy]
			   ,[ModifiedOn]
			   ,[ModifiedBy])

    output inserted.ID into @newPriceID
	Select 
	price.BookingID,
	price.[Total],
	price.[TimeStamp],
	price.[TagID],
	price.[state],
	price.[CreatedOn],
	price.[CreatedBy],
	price.[ModifiedOn],
	price.[ModifiedBy]
	from @PrincingInfo as price



	INSERT INTO [dbo].[BookingPriceItem]
			   ([PrincingID]
			   ,[Description]
			   ,[Group]
			   ,[Category]
			   ,[Type]
			   ,[UnitPrice]
			   ,[Quantity]
			   ,[Total]
			   ,[State]
			   ,[CreatedOn]
			   ,[CreatedBy]
			   ,[ModifiedOn]
			   ,[ModifiedBy])
     select 
	 (SELECT TOP 1 InsertedID FROM @newPriceID),
	 priceDetail.[Description],
	 priceDetail.[Group],
	 priceDetail.[Category],
	 priceDetail.[Type],
	 priceDetail.[UnitPrice],
	 priceDetail.[Quantity],
	 priceDetail.[Total],
	 priceDetail.[State],
	 priceDetail.[CreatedOn],
	 priceDetail.[CreatedBy],
	 priceDetail.[ModifiedOn],
	 priceDetail.[ModifiedBy] 
	 from @PrincingDetailInfo as priceDetail

	 --update booking price info incase price changed
	 update VrentBookings
	 set TotalAmount = (select pi.Total from @PrincingInfo as pi)
	 where ID = @BookingID and State != 'swBookingModel/deleted'

--finally read out
DECLARE	@return_value int
EXEC	@return_value = [dbo].[Sp_VrentPrincing_Read]
		@BookingID = @BookingID
END

