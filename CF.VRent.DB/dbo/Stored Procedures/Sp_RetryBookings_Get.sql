CREATE PROCEDURE [dbo].[Sp_RetryBookings_Get]
WITH EXEC AS CALLER
AS
--table parameter for indirect fee
WITH indirectBookings
     AS (SELECT vb.ID AS BookingId,
                voi.ID AS OrderItemId,
                ume.ID AS PaymentId,
                uci.Card_ID AS OldCard,
                ume.PreAuthPrice,
                ume.RealPreAuthPrice,
                ume.PreAuthQueryID,
                ume.DeductionPrice,
                ume.State,
                vb.UserID
           FROM VrentBookings vb
                INNER JOIN VrentOrders vo ON vb.ID = vo.ProxyBookingID
                INNER JOIN VrentOrderItems voi ON vo.ID = voi.OrderID
                LEFT JOIN BookingIndirectFeePayment bp
                   ON     vb.ID = bp.BookingID
                      AND bp.state = 1
                      AND voi.ID = bp.OrderItemID
                LEFT JOIN UnionPaymentMessageExchange ume
                   ON bp.UPPaymentID = ume.ID
                LEFT JOIN UnionCardInfo uci ON ume.Card_ID = uci.Card_ID
          WHERE vb.BookingType = 3 AND ume.Retry = 1)
--DECLARE @indirectBookings TABLE
--                          (
--                             BookingId          INT,
--                             OrderItemId        NVARCHAR (50),
--                             PaymentId          INT,
--                             OldCard            NVARCHAR (MAX),
--                             PreAuthPrice       NVARCHAR (10),
--                             RealPreAuthPrice   NVARCHAR (10),
--                             PreAuthQueryID     NVARCHAR (MAX),
--                             DeductionPrice     NVARCHAR (10),
--                             State              INT,
--                             UserID             NVARCHAR (50)
--                          );
--
----insert records
--INSERT INTO @indirectBookings (BookingId,
--                               OrderItemId,
--                               PaymentId,
--                               OldCard,
--                               PreAuthPrice,
--                               RealPreAuthPrice,
--                               PreAuthQueryID,
--                               DeductionPrice,
--                               State,
--                               UserID)
--   SELECT vb.ID AS BookingId,
--          voi.ID AS OrderItemId,
--          ume.ID AS PaymentId,
--          uci.Card_ID AS OldCard,
--          ume.PreAuthPrice,
--          ume.RealPreAuthPrice,
--          ume.PreAuthQueryID,
--          ume.DeductionPrice,
--          ume.State,
--          vb.UserID
--     FROM VrentBookings vb
--          INNER JOIN VrentOrders vo ON vb.ID = vo.ProxyBookingID
--          INNER JOIN VrentOrderItems voi ON vo.ID = voi.OrderID
--          LEFT JOIN BookingIndirectFeePayment bp
--             ON     vb.ID = bp.BookingID
--                AND bp.state = 1
--                AND voi.ID = bp.OrderItemID
--          LEFT JOIN UnionPaymentMessageExchange ume
--             ON bp.UPPaymentID = ume.ID
--          LEFT JOIN UnionCardInfo uci ON ume.Card_ID = uci.Card_ID
--    WHERE vb.BookingType = 3 AND ume.Retry = 1
--get the sets
SELECT regularBookings.BookingId,
       NULL AS OrderItemId,
       regularBookings.PaymentId,
       regularBookings.OldCard,
       regularBookings.PreAuthPrice,
       regularBookings.RealPreAuthPrice,
       regularBookings.PreAuthQueryID,
       regularBookings.DeductionPrice,
       regularBookings.State,
       regularBookings.UserID,
       uc.Card_ID AS NewCard,
       '0' AS Operation                                                 -- Fee
  FROM (SELECT vb.ID AS BookingId,
               uci.Card_ID AS OldCard,
               ume.ID AS PaymentId,
               ume.PreAuthPrice,
               ume.RealPreAuthPrice,
               ume.PreAuthQueryID,
               ume.DeductionPrice,
               ume.State,
               vb.UserID
          FROM VrentBookings vb
               LEFT JOIN BookingPayment bp
                  ON vb.ID = bp.BookingID AND bp.state = 1
               LEFT JOIN UnionPaymentMessageExchange ume
                  ON bp.UPPaymentID = ume.ID
               LEFT JOIN UnionCardInfo uci ON ume.Card_ID = uci.Card_ID
         WHERE vb.BookingType = 3 AND ume.Retry = 1) regularBookings
       LEFT JOIN UnionCardInfo uc
          ON regularBookings.UserID = uc.[User_ID] AND uc.State = 1
UNION
SELECT regularBookings.BookingId,
       regularBookings.OrderItemId,
       regularBookings.PaymentId,
       regularBookings.OldCard,
       regularBookings.PreAuthPrice,
       regularBookings.RealPreAuthPrice,
       regularBookings.PreAuthQueryID,
       regularBookings.DeductionPrice,
       regularBookings.State,
       regularBookings.UserID,
       uc.Card_ID AS NewCard,
       '2' AS Operation                                        -- Indirect fee
  FROM (SELECT DISTINCT
               ibs.BookingId,
               ibs.PaymentId,
               ibs.OldCard,
               ibs.PreAuthPrice,
               ibs.RealPreAuthPrice,
               ibs.PreAuthQueryID,
               ibs.DeductionPrice,
               ibs.State,
               ibs.UserID,
               Stuff (
                  (SELECT ',' + CONVERT (NVARCHAR, ib.OrderItemId)
                     FROM indirectBookings ib
                    WHERE     ib.BookingId = ibs.BookingId
                          AND ib.PaymentId = ibs.PaymentId
                   FOR XML PATH ( '' )),
                  1,
                  1,
                  '')
                  AS OrderItemId
          FROM indirectBookings ibs) regularBookings
       LEFT JOIN UnionCardInfo uc
          ON regularBookings.UserID = uc.[User_ID] AND uc.State = 1