CREATE procedure [dbo].[Sp_DebitNotes_Retrieve]
@clientID nvarchar(50),
@status int,
@periodBegin Datetime,
@periodEnd datetime,

@itemsPerPage int,
@pageNumber int,
@totalPages int output,
@return_value int output

as
declare 
		@tempBeginDate nvarchar(8),
		@tempEndDate nvarchar(8),
		@selectSql nvarchar(max) = '',
        @clientIDParam nvarchar(100),
        @statusParam nvarchar(100),
		@periodBeginParam nvarchar(100),
		@periodEndParam nvarchar(100)
begin
	if(@clientID is not null)
		begin
			set @clientIDParam = ' AND notes.ClientID = '''+ @clientID + ''''
		end
	else 
		begin
			set @clientIDParam = ''
		end

	if(@status is not null)
		begin
		--Unknown = -1, Pending = 0, Due = 1, OverDue = 2, Paid = 3
			set @statusParam = ' AND notes.PaymentStatus = '+ cast(@status as nvarchar(1))
		end
	else 
		begin
			set @statusParam = ''
		end

	if(@periodBegin IS NOT NULL)
		begin
			set @tempBeginDate = CONVERT(nvarchar(8), @periodBegin, 112)
			set @periodBeginParam = ' AND dnh.PeriodBegin >=''' + @tempBeginDate +''''
		end
	else 
		begin
			set @periodBeginParam = ''
		end

	if(@periodEnd IS NOT NULL)
		begin
			set @tempEndDate = CONVERT(nvarchar(8), DATEADD(Day,1, @periodEnd), 112)
			set @periodEndParam = ' AND dnh.PeriodEnd <  '''+ @tempEndDate + ''''
		end
	else 
		begin
			set @periodEndParam = ''
		end

    set @selectSql =  
			N'Select 
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
				notes.ModifiedBy from DebitNotes as notes 
				inner join DebitNoteHistory as dnh on dnh.ID=notes.PeriodID
		 Where 1 = 1  '
		 + @clientIDParam
		 + @statusParam
		 + @periodBeginParam
		 + @periodEndParam

    print @selectSql

	declare 
		@TotalRows int,
		@TotalCountSql nvarchar(max),
		@TotalCountParamerterDef nvarchar(1000),

		@Direction nvarchar(10),

		@StartRowSql nvarchar(max),
		@StartRowParameterDef nvarchar(1000),
		@StartRow int,
		@StartRowID int,


		@FindReadSql nvarchar(max),
		@FindReadParameterDef nvarchar(1000),
	
		@Reminder int

	Set @TotalCountParamerterDef = '@TotalRows int output'
    Set @TotalCountSql = N'Select @TotalRows = Count(*) from ('+@selectSql+') as Source'  
	--print '@TotalCountSql:'+@TotalCountSql

	EXEC sp_executesql @TotalCountSql, @TotalCountParamerterDef,@TotalRows OUTPUT
	--print @TotalRows

	if(@TotalRows > 0)
		Begin
			set @Reminder = @TotalRows % @itemsPerPage
			if(@Reminder = 0)
				set @totalPages = @TotalRows / @itemsPerPage
			else		    
				set @totalPages = @TotalRows / @itemsPerPage + 1
		End
	else
		begin
			set @totalPages = 0
		end
	
	--print @totalPages
	Set NOCount ON
	Set @StartRow = (@PageNumber - 1) * @itemsPerPage + 1

	Set @StartRowSql = N'Select @StartRowID = ID from ('+@selectSql+') as Source Order By Source.ID'
	--print N'@StartRowSql:'+@StartRowSql

    if(@StartRow <= @TotalRows)
		Begin
			Set RowCount @StartRow
			Set @StartRowParameterDef = '@StartRowID int output'
			EXEC sp_executesql @StartRowSql, @StartRowParameterDef, @StartRowID OUTPUT
			--print @StartRowID

			Set RowCount @ItemsPerPage;		
			Set @FindReadParameterDef = '@StartRowID int'
			Set @FindReadSql = N'Select * from ('+@selectSql+') as Source Where Source.ID >= @StartRowID Order By Source.ID' 
			--print '@FindReadSql:'+@FindReadSql

			EXEC sp_executesql @FindReadSql, @FindReadParameterDef, @StartRowID
		END

end