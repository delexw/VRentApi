CREATE PROCEDURE [dbo].[Sp_UserTermsConditionAgreement_Create]
@UserID uniqueidentifier, @TCID tinyint, @CreatedBy uniqueidentifier
WITH EXEC AS CALLER
AS
INSERT INTO UserTermsConditionAgreement (UserID,
                                         TCID,
                                         CreatedOn,
                                         CreatedBy)
VALUES (@UserID,
        @TCID,
        getdate (),
        @CreatedBy)
        
RETURN @@IDENTITY