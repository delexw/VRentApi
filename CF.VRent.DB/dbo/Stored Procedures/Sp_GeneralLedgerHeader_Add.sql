CREATE PROCEDURE [dbo].[Sp_GeneralLedgerHeader_Add]
@PostingFrom datetime, @PostingEnd datetime, @HeaderType int, @CreatedBy uniqueidentifier, @NewID numeric(18, 0) OUTPUT
WITH EXEC AS CALLER
AS
INSERT INTO GeneralLedgerHeader (PostingFrom,
                                 PostingEnd,
                                 HeaderType,
                                 CreatedOn,
                                 CreatedBy)
VALUES (@PostingFrom                                 -- PostingFrom - datetime
                    ,
        @PostingEnd                                   -- PostingEnd - datetime
                   ,
        @HeaderType,
        getdate ()                                     -- CreatedOn - datetime
                  ,
        @CreatedBy                             -- CreatedBy - uniqueidentifier
                  )



SELECT @NewID = IDENT_CURRENT ('GeneralLedgerHeader')