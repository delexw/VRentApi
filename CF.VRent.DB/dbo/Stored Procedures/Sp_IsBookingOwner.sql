Create PROCEDURE [dbo].[Sp_IsBookingOwner]
	@ProxyBookingID int, 
	@OperatorID uniqueidentifier
--Ret
-- 1: valid
-- 0: invalid	
WITH EXEC AS CALLER AS
declare    @return_value int
BEGIN
    if(@OperatorID = 'f5b3da9d-8996-45af-81fc-405927ac238e' OR @OperatorID = '1c9d9c82-d074-45a4-863e-e7eeb2384c64')
		begin
			set @return_value = 1
		end
    else
    begin
		set @return_value = 
		(
			Select COUNT(*) from VrentBookings as vb
			where 
				vb.UserID = @OperatorID 
				AND vb.ID = @ProxyBookingID
				AND vb.[State] != 'swBookingModel/deleted'
		)
	end
END
return @return_value