CREATE TABLE [dbo].[GT_FapiaoPreferences] (
    [Unique_ID]      NVARCHAR (200)   NOT NULL,
    [User_ID]        NVARCHAR (160)   NULL,
    [Customer_Name]  NVARCHAR (80)    NULL,
    [Mail_Type]      NVARCHAR (80)    NULL,
    [Mail_Address]   NVARCHAR (80)    NULL,
    [Mail_Phone]     NVARCHAR (20)    NULL,
    [Addressee_Name] NVARCHAR (50)    NULL,
    [Fapiao_Type]    NUMERIC (3)      NULL,
    [State]          TINYINT          NOT NULL,
    [CreatedOn]      DATETIME         NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NULL,
    [ModifiedOn]     DATETIME         NULL,
    [ModifiedBy]     UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_GT_FapiaoPreferences] PRIMARY KEY CLUSTERED ([Unique_ID] ASC)
);

