CREATE PROCEDURE [dbo].[Sp_GeneralLedgerItem_Add]
@HeaderID numeric(18, 0), @ItemType int, @PostingBody nvarchar(500), @CreatedBy uniqueidentifier, @ClientID uniqueidentifier, @CompanyCode nvarchar(50), @BusinessArea nvarchar(10), @NewID numeric(18, 0) OUTPUT
WITH EXEC AS CALLER
AS
INSERT INTO GeneralLedgerItem (HeaderID,
                               ItemType,
                               PostingBody,
                               ClientID,
                               CompanyCode,
                               BusinessArea,
                               CreatedOn,
                               CreatedBy)
VALUES (@HeaderID                                 -- HeaderID - numeric(18, 0)
                 ,
        @ItemType                                            -- ItemType - int
                 ,
        @PostingBody                            -- PostingBody - nvarchar(500)
                    ,
        @ClientID,
        @CompanyCode,
        @BusinessArea,
        getdate ()                                     -- CreatedOn - datetime
                  ,
        @CreatedBy                             -- CreatedBy - uniqueidentifier
                  )

SELECT @NewID = IDENT_CURRENT ('GeneralLedgerItem')