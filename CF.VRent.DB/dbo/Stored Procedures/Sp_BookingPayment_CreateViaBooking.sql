
Create PROCEDURE [dbo].[Sp_BookingPayment_CreateViaBooking] 
	(@BookingID int,
	@UPPayment dbo.BookingPayment readonly)

AS

BEGIN
   DECLARE @newPriceID      TABLE (InsertedID   INT)


	INSERT INTO [dbo].[BookingPayment]
			   (
			   [BookingID],
			   UPPaymentID
			   ,[state]
			   ,[CreatedOn]
			   ,[CreatedBy]
			   ,[ModifiedOn]
			   ,[ModifiedBy])
	Select 
	@BookingID,
	payment.UPPaymentID,
	payment.[state],
	payment.[CreatedOn],
	payment.[CreatedBy],
	payment.[ModifiedOn],
	payment.[ModifiedBy]
	from @UPPayment as payment
END




