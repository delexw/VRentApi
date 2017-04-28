
CREATE PROCEDURE [dbo].[Sp_GetBookingPaymentByPaymentID]
@paymentId int
WITH EXEC AS CALLER
AS
SELECT BookingID, state, CreatedOn
  FROM BookingPayment
 WHERE UPPaymentID = @paymentId
