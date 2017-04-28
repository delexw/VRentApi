CREATE PROCEDURE [dbo].[Sp_GetPaymentExchangeStateByBookingID]
@BookingId int
WITH EXEC AS CALLER
AS
SELECT up.State
  FROM DUB_RentalFee_BookingTransaction bp, UnionPaymentMessageExchange up
 WHERE bp.TransactionId = up.Unique_ID AND bp.BookingID = @BookingId
 order by up.ID desc
