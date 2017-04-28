CREATE TABLE [dbo].[DebitNoteDetail](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DebitNoteID] [int] NOT NULL,
	[ClientID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[BookingID] [int] NOT NULL,
	[KemasBookingID] [uniqueidentifier] NOT NULL,
	[KemasBookingNumber] [nvarchar](20) NOT NULL,
	[OrderID] [int] NULL,
	[OrderItemID] [int] NULL,
	[Category] [nvarchar](20) NULL,
	[OrderDate] [datetime] NOT NULL,
	[TotalAmount] [decimal](18, 3) NULL,
	[State] [tinyint] NOT NULL,
 CONSTRAINT [PK_DebitNoteDetail] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)) ON [PRIMARY]