CREATE TABLE [dbo].[FapiaoRequests] (
    [ID]                 INT              IDENTITY (1, 1) NOT NULL,
    [ProxyBookingID]     INT              NOT NULL,
    [FapiaoPreferenceID] UNIQUEIDENTIFIER NULL,
    [FapiaoSource]       TINYINT          NOT NULL,
    [State]              TINYINT          NOT NULL,
    [CreatedOn]          DATETIME         NOT NULL,
    [CreatedBy]          UNIQUEIDENTIFIER NOT NULL,
    [ModifiedOn]         DATETIME         NULL,
    [ModifiedBy]         UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_FapiaoRequests] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_FapiaoRequests_VrentBookings] FOREIGN KEY ([ProxyBookingID]) REFERENCES [dbo].[VrentBookings] ([ID])
);





