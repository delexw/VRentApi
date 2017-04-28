
CREATE procedure [dbo].[Sp_RetrieveBookingsWithPaging]
    
	--Role
	@Role nvarchar(20), --Service Center, --VRent Manager
	@CorporateID nvarchar(50), --operator's company
    
	@WhereCondition Nvarchar(max),
	@OrderByCondition Nvarchar(max),
	@itemsPerPage int,
	@PageNumber int,
	@TotalPage int output

as
DECLARE 
	@TotalRows int,
	@TotalCountSql nvarchar(1000),
	@TotalCountParamerterDef nvarchar(1000),

	@Direction nvarchar(10),

	@StartRowSql nvarchar(1000),
	@StartRowParameterDef nvarchar(1000),
	@StartRow int,
	@StartRowID int,


	@FindReadSql nvarchar(1000),
	@FindReadParameterDef nvarchar(1000),
	
	@Reminder int
	

		declare @ASCIndex int, @DESCIndex int

		--bug
		set @ASCIndex = PATINDEX('% ID ASC%',@OrderByCondition);
		set @DESCIndex = PATINDEX('% ID DESC%',@OrderByCondition);

		--print @ASCIndex
		--print @DEscIndex

		begin
			if(@ASCIndex > 0)
				set @Direction = 'ASC'
			if(@DESCIndex > 0)
				set @Direction = 'DESC';
		end
		print 'PK Direction:'+@Direction

BEGIN
    --filter by role
	if(@Role = 'VM')
	begin
	    set @WhereCondition = @WhereCondition + ' AND CorporateID = ''' + @CorporateID + ''''
	end
    --filter by role
    
    Set @TotalCountParamerterDef = '@TotalRows int output'
    Set @TotalCountSql = N'Select @TotalRows = Count(ID) from VrentBookings ' + @WhereCondition  
	print '@TotalCountSql:'+@TotalCountSql

	EXEC sp_executesql @TotalCountSql, @TotalCountParamerterDef,@TotalRows OUTPUT
	print @TotalRows

	if(@TotalRows > 0)
		Begin
			set @Reminder = @TotalRows % @itemsPerPage
			if(@Reminder = 0)
				set @TotalPage = @TotalRows / @itemsPerPage
			else		    
				set @TotalPage = @TotalRows / @itemsPerPage + 1
		End
	else
		set @TotalPage = 0
	   
	Set NOCount ON
	Set @StartRow = (@PageNumber - 1) * @itemsPerPage + 1

			Set @StartRowSql = N'Select @StartRowID = ID from VrentBookings ' + @WhereCondition +' '+ @OrderByCondition
			print N'@StartRowSql:'+@StartRowSql

    if(@StartRow <= @TotalRows)
		Begin

			Set RowCount @StartRow
			Set @StartRowParameterDef = '@StartRowID int output'
			EXEC sp_executesql @StartRowSql, @StartRowParameterDef, @StartRowID OUTPUT
		END
			print @StartRowID

		BEGIN
			Set RowCount @ItemsPerPage;		
			Set @FindReadParameterDef = '@StartRowID int'
			if(@Direction = 'ASC')
				Set @FindReadSql = N'Select * from VrentBookings as vb ' 
					+ @WhereCondition + ' AND vb.ID >= @StartRowID '+@OrderByCondition
			else
				Set @FindReadSql = N'Select * from VrentBookings as vb ' 
				+ @WhereCondition + ' AND vb.ID <= @StartRowID '+ @OrderByCondition  
			print '@FindReadSql:'+@FindReadSql

			EXEC sp_executesql @FindReadSql, @FindReadParameterDef, @StartRowID
		END
	
		SET NOCOUNT OFF
End