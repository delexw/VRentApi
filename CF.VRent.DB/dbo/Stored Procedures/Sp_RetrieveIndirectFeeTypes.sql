-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [dbo].[Sp_RetrieveIndirectFeeTypes]
WITH EXEC AS CALLER
AS			   
		 select 
		 ift.ID,
		 ift.[Type],
		 ift.[Group],
 		 ift.[SourceType],
		 ift.[Note],
         ift.[State],
	     ift.[CreatedOn],
	     ift.[CreatedBy],
		 ift.[ModifiedOn],
		 ift.[ModifiedBy]
		 from IndirectFeeTypes as ift
		 where ift.[SourceType] = 0 and [State] = 0
		  --0:builtIn, 1: WriteIn