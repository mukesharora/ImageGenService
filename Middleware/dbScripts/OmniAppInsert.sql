USE [CALCMan]
GO
SET IDENTITY_INSERT [dbo].[OmniApplication_YesNo] ON
INSERT [dbo].[OmniApplication_YesNo](ID, Text) Values(1, N'Yes')
INSERT [dbo].[OmniApplication_YesNo](ID, Text) Values(2, N'No')
SET IDENTITY_INSERT [dbo].[OmniApplication_YesNo] OFF
GO

SET IDENTITY_INSERT [OmniApplication_ScheduleType] ON
INSERT [OmniApplication_ScheduleType] (ID, Description) VALUES (1, N'Always Running')
INSERT [OmniApplication_ScheduleType] (ID, Description) VALUES (2, N'Interval')
INSERT [OmniApplication_ScheduleType] (ID, Description) VALUES (3, N'Daily At Time')
INSERT [OmniApplication_ScheduleType] (ID, Description) VALUES (4, N'Once')
SET IDENTITY_INSERT [OmniApplication_ScheduleType] OFF
GO
/****** Object:  Table [dbo].[OmniApplication_ScheduledApps]    Script Date: 09/17/2013 15:41:22 ******/
SET IDENTITY_INSERT [dbo].[OmniApplication_ScheduledApps] ON
INSERT [dbo].[OmniApplication_ScheduledApps] ([ID], [AppName], [FullyQualifiedExePath], [WorkingDirectoryPath], [CommandLine], [HideGUI], [AppGUID], [dtLastRun], [dtNextRun], [RunIntervalMinutes], [FK_ID_ScheduleType], [SingleRunTimeHHMMSS], [Enabled]) VALUES (3, N'OmniLogServer', N'C:\Program Files (x86)\Omni-ID\OmniLogServer\OmniLogServer.exe', NULL, NULL, 2, N'5B8AA4EC-63BA-44F1-8CE3-973F2C47CC38', CAST(0x0000A23C00DA7877 AS DateTime), NULL, NULL, 1, NULL, 1)
INSERT [dbo].[OmniApplication_ScheduledApps] ([ID], [AppName], [FullyQualifiedExePath], [WorkingDirectoryPath], [CommandLine], [HideGUI], [AppGUID], [dtLastRun], [dtNextRun], [RunIntervalMinutes], [FK_ID_ScheduleType], [SingleRunTimeHHMMSS], [Enabled]) VALUES (4, N'OmniConfigServer', N'C:\Program Files (x86)\OMNI-ID\bin\OmniConfigServer.exe', NULL, NULL, 2, N'3B37D0CC-9960-4BF7-8B9C-5BF23DF7174D', CAST(0x0000A1D10156549B AS DateTime), NULL, NULL, 1, NULL, 2)
INSERT [dbo].[OmniApplication_ScheduledApps] ([ID], [AppName], [FullyQualifiedExePath], [WorkingDirectoryPath], [CommandLine], [HideGUI], [AppGUID], [dtLastRun], [dtNextRun], [RunIntervalMinutes], [FK_ID_ScheduleType], [SingleRunTimeHHMMSS], [Enabled]) VALUES (5, N'OmniStartupLoggingManagedApp', N'C:\Program Files (x86)\OMNI-ID\bin\OmniStartupLoggingManagedApp.exe', NULL, NULL, 1, N'DC5475C2-7AF2-4CA4-9349-0BFC81A2F58E', CAST(0x0000A1D10153BD7B AS DateTime), CAST(0x0000A1D101567C9B AS DateTime), 10, 2, NULL, 2)
INSERT [dbo].[OmniApplication_ScheduledApps] ([ID], [AppName], [FullyQualifiedExePath], [WorkingDirectoryPath], [CommandLine], [HideGUI], [AppGUID], [dtLastRun], [dtNextRun], [RunIntervalMinutes], [FK_ID_ScheduleType], [SingleRunTimeHHMMSS], [Enabled]) VALUES (6, N'OmniWebIPCRelay', N'C:\Program Files (x86)\OMNI-ID\bin\OmniWebIPCRelay.exe', NULL, NULL, 1, N'49246E65-9337-4BE0-ADF9-070D71237DEA', CAST(0x0000A23B011313B3 AS DateTime), NULL, NULL, 1, NULL, 2)
SET IDENTITY_INSERT [dbo].[OmniApplication_ScheduledApps] OFF
/****** Object:  Table [dbo].[OmniApplication_ParameterType]    Script Date: 09/17/2013 15:41:22 ******/
SET IDENTITY_INSERT [dbo].[OmniApplication_ParameterType] ON
INSERT [dbo].[OmniApplication_ParameterType] ([ID], [Name], [Description], [ValidationRegularExpression]) VALUES (1, N'String', N'String', NULL)
SET IDENTITY_INSERT [dbo].[OmniApplication_ParameterType] OFF
/****** Object:  Table [dbo].[OmniApplication_ParameterCategory]    Script Date: 10/21/2013 11:44:26 ******/
update [dbo].[AuditSystem] set RequestRfidTriggerCount=1, RequestAwakeTime=1, RequestAnnounceCnt=1, RequestPageFlipsCnt=1, RequestResetsCnt=1, MaxRequestIntervalInMinutes=60, AuditAnnouncements=1 



