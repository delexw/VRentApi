-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Sp_EnableDisableFaPiaoRequests]
	@ProxyBookingID int,	
	@State tinyint,
	@ModifiedOn datetime,
    @ModifiedBy uniqueidentifier


WITH EXEC AS CALLER AS
declare  
@FapiaoRequestsCnt int,
@return_value int
BEGIN
	SET NOCOUNT ON
	
	select @FapiaoRequestsCnt = COUNT(*) from FapiaoRequests as fr
	WHERE
		ProxyBookingID = @ProxyBookingID

	if(@FapiaoRequestsCnt = 0)
		begin
			 EXEC	@return_value = [dbo].[Sp_CreateFaPiaoRequestItems]
					@ProxyBookingID = @ProxyBookingID,
					@CreatedOn = @ModifiedOn,
					@CreatedBy = @ModifiedBy 
		end

	begin
		UPDATE [dbo].[FapiaoRequests]
			SET
				[State] = @State
				,[FapiaoPreferenceID] = null
				,[ModifiedOn] = @ModifiedOn
				,[ModifiedBy] = @ModifiedBy
			WHERE
				ProxyBookingID = @ProxyBookingID
				AND [State] in (0,1)
	end
--Valid Fapiao State
-- Active = 0, Deleted = 1,Generated = 2, Exported = 3, Imported = 4, Delivered = 5
END