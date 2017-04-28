CREATE PROCEDURE [dbo].[Sp_GetUncompletedDUBBookings]
WITH EXEC AS CALLER
AS
SELECT vb.KemasBookingID,
       vb.UserID,
       vb.ID AS BookingID,
       vb.KemasBookingNumber,
       up.ID,
       up.Card_ID,
       vb.BookingType
  FROM VrentBookings vb
       LEFT JOIN BookingPayment bp ON vb.ID = bp.BookingID
       LEFT JOIN UnionPaymentMessageExchange up
          ON bp.UPPaymentID = up.ID AND bp.state = 1 AND up.State = 1
 WHERE vb.State IN ('swBookingModel/created',
                    'swBookingModel/released',
                    'swBookingModel/taken')