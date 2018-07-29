CREATE TABLE [dbo].[Sections] (
    [Id] INT NOT NULL IDENTITY(1,1),
    [SurveyId] INT NOT NULL,
    [Name] NVARCHAR(256),
    [Order] INT NOT NULL,
	[SelectorType] INT NOT NULL CONSTRAINT [DF_Sections_SelectorType] DEFAULT 0,
    CONSTRAINT [PK_Sections_Id] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Sections_SurveId] FOREIGN KEY ([SurveyId]) REFERENCES [dbo].[Surveys] ([Id]) ON DELETE CASCADE
)
GO
CREATE INDEX [IX_Sections_SurveyId] ON [dbo].[Sections]([SurveyId])