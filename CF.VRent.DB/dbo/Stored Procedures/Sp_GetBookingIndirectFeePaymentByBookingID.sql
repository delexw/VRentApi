
CREATE PROCEDURE [dbo].[Sp_GetBookingIndirectFeePaymentByBookingID]
@BookingID int
WITH EXEC AS CALLER
AS
SELECT *
  FROM BookingIndirectFeePayment
 WHERE BookingID = @BookingID AND State = 1
