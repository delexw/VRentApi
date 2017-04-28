CREATE PROCEDURE [dbo].[Sp_BookingFaliedTransaction_Get]
@BookingId int
WITH EXEC AS CALLER
AS
DECLARE @bookingType   TINYINT

SELECT @bookingType = BookingType
FROM VrentBookings
WHERE ID = @BookingId

IF @bookingType = 2
   BEGIN
      SELECT DISTINCT ume.ID AS PaymentID, ume.State, ume.Retry
        FROM VrentBookings vb,
             DUB_IndirectFee_BookingTransaction bif,
             UnionPaymentMessageExchange ume
       WHERE     vb.ID = bif.BookingID
             AND bif.TransactionId = ume.Unique_ID
             AND vb.ID = @BookingId
   END
ELSE
   BEGIN
      SELECT DISTINCT ume.ID AS PaymentID, ume.State, ume.Retry
        FROM VrentBookings vb,
             DUB_RentalFee_BookingTransaction bp,
             UnionPaymentMessageExchange ume
       WHERE     vb.ID = bp.BookingID
             AND bp.TransactionId = ume.Unique_ID
             AND vb.ID = @BookingId
      UNION ALL
      SELECT DISTINCT ume.ID AS PaymentID, ume.State, ume.Retry
        FROM VrentBookings vb,
             DUB_IndirectFee_BookingTransaction bif,
             UnionPaymentMessageExchange ume
       WHERE     vb.ID = bif.BookingID
             AND bif.TransactionId = ume.Unique_ID
             AND vb.ID = @BookingId
   END