CREATE PROCEDURE [dbo].[Sp_BookingPaymentState_Update]
@State tinyint, @UserId uniqueidentifier, @BookingId int, @PaymentId int
WITH EXEC AS CALLER
AS
UPDATE BookingPayment
   SET state = @state, ModifiedOn = getdate (), ModifiedBy = @UserId
 WHERE BookingID = @BookingId