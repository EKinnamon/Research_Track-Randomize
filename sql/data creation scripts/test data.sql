USE [EKSurveyDb]
GO
/****** Object:  Table [dbo].[Pages]    Script Date: 8/4/2018 1:02:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Pages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SectionId] [int] NOT NULL,
	[Order] [int] NOT NULL,
	[Range] [int] NULL,
	[IsHtml] [bit] NOT NULL,
	[Text] [nvarchar](max) NULL,
	[True] [nvarchar](max) NULL,
	[False] [nvarchar](max) NULL,
	[Discriminator] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_Pages_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sections]    Script Date: 8/4/2018 1:02:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sections](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SurveyId] [int] NOT NULL,
	[Name] [nvarchar](256) NULL,
	[Order] [int] NOT NULL,
	[SelectorType] [int] NOT NULL,
 CONSTRAINT [PK_Sections_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Surveys]    Script Date: 8/4/2018 1:02:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Surveys](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[Version] [nvarchar](64) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NULL,
	[Deleted] [datetime] NULL,
 CONSTRAINT [PK_Surveys_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TestResponses]    Script Date: 8/4/2018 1:02:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TestResponses](
	[TestId] [uniqueidentifier] NOT NULL,
	[PageId] [int] NOT NULL,
	[Response] [nvarchar](max) NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NULL,
	[Responded] [datetime] NULL,
 CONSTRAINT [PK_TestResponses_TestId_SectionId_PageId] PRIMARY KEY CLUSTERED 
(
	[TestId] ASC,
	[PageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tests]    Script Date: 8/4/2018 1:02:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tests](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[SurveyId] [int] NOT NULL,
	[Started] [datetime] NOT NULL,
	[Modified] [datetime] NULL,
	[Completed] [datetime] NULL,
 CONSTRAINT [PK_Tests_UserId_SurveyId] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[SurveyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_Tests_Id] UNIQUE NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TestSectionMarkers]    Script Date: 8/4/2018 1:02:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TestSectionMarkers](
	[TestId] [uniqueidentifier] NOT NULL,
	[SectionId] [int] NOT NULL,
	[Started] [datetime] NOT NULL,
	[Completed] [datetime] NULL,
 CONSTRAINT [PK_TestSectionMarkers_TestId_SectionId] PRIMARY KEY CLUSTERED 
(
	[TestId] ASC,
	[SectionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Pages] ADD  CONSTRAINT [DF_Pages_IsHtml]  DEFAULT ((0)) FOR [IsHtml]
GO
ALTER TABLE [dbo].[Sections] ADD  CONSTRAINT [DF_Sections_SelectorType]  DEFAULT ((0)) FOR [SelectorType]
GO
ALTER TABLE [dbo].[Surveys] ADD  CONSTRAINT [DF_Surveys_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Surveys] ADD  CONSTRAINT [DF_Surveys_Created]  DEFAULT (getutcdate()) FOR [Created]
GO
ALTER TABLE [dbo].[Tests] ADD  CONSTRAINT [DF_Tests_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Tests] ADD  CONSTRAINT [DF_Tests_Started]  DEFAULT (getutcdate()) FOR [Started]
GO
ALTER TABLE [dbo].[Pages]  WITH CHECK ADD  CONSTRAINT [FK_Pages_SectionId] FOREIGN KEY([SectionId])
REFERENCES [dbo].[Sections] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Pages] CHECK CONSTRAINT [FK_Pages_SectionId]
GO
ALTER TABLE [dbo].[Sections]  WITH CHECK ADD  CONSTRAINT [FK_Sections_SurveId] FOREIGN KEY([SurveyId])
REFERENCES [dbo].[Surveys] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Sections] CHECK CONSTRAINT [FK_Sections_SurveId]
GO
ALTER TABLE [dbo].[TestResponses]  WITH CHECK ADD  CONSTRAINT [FK_TestResponses_PageId] FOREIGN KEY([PageId])
REFERENCES [dbo].[Pages] ([Id])
GO
ALTER TABLE [dbo].[TestResponses] CHECK CONSTRAINT [FK_TestResponses_PageId]
GO
ALTER TABLE [dbo].[TestResponses]  WITH CHECK ADD  CONSTRAINT [FK_TestResponses_TestId] FOREIGN KEY([TestId])
REFERENCES [dbo].[Tests] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TestResponses] CHECK CONSTRAINT [FK_TestResponses_TestId]
GO
ALTER TABLE [dbo].[Tests]  WITH CHECK ADD  CONSTRAINT [FK_Tests_SurveyId] FOREIGN KEY([SurveyId])
REFERENCES [dbo].[Surveys] ([Id])
GO
ALTER TABLE [dbo].[Tests] CHECK CONSTRAINT [FK_Tests_SurveyId]
GO
ALTER TABLE [dbo].[TestSectionMarkers]  WITH CHECK ADD  CONSTRAINT [FK_TestSectionMarkers_SectionId] FOREIGN KEY([SectionId])
REFERENCES [dbo].[Sections] ([Id])
GO
ALTER TABLE [dbo].[TestSectionMarkers] CHECK CONSTRAINT [FK_TestSectionMarkers_SectionId]
GO
ALTER TABLE [dbo].[TestSectionMarkers]  WITH CHECK ADD  CONSTRAINT [FK_TestSectionMarkers_TestId] FOREIGN KEY([TestId])
REFERENCES [dbo].[Tests] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TestSectionMarkers] CHECK CONSTRAINT [FK_TestSectionMarkers_TestId]
GO
