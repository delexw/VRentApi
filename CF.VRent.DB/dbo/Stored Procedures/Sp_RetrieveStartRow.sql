CREATE procedure [dbo].[Sp_RetrieveStartRow]
	@FromSql nvarchar(max),
	@WhereCondition Nvarchar(max),
	@OrderByCondition nvarchar(max),
	@TotalRows int,
	@ItemsPerPage int,
	@PageNumber int,
	@StartRowID int output,
	@TotalPage int output
as
DECLARE 
	@StartRowSql nvarchar(1000),
	@StartRowParameterDef nvarchar(1000),
	@StartRow int,
    @Reminder int,
    @ReturnValue int
	
BEGIN
	if(@TotalRows > 0)
		Begin
			set @Reminder = @TotalRows % @itemsPerPage
			if(@Reminder = 0)
				set @TotalPage = @TotalRows / @itemsPerPage
			else		    
				set @TotalPage = @TotalRows / @itemsPerPage + 1

			Set @StartRow = (@PageNumber - 1) * @itemsPerPage + 1
			Set @StartRowSql = N'Select @StartRowID= notes.ID ' +@FromSql+ ' ' + @WhereCondition + ' '+ @OrderByCondition
			print N'@StartRowSql:'+@StartRowSql
		End
	else
		Begin
			set @TotalPage = 0
		End   
	Set NOCount ON	    

	print @StartRow
	print @TotalPage
    if( @StartRow >= 1 AND @StartRow <= @TotalRows)
		Begin
			Set RowCount @StartRow
			Set @StartRowParameterDef = '@WhereCondition nvarchar(max),@OrderByCondition nvarchar(max),@StartRowID int output'
			EXEC sp_executesql @StartRowSql, @StartRowParameterDef, @WhereCondition,@OrderByCondition, @StartRowID OUTPUT
			set @ReturnValue = 0
		END
	else
		set @ReturnValue = -1
END

return @ReturnValue