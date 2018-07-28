CREATE TABLE [dbo].[Tests] (
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_Tests_Id] DEFAULT NEWID(),
    [UserId] NVARCHAR(128) NOT NULL,
    [SurveyId] INT NOT NULL,
    [Started] DATETIME NOT NULL CONSTRAINT [DF_Tests_Started] DEFAULT GETUTCDATE(),
    [Modified] DATETIME,
    [Completed] DATETIME,
    CONSTRAINT [UK_Tests_Id] UNIQUE ([Id]),
	CONSTRAINT [PK_Tests_UserId_SurveyId] PRIMARY KEY ([UserId], SurveyId),
    CONSTRAINT [FK_Tests_SurveyId] FOREIGN KEY ([SurveyId]) REFERENCES [dbo].[Surveys] ([Id])
)
GO
CREATE INDEX [IX_Tests_Id] ON [dbo].[Tests]([Id])
GO
CREATE INDEX [IX_Tests_SurveyId] ON [dbo].[Tests]([SurveyId])
