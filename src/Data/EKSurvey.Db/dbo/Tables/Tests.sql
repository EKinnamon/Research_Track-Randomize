CREATE TABLE [dbo].[Tests] (
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_Tests_Id] DEFAULT NEWID(),
    [UserId] NVARCHAR(128) NOT NULL,
    [SurveyId] INT NOT NULL,
    [Started] DATETIME NOT NULL CONSTRAINT [DF_Tests_Started] DEFAULT GETUTCDATE(),
    [Modified] DATETIME,
    [Completed] DATETIME,
	CONSTRAINT [PK_Tests_Id] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Tests_SurveyId] FOREIGN KEY ([SurveyId]) REFERENCES [dbo].[Surveys] ([Id]),
    CONSTRAINT [UK_Tests_UserId_SurveyId] UNIQUE ([UserId], [SurveyId])
)
GO
CREATE INDEX [IX_Tests_UserId] ON [dbo].[Tests]([UserId])
GO
CREATE INDEX [IX_Tests_SurveyId] ON [dbo].[Tests]([SurveyId])
