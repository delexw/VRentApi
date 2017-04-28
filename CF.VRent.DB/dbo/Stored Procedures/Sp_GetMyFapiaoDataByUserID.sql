-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Sp_GetMyFapiaoDataByUserID] 
	@UserID uniqueidentifier
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT

	  vbs.[ID]
      ,[ItemID]
      ,[UniqueID]
      ,[DealNumber]
      ,[ContractNumber]
      ,[CustomerID]
      ,[CustomerName]
      ,[TaxID]
      ,[CustomerAddress]
      ,[CustomerPhone]
      ,[BankName]
      ,[BankAccount]
      ,[FPCustomerName]
      ,[FPMailType]
      ,[FPMailAddress]
      ,[FPMailPhone]
      ,[FPAddresseeName]
      ,[ProductCode]
      ,[Spec_Mode]
      ,[UnitMeasure]
      ,[SalesQuantity]
      ,[UnitPrice]
      ,[NetAmount]
      ,[TaxRate]
      ,[Tax]
      ,[TotalAmount]
      ,[FapiaoType]
      ,[Remark]
      ,[GeneratedFapiaoNumber]
      ,[GeneratedFapiaoCode]
      ,[FapiaoIssueDate]
      ,[DeliverID]
      ,vbs.[State]
      ,vbs.[CreatedOn]
      ,vbs.[CreatedBy]
      ,vbs.[ModifiedOn]
      ,vbs.[ModifiedBy]
  FROM [dbo].[FapiaoData] as vbs 
  --inner join VrentOrders as vo on vbs.CustomerID = vo.BookingUserID
  --Where  vo.BookingUserID = @UserID

  --FROM [dbo].[FapiaoData] as vbs inner join VrentOrders as vo on vbs.CustomerID = vo.BookingUserID
  --Where  vo.BookingUserID = @UserID
END
