CREATE PROCEDURE [dbo].[Sp_DebitNotePeriod_RetrieveByState]
@state tinyint, @CreatedOn datetime, @CreatedBy uniqueidentifier, @debitMonth int
WITH EXEC AS CALLER
AS
DECLARE @initialRun   TINYINT;                    -- 0: initial, 1: has runned
DECLARE @return_value   TINYINT
DECLARE @jobMonth   NVARCHAR (8)
DECLARE @currentMonth   NVARCHAR (8)

BEGIN
   SET  NOCOUNT ON;

   IF NOT EXISTS
         (SELECT *
            FROM DebitNoteHistory AS dnh
           WHERE dnh.State != 3)
      BEGIN
         EXEC @return_value =
                 [dbo].[Sp_DebitNoteSchedule_Generate] @createdOn = @CreatedOn,
                                                       @createdBy = @CreatedBy,
                                                       @return_value = @return_value OUTPUT
      END

   -- >0 means a customized month
   IF @debitMonth > 0
      BEGIN
         -- in this case, find the month from previous time
         IF @debitMonth > DatePART (m, getdate ())
            SET @jobMonth =
                     CONVERT (
                        NVARCHAR (6),
                        DATEADD (
                           Month,
                           -12 + @debitMonth - DatePART (m, getdate ()),
                           GETDATE ()),
                        112)
                   + '01'
         ELSE
            SET @jobMonth =
                     CONVERT (
                        NVARCHAR (6),
                        DATEADD (Month,
                                 @debitMonth - DatePART (m, getdate ()),
                                 GETDATE ()),
                        112)
                   + '01'

         PRINT @jobMonth
      END
   -- <=0 means get the previous month base on current date
   ELSE
      BEGIN
         SET @jobMonth =
                  CONVERT (NVARCHAR (6),
                           DATEADD (Month, -1, GETDATE ()),
                           112)
                + '01'
      END

   --SET @jobMonth =
   --CONVERT (NVARCHAR (6), DATEADD (Month, -1, GETDATE ()), 112) + '01'
   --PRINT @jobMonth

   SET @currentMonth = CONVERT (NVARCHAR (6), GETDATE (), 112) + '01'

   --PRINT @currentMonth

   SELECT [ID],
          [Period],
          [PeriodBegin],
          [PeriodEnd],
          [BillingDate],
          [DueDate],
          [State],
          [CreatedOn],
          [CreatedBy],
          [ModifiedOn],
          [ModifiedBy]
     FROM [dbo].[DebitNoteHistory] AS dnh
    WHERE     dnh.[State] = @state
          AND CONVERT (NVARCHAR (8), dnh.PeriodBegin, 112) = @jobMonth
--AND dnh.PeriodBegin < @currentMonth
END
GO
