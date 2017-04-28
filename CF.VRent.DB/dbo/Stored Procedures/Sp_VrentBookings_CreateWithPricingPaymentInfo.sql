-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Sp_VrentBookings_CreateWithPricingPaymentInfo]
	  --[ID]
	  
   --   ,[BookingType]
   --   ,[KemasBookingID]
   --   ,[KemasBookingNumber]
	@BookingType tinyint, 
	@KemasBookingID uniqueidentifier, 
	@KemasBookingNumber nvarchar(20),

   --   ,[DateBegin]
   --   ,[DateEnd]
   --   ,[TotalAmount]
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
   
   --   ,[State]
   --   ,[CreatedOn]
   --   ,[CreatedBy]
   --   ,[ModifiedOn]
   --   ,[ModifiedBy]
	@State nvarchar(50), 
	@CreatedOn datetime,
	@CreatedBy uniqueidentifier,
	@PricingInfoInput dbo.BookingPrice readonly,
	@PricingDetailsInput dbo.BookingPriceItem readonly,
	@UPPaymentInput dbo.BookingPayment readonly
	


WITH EXEC AS CALLER
AS
BEGIN
   DECLARE @lastID      TABLE (InsertedID   INT)
   DECLARE	@return_value int
   DECLARE  @InsertedID int
	DECLARE	@Payment_ret int

   INSERT INTO [dbo].[VrentBookings] 
   (
        [BookingType]
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
        
        ,CreatorID
        ,CreatorFirstName 
        ,CreatorLastName
        
        ,[StartLocationID]
        ,[StartLocationName] 

        ,[State]
        ,[CreatedOn]
        ,[CreatedBy])

   OUTPUT inserted.ID
     INTO @lastID
   VALUES (
		   @BookingType,
		   @KemasBookingID,
		   @KemasBookingNumber,
		   @DateBegin,
		   @DateEnd,
		   @TotalAmount,

           @UserID,
           @UserFirstName,
           @UserLastName,
                      
		   @CorporateID,
		   @CorporateName,
		   
		   @CreatorID,
		   @CreatorFirstName,
		   @CreatorLastName,
		   
		   @StartLocationID,
		   @StartLocationName,
		   
           @State,
           @CreatedOn,
           @CreatedBy)

set @InsertedID = (SELECT TOP 1 InsertedID FROM @lastID)

EXEC	@return_value = [dbo].[Sp_VrentPrincing_CreateByBooking]
		@BookingID = @InsertedID,
		@PrincingInfo = @PricingInfoInput,
		@PrincingDetailInfo = @pricingDetailsInput


EXEC	@return_value = [dbo].[Sp_BookingPayment_CreateViaBooking]
		@BookingID = @InsertedID,
		@UPPayment = @UPPaymentInput
		

        if(@BookingType = 3)
        begin
			EXEC	@return_value = [dbo].[Sp_CreateFaPiaoRequestItems]
					@ProxyBookingID = @InsertedID,
					@CreatedOn = @CreatedOn,
					@CreatedBy = @CreatedBy        
        end

   if(@return_value = 0)
   Begin
	   SELECT 
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
		 FROM VrentBookings AS vb
		WHERE     vb.KemasBookingID = @KemasBookingID
			  AND [ID] = (SELECT TOP 1 InsertedID FROM @lastID)
			  AND [State] != 'swBookingModel/deleted'
	End
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