CREATE PROCEDURE [dbo].[Sp_VrentBookings_SyncState]
   	@BookingType tinyint, 
	@KemasBookingID uniqueidentifier, 
	@KemasBookingNumber nvarchar(20),

	@DateBegin datetime,
	@DateEnd datetime,
	@TotalAmount decimal(10,3),

	@UserID uniqueidentifier, 
	@CorporateID nvarchar(50),

	@State nvarchar(50), 
	@CreatedOn datetime,
	@CreatedBy uniqueidentifier,
	@ModifiedOn datetime,
    @ModifiedBy uniqueidentifier,

	@PricingInfoInput dbo.BookingPrice readonly,
	@PricingDetailsInput dbo.BookingPriceItem readonly
    
AS
DECLARE	@update_ret int
DECLARE	@Insert_ret int

BEGIN
	IF  NOT EXISTS(SELECT * FROM VRentBookings WHERE KemasBookingID = @KemasBookingID)
		BEGIN
			EXEC	@Insert_ret = [dbo].[Sp_VrentBookings_Create]
					@BookingType = @BookingType,
					@KemasBookingID = @KemasBookingID,
					@KemasBookingNumber = @KemasBookingNumber,
					@DateBegin = @DateBegin,
					@DateEnd = @DateEnd,
					@TotalAmount = @TotalAmount,
					@UserID = @UserID,
					@CorporateID = @CorporateID,
					@State = @State,
					@CreatedOn = @CreatedOn,
					@CreatedBy = @CreatedBy,
					@PricingInfoInput = @PricingInfoInput,
					@PricingDetailsInput = @PricingDetailsInput
		END
	ELSE
		BEGIN
		EXEC	@update_ret = [dbo].[Sp_VrentBookings_UpdateState]
				@BookingType = @BookingType,
				@KemasBookingID = @KemasBookingID,
				@KemasBookingNumber = @KemasBookingNumber,
				
				@DateBegin = @DateBegin,
				@DateEnd = @DateEnd,
				@TotalAmount = @TotalAmount,
				@UserID = @UserID,
				@CorporateID = @CorporateID,		

				@State = @State,
				@ModifiedOn = @ModifiedOn,
				@ModifiedBy = @ModifiedBy
    
		END

	
END