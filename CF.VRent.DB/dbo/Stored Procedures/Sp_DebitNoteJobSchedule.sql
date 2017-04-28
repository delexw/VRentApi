

CREATE Procedure [dbo].[Sp_DebitNoteJobSchedule]
@initialDate datetime,
@number int,
@createdOn datetime,
@createdBy uniqueidentifier

as
begin
		;WITH
		Pass0 as (select 1 as C union all select 1), --2 rows
		Pass1 as (select 1 as C from Pass0 as A, Pass0 as B),--4 rows
		Pass2 as (select 1 as C from Pass1 as A, Pass1 as B),--16 rows
		Tally as (select row_number() over(order by C) as Number from Pass2)

		Insert into DebitNoteHistory
			([Period]
			,[PeriodBegin]
			,[PeriodEnd]
			,[BillingDate]
			,[DueDate]
			,[State]
			,[CreatedOn]
			,[CreatedBy])
		SELECT
			CONVERT(varchar(8), DATEAdd(m, t.Number-1, @initialDate), 112) + '-' 
			+CONVERT(varchar(8),DATEADD(MILLISECOND, -2, DATEADD(m, DATEDIFF(m, 0, @initialDate) + t.Number, 0)),112) as Period,
			DATEAdd(m, t.Number-1, @initialDate) as PeriodBegin,
			DATEADD(m, DATEDIFF(m, 0, @initialDate) + t.Number, 0) as PeriodEnd,
			DATEADD(DAY, 1, DATEADD(m, DATEDIFF(m, 0, @initialDate) + t.Number, 0)) as BillingDate,
			DATEADD(DAY, 31, DATEADD(m, DATEDIFF(m, 0, @initialDate) + t.Number, 0)) as DueDate,
			0 as State,
			@createdOn as CreatedOn,
			@createdBy as CreatedBy
		FROM Tally as t
		WHERE Number <= @number
end