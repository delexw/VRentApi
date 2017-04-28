CREATE PROCEDURE [dbo].[Sp_DebitNotes_RetrieveByID] 
	@debitNoteID int 
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		notes.ID,
		notes.ClientID,
		notes.PeriodID,
		dnh.Period,
		dnh.PeriodBegin,
		dnh.PeriodEnd,
		notes.BillingDate,
		notes.DueDate, 
		notes.PaymentDate,
		notes.TotalAmount,
		notes.PaymentStatus,
		notes.Note,
		notes.State,
		notes.CreatedOn,
		notes.CreatedBy,
		notes.ModifiedOn,
		notes.ModifiedBy
	from DebitNotes as notes 
	inner join DebitNoteHistory as dnh on notes.PeriodID = dnh.ID
	where notes.ID = @debitNoteID
END