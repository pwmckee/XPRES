
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 07/05/2016 09:10:46
-- Generated from EDMX file: C:\Users\pwmck\Dropbox\Programming\WPF\MyProjects\XPRES\XPRES\XPRES\DAL\XpresModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [XpresTest];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Adjustments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Adjustments];
GO
IF OBJECT_ID(N'[dbo].[BomChanges]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BomChanges];
GO
IF OBJECT_ID(N'[dbo].[CarrierList]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CarrierList];
GO
IF OBJECT_ID(N'[dbo].[CCTracker]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CCTracker];
GO
IF OBJECT_ID(N'[dbo].[CountSchedule]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CountSchedule];
GO
IF OBJECT_ID(N'[dbo].[DynamicInbMetrics]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DynamicInbMetrics];
GO
IF OBJECT_ID(N'[dbo].[Employees]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Employees];
GO
IF OBJECT_ID(N'[dbo].[InboundActivity]', 'U') IS NOT NULL
    DROP TABLE [dbo].[InboundActivity];
GO
IF OBJECT_ID(N'[dbo].[MaterialReference]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MaterialReference];
GO
IF OBJECT_ID(N'[dbo].[MaterialRequests]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MaterialRequests];
GO
IF OBJECT_ID(N'[dbo].[MetricsAdj]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MetricsAdj];
GO
IF OBJECT_ID(N'[dbo].[MetricsInbound]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MetricsInbound];
GO
IF OBJECT_ID(N'[dbo].[MetricsInv]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MetricsInv];
GO
IF OBJECT_ID(N'[dbo].[Orders]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Orders];
GO
IF OBJECT_ID(N'[dbo].[ProductionArea]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProductionArea];
GO
IF OBJECT_ID(N'[dbo].[RcvSchedule]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RcvSchedule];
GO
IF OBJECT_ID(N'[dbo].[ReplenSAAG]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ReplenSAAG];
GO
IF OBJECT_ID(N'[dbo].[UnitCost]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UnitCost];
GO
IF OBJECT_ID(N'[dbo].[WorkBenchCounts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WorkBenchCounts];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Adjustments'
CREATE TABLE [dbo].[Adjustments] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [PID] nvarchar(255)  NULL,
    [SEQ] nvarchar(255)  NULL,
    [Desc] nvarchar(255)  NULL,
    [Qty] float  NULL,
    [ABSQty] float  NULL,
    [UnitCost] float  NULL,
    [NetValue] float  NULL,
    [ABSValue] float  NULL,
    [Comments] nvarchar(255)  NULL,
    [ReqDate] datetime  NULL,
    [CompDate] datetime  NULL,
    [Type] nvarchar(255)  NULL,
    [Status] nvarchar(255)  NULL,
    [FileID] int  NULL,
    [ReqID] nvarchar(255)  NULL,
    [Requester] nvarchar(255)  NULL
);
GO

-- Creating table 'BomChanges'
CREATE TABLE [dbo].[BomChanges] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [DR] nvarchar(255)  NULL,
    [RevisedItemStatus] nvarchar(255)  NULL,
    [RevisedItem] nvarchar(255)  NULL,
    [RevisedItemDesc] nvarchar(255)  NULL,
    [MorB] nvarchar(255)  NULL,
    [MIP] datetime  NULL,
    [ComponentAction] nvarchar(255)  NULL,
    [ComponentItem] nvarchar(255)  NULL,
    [ComponentDesc] nvarchar(255)  NULL,
    [remarks] nvarchar(255)  NULL
);
GO

-- Creating table 'CarrierLists'
CREATE TABLE [dbo].[CarrierLists] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Carrier] nvarchar(50)  NULL
);
GO

-- Creating table 'CCTrackers'
CREATE TABLE [dbo].[CCTrackers] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [CountDate] datetime  NULL,
    [ORG] nvarchar(255)  NULL,
    [Subinventory] nvarchar(255)  NULL,
    [Locator] nvarchar(255)  NULL,
    [PID] nvarchar(255)  NULL,
    [Description] nvarchar(255)  NULL,
    [SystemQTY] float  NULL,
    [CountedQTY] float  NULL,
    [Difference] float  NULL,
    [Action] nvarchar(255)  NULL,
    [Error] nvarchar(255)  NULL,
    [FirstCount] nvarchar(255)  NULL,
    [SecondCount] nvarchar(255)  NULL,
    [Zone] int  NULL,
    [UnitCost] float  NULL,
    [CountStatus] nvarchar(255)  NULL,
    [CountID] int  NULL
);
GO

-- Creating table 'CountSchedules'
CREATE TABLE [dbo].[CountSchedules] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Zone] int  NULL,
    [CountArea] nvarchar(50)  NULL,
    [CountRange] nvarchar(50)  NULL,
    [GoalYear] nvarchar(4)  NULL,
    [GoalQuarter] int  NULL,
    [GoalMonth] int  NULL,
    [GoalWeek] int  NULL,
    [GoalDate] datetime  NULL,
    [ActualYear] nvarchar(4)  NULL,
    [ActualQuarter] int  NULL,
    [ActualMonth] int  NULL,
    [ActualWeek] int  NULL,
    [ActualDate] datetime  NULL,
    [SecondPassDate] datetime  NULL,
    [FirstCount] nvarchar(50)  NULL,
    [SecondCount] nvarchar(50)  NULL,
    [CountStatus] nvarchar(100)  NULL,
    [CountID] int  NULL
);
GO

-- Creating table 'Employees'
CREATE TABLE [dbo].[Employees] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [FirstName] nvarchar(100)  NULL,
    [LastName] nvarchar(100)  NULL,
    [HireDate] datetime  NULL,
    [TermDate] datetime  NULL,
    [Title] nvarchar(255)  NULL,
    [Dept] nvarchar(255)  NULL,
    [Status] nvarchar(255)  NULL,
    [Company] nvarchar(255)  NULL,
    [XpresAccess] nvarchar(255)  NULL,
    [Phone] nvarchar(20)  NULL,
    [UserName] nvarchar(20)  NULL,
    [Password] nvarchar(20)  NULL,
    [Picture] varbinary(max)  NULL,
    [FullName] nvarchar(255)  NULL,
    [Role] nvarchar(255)  NULL,
    [OutBound] bit  NULL,
    [InBound] bit  NULL,
    [Inventory] bit  NULL,
    [Replen] bit  NULL
);
GO

-- Creating table 'MaterialReferences'
CREATE TABLE [dbo].[MaterialReferences] (
    [ID] int  NOT NULL,
    [PID] nvarchar(255)  NULL,
    [Description] nvarchar(255)  NULL,
    [ProdLine] nvarchar(255)  NULL,
    [Sub] nvarchar(255)  NULL,
    [Location] nvarchar(255)  NULL
);
GO

-- Creating table 'MaterialRequests'
CREATE TABLE [dbo].[MaterialRequests] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [RequestNum] nvarchar(50)  NULL,
    [FacilityCode] varchar(50)  NULL,
    [ProdLine] varchar(50)  NULL,
    [SubAssem] varchar(50)  NULL,
    [PartNum] varchar(50)  NULL,
    [SubTimestamp] datetime  NULL,
    [AckTimestamp] datetime  NULL,
    [FillTimestamp] datetime  NULL,
    [DelvrTimestamp] datetime  NULL,
    [ReqStatus] varchar(50)  NULL,
    [Comments] varchar(255)  NULL,
    [InvStatus] varchar(50)  NULL,
    [ReqQty] varchar(50)  NULL
);
GO

-- Creating table 'MetricsAdjs'
CREATE TABLE [dbo].[MetricsAdjs] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Date] datetime  NULL,
    [PID] nchar(50)  NULL,
    [Description] nchar(255)  NULL,
    [Source] nchar(50)  NULL,
    [Sub] nchar(50)  NULL,
    [UnitCost] float  NULL,
    [AdjQty] float  NULL,
    [AdjValue] float  NULL,
    [AbsValue] float  NULL,
    [Type] nchar(50)  NULL
);
GO

-- Creating table 'MetricsInvs'
CREATE TABLE [dbo].[MetricsInvs] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Date] datetime  NULL,
    [TotalUnits] float  NULL,
    [UnitsSub] float  NULL,
    [UnitsAdd] float  NULL,
    [UnitsNet] float  NULL,
    [UnitsAbs] float  NULL,
    [TotalValue] float  NULL,
    [ValueSub] float  NULL,
    [ValueAdd] float  NULL,
    [ValueNet] float  NULL,
    [ValueAbs] float  NULL,
    [TotalLocations] float  NULL,
    [FirstPassLocations] float  NULL,
    [SecondPassLocations] float  NULL,
    [GCNIL] float  NULL,
    [ExpectedZones] float  NULL,
    [CountedZones] float  NULL,
    [CountArea] nvarchar(255)  NULL,
    [CountYear] int  NULL,
    [Qtr] int  NULL,
    [CountMonth] int  NULL
);
GO

-- Creating table 'Orders'
CREATE TABLE [dbo].[Orders] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [DeliveryID] int  NULL,
    [StartTime] datetime  NULL,
    [EndTime] datetime  NULL,
    [LineCount] int  NULL,
    [Picker] nchar(50)  NULL,
    [AuditStart] datetime  NULL,
    [AuditEnd] datetime  NULL,
    [DeliveryTime] datetime  NULL,
    [MultiPick] bit  NULL,
    [LPH] int  NULL,
    [PickNum] int  NULL
);
GO

-- Creating table 'RcvSchedules'
CREATE TABLE [dbo].[RcvSchedules] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Appt] datetime  NULL,
    [Carrier] nchar(100)  NULL,
    [PalletNum] int  NULL,
    [Arrive] datetime  NULL,
    [UnloadStart] datetime  NULL,
    [UnloadStop] datetime  NULL,
    [RecStop] datetime  NULL,
    [PutawayStop] datetime  NULL,
    [Ltl] bit  NULL,
    [ApptID] nvarchar(100)  NULL,
    [PoRcvd] int  NULL,
    [PoNotRcvd] int  NULL,
    [PoPtwy] int  NULL,
    [PoNotPtwy] int  NULL,
    [VNC] int  NULL
);
GO

-- Creating table 'ReplenSAAGs'
CREATE TABLE [dbo].[ReplenSAAGs] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [TimeStamp] datetime  NULL,
    [Employee] nchar(100)  NULL,
    [LK00001] int  NULL,
    [KARDEX] int  NULL,
    [PL] int  NULL,
    [HUBSECURE] int  NULL,
    [CtrlID] nchar(100)  NULL
);
GO

-- Creating table 'UnitCosts'
CREATE TABLE [dbo].[UnitCosts] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Item] nvarchar(255)  NULL,
    [Description] nvarchar(255)  NULL,
    [Unit_Cost] float  NULL
);
GO

-- Creating table 'WorkBenchCounts'
CREATE TABLE [dbo].[WorkBenchCounts] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [CountDate] datetime  NULL,
    [ORG] nvarchar(255)  NULL,
    [Subinventory] nvarchar(255)  NULL,
    [Locator] nvarchar(255)  NULL,
    [PID] nvarchar(255)  NULL,
    [Description] nvarchar(255)  NULL,
    [SystemQTY] float  NULL,
    [Unpacked] float  NULL,
    [Packed] float  NULL,
    [CountedQTY] float  NULL,
    [SysVar] float  NULL,
    [CorrAmnt] float  NULL,
    [Action] nvarchar(255)  NULL,
    [CounterName] nvarchar(255)  NULL
);
GO

-- Creating table 'ProductionAreas'
CREATE TABLE [dbo].[ProductionAreas] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [ProdLine] nvarchar(50)  NULL,
    [SubAssem] nvarchar(50)  NULL
);
GO

-- Creating table 'DynamicInbMetrics'
CREATE TABLE [dbo].[DynamicInbMetrics] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Date] datetime  NULL,
    [CategoryName] nchar(255)  NULL,
    [CategoryType] nchar(255)  NULL,
    [CategoryNumValue] float  NULL,
    [CategoryTextValue] nchar(255)  NULL,
    [CategoryTreeLevel] int  NULL,
    [State] bit  NULL,
    [Parent] nchar(255)  NULL,
    [ChildNum] int  NULL
);
GO

-- Creating table 'InboundActivities'
CREATE TABLE [dbo].[InboundActivities] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [PO_LPN] nvarchar(50)  NULL,
    [Start] datetime  NULL,
    [Finish] datetime  NULL,
    [LineItems] int  NULL,
    [Operator] nvarchar(50)  NULL,
    [CtrlId] nvarchar(255)  NULL,
    [Type] nvarchar(255)  NULL,
    [LPH] int  NULL
);
GO

-- Creating table 'MetricsInbounds'
CREATE TABLE [dbo].[MetricsInbounds] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Date] datetime  NULL,
    [CategoryName] nchar(255)  NULL,
    [CategoryType] nchar(255)  NULL,
    [CategoryTreeLevel] int  NULL,
    [State] bit  NULL,
    [Parent] nchar(255)  NULL,
    [TextValue] nchar(255)  NULL,
    [NumValue] float  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'Adjustments'
ALTER TABLE [dbo].[Adjustments]
ADD CONSTRAINT [PK_Adjustments]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'BomChanges'
ALTER TABLE [dbo].[BomChanges]
ADD CONSTRAINT [PK_BomChanges]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [Id] in table 'CarrierLists'
ALTER TABLE [dbo].[CarrierLists]
ADD CONSTRAINT [PK_CarrierLists]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [ID] in table 'CCTrackers'
ALTER TABLE [dbo].[CCTrackers]
ADD CONSTRAINT [PK_CCTrackers]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'CountSchedules'
ALTER TABLE [dbo].[CountSchedules]
ADD CONSTRAINT [PK_CountSchedules]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Employees'
ALTER TABLE [dbo].[Employees]
ADD CONSTRAINT [PK_Employees]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'MaterialReferences'
ALTER TABLE [dbo].[MaterialReferences]
ADD CONSTRAINT [PK_MaterialReferences]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'MaterialRequests'
ALTER TABLE [dbo].[MaterialRequests]
ADD CONSTRAINT [PK_MaterialRequests]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'MetricsAdjs'
ALTER TABLE [dbo].[MetricsAdjs]
ADD CONSTRAINT [PK_MetricsAdjs]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'MetricsInvs'
ALTER TABLE [dbo].[MetricsInvs]
ADD CONSTRAINT [PK_MetricsInvs]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Orders'
ALTER TABLE [dbo].[Orders]
ADD CONSTRAINT [PK_Orders]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [Id] in table 'RcvSchedules'
ALTER TABLE [dbo].[RcvSchedules]
ADD CONSTRAINT [PK_RcvSchedules]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [ID] in table 'ReplenSAAGs'
ALTER TABLE [dbo].[ReplenSAAGs]
ADD CONSTRAINT [PK_ReplenSAAGs]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'UnitCosts'
ALTER TABLE [dbo].[UnitCosts]
ADD CONSTRAINT [PK_UnitCosts]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'WorkBenchCounts'
ALTER TABLE [dbo].[WorkBenchCounts]
ADD CONSTRAINT [PK_WorkBenchCounts]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'ProductionAreas'
ALTER TABLE [dbo].[ProductionAreas]
ADD CONSTRAINT [PK_ProductionAreas]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'DynamicInbMetrics'
ALTER TABLE [dbo].[DynamicInbMetrics]
ADD CONSTRAINT [PK_DynamicInbMetrics]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [Id] in table 'InboundActivities'
ALTER TABLE [dbo].[InboundActivities]
ADD CONSTRAINT [PK_InboundActivities]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [ID] in table 'MetricsInbounds'
ALTER TABLE [dbo].[MetricsInbounds]
ADD CONSTRAINT [PK_MetricsInbounds]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------