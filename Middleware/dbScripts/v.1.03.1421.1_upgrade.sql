/*
Created upgrade script called V1_to_V2.sql. It upgrades the CALC Man database from what it was before this commit to the latest 
version. All of the creation scripts were updated. This means a database at the version before this commit can be upgraded using 
the script and a new database can be created with the creation scripts. Both will give the same result.
*/
USE [CALCMan]
GO

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
(0,0,14)
SET IDENTITY_INSERT AuditSystem OFF

update [dbo].[AuditSystem] set RequestRfidTriggerCount=1, RequestAwakeTime=1, RequestAnnounceCnt=1, RequestPageFlipsCnt=1, RequestResetsCnt=1, MaxRequestIntervalInMinutes=60, AuditAnnouncements=1 



