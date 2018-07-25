CREATE TABLE [dbo].[Sections] (
    [Id] [int] NOT NULL IDENTITY,
    [SurveyId] [int] NOT NULL,
    [Name] [nvarchar](MAX),
    [Order] [int] NOT NULL,
    CONSTRAINT [PK_dbo.Sections] PRIMARY KEY ([Id])
)
GO
ALTER TABLE [dbo].[Sections] ADD CONSTRAINT [FK_dbo.Sections_dbo.Surveys_SurveyId] FOREIGN KEY ([SurveyId]) REFERENCES [dbo].[Surveys] ([Id]) ON DELETE CASCADE
GO
CREATE INDEX [IX_SurveyId] ON [dbo].[Sections]([SurveyId])