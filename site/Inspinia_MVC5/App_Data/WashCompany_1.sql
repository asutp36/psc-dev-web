USE [master]
GO
/****** Object:  Database [WashCompany]    Script Date: 15.09.2019 17:07:26 ******/
CREATE DATABASE [WashCompany]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'WashCompany', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\WashCompany.mdf' , SIZE = 21504KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'WashCompany_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\WashCompany_log.ldf' , SIZE = 2048KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [WashCompany] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [WashCompany].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [WashCompany] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [WashCompany] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [WashCompany] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [WashCompany] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [WashCompany] SET ARITHABORT OFF 
GO
ALTER DATABASE [WashCompany] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [WashCompany] SET AUTO_SHRINK ON 
GO
ALTER DATABASE [WashCompany] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [WashCompany] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [WashCompany] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [WashCompany] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [WashCompany] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [WashCompany] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [WashCompany] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [WashCompany] SET  DISABLE_BROKER 
GO
ALTER DATABASE [WashCompany] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [WashCompany] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [WashCompany] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [WashCompany] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [WashCompany] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [WashCompany] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [WashCompany] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [WashCompany] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [WashCompany] SET  MULTI_USER 
GO
ALTER DATABASE [WashCompany] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [WashCompany] SET DB_CHAINING OFF 
GO
ALTER DATABASE [WashCompany] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [WashCompany] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [WashCompany] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [WashCompany] SET QUERY_STORE = OFF
GO
USE [WashCompany]
GO
/****** Object:  Table [dbo].[CountersTotal]    Script Date: 15.09.2019 17:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CountersTotal](
	[IDCountersTotal] [int] IDENTITY(1,1) NOT NULL,
	[IDPost] [int] NOT NULL,
	[DTime] [datetime2](7) NOT NULL,
	[b10] [int] NOT NULL,
	[b50] [int] NOT NULL,
	[b100] [int] NOT NULL,
	[b500] [int] NOT NULL,
	[b1k] [int] NOT NULL,
	[m10] [int] NOT NULL,
	[amount] [int] NULL,
 CONSTRAINT [PK_CountersTotal] PRIMARY KEY CLUSTERED 
(
	[IDCountersTotal] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vPostsAmount]    Script Date: 15.09.2019 17:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*and 
and co2.DTime >= CAST('2019-06-01 00:29:00' AS datetime2(7)) AND co2.DTime < CAST('2019-06-01 00:40:00' AS datetime2(7))*/
CREATE VIEW [dbo].[vPostsAmount]
AS
SELECT        IDPost, DTime, amount
FROM            dbo.CountersTotal
WHERE        (1 = 1)
GO
/****** Object:  Table [dbo].[Cards]    Script Date: 15.09.2019 17:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cards](
	[IDCard] [int] IDENTITY(1,1) NOT NULL,
	[IDOwner] [int] NOT NULL,
	[CardNum] [nvarchar](20) NOT NULL,
	[IDCardStatus] [int] NOT NULL,
	[IDCardType] [int] NOT NULL,
	[LocalizedBy] [int] NOT NULL,
	[LocalizedID] [int] NOT NULL,
 CONSTRAINT [PK_Cards] PRIMARY KEY CLUSTERED 
(
	[IDCard] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CardStatuses]    Script Date: 15.09.2019 17:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CardStatuses](
	[IDCardStatus] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](10) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_CardStatuses] PRIMARY KEY CLUSTERED 
(
	[IDCardStatus] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CardTypes]    Script Date: 15.09.2019 17:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CardTypes](
	[IDCardType] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](10) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_CardTypes] PRIMARY KEY CLUSTERED 
(
	[IDCardType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Companies]    Script Date: 15.09.2019 17:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Companies](
	[IDCompany] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Companies] PRIMARY KEY CLUSTERED 
(
	[IDCompany] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CountersOperating]    Script Date: 15.09.2019 17:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CountersOperating](
	[IDCountersOperating] [int] IDENTITY(1,1) NOT NULL,
	[IDPost] [int] NOT NULL,
	[DTime] [datetime2](7) NOT NULL,
	[b10] [int] NOT NULL,
	[b50] [int] NOT NULL,
	[b100] [int] NOT NULL,
	[b500] [int] NOT NULL,
	[b1k] [int] NOT NULL,
	[m10] [int] NOT NULL,
 CONSTRAINT [PK_CountersOperating] PRIMARY KEY CLUSTERED 
(
	[IDCountersOperating] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Event]    Script Date: 15.09.2019 17:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Event](
	[IDEvent] [int] IDENTITY(1,1) NOT NULL,
	[IDPost] [int] NOT NULL,
	[IDPostEvent] [int] NOT NULL,
	[IDEventKind] [int] NOT NULL,
	[DTime] [datetime] NULL,
 CONSTRAINT [PK_Event] PRIMARY KEY CLUSTERED 
(
	[IDEvent] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EventCash]    Script Date: 15.09.2019 17:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EventCash](
	[IDEvent] [int] NOT NULL,
	[tb10] [int] NULL,
	[tb50] [int] NULL,
	[tb100] [int] NULL,
	[tb200] [int] NULL,
	[tb500] [int] NULL,
	[tb1k] [int] NULL,
	[tm10] [int] NULL,
	[cb10] [int] NULL,
	[cb50] [int] NULL,
	[cb100] [int] NULL,
	[cb200] [int] NULL,
	[cb500] [int] NULL,
	[cb1k] [int] NULL,
	[cm10] [int] NULL,
 CONSTRAINT [PK_EventCash] PRIMARY KEY CLUSTERED 
(
	[IDEvent] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EventKind]    Script Date: 15.09.2019 17:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EventKind](
	[IDEventKind] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_EventKind] PRIMARY KEY CLUSTERED 
(
	[IDEventKind] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EventMode]    Script Date: 15.09.2019 17:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EventMode](
	[IDEvent] [int] NOT NULL,
	[IDMode] [int] NULL,
	[RemainLimit] [int] NULL,
	[Paid] [int] NULL,
	[DTimeFinish] [datetime] NULL,
 CONSTRAINT [PK_EventMode] PRIMARY KEY CLUSTERED 
(
	[IDEvent] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EventSimple]    Script Date: 15.09.2019 17:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EventSimple](
	[IDEvent] [int] NOT NULL,
	[Counter] [int] NULL,
 CONSTRAINT [PK_EventSimple] PRIMARY KEY CLUSTERED 
(
	[IDEvent] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Mode]    Script Date: 15.09.2019 17:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Mode](
	[IDMode] [int] IDENTITY(1,1) NOT NULL,
	[Code] [smallint] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Mode] PRIMARY KEY CLUSTERED 
(
	[IDMode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Operations]    Script Date: 15.09.2019 17:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Operations](
	[IDOperation] [int] IDENTITY(1,1) NOT NULL,
	[IDPsc] [int] NOT NULL,
	[IDOperationType] [int] NOT NULL,
	[IDCard] [int] NOT NULL,
	[DTime] [datetime] NOT NULL,
	[Amount] [int] NOT NULL,
	[Balance] [int] NOT NULL,
	[LocalizedBy] [int] NOT NULL,
	[LocalizedID] [int] NOT NULL,
 CONSTRAINT [PK_Operations] PRIMARY KEY CLUSTERED 
(
	[IDOperation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OperationTypes]    Script Date: 15.09.2019 17:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OperationTypes](
	[IDOperationType] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](10) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_OperationTypes] PRIMARY KEY CLUSTERED 
(
	[IDOperationType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Owners]    Script Date: 15.09.2019 17:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Owners](
	[IDOwner] [int] IDENTITY(1,1) NOT NULL,
	[Phone] [nvarchar](20) NOT NULL,
	[LocalizedBy] [int] NOT NULL,
	[LocalizedID] [int] NOT NULL,
 CONSTRAINT [PK_Clients] PRIMARY KEY CLUSTERED 
(
	[IDOwner] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Posts]    Script Date: 15.09.2019 17:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Posts](
	[IDPost] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](10) NULL,
	[Name] [nvarchar](100) NULL,
	[IDWash] [int] NOT NULL,
 CONSTRAINT [PK_Posts] PRIMARY KEY CLUSTERED 
(
	[IDPost] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Psces]    Script Date: 15.09.2019 17:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Psces](
	[IDPsc] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[IDWash] [int] NULL,
 CONSTRAINT [PK_Pcses] PRIMARY KEY CLUSTERED 
(
	[IDPsc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Regions]    Script Date: 15.09.2019 17:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Regions](
	[IDRegion] [int] IDENTITY(1,1) NOT NULL,
	[Code] [smallint] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[IDCompany] [int] NOT NULL,
 CONSTRAINT [PK_Regions] PRIMARY KEY CLUSTERED 
(
	[IDRegion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 15.09.2019 17:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[IDUser] [int] IDENTITY(1,1) NOT NULL,
	[Login] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](200) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[IDUser] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Wash]    Script Date: 15.09.2019 17:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Wash](
	[IDWash] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](5) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[Address] [nvarchar](100) NULL,
	[IDRegion] [int] NOT NULL,
 CONSTRAINT [PK_Wash] PRIMARY KEY CLUSTERED 
(
	[IDWash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Cards]  WITH CHECK ADD  CONSTRAINT [FK_Cards_CardStatuses] FOREIGN KEY([IDCardStatus])
REFERENCES [dbo].[CardStatuses] ([IDCardStatus])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Cards] CHECK CONSTRAINT [FK_Cards_CardStatuses]
GO
ALTER TABLE [dbo].[Cards]  WITH CHECK ADD  CONSTRAINT [FK_Cards_CardTypes] FOREIGN KEY([IDCardType])
REFERENCES [dbo].[CardTypes] ([IDCardType])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Cards] CHECK CONSTRAINT [FK_Cards_CardTypes]
GO
ALTER TABLE [dbo].[Cards]  WITH CHECK ADD  CONSTRAINT [FK_Cards_Owners] FOREIGN KEY([IDOwner])
REFERENCES [dbo].[Owners] ([IDOwner])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Cards] CHECK CONSTRAINT [FK_Cards_Owners]
GO
ALTER TABLE [dbo].[CountersOperating]  WITH CHECK ADD  CONSTRAINT [FK_CountersOperating_Posts] FOREIGN KEY([IDPost])
REFERENCES [dbo].[Posts] ([IDPost])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CountersOperating] CHECK CONSTRAINT [FK_CountersOperating_Posts]
GO
ALTER TABLE [dbo].[CountersTotal]  WITH CHECK ADD  CONSTRAINT [FK_CountersTotal_Posts] FOREIGN KEY([IDPost])
REFERENCES [dbo].[Posts] ([IDPost])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CountersTotal] CHECK CONSTRAINT [FK_CountersTotal_Posts]
GO
ALTER TABLE [dbo].[Event]  WITH CHECK ADD  CONSTRAINT [FK_Event_EventKind] FOREIGN KEY([IDEventKind])
REFERENCES [dbo].[EventKind] ([IDEventKind])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Event] CHECK CONSTRAINT [FK_Event_EventKind]
GO
ALTER TABLE [dbo].[Event]  WITH CHECK ADD  CONSTRAINT [FK_Event_Point] FOREIGN KEY([IDPost])
REFERENCES [dbo].[Posts] ([IDPost])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Event] CHECK CONSTRAINT [FK_Event_Point]
GO
ALTER TABLE [dbo].[EventCash]  WITH CHECK ADD  CONSTRAINT [FK_EventCash_Event] FOREIGN KEY([IDEvent])
REFERENCES [dbo].[Event] ([IDEvent])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EventCash] CHECK CONSTRAINT [FK_EventCash_Event]
GO
ALTER TABLE [dbo].[EventMode]  WITH CHECK ADD  CONSTRAINT [FK_EventMode_Event] FOREIGN KEY([IDEvent])
REFERENCES [dbo].[Event] ([IDEvent])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EventMode] CHECK CONSTRAINT [FK_EventMode_Event]
GO
ALTER TABLE [dbo].[EventMode]  WITH CHECK ADD  CONSTRAINT [FK_EventMode_Mode] FOREIGN KEY([IDMode])
REFERENCES [dbo].[Mode] ([IDMode])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EventMode] CHECK CONSTRAINT [FK_EventMode_Mode]
GO
ALTER TABLE [dbo].[EventSimple]  WITH CHECK ADD  CONSTRAINT [FK_EventSimple_Event] FOREIGN KEY([IDEvent])
REFERENCES [dbo].[Event] ([IDEvent])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EventSimple] CHECK CONSTRAINT [FK_EventSimple_Event]
GO
ALTER TABLE [dbo].[Operations]  WITH CHECK ADD  CONSTRAINT [FK_Operations_Cards] FOREIGN KEY([IDCard])
REFERENCES [dbo].[Cards] ([IDCard])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Operations] CHECK CONSTRAINT [FK_Operations_Cards]
GO
ALTER TABLE [dbo].[Operations]  WITH CHECK ADD  CONSTRAINT [FK_Operations_OperationTypes] FOREIGN KEY([IDOperationType])
REFERENCES [dbo].[OperationTypes] ([IDOperationType])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Operations] CHECK CONSTRAINT [FK_Operations_OperationTypes]
GO
ALTER TABLE [dbo].[Operations]  WITH CHECK ADD  CONSTRAINT [FK_Operations_Pcses] FOREIGN KEY([IDPsc])
REFERENCES [dbo].[Psces] ([IDPsc])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Operations] CHECK CONSTRAINT [FK_Operations_Pcses]
GO
ALTER TABLE [dbo].[Posts]  WITH CHECK ADD  CONSTRAINT [FK_Posts_Wash] FOREIGN KEY([IDWash])
REFERENCES [dbo].[Wash] ([IDWash])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Posts] CHECK CONSTRAINT [FK_Posts_Wash]
GO
ALTER TABLE [dbo].[Psces]  WITH CHECK ADD  CONSTRAINT [FK_Pcses_Wash] FOREIGN KEY([IDWash])
REFERENCES [dbo].[Wash] ([IDWash])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Psces] CHECK CONSTRAINT [FK_Pcses_Wash]
GO
ALTER TABLE [dbo].[Regions]  WITH CHECK ADD  CONSTRAINT [FK_Regions_Companies] FOREIGN KEY([IDCompany])
REFERENCES [dbo].[Companies] ([IDCompany])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Regions] CHECK CONSTRAINT [FK_Regions_Companies]
GO
ALTER TABLE [dbo].[Wash]  WITH CHECK ADD  CONSTRAINT [FK_Wash_Regions] FOREIGN KEY([IDRegion])
REFERENCES [dbo].[Regions] ([IDRegion])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Wash] CHECK CONSTRAINT [FK_Wash_Regions]
GO
/****** Object:  StoredProcedure [dbo].[GetCardsOperations]    Script Date: 15.09.2019 17:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetCardsOperations] (      
         @p_Phone nvarchar(20)
       , @p_Cardnum nvarchar(20)
       , @p_CardTypeCode nvarchar(10)
       , @p_CardStatusName nvarchar(10)
       , @p_OperationTypeName nvarchar(10)
	   , @p_OperationDateBeg DateTime
	   , @p_OperationDateEnd DateTime
       , @p_LocalizedBy int
       , @p_LocalizedID int
	)
AS

BEGIN
	select
	  own.Phone
	, c.CardNum
	, ct.Name as CardTypeName
	, cs.Name as CardStatusName
	, ot.Name as OperationTypeName
	--, ct.Code, cs.Code, op.Code
	, o.DTime
	, o.Amount
	, o.Balance
	, o.LocalizedBy
	, o.LocalizedID
	from
	Cards c
	join CardTypes ct on ct.IDCardType = c.IDCardType
	join CardStatuses cs on cs.IDCardStatus = c.IDCardStatus
	join Operations o on o.IDCard = c.IDCard
	join OperationTypes ot on ot.IDOperationType = o.IDOperationType
	join Owners own on own.IDOwner = c.IDOwner
	where 1=1
	  and (@p_Phone IS NULL OR @p_Phone = '' OR LOWER(own.Phone) like '%'+lower(@p_Phone) +'%')
	  and (@p_Cardnum IS NULL OR @p_Cardnum = '' OR LOWER(c.CardNum) like '%'+lower(@p_Cardnum) +'%')
	  and (@p_CardTypeCode IS NULL OR @p_CardTypeCode = '' OR LOWER(ct.Code) like '%'+lower(@p_CardTypeCode) +'%')
	  and (@p_CardStatusName IS NULL OR @p_CardStatusName = '' OR LOWER(cs.Code) like '%'+lower(@p_CardStatusName) +'%')
	  and (@p_OperationTypeName IS NULL OR @p_OperationTypeName = '' OR LOWER(ot.Code) like '%'+lower(@p_OperationTypeName) +'%')
	  and (@p_OperationDateBeg IS NULL or o.DTime >= @p_OperationDateBeg) 
	  and (@p_OperationDateEnd IS NULL or o.DTime <= @p_OperationDateEnd) 
	  and (coalesce(@p_LocalizedBy, 0) = 0 OR o.LocalizedBy = @p_LocalizedBy)
	  and (coalesce(@p_LocalizedID, 0) = 0 OR o.LocalizedID = @p_LocalizedID)
    order by o.DTime
END
GO
/****** Object:  StoredProcedure [dbo].[GetPostAmounts]    Script Date: 15.09.2019 17:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetPostAmounts] (      
         @p_RegionCode int
       , @p_WashCode nvarchar(5)
       , @p_PostCode nvarchar(10)
	   , @p_DateBeg DateTime
	   , @p_DateEnd DateTime
	)
AS

BEGIN
	select
	  r.Name as region
	, w.IDWash
	, w.Code as washName
	, w.Name as washAddress
	, p.Name
	, sum(pa.Amount) as amount
	, sum(pa.m10) as m10
	, sum(pa.b10) as b10
	, sum(pa.b50) as b50
	, sum(pa.b100) as b100
	, sum(pa.b500) as b500
	, sum(pa.b1k) as b1k
	from
	CountersTotal pa
	join Posts p on p.IDPost = pa.IDPost
	join Wash w on w.IDWash = p.IDWash
	join Regions r on r.IDRegion = w.IDRegion
	where 1=1
  	  and (coalesce(@p_RegionCode, 0) = 0 OR r.Code = @p_RegionCode)
	  and (@p_WashCode IS NULL OR @p_WashCode = '' OR @p_WashCode = '0' OR LOWER(w.Code) like '%'+lower(@p_WashCode) +'%')
	  and (@p_PostCode IS NULL OR @p_PostCode = '' OR @p_PostCode = '0' OR LOWER(p.Code) like '%'+lower(@p_PostCode) +'%')
  	  --and pa.DTime >= CAST('2019-06-15 00:00:00' AS datetime2(7)) AND pa.DTime < CAST('2019-06-16 00:00:00' AS datetime2(7))
	  and (@p_DateBeg IS NULL or pa.DTime >= @p_DateBeg) 
	  and (@p_DateEnd IS NULL or pa.DTime < @p_DateEnd) 
	group by r.Name, w.IDWash, w.Code, w.Name, p.Name
	order by w.IDWash, p.Name
END
GO
/****** Object:  StoredProcedure [dbo].[GetWashAmounts]    Script Date: 15.09.2019 17:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetWashAmounts] (      
         @p_RegionCode int
       , @p_WashCode nvarchar(5)
       , @p_PostCode nvarchar(10)
	   , @p_DateBeg DateTime
	   , @p_DateEnd DateTime
	)
AS

BEGIN
	select
	  r.Name as region
	, w.IDWash
	, w.Code as washName
	, w.Name as washAddress
	, sum(pa.Amount) as amount
	from
	vPostsAmount pa
	join Posts p on p.IDPost = pa.IDPost
	join Wash w on w.IDWash = p.IDWash
	join Regions r on r.IDRegion = w.IDRegion
	where 1=1
  	  and (coalesce(@p_RegionCode, 0) = 0 OR r.Code = @p_RegionCode)
	  and (@p_WashCode IS NULL OR @p_WashCode = '' OR @p_WashCode = '0' OR LOWER(w.Code) like '%'+lower(@p_WashCode) +'%')
	  and (@p_PostCode IS NULL OR @p_PostCode = '' OR @p_PostCode = '0' OR LOWER(p.Code) like '%'+lower(@p_PostCode) +'%')
  	  --and pa.DTime >= CAST('2019-06-15 00:00:00' AS datetime2(7)) AND pa.DTime < CAST('2019-06-16 00:00:00' AS datetime2(7))
	  and (@p_DateBeg IS NULL or pa.DTime >= @p_DateBeg) 
	  and (@p_DateEnd IS NULL or pa.DTime < @p_DateEnd) 
	group by r.Name, w.IDWash, w.Code, w.Name
	order by w.IDWash
END
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Типы событий' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'EventKind'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "CountersTotal"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 135
               Right = 213
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vPostsAmount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vPostsAmount'
GO
USE [master]
GO
ALTER DATABASE [WashCompany] SET  READ_WRITE 
GO
