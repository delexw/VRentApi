CREATE PROCEDURE [dbo].[Sp_GeneralLedgerStatisticForCCB_Get]
@DateFrom datetime, @DateEnd datetime
WITH EXEC AS CALLER
AS
SELECT DebitNotes.ClientID,
       DebitNotes.ID,
       ISNULL (DebitNotes.TotalAmount, 0) AS CCBTotalDebit,
       ISNULL (DebitNotes.TotalAmount, 0) AS CCBTotalCredit
  FROM DebitNotes, DebitNoteHistory
 WHERE     DebitNotes.PeriodID = DebitNoteHistory.ID
       AND DebitNoteHistory.PeriodBegin >= @DateFrom
       AND DebitNoteHistory.PeriodEnd < @DateEnd
       AND NOT EXISTS
              (SELECT 'A'
                 FROM GeneralLedgerItemDetail
                WHERE DebitNoteID = DebitNotes.ID)