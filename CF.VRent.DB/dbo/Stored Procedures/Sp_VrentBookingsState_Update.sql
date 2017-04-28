
CREATE PROCEDURE [dbo].[Sp_VrentBookingsState_Update]
@KBID uniqueidentifier, @State nvarchar(50), @UserId uniqueidentifier
WITH EXEC AS CALLER
AS
UPDATE VrentBookings
   SET State = @State, ModifiedOn = getdate (), ModifiedBy = @UserId
 WHERE KemasBookingID = @KBID
