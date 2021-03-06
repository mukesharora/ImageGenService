USE [CALCMan]
GO
/****** Object:  Table [dbo].[OmniApplication_Application]    Script Date: 09/17/2013 13:34:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OmniApplication_Application](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[GUID] [nvarchar](40) NOT NULL,
 CONSTRAINT [PK_OmniApplication_Application] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [IX_OmniApplication_Application_GUID] UNIQUE NONCLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [IX_OmniApplication_Application_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OmniApplication_ParameterType]    Script Date: 09/17/2013 13:34:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OmniApplication_ParameterType](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](60) NOT NULL,
	[Description] [nvarchar](60) NOT NULL,
	[ValidationRegularExpression] [nvarchar](255) NULL,
 CONSTRAINT [PK_OmniApplication_ParameterType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [IX_OmniApplication_ParameterType_Description] UNIQUE NONCLUSTERED 
(
	[Description] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [IX_OmniApplication_ParameterType_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OmniApplication_YesNo]    Script Date: 09/17/2013 13:34:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OmniApplication_YesNo](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Text] [nvarchar](5) NOT NULL,
 CONSTRAINT [PK_OmniApplication_YesNo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [IX_OmniApplication_YesNo_Text] UNIQUE NONCLUSTERED 
(
	[Text] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OmniApplication_ScheduleType]    Script Date: 09/17/2013 13:34:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OmniApplication_ScheduleType](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](60) NOT NULL,
 CONSTRAINT [PK_OmniApplication_ScheduleType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [IX_Table_Description] UNIQUE NONCLUSTERED 
(
	[Description] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OmniApplication_ScheduledApps]    Script Date: 09/17/2013 13:34:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OmniApplication_ScheduledApps](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AppName] [nvarchar](255) NOT NULL,
	[FullyQualifiedExePath] [nvarchar](255) NOT NULL,
	[WorkingDirectoryPath] [nvarchar](255) NULL,
	[CommandLine] [nvarchar](1024) NULL,
	[HideGUI] [int] NOT NULL,
	[AppGUID] [nvarchar](38) NOT NULL,
	[dtLastRun] [datetime] NULL,
	[dtNextRun] [datetime] NULL,
	[RunIntervalMinutes] [int] NULL,
	[FK_ID_ScheduleType] [int] NOT NULL,
	[SingleRunTimeHHMMSS] [nvarchar](60) NULL,
	[Enabled] [int] NOT NULL,
 CONSTRAINT [PK_OmniApplication_ScheduledApps] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OmniApplication_ParameterCategory]    Script Date: 09/17/2013 13:34:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OmniApplication_ParameterCategory](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](60) NOT NULL,
	[Description] [nvarchar](60) NOT NULL,
	[FK_ID_OmniApplication_Application] [int] NOT NULL,
 CONSTRAINT [PK_OmniApplication_ParameterCategory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [IX_OmniApplication_ParameterCategory_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC,
	[FK_ID_OmniApplication_Application] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OmniApplication_Parameter]    Script Date: 09/17/2013 13:34:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OmniApplication_Parameter](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FK_ID_OmniApplication_Application] [int] NOT NULL,
	[FK_ID_OmniApplication_ParameterType] [int] NOT NULL,
	[FK_ID_OmniApplication_ParameterCategory] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Value] [nvarchar](1024) NULL,
 CONSTRAINT [PK_OmniApplication_Parameter] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  ForeignKey [FK_OmniApplication_Parameter_OmniApplication_Application]    Script Date: 09/17/2013 13:34:51 ******/
ALTER TABLE [dbo].[OmniApplication_Parameter]  WITH CHECK ADD  CONSTRAINT [FK_OmniApplication_Parameter_OmniApplication_Application] FOREIGN KEY([FK_ID_OmniApplication_Application])
REFERENCES [dbo].[OmniApplication_Application] ([ID])
GO
ALTER TABLE [dbo].[OmniApplication_Parameter] CHECK CONSTRAINT [FK_OmniApplication_Parameter_OmniApplication_Application]
GO
/****** Object:  ForeignKey [FK_OmniApplication_Parameter_OmniApplication_ParameterCategory]    Script Date: 09/17/2013 13:34:51 ******/
ALTER TABLE [dbo].[OmniApplication_Parameter]  WITH CHECK ADD  CONSTRAINT [FK_OmniApplication_Parameter_OmniApplication_ParameterCategory] FOREIGN KEY([FK_ID_OmniApplication_ParameterCategory])
REFERENCES [dbo].[OmniApplication_ParameterCategory] ([ID])
GO
ALTER TABLE [dbo].[OmniApplication_Parameter] CHECK CONSTRAINT [FK_OmniApplication_Parameter_OmniApplication_ParameterCategory]
GO
/****** Object:  ForeignKey [FK_OmniApplication_Parameter_OmniApplication_ParameterType]    Script Date: 09/17/2013 13:34:51 ******/
ALTER TABLE [dbo].[OmniApplication_Parameter]  WITH CHECK ADD  CONSTRAINT [FK_OmniApplication_Parameter_OmniApplication_ParameterType] FOREIGN KEY([FK_ID_OmniApplication_ParameterType])
REFERENCES [dbo].[OmniApplication_ParameterType] ([ID])
GO
ALTER TABLE [dbo].[OmniApplication_Parameter] CHECK CONSTRAINT [FK_OmniApplication_Parameter_OmniApplication_ParameterType]
GO
/****** Object:  ForeignKey [FK_OmniApplication_ParameterCategory_OmniApplication_Application]    Script Date: 09/17/2013 13:34:51 ******/
ALTER TABLE [dbo].[OmniApplication_ParameterCategory]  WITH CHECK ADD  CONSTRAINT [FK_OmniApplication_ParameterCategory_OmniApplication_Application] FOREIGN KEY([FK_ID_OmniApplication_Application])
REFERENCES [dbo].[OmniApplication_Application] ([ID])
GO
ALTER TABLE [dbo].[OmniApplication_ParameterCategory] CHECK CONSTRAINT [FK_OmniApplication_ParameterCategory_OmniApplication_Application]
GO
/****** Object:  ForeignKey [FK_OmniApplication_ScheduledApps_ScheduleType]    Script Date: 09/17/2013 13:34:51 ******/
ALTER TABLE [dbo].[OmniApplication_ScheduledApps]  WITH CHECK ADD  CONSTRAINT [FK_OmniApplication_ScheduledApps_ScheduleType] FOREIGN KEY([FK_ID_ScheduleType])
REFERENCES [dbo].[OmniApplication_ScheduleType] ([ID])
GO
ALTER TABLE [dbo].[OmniApplication_ScheduledApps] CHECK CONSTRAINT [FK_OmniApplication_ScheduledApps_ScheduleType]
GO
/****** Object:  ForeignKey [FK_OmniApplication_ScheduledApps_YesNo]    Script Date: 09/17/2013 13:34:51 ******/
ALTER TABLE [dbo].[OmniApplication_ScheduledApps]  WITH CHECK ADD  CONSTRAINT [FK_OmniApplication_ScheduledApps_YesNo] FOREIGN KEY([HideGUI])
REFERENCES [dbo].[OmniApplication_YesNo] ([ID])
GO
ALTER TABLE [dbo].[OmniApplication_ScheduledApps] CHECK CONSTRAINT [FK_OmniApplication_ScheduledApps_YesNo]
GO
/****** Object:  ForeignKey [FK_OmniApplication_ScheduledApps_YesNo1]    Script Date: 09/17/2013 13:34:51 ******/
ALTER TABLE [dbo].[OmniApplication_ScheduledApps]  WITH CHECK ADD  CONSTRAINT [FK_OmniApplication_ScheduledApps_YesNo1] FOREIGN KEY([Enabled])
REFERENCES [dbo].[OmniApplication_YesNo] ([ID])
GO
ALTER TABLE [dbo].[OmniApplication_ScheduledApps] CHECK CONSTRAINT [FK_OmniApplication_ScheduledApps_YesNo1]
GO

/*
Created upgrade script called V1_to_V2.sql. It upgrades the CALC Man database from what it was before this commit to the latest 
version. All of the creation scripts were updated. This means a database at the version before this commit can be upgraded using 
the script and a new database can be created with the creation scripts. Both will give the same result.
*/

ALTER TABLE Coral
ALTER COLUMN FirmwareRevision [nvarchar](64) NULL;

ALTER TABLE Coral
DROP COLUMN PowerFeedbackValue;

ALTER TABLE Coral
ALTER COLUMN RxSignalStrength float;

/****** Object:  Table [dbo].[AuditSystem]    Script Date: 01/30/2014 15:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditSystem](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AuditAnnouncements] [bit] NOT NULL,
	[KeepAuditHistoryInDays] [int] NULL,
	[AnnouncesPerMinuteThreshold] [int] NULL,
	[NumAnnouncesWindow] [int] NULL,
	[MaxRequestIntervalInMinutes] [int] NULL,
	[RequestCurrentPage] [bit] NULL,
	[RequestNumLoadedPages] [bit] NULL,
	[RequestMaxNumPages] [bit] NULL,
	[RequestSleepDwell] [bit] NULL,
	[RequestAwakeDwell] [bit] NULL,
	[RequestRetryInterval] [bit] NULL,
	[RequestRetryAttempts] [bit] NULL,
	[RequestRfChannel] [bit] NULL,
	[RequestTxPower] [bit] NULL,
	[RequestHardwareRevision] [bit] NULL,
	[RequestFirmwareRevision] [bit] NULL,
	[RequestBattery] [bit] NULL,
	[RequestTemperature] [bit] NULL,
	[RequestLastRxSignalStrength] [bit] NULL,
	[RequestNumBeaconsTransmitted] [bit] NULL,
	[RequestRfidTriggerCount] [bit] NULL,
	[RequestAwakeTime] [bit] NULL,
	[RequestAnnounceCnt] [bit] NULL,
	[RequestPageFlipsCnt] [bit] NULL,
	[RequestResetsCnt] [bit] NULL,
 CONSTRAINT [PK_AuditSystem] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[AuditAnnounces]    Script Date: 01/30/2014 15:02:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditAnnounces](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CurrentPageNum] [int] NULL,
	[Family] [nchar](4) NULL,
	[SleepDwell] [int] NULL,
	[AwakeDwell] [int] NULL,
	[Battery] [int] NULL,
	[RxSignalStrength] [float] NULL,
	[FK_ID_WakeupReason] [int] NOT NULL,
	[RfChannel] [int] NULL,
	[TxPower] [int] NULL,
	[HardwareRevision] [int] NULL,
	[FirmwareRevision] [nvarchar](64) NULL,
	[LastSeenDateTime] [datetime] NULL,
	[FK_ID_CALC] [int] NOT NULL,
	[FK_ID_CORAL] [int] NOT NULL,
	[AnnounceCnt] [int] NULL,
	[Antenna] [int] NULL,
	[AwakeTime] [int] NULL,
	[FrequencyOffset] [int] NULL,
	[LastRxSignalStrength] [int] NULL,
	[LinkQuality] [int] NULL,
	[MaxNumPages] [int] NULL,
	[NumBeaconsTransmitted] [int] NULL,
	[NumLoadedPages] [int] NULL,
	[PageFlipsCnt] [int] NULL,
	[ResetsCnt] [int] NULL,
	[RetryAttempts] [int] NULL,
	[RetryInterval] [int] NULL,
	[RfidTriggerCount] [int] NULL,
	[Temperature] [int] NULL,
 CONSTRAINT [PK_AuditAnnounces] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  ForeignKey [FK_AuditAnnounces_Calc]    Script Date: 01/30/2014 15:02:50 ******/
ALTER TABLE [dbo].[AuditAnnounces]  WITH CHECK ADD  CONSTRAINT [FK_AuditAnnounces_Calc] FOREIGN KEY([FK_ID_CALC])
REFERENCES [dbo].[Calc] ([ID])
GO
ALTER TABLE [dbo].[AuditAnnounces] CHECK CONSTRAINT [FK_AuditAnnounces_Calc]
GO
/****** Object:  ForeignKey [FK_AuditAnnounces_Coral]    Script Date: 01/30/2014 15:02:50 ******/
ALTER TABLE [dbo].[AuditAnnounces]  WITH CHECK ADD  CONSTRAINT [FK_AuditAnnounces_Coral] FOREIGN KEY([FK_ID_CORAL])
REFERENCES [dbo].[Coral] ([ID])
GO
ALTER TABLE [dbo].[AuditAnnounces] CHECK CONSTRAINT [FK_AuditAnnounces_Coral]
GO
/****** Object:  ForeignKey [FK_AuditAnnounces_WakeupReason]    Script Date: 01/30/2014 15:02:50 ******/
ALTER TABLE [dbo].[AuditAnnounces]  WITH CHECK ADD  CONSTRAINT [FK_AuditAnnounces_WakeupReason] FOREIGN KEY([FK_ID_WakeupReason])
REFERENCES [dbo].[WakeupReason] ([ID])
GO
ALTER TABLE [dbo].[AuditAnnounces] CHECK CONSTRAINT [FK_AuditAnnounces_WakeupReason]
GO

SET IDENTITY_INSERT AuditSystem ON
INSERT INTO AuditSystem (ID,AuditAnnouncements,KeepAuditHistoryInDays) VALUES
(0,0,5)
SET IDENTITY_INSERT AuditSystem OFF
