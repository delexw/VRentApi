CREATE procedure [dbo].[Sp_RetrieveTotalRows]
	@WhereCondition Nvarchar(max),
	@TotalRows int output
as
DECLARE 
	@TotalCountSql nvarchar(max),
	@TotalCountParamerterDef nvarchar(max)
BEGIN
    Set @TotalCountParamerterDef = '@WhereCondition nvarchar(max), @TotalRows int output'
    Set @TotalCountSql = N'Select @TotalRows=Count(*) '+ @WhereCondition
	print '@TotalCountSql:'+@TotalCountSql

	EXEC sp_executesql 
	@TotalCountSql, 
	@TotalCountParamerterDef,
	@WhereCondition,@TotalRows OUTPUT
END