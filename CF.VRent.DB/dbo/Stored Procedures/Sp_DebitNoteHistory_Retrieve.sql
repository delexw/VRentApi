CREATE PROCEDURE [dbo].[Sp_DebitNoteHistory_Retrieve]
AS
declare @return_value int, 
@DefaultCreateDate datetime, 
@DefaultCreateBy uniqueidentifier,
@ret int

BEGIN
declare @currentMonth nvarchar(8)

	SET NOCOUNT ON;

	if NOT Exists(select * from DebitNoteHistory as dnh where dnh.State != 3)
		begin
			set @DefaultCreateDate = GETDATE()
			set @DefaultCreateBy = '99999999-9999-9999-9999-999999999999'
			EXEC @return_value = [dbo].[Sp_DebitNoteSchedule_Generate]
				@createdOn = @DefaultCreateDate,
				@createdBy = @DefaultCreateBy,
				@return_value = @ret output
		end

	set @currentMonth = CONVERT(nvarchar(6), GETDATE(),112) + '01' 
	print @currentMonth

	--only retrieve period in the past
	SELECT 
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
	  FROM [dbo].[DebitNoteHistory] as dnh 
	  where dnh.[State] in (0,1) and dnh.PeriodBegin < @currentMonth
END
