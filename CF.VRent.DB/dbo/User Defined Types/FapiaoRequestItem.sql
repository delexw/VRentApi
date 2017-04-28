CREATE TYPE [dbo].[FapiaoRequestItem] AS TABLE (
    [ID]                 INT              NULL,
    [ProxyBookingID]     INT              NOT NULL,
    [FapiaoPreferenceID] UNIQUEIDENTIFIER NULL,
    [FapiaoSource]       TINYINT          NOT NULL,
    [State]              TINYINT          NOT NULL,
    [CreatedOn]          DATETIME         NOT NULL,
    [CreatedBy]          UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn]         DATETIME         NULL,
    [ModifiedBy]         UNIQUEIDENTIFIER NULL);

