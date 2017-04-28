CREATE PROCEDURE [dbo].[Sp_TermsCondition_Create]
@Title nvarchar(20), @DisplayTitle nvarchar(50), @Content ntext, @Key uniqueidentifier, @Type int, @Category int, @Version nvarchar(20), @IsNewVersion tinyint, @IsActive tinyint, @CreatedBy uniqueidentifier
WITH EXEC AS CALLER
AS
DECLARE
   @CurrentVersion   NVARCHAR (20),
   @NewID            INT

SELECT @CurrentVersion = Version
FROM TermsCondition
WHERE [Key] = @Key AND IsNewVersion = 1 AND IsActive = 1;

-- init version or increase version
IF (@CurrentVersion IS NULL)
   SET @CurrentVersion = 1
ELSE
   SET @CurrentVersion = CONVERT (INT, @CurrentVersion) + 1

IF (@IsNewVersion = 1)
   BEGIN
      -- deactive old
      UPDATE TermsCondition
         SET IsNewVersion = 0,
             IsActive = 0,
             ModifiedOn = GETDATE (),
             ModifiedBy = @CreatedBy
       WHERE ID IN (SELECT TOP 1 ID
                      FROM TermsCondition
                     WHERE [Key] = @Key
                    ORDER BY ID DESC)
   END

INSERT INTO TermsCondition (Title,
                            DisplayTitle,
                            [Content],
                            [Key],
                            [Type],
                            Category,
                            Version,
                            IsNewVersion,
                            IsActive,
                            CreatedOn,
                            CreatedBy)
VALUES (@Title,
        @DisplayTitle,
        @Content,
        @Key,
        @Type,
        @Category,
        @CurrentVersion,
        @IsNewVersion,
        @IsActive,
        GETDATE (),
        @CreatedBy)

SELECT @NewID = IDENT_CURRENT ('TermsCondition')

RETURN @NewID