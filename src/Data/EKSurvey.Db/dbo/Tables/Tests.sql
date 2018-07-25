CREATE TABLE [dbo].[Tests] (
    [Id] [uniqueidentifier] NOT NULL,
    [UserId] [nvarchar](max),
    [SurveyId] [int] NOT NULL,
    [Started] [datetime] NOT NULL,
    [Modified] [datetime],
    [Completed] [datetime],
    CONSTRAINT [PK_dbo.Tests] PRIMARY KEY ([Id])
)
GO
ALTER TABLE [dbo].[Tests] ADD CONSTRAINT [FK_dbo.Tests_dbo.Surveys_SurveyId] FOREIGN KEY ([SurveyId]) REFERENCES [dbo].[Surveys] ([Id]) ON DELETE CASCADE
GO
CREATE INDEX [IX_SurveyId] ON [dbo].[Tests]([SurveyId])