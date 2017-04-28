CREATE PROCEDURE [dbo].[Sp_GeneralLedgerItemDetail_Add]
@HeaderID numeric(18, 0), @ItemID numeric(18, 0), @PaymentID int, @DebitNoteID int, @DetailType nvarchar(10), @CreatedBy uniqueidentifier
WITH EXEC AS CALLER
AS
INSERT INTO GeneralLedgerItemDetail (HeaderID,
                                     ItemID,
                                     PaymentID,
                                     DebitNoteID,
                                     DetailType,
                                     CreatedOn,
                                     CreatedBy)
VALUES (@HeaderID                                 -- HeaderID - numeric(18, 0)
                 ,
        @ItemID                                     -- ItemID - numeric(18, 0)
               ,
        @PaymentID                                          -- PaymentID - int
                  ,
        @DebitNoteID,
        @DetailType                               -- DetailType - nvarchar(10)
                   ,
        getdate ()                                     -- CreatedOn - datetime
                  ,
        @CreatedBy                            -- CreatedBy - uniqueidentifier
                   )