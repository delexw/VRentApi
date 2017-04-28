CREATE PROCEDURE [dbo].[Sp_TermsCondition_Latest_Get]
@Type int, @isIncludeContent tinyint
WITH EXEC AS CALLER
AS
-- show the content
IF (@isIncludeContent = '1')
   SELECT *
     FROM TermsCondition
    WHERE ([Type] & @Type) = @Type AND IsNewVersion = 1 AND IsActive = 1
   ORDER BY CreatedOn DESC
ELSE
   SELECT ID,
          Title,
          DisplayTitle,
          [Key],
          [Type],
          Category,
          Version,
          IsNewVersion,
          IsActive,
          CreatedOn,
          CreatedBy,
          ModifiedOn,
          ModifiedBy
     FROM TermsCondition
    WHERE ([Type] & @Type) = @Type AND IsNewVersion = 1 AND IsActive = 1
   ORDER BY CreatedOn DESC