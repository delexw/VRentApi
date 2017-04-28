CREATE TYPE [dbo].[BookingPrice] AS TABLE (
    [BookingID]  INT              NOT NULL,
    [Total]      DECIMAL (10, 3)  NOT NULL,
    [TimeStamp]  NVARCHAR (20)    NULL,
    [TagID]      NVARCHAR (50)    NULL,
    [state]      TINYINT          NOT NULL,
    [CreatedOn]  DATETIME         NOT NULL,
    [CreatedBy]  UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn] DATETIME         NULL,
    [ModifiedBy] UNIQUEIDENTIFIER NULL);



