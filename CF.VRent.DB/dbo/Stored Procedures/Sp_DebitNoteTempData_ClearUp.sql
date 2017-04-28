-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE Sp_DebitNoteTempData_ClearUp
	@periodID int
AS
BEGIN
declare @periodStart nvarchar(8)
declare @periodEnd nvarchar(8)
	SET NOCOUNT ON;

	select 
		@periodStart = CONVERT(nvarchar(8), dnh.PeriodBegin,112),
		@periodEnd = CONVERT(nvarchar(8), dnh.PeriodEnd,112)
	from DebitNoteHistory as dnh where dnh.ID = @periodID

	if(@periodStart is not null AND @periodEnd is not null)
	begin
		delete 
		from CompletedBookings
		where 
			( KeyOut is not null and KeyIn is not null and KeyOut >= @periodStart and KeyIn < @periodEnd) 
			OR ((KeyOut is null or KeyIn is null) and DateBegin >= @periodStart and DateEnd < @periodEnd)
	end

END