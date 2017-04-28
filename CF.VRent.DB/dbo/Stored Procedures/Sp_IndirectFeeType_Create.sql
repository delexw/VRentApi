-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [dbo].[Sp_IndirectFeeType_Create]
	(@IndirectFeeTypesInput dbo.IndirectFeeType readonly)
WITH EXEC AS CALLER
AS
BEGIN
	INSERT INTO [IndirectFeeTypes]
			   ([Type]
			   ,[Group]
			   ,[SourceType]
			   ,[Note]
			   ,[State]
			   ,[CreatedOn]
			   ,[CreatedBy]
			   ,[ModifiedOn]
			   ,[ModifiedBy])
			   
		 select 
		 ift.[Type],
		 ift.[Group],
		 ift.[SourceType],
		 ift.[Note],
         ift.[State],
	     ift.[CreatedOn],
	     ift.[CreatedBy],
		 ift.[ModifiedOn],
		 ift.[ModifiedBy]
		 from @IndirectFeeTypesInput as ift

END