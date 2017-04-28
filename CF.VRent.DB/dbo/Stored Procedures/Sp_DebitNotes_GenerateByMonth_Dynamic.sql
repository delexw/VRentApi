CREATE PROCEDURE [dbo].[Sp_DebitNotes_GenerateByMonth_Dynamic]
@periodID int, @createdOn datetime, @createdBy uniqueidentifier, @return_value int OUTPUT
WITH EXEC AS CALLER
AS
DECLARE
   @tempBeginDate     DATETIME,
   @tempEndDate       DATETIME,
   @tempBillingDate   DATETIME,
   @tempDueDate       DATETIME,
   @state             TINYINT

BEGIN
   IF EXISTS
         (SELECT *
            FROM DebitNoteHistory AS dnh
           WHERE dnh.ID = @periodID)
      BEGIN
         SELECT @tempBeginDate = dnh.PeriodBegin,
                @tempEndDate = dnh.PeriodEnd,
                @tempBillingDate = dnh.BillingDate,
                @tempDueDate = dnh.DueDate,
                @state = dnh.State
         FROM DebitNoteHistory AS dnh
         WHERE dnh.ID = @periodID

         PRINT @tempBeginDate
         PRINT @tempEndDate
         PRINT @tempBillingDate
         PRINT @tempDueDate
         PRINT @state

         SET @return_value = 0
      END
   ELSE
      BEGIN
         SET @return_value = -1
      END

   PRINT @return_value

   IF (@return_value = 0)
      BEGIN
         --Step 1: insert new ones identified
         INSERT INTO dbo.debitnotes ([ClientID],
                                     [PeriodID],
                                     [BillingDate],
                                     [DueDate],
                                     [PaymentDate],
                                     [TotalAmount],
                                     [PaymentStatus],
                                     [Note],
                                     [State],
                                     [CreatedOn],
                                     [CreatedBy])
            SELECT Revenue.CorporateID AS ClientID,
                   @periodID AS PeriodID,
                   @tempBillingDate,
                   @tempDueDate,
                   NULL AS PaymentDate,
                   Revenue.Total AS TotalAmount,
                   0 AS PaymentStatus,
                   NULL AS Note,
                   0 AS State,
                   @createdOn AS CreatedOn,
                   @createdBy AS CreatedBy
              FROM (SELECT Source.CorporateID, sum (Source.Price) AS Total
                      FROM (--rental fee part
                            SELECT cb.KemasBookingID,
                                   cb.CorporateID,
                                   cb.Price
                              FROM CompletedBookings AS cb
                             WHERE     cb.BillingOption = 2
                                   AND cb.Price > 0
                                   AND (   (    (   cb.KeyIn IS NULL
                                                 OR cb.KeyOut IS NULL)
                                            AND cb.DateEnd >= @tempBeginDate
                                            AND cb.DateEnd < @tempEndDate)
                                        OR (    (   cb.KeyIn IS NOT NULL
                                                 OR cb.KeyOut IS NOT NULL)
                                            AND cb.KeyIn >= @tempBeginDate
                                            AND cb.KeyIn < @tempEndDate))
                            UNION ALL
                            SELECT vb.KemasBookingID,
                                   vb.CorporateID,
                                   voi.Total AS TotalAmount
                              FROM (SELECT vo.ProxyBookingID,
                                           sum (voi.TotalAmount) AS Total
                                      FROM dbo.VrentOrderItems AS voi
                                           INNER JOIN VrentOrders AS vo
                                              ON vo.ID = voi.OrderID
                                     WHERE     voi.Category = 'INDIRECTFEE'
                                           AND voi.TotalAmount > 0
                                           AND voi.CreatedOn >=
                                                  @tempBeginDate
                                           AND voi.CreatedOn < @tempEndDate
                                    GROUP BY vo.ProxyBookingID) AS voi
                                   INNER JOIN
                                   (SELECT vb.ID,
                                           vb.KemasBookingID,
                                           vb.CorporateID
                                      FROM VrentBookings AS vb
                                     WHERE     vb.BookingType = 2
                                           AND vb.TotalAmount > 0) AS vb
                                      ON vb.ID = voi.ProxyBookingID)
                           AS Source
                    GROUP BY Source.CorporateID) AS Revenue
             WHERE NOT EXISTS
                      (SELECT dn.ID
                         FROM DebitNotes AS dn
                        WHERE     dn.ClientID = Revenue.CorporateID
                              AND dn.ID = @periodID)


         --Step 2: update existing ones
         UPDATE DebitNotes
            SET TotalAmount = Revenue.Total,
                ModifiedOn = @createdOn,
                ModifiedBy = @createdBy
           FROM DebitNotes AS dn
                INNER JOIN
                (SELECT Source.CorporateID, sum (Source.Price) AS Total
                   FROM (--rental fee part
                         SELECT cb.KemasBookingID, cb.CorporateID, cb.Price
                           FROM CompletedBookings AS cb
                          WHERE     cb.BillingOption = 2
                                AND cb.Price > 0
                                AND (   (    cb.KeyIn IS NULL
                                         AND cb.DateEnd >= @tempBeginDate
                                         AND cb.DateEnd < @tempEndDate)
                                     OR (    cb.KeyIn IS NOT NULL
                                         AND cb.KeyIn >= @tempBeginDate
                                         AND cb.KeyIn < @tempEndDate))
                         UNION ALL
                         SELECT vb.KemasBookingID,
                                vb.CorporateID,
                                voi.Total AS TotalAmount
                           FROM (SELECT vo.ProxyBookingID,
                                        sum (voi.TotalAmount) AS Total
                                   FROM dbo.VrentOrderItems AS voi
                                        INNER JOIN VrentOrders AS vo
                                           ON vo.ID = voi.OrderID
                                  WHERE     voi.Category = 'INDIRECTFEE'
                                        AND voi.TotalAmount > 0
                                        AND voi.CreatedOn >= @tempBeginDate
                                        AND voi.CreatedOn < @tempEndDate
                                 GROUP BY vo.ProxyBookingID) AS voi
                                INNER JOIN
                                (SELECT vb.ID,
                                        vb.KemasBookingID,
                                        vb.CorporateID
                                   FROM VrentBookings AS vb
                                  WHERE     vb.BookingType = 2
                                        AND vb.TotalAmount > 0) AS vb
                                   ON vb.ID = voi.ProxyBookingID) AS Source
                 GROUP BY Source.CorporateID) AS Revenue
                   ON Revenue.CorporateID = dn.ClientID AND dn.ID = @periodID

         --Step3: final processing
         IF (@state = 0)
            BEGIN
               ----clean data in temp table
               --delete from CompletedBookings
               --where
               -- (KeyIn is null and DateEnd >= @tempBeginDate AND DateEnd < @tempEndDate)
               -- OR (KeyIn is not null and KeyIn >= @tempBeginDate AND KeyIn < @tempEndDate)

               UPDATE t
                  SET State = 1 --NotRun = 0, Preview = 1, Final = 2, Closed = 3;
                 FROM DebitNoteHistory AS t
                WHERE t.ID = @periodID
            END
         ELSE
            IF (@state = 1)
               BEGIN
                  --delete from CompletedBookings
                  --where
                  -- (KeyIn is null and DateEnd >= @tempBeginDate AND DateEnd < @tempEndDate)
                  -- OR (KeyIn is not null and KeyIn >= @tempBeginDate AND KeyIn < @tempEndDate)

                  UPDATE t
                     SET State = 2 --NotRun = 0, Preview = 1, Final = 2, Closed = 3;
                    FROM DebitNoteHistory AS t
                   WHERE t.ID = @periodID
               --clean data in temp table
               END
      END
END
GO