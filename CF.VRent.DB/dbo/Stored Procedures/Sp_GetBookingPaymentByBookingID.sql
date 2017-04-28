
CREATE PROCEDURE [dbo].[Sp_GetBookingPaymentByBookingID]
@bookingId int
WITH EXEC AS CALLER
AS
SELECT UPPaymentID, state, CreatedOn
  FROM BookingPayment
 WHERE BookingID = @bookingId
