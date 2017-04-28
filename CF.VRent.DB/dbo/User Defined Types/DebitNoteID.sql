CREATE TYPE [dbo].[DebitNoteID] AS TABLE (
    [ID]       INT              NULL,
    [PeriodID] INT              NULL,
    [clientID] UNIQUEIDENTIFIER NULL);

