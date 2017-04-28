Create PROCEDURE [dbo].[Sp_DebitNote_RetrieveCompletedPeriods]

AS
BEGIN
	SET NOCOUNT ON;

	select 
	[ID]
      ,[Period]
      ,[PeriodBegin]
      ,[PeriodEnd]
      ,[BillingDate]
      ,[DueDate]
      ,[State]
      ,[CreatedOn]
      ,[CreatedBy]
      ,[ModifiedOn]
      ,[ModifiedBy]
	  from DebitNoteHistory as dnh
	  where dnh.ID <= 
		(SELECT MAX(ID) as ID FROM [dbo].[DebitNoteHistory] as dnh  where dnh.[State] > 0)

END
