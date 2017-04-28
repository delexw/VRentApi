-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Sp_RetrieveFapiaoRequestsBySource]
	@ProxyBookingID int,
	@OperatorID uniqueidentifier,
	@FapiaoSourceInput dbo.FapiaoSource readonly,
	@return_value int output

--    public enum RetrieveFapiaoRequestBySourceResult { Success = 0, InvalidOperator = 1  }; 
--Ret
-- 1: invalid operator
-- 0: success

WITH EXEC AS CALLER AS
	Declare	@IsBookingOnwer int
BEGIN
	--check
	EXEC @IsBookingOnwer = [dbo].[Sp_IsBookingOwner]
		@ProxyBookingID = @ProxyBookingID,
		@OperatorID = @OperatorID
	
	if(@IsBookingOnwer = 1)
		Begin
			SELECT 
					[ID]
				  ,[ProxyBookingID]
				  ,[FapiaoPreferenceID]
				  ,[FapiaoSource]
				  ,fr.[State]
				  ,fr.[CreatedOn]
				  ,fr.[CreatedBy]
				  ,fr.[ModifiedOn]
				  ,fr.[ModifiedBy]

				  ,fp.[Unique_ID]
				  ,fp.[User_ID]
				  ,fp.[Customer_Name]
				  ,fp.[Mail_Type]
				  ,fp.[Mail_Address]
				  ,fp.[Mail_Phone]
				  ,fp.[Addressee_Name]
				  ,fp.[Fapiao_Type]
				  ,fp.[State]
				  ,fp.[CreatedOn]
				  ,fp.[CreatedBy]
				  ,fp.[ModifiedOn]
				  ,fp.[ModifiedBy]

			  FROM [dbo].[FapiaoRequests] as fr left join dbo.GT_FapiaoPreferences as fp
	  on fr.FapiaoPreferenceID = fp.Unique_ID
				WHERE fr.ProxyBookingID = @ProxyBookingID 
				and fr.FapiaoSource in (select FapiaoSource from @FapiaoSourceInput as fsi) 
				and fr.State != 1
			set @return_value = 0
		End
	else
		begin
			set @return_value = 1
		end

--Valid Fapiao State
-- Active = 0, Deleted = 1,Generated, Exported, Imported, Delivered,Unknown

END