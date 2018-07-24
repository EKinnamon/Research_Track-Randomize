CREATE TABLE [dbo].[SurveySections]
(
	[SurveyId] INT NOT NULL,
	[SectionId] INT NOT NULL

	CONSTRAINT [PK_SurveySections_SurveyId_SectionId] PRIMARY KEY ([SurveyId], [SectionId]),
	CONSTRAINT [FK_SurveySections_SurveyId] FOREIGN KEY ([SurveyId]) REFERENCES [dbo].[Surveys] ([Id]),
	CONSTRAINT [FK_SurveySections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [dbo].[Sections] ([Id])
)
