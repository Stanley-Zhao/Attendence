DECLARE @RChouID uniqueidentifier
SELECT @RChouID = PKEmployeeID FROM [dbo].[Employee] WHERE Email = N'RChou@Advent.com'

DECLARE @HNiuID uniqueidentifier
SELECT @HNiuID = PKEmployeeID FROM [dbo].[Employee] WHERE Email = N'HNiu@Advent.com'

DECLARE @AdminRoleID uniqueidentifier
SELECT @AdminRoleID = PKRoleID FROM [dbo].[Role] WHERE [Name] = N'Admin'

DECLARE @ManagerRoleID uniqueidentifier
SELECT @ManagerRoleID = PKRoleID FROM [dbo].[Role] WHERE [Name] = N'Manager'

DECLARE @EmployeeRoleID uniqueidentifier
SELECT @EmployeeRoleID = PKRoleID FROM [dbo].[Role] WHERE [Name] = N'Employee'

--Scott
INSERT INTO [dbo].[Employee]
([FKReportManagerID], [Email], [Password], [Gender], [FirstName], [LastName], [ServiceYears], [HiredDate], [IsActive])
VALUES
(@HNiuID, N'SSun@Advent.com', '68103bbdaad8b9aa9c7504644dadb4a7', 1, N'Scott', N'Sun', 0, '2009-11-01', 1)

DECLARE @SSunID uniqueidentifier
SELECT @SSunID = PKEmployeeID FROM [dbo].[Employee] WHERE Email = N'SSun@Advent.com'

INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@SSunID, @ManagerRoleID, 0)

INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@SSunID, @EmployeeRoleID, 0)

--Stanley
INSERT INTO [dbo].[Employee]
([FKReportManagerID], [Email], [Password], [Gender], [FirstName], [LastName], [ServiceYears], [HiredDate], [IsActive])
VALUES
(@SSunID, N'SZhao@Advent.com', '68103bbdaad8b9aa9c7504644dadb4a7', 1, N'Stanley', N'Zhao', 0, '2010-12-01', 1)

DECLARE @SZhaoID uniqueidentifier
SELECT @SZhaoID = PKEmployeeID FROM [dbo].[Employee] WHERE Email = N'SZhao@Advent.com'

INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@SZhaoID, @EmployeeRoleID, 0)

--Jing
INSERT INTO [dbo].[Employee]
([FKReportManagerID], [Email], [Password], [Gender], [FirstName], [LastName], [ServiceYears], [HiredDate], [IsActive])
VALUES
(@HNiuID, N'JLi@Advent.com', '68103bbdaad8b9aa9c7504644dadb4a7', 2, N'Jing', N'Li', 0, '2009-11-01', 1)

DECLARE @JLiID uniqueidentifier
SELECT @JLiID = PKEmployeeID FROM [dbo].[Employee] WHERE Email = N'JLi@Advent.com'

INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@JLiID, @ManagerRoleID, 0)

INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@JLiID, @EmployeeRoleID, 0)

--Hao
INSERT INTO [dbo].[Employee]
([FKReportManagerID], [Email], [Password], [Gender], [FirstName], [LastName], [ServiceYears], [HiredDate], [IsActive])
VALUES
(@JLiID, N'HWu@Advent.com', '68103bbdaad8b9aa9c7504644dadb4a7', 1, N'Hao', N'Wu', 0, '2009-11-01', 1)

DECLARE @HWuID uniqueidentifier
SELECT @HWuID = PKEmployeeID FROM [dbo].[Employee] WHERE Email = N'HWu@Advent.com'

INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@HWuID, @ManagerRoleID, 0)

INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@HWuID, @EmployeeRoleID, 0)

--Shan
INSERT INTO [dbo].[Employee]
([FKReportManagerID], [Email], [Password], [Gender], [FirstName], [LastName], [ServiceYears], [HiredDate], [IsActive])
VALUES
(@HWuID, N'SHao@Advent.com', '68103bbdaad8b9aa9c7504644dadb4a7', 2, N'Shan', N'Hao', 0, '2009-11-01', 1)

DECLARE @SHaoID uniqueidentifier
SELECT @SHaoID = PKEmployeeID FROM [dbo].[Employee] WHERE Email = N'SHao@Advent.com'

INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@SHaoID, @EmployeeRoleID, 0)

--Joel
INSERT INTO [dbo].[Employee]
([FKReportManagerID], [Email], [Password], [Gender], [FirstName], [LastName], [ServiceYears], [HiredDate], [IsActive])
VALUES
(@HNiuID, N'JZhao@Advent.com', '68103bbdaad8b9aa9c7504644dadb4a7', 1, N'Joel', N'Zhao', 0, '2009-11-01', 1)

DECLARE @JZhaoID uniqueidentifier
SELECT @JZhaoID = PKEmployeeID FROM [dbo].[Employee] WHERE Email = N'JZhao@Advent.com'

INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@JZhaoID, @ManagerRoleID, 0)

INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@JZhaoID, @EmployeeRoleID, 0)

--Brandon
INSERT INTO [dbo].[Employee]
([FKReportManagerID], [Email], [Password], [Gender], [FirstName], [LastName], [ServiceYears], [HiredDate], [IsActive])
VALUES
(@JZhaoID, N'BShen@Advent.com', '68103bbdaad8b9aa9c7504644dadb4a7', 1, N'Brandon', N'Shen', 0, '2009-11-01', 1)

DECLARE @BShenID uniqueidentifier
SELECT @BShenID = PKEmployeeID FROM [dbo].[Employee] WHERE Email = N'BShen@Advent.com'

INSERT INTO [dbo].[EmployeeRoleRL]
([FKEmployeeID], [FKRoleID], [IsDeleted])
VALUES
(@BShenID, @EmployeeRoleID, 0)

--fill ReportPeriod for reports
INSERT INTO [CARS].[dbo].[ReportPeriod]
           ([Month]
           ,[StartTime]
           ,[EndTime])
     VALUES
           (1
           ,'2011-1-1'
           ,'2011-1-25')
GO
INSERT INTO [CARS].[dbo].[ReportPeriod]
           ([Month]
           ,[StartTime]
           ,[EndTime])
     VALUES
           (2
           ,'2011-1-26'
           ,'2011-2-25')
GO
INSERT INTO [CARS].[dbo].[ReportPeriod]
           ([Month]
           ,[StartTime]
           ,[EndTime])
     VALUES
           (3
           ,'2011-2-26'
           ,'2011-3-25')
GO
INSERT INTO [CARS].[dbo].[ReportPeriod]
           ([Month]
           ,[StartTime]
           ,[EndTime])
     VALUES
           (4
           ,'2011-3-26'
           ,'2011-4-25')
GO
INSERT INTO [CARS].[dbo].[ReportPeriod]
           ([Month]
           ,[StartTime]
           ,[EndTime])
     VALUES
           (5
           ,'2011-4-26'
           ,'2011-5-25')
GO
INSERT INTO [CARS].[dbo].[ReportPeriod]
           ([Month]
           ,[StartTime]
           ,[EndTime])
     VALUES
           (6
           ,'2011-5-26'
           ,'2011-6-25')
GO
INSERT INTO [CARS].[dbo].[ReportPeriod]
           ([Month]
           ,[StartTime]
           ,[EndTime])
     VALUES
           (7
           ,'2011-6-26'
           ,'2011-7-25')
GO
INSERT INTO [CARS].[dbo].[ReportPeriod]
           ([Month]
           ,[StartTime]
           ,[EndTime])
     VALUES
           (8
           ,'2011-7-26'
           ,'2011-8-25')
GO
INSERT INTO [CARS].[dbo].[ReportPeriod]
           ([Month]
           ,[StartTime]
           ,[EndTime])
     VALUES
           (9
           ,'2011-8-26'
           ,'2011-9-25')
GO
INSERT INTO [CARS].[dbo].[ReportPeriod]
           ([Month]
           ,[StartTime]
           ,[EndTime])
     VALUES
           (10
           ,'2011-9-26'
           ,'2011-10-25')
GO


