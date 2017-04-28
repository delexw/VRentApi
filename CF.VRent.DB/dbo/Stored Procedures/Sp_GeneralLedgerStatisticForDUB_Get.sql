CREATE PROCEDURE [dbo].[Sp_GeneralLedgerStatisticForDUB_Get]
@DateFrom datetime, @DateEnd datetime
WITH EXEC AS CALLER
AS
SELECT *
  FROM (SELECT vb.ID,
               vb.UserID,
               vb.BookingType,
               drb.UPPaymentID,
               2 AS FeeType
          FROM VrentBookings vb
               LEFT JOIN DUB_RentalFee_BookingTransaction drb
                  ON vb.ID = drb.BookingID
         WHERE     vb.State IN ('swBookingModel/completed',
                                'swBookingModel/canceled',
                                'swBookingModel/autocanceled')
               AND NOT EXISTS
                      (SELECT 'A'
                         FROM GeneralLedgerItemDetail glid
                        WHERE     PaymentID = drb.UPPaymentID
                              AND drb.UPPaymentID IS NOT NULL)) AS dub
       OUTER APPLY
       (SELECT ISNULL (
                    CONVERT (INT, ume.RealPreAuthPrice)
                  + CONVERT (INT, ume.DeductionPrice),
                  0)
                  AS RentCreditPrice,
               ISNULL (
                    CONVERT (INT, ume.RealPreAuthPrice)
                  + CONVERT (INT, ume.DeductionPrice),
                  0)
                  AS RentDebitPrice,
               ume.ModifiedOn AS RentalTime,
               ume.State AS RentalPaymentStatus
          FROM UnionPaymentMessageExchange ume
         WHERE     dub.UPPaymentID = ume.ID
               AND ume.ModifiedOn >= @DateFrom
               AND ume.ModifiedOn <= @DateEnd) AS bp2
UNION ALL
SELECT *
  FROM (SELECT vb.ID,
               vb.UserID,
               vb.BookingType,
               dib.UPPaymentID,
               1 AS FeeType
          FROM VrentBookings vb
               LEFT JOIN
               (SELECT DISTINCT BookingID, UPPaymentID
                  FROM DUB_IndirectFee_BookingTransaction) dib
                  ON vb.ID = dib.BookingID
         WHERE     vb.State IN ('swBookingModel/completed',
                                'swBookingModel/canceled',
                                'swBookingModel/autocanceled')
               AND NOT EXISTS
                      (SELECT 'A'
                         FROM GeneralLedgerItemDetail glid
                        WHERE     PaymentID = dib.UPPaymentID
                              AND dib.UPPaymentID IS NOT NULL)) AS dub
       OUTER APPLY
       (SELECT ISNULL (CONVERT (INT, ume.DeductionPrice), 0)
                  AS RentCreditPrice,
               ISNULL (CONVERT (INT, ume.DeductionPrice), 0)
                  AS RentDebitPrice,
               ume.ModifiedOn AS RentalTime,
               ume.State AS RentalPaymentStatus
          FROM UnionPaymentMessageExchange ume
         WHERE     dub.UPPaymentID = ume.ID
               AND ume.ModifiedOn >= @DateFrom
               AND ume.ModifiedOn <= @DateEnd
               AND ISNULL (CONVERT (INT, ume.DeductionPrice), 0) >= 0) AS bp2