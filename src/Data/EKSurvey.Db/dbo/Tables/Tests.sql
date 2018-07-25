CREATE TABLE [dbo].[Tests] (
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_Tests_Id] DEFAULT NEWID(),
    [UserId] NVARCHAR(128),
    [SurveyId] INT NOT NULL,
    [Started] DATETIME NOT NULL CONSTRAINT [DF_Tests_Started] DEFAULT GETUTCDATE(),
    [Modified] DATETIME,
    [Completed] DATETIME,
    CONSTRAINT [PK_Tests_Id] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Tests_SurveyId] FOREIGN KEY ([SurveyId]) REFERENCES [dbo].[Surveys] ([Id]) ON DELETE CASCADE
)
GO
CREATE INDEX [IX_Tests_SurveyId] ON [dbo].[Tests]([SurveyId])