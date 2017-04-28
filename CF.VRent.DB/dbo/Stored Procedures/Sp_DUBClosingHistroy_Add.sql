
CREATE PROCEDURE Sp_DUBClosingHistroy_Add
@period nvarchar(50),
@beginDate datetime,
@endDate datetime,
@generationType tinyint,
@state tinyint,
@createdOn datetime,
@createdBy uniqueidentifier,
@return_value int output

--return_value: -1, not right gap
as
begin
	IF exists
	(
		select dch.ID from DubClosingHistroy as dch 
		where (@beginDate >= dch.PeriodBegin and @beginDate <= dch.PeriodEnd) OR (@endDate >= dch.PeriodBegin and @endDate <= dch.PeriodEnd)
	)
		begin
			set @return_value = -1
		end
	else
		begin
			INSERT INTO [dbo].[DUBClosingHistory]
			   (
				   [Period]
				   ,[PeriodBegin]
				   ,[PeriodEnd]
				   ,[GenerationType]
				   ,[State]
				   ,[CreatedOn]
				   ,[CreatedBy]
			   )
			VALUES
			   (
					@period
					,@beginDate
					,@endDate
					,@generationType
					,@state
					,@createdOn
					,@createdBy
				)
		end
end