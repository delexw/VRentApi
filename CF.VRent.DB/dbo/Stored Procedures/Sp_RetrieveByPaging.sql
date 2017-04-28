CREATE procedure [dbo].[Sp_RetrieveByPaging]
	@ColumnsSql nvarchar(max),
	@FromSql nvarchar(max),
	@WhereCondition Nvarchar(max),
	@OrderByCondition Nvarchar(max),
	@itemsPerPage int,
	@PageNumber int,
	@TotalPage int output,
	@return_value int output

as
DECLARE 
	@TotalRows int,
	@Direction nvarchar(10),
	@StartRowID int,

	@FindReadSql nvarchar(1000),
	@FindReadParameterDef nvarchar(1000)
	
	declare @ASCIndex int, @DESCIndex int

		set @ASCIndex = PATINDEX('% ID ASC%',@OrderByCondition);
		set @DESCIndex = PATINDEX('% ID DESC%',@OrderByCondition);

		begin
			if(@ASCIndex > 0)
				set @Direction = 'ASC'
			if(@DESCIndex > 0)
				set @Direction = 'DESC';
		end

BEGIN


EXEC	@return_value = [dbo].[Sp_RetrieveTotalRows]
		@WhereCondition = @FromSql,
		@TotalRows = @TotalRows OUTPUT
	print @TotalRows

	if(@return_value = 0 AND @TotalRows > 0)
		Begin
			EXEC @return_value = [dbo].[Sp_RetrieveStartRow]
			@FromSql = @FromSql,
			@WhereCondition = @WhereCondition,
			@OrderByCondition = @OrderByCondition,
			@TotalRows = @TotalRows,
			@ItemsPerPage = @itemsPerPage,
			@PageNumber = @PageNumber,
			@StartRowID = @StartRowID OUTPUT,
			@TotalPage = @TotalPage OUTPUT
		End
	else
		Begin
			set @TotalPage = 0
		End	

	if(@return_value = 0 AND @StartRowID >= 1)
		BEGIN
			Set RowCount @ItemsPerPage;		
			Set @FindReadParameterDef = '@StartRowID int,@WhereCondition nvarchar(max),@OrderByCondition nvarchar(max)'
			if(@Direction = 'ASC')
				Set @FindReadSql = @ColumnsSql + ' ' + @FromSql + ' ' +@WhereCondition + ' AND notes.ID >= @StartRowID '+@OrderByCondition
			else
				Set @FindReadSql = @ColumnsSql + ' ' + @FromSql + ' ' +@WhereCondition + ' AND notes.ID <= @StartRowID '+@OrderByCondition  
			print '@FindReadSql:'+@FindReadSql

			EXEC sp_executesql @FindReadSql, @FindReadParameterDef, @StartRowID,@WhereCondition,@OrderByCondition
		END


	SET NOCOUNT OFF
End