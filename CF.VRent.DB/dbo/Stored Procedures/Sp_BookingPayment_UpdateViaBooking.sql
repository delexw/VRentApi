
CREATE PROCEDURE [dbo].[Sp_BookingPayment_UpdateViaBooking] 
	(
		@bookingID int,
		@upPaymentID int,
		@state tinyint,
		@createdOn datetime,
		@createdBy uniqueidentifier,
		@returnValue int output
	)
AS
-- 0: update success
-- 1: booking Type is 2

declare @bookingType int
BEGIN

   select @bookingType =vb.BookingType from VrentBookings as vb where vb.ID = @bookingID and State != 'swBookingModel/deleted'
    
	if(@bookingType = 3)
		Begin
			Update bp
			set 
			bp.state = 0 -- disabled
			from BookingPayment as bp
			where bp.BookingID = @bookingID

			INSERT INTO [dbo].[BookingPayment]
			(
				[BookingID],
				UPPaymentID
				,[state]
				,[CreatedOn]
				,[CreatedBy]
			)
			values
			( 
				@BookingID,
				@upPaymentID,
				@state,
				@CreatedOn,
				@CreatedBy
			)
			set @returnValue = 0
		End
	else
		begin
			set @returnValue = 1
		End
END