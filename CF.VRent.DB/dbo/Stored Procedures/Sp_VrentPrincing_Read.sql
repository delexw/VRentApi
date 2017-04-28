CREATE PROCEDURE [dbo].[Sp_VrentPrincing_Read]
@BookingID int
WITH EXEC AS CALLER
AS
BEGIN
   SELECT [ID],
          [BookingID],
          [Total],
          [TimeStamp],
          [TagID],
          [state],
          [CreatedOn],
          [CreatedBy],
          [ModifiedOn],
          [ModifiedBy]
     FROM [dbo].[BookingPrice]
    WHERE [BookingID] = @BookingID AND [state] != 1

   SELECT bpi.[ID],
          [PrincingID],
          [Description],
          [Group],
          [Category],
          [Type],
          [UnitPrice],
          [Quantity],
          bpi.[Total],
          bpi.[State],
          bpi.[CreatedOn],
          bpi.[CreatedBy],
          bpi.[ModifiedOn],
          bpi.[ModifiedBy]
     FROM [dbo].[BookingPriceItem] AS bpi
          INNER JOIN BookingPrice AS bp
             ON bpi.PrincingID = bp.ID AND bp.BookingID = @BookingID
    WHERE bp.BookingID = @BookingID AND bp.state != 1 AND bpi.[State] != 1
END