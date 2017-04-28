CREATE PROCEDURE [dbo].[Sp_DebitNotes_UpdateByID]
@debitNoteID int,
@clientID uniqueidentifier,
@paymentDate datetime,
@totalAmount decimal(18,3),
@note nvarchar(200),
@paymentState tinyint,
@state tinyint,
@modifiedOn datetime,
@modifiedBy uniqueidentifier
AS
BEGIN
	SET NOCOUNT ON;
	update notes
		set 
			notes.PaymentDate = @paymentDate
			,notes.Note = @note
			,notes.PaymentStatus = @paymentState
			,notes.[State] = @state
			,ModifiedOn = @modifiedOn
			,ModifiedBy = @modifiedBy
	from DebitNotes as notes
	where notes.ID = @debitNoteID

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