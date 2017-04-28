-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Sp_VrentBookings_Update]

    @ProxyBookingID int,
	@BookingType tinyint, 
	@KemasBookingID uniqueidentifier, 
	@KemasBookingNumber nvarchar(20),

	@DateBegin datetime,
	@DateEnd datetime,
	@TotalAmount decimal(10,3),

   --   ,[UserID]
   --   ,[UserFirstName]
   --   ,[UserLastName]
	@UserID uniqueidentifier, 
	@UserFirstName nvarchar(50),
	@UserLastName nvarchar(50),
	
   --   ,[CorporateID]
   --   ,[CorporateName]
	@CorporateID nvarchar(50),
	@CorporateName nvarchar(50),
	
	   --   ,[CreatorID]
   --   ,[CreatorFirstName]
   --   ,[CreatorLastName]
	@CreatorID uniqueidentifier, 
	@CreatorFirstName nvarchar(50),
	@CreatorLastName nvarchar(50),

	   --   ,[StartLocationID]
   --   ,[StartLocationName]
	@StartLocationID uniqueidentifier,
	@StartLocationName nvarchar(50),

		
	@State nvarchar(50), 
	@ModifiedOn datetime, 
	@ModifiedBy uniqueidentifier
	,
	@PricingInfoInput dbo.BookingPrice readonly,
	@PricingDetailsInput dbo.BookingPriceItem readonly
	

WITH EXEC AS CALLER
AS
declare 
@PricingRet int,
@PaymentRet int,
@Ret_UpdateFapiaoRequests int,
@BookingTypeOri int
--return value
--0: success
BEGIN
   
   set @BookingTypeOri = (select vb.BookingType from VrentBookings as vb 
   where vb.ID = @ProxyBookingID)
   
   Update [dbo].[VrentBookings] 
	Set 
		[BookingType] = @BookingType,
        [KemasBookingID] = @KemasBookingID,
        [KemasBookingNumber] = @KemasBookingNumber,
        [DateBegin] = @DateBegin,
        [DateEnd] = @DateEnd,
        [TotalAmount] = @TotalAmount,
        
        [UserID] = @UserID,
        [UserFirstName] = @UserFirstName,
        [UserLastName] = @UserLastName,
        
        [CorporateID] = @CorporateID,
        [CorporateName] = @CorporateName,
        
        [CreatorID] = @UserID,
        [CreatorFirstName] = @CreatorFirstName ,
        [CreatorLastName] = @CreatorLastName,

        [State] = @State,
        [ModifiedOn] = @ModifiedOn,
        [ModifiedBy] = @ModifiedBy
    where ID = @ProxyBookingID


EXEC	@PricingRet = [dbo].[Sp_VrentPrincing_CreateByBooking]
		@BookingID = @ProxyBookingID,
		@PrincingInfo = @PricingInfoInput,
		@PrincingDetailInfo = @pricingDetailsInput


        if(@BookingTypeOri = 3 and @BookingType = 2)
			Begin
				EXEC	@Ret_UpdateFapiaoRequests = [dbo].[Sp_EnableDisableFaPiaoRequests]
						@ProxyBookingID = @ProxyBookingID,
						@State = 1,
						@ModifiedOn = @ModifiedOn,
						@ModifiedBy = @ModifiedBy
			End
		else if(@BookingTypeOri = 2 AND @BookingType = 3)
			Begin
				EXEC	@Ret_UpdateFapiaoRequests = [dbo].[Sp_EnableDisableFaPiaoRequests]
						@ProxyBookingID = @ProxyBookingID,
						@State = 0,
						@ModifiedOn = @ModifiedOn,
						@ModifiedBy = @ModifiedBy
			End
		
   
	   begin
		   SELECT 		[ID]
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
			 FROM VrentBookings AS vb
			WHERE 
				  [ID] = @ProxyBookingID
				  AND [State] != 'swBookingModel/deleted'
	   end
	
--//Ref: Web Service Contract 2015-05-05.pdf

--   //swBookingModel/created,
--   //swBookingModel/released,
--   //swBookingModel/taken,
--   //swBookingModel/interrupted,
--   //swBookingModel/latereturn,
--   //swBookingModel/lostitem,
--   //swBookingModel/lostitem latereturn
--   //swBookingModel/completed,
--   //swBookingModel/canceled,
--   //swBookingModel/autocanceled,
--   //swBookingModel/deleted

END