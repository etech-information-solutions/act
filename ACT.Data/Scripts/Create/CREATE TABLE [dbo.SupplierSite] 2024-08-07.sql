USE [ACT]
GO

/****** Object:  Table [dbo].[Site]    Script Date: 2024/08/02 09:50:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SupplierSite](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClientId] [int] NULL,
	[RegionId] [int] NULL,
	[ARPMSalesManagerId] [int] NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
	[ModifiedBy] [varchar](150) NOT NULL,
	[Name] [varchar](150) NOT NULL,
	[Description] [varchar](500) NOT NULL,
	[XCord] [varchar](150) NULL,
	[YCord] [varchar](150) NULL,
	[Address] [varchar](500) NULL,
	[Town] [varchar](100) NULL,
	[PostalCode] [varchar](150) NULL,
	[ContactNo] [varchar](150) NULL,
	[ContactName] [varchar](150) NULL,
	[PlanningPoint] [varchar](150) NULL,
	[SiteType] [int] NULL,
	[AccountCode] [varchar](150) NULL,
	[Depot] [varchar](150) NULL,
	[SiteCodeChep] [varchar](150) NULL,
	[Status] [int] NOT NULL,
	[FinanceContact] [varchar](150) NULL,
	[FinanceContactNo] [varchar](150) NULL,
	[ReceivingContact] [varchar](150) NULL,
	[ReceivingContactNo] [varchar](150) NULL,
	[DepotManager] [varchar](150) NULL,
	[DepotManagerContact] [varchar](150) NULL,
	[FinanceEmail] [varchar](150) NULL,
	[ReceivingEmail] [varchar](150) NULL,
	[DepotManagerEmail] [varchar](150) NULL,
	[Province] [int] NULL,
	[LocationNumber] [varchar](150) NULL,
	[CLCode] [varchar](150) NULL,
 CONSTRAINT [PK_SupplierSite] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SupplierSite]  WITH CHECK ADD  CONSTRAINT [FK_SupplierSite_Region] FOREIGN KEY([RegionId])
REFERENCES [dbo].[Region] ([Id])
GO

ALTER TABLE [dbo].[SupplierSite] CHECK CONSTRAINT [FK_SupplierSite_Region]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Document Type' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SupplierSite', @level2type=N'COLUMN',@level2name=N'PlanningPoint'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1=Transfer Cusotmer(no Pallets to collect), 2=Exchange Customer, 0=Depo (Default)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SupplierSite', @level2type=N'COLUMN',@level2name=N'SiteType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Depo Code' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SupplierSite', @level2type=N'COLUMN',@level2name=N'Depot'
GO