CREATE PROCEDURE [dbo].[Sp_VrentBookings_CancelWithPricing] 
	@ProxyBookingID int,
	@TotalAmount decimal(10,3),
	@State nvarchar(50),
	@ModifiedOn datetime,
    @ModifiedBy uniqueidentifier
    
    ,
    
    --pricing info
	@PricingInfoInput dbo.BookingPrice readonly,
	@PricingDetailsInput dbo.BookingPriceItem readonly
    
AS
BEGIN

   DECLARE	@return_value int,
   @BookingType int
   
Update [dbo].[VrentBookings]
 set  
     [TotalAmount] = @TotalAmount
     ,[State] = @State
     ,[ModifiedOn] = @ModifiedOn
     ,[ModifiedBy] = @ModifiedBy
where ID = @ProxyBookingID and [State] != 'swBookingModel/deleted' --0: active, 1: deleted 

--update corresponding pricing
EXEC	@return_value = [dbo].[Sp_VrentPrincing_CreateByBooking]
		@BookingID = @ProxyBookingID,
		@PrincingInfo = @PricingInfoInput,
		@PrincingDetailInfo = @pricingDetailsInput

	--remove Fapiao Requests in case booking is cancelled
	Select @BookingType = vb.BookingType from VrentBookings as vb where vb.ID = @ProxyBookingID and State != 'swBookingModel/deleted'
	if(@BookingType = 3 AND cast(@TotalAmount as decimal(10,3)) = 0.000) --decimal(10,3)
	Begin
		Update FR
		set FR.State = 1
		from FapiaoRequests as FR
		Where FR.ProxyBookingID = @ProxyBookingID and FR.State = 0
	End
	--remove Fapiao Requests in case booking is cancelled
END

	Select
		[ID]
      ,[BookingType]
      ,[KemasBookingID]
      ,[KemasBookingNumber]
      ,[DateBegin]
      ,[DateEnd]
      ,[TotalAmount]

      ,[UserID]
      ,[UserFirstName]
      ,[UserLastName]

      ,[CorporateID]
      ,[CorporateName]

      ,[CreatorID]
      ,[CreatorFirstName]
      ,[CreatorLastName]

      ,[StartLocationID]
      ,[StartLocationName]

      ,[State]
      ,[CreatedOn]
      ,[CreatedBy]
      ,[ModifiedOn]
      ,[ModifiedBy]
	from VrentBookings as vb 
	where vb.ID = @ProxyBookingID and [State] != 'swBookingModel/deleted'