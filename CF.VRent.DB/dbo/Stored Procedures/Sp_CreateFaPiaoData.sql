CREATE PROCEDURE [dbo].[Sp_CreateFaPiaoData]

	@ItemID int,
	@UniqueID nvarchar(20),
	@DealNumber nvarchar(20),
	@ContractNumber nvarchar(20),
	@CustomerID nvarchar(20),
	@CustomerName nvarchar(20),
	@TaxID nvarchar(20),
	@CustomerAddress nvarchar(120),
	@CustomerPhone nvarchar(20),
	@BankName nvarchar(80),
	@BankAccount nvarchar(80),
	@FPCustomerName nvarchar(20),
    @FPMailType nvarchar(80),
	@FPMailAddress nvarchar(80),
	@FPMailPhone nvarchar(20),
    @FPAddresseeName nvarchar(50),
	@ProductCode nvarchar(20),
	@Spec_Mode nvarchar(20),
	@UnitMeasure nvarchar(10),
    @SalesQuantity numeric(3,0),
    @UnitPrice numeric(14,2),
    @NetAmount numeric(14,2),
	@TaxRate numeric(5,4),
	@Tax numeric(14,2),
	@TotalAmount numeric(14,2),
	@FapiaoType numeric(3,0),
	@Remark nvarchar(230),
	@GeneratedFapiaoNumber nvarchar(20),
	@GeneratedFapiaoCode nvarchar(20),
	@FapiaoIssueDate datetime,
	@DeliverID nvarchar(100),
	@State tinyint,
	@CreatedOn datetime,
	@CreatedBy uniqueidentifier

AS
BEGIN
   DECLARE @lastID      TABLE (InsertedID   INT)

INSERT INTO [dbo].[FapiaoData]
           (

           [ItemID]
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
           ,[State]
           ,[CreatedOn]
           ,[CreatedBy]
		   )
	output inserted.ID into @lastID
     VALUES
           (
			@ItemID,
			@UniqueID,
			@DealNumber,
			@ContractNumber,
			@CustomerID,
			@CustomerName,
			@TaxID,
			@CustomerAddress,
			@CustomerPhone,
			@BankName,
			@BankAccount,
			@FPCustomerName,
			@FPMailType,
			@FPMailAddress,
			@FPMailPhone,
			@FPAddresseeName,
			@ProductCode,
			@Spec_Mode,
			@UnitMeasure,
			@SalesQuantity,
			@UnitPrice,
			@NetAmount,
			@TaxRate,
			@Tax,
			@TotalAmount,
			@FapiaoType,
			@Remark,
			@GeneratedFapiaoNumber,
			@GeneratedFapiaoCode,
			@FapiaoIssueDate,
			@DeliverID,
			@State,
			@CreatedOn,
			@CreatedBy		  
	 )
end

   SELECT 
		   [ID]
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
           
           ,[State]
           ,[CreatedOn]
           ,[CreatedBy]
			,[ModifiedOn]
			,[ModifiedBy]
     FROM FapiaoData AS vb
    WHERE [ID] = (SELECT TOP 1 InsertedID FROM @lastID)
          AND [State] != 1 -- 0:active 1:deleted

