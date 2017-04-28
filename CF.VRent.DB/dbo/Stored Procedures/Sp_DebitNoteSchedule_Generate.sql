

CREATE Procedure [dbo].[Sp_DebitNoteSchedule_Generate]
@createdOn datetime,
@createdBy uniqueidentifier,
@return_value int output

as
	declare @beginDateStr nvarchar(8)
	declare @beginDate datetime

	declare @earliestMonth datetime
	declare @monthToCreate int

	if not exists(select dnh.ID from DebitNoteHistory as dnh where State = 0)
		begin
			Select @earliestMonth = cast(CONVERT(varchar(6), min(vb.DateEnd), 112) +'01' as datetime), @monthToCreate = 12 - Month(min(vb.DateEnd)) + 1
			from VrentBookings as vb where vb.BookingType = 2
			print @earliestMonth
			print @monthToCreate

		EXEC	@return_value = [dbo].[Sp_DebitNoteJobSchedule]
		@initialDate = @earliestMonth,
		@number = @monthToCreate,
		@createdOn = @createdOn,
		@createdBy = @createdBy

		end
	else
		begin
			set @beginDateStr = cast(Year(GETDATE()) as nvarchar(4)) + '0101'
			set @beginDate = CAST( @beginDateStr as smalldatetime)

			print @beginDateStr

			if( NOT EXISTS(select * from DebitNoteHistory as dnh where dnh.State = 0 AND dnh.PeriodBegin >= @beginDate or dnh.PeriodEnd >= @beginDate ))
				begin
					EXEC	@return_value = [dbo].[Sp_DebitNoteJobSchedule]
					@initialDate = @beginDateStr,
					@number = 12,
					@createdOn = @createdOn,
					@createdBy = @createdBy
				end
		end
GO