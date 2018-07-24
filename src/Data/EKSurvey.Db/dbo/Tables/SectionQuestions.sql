CREATE TABLE [dbo].[SectionQuestions]
(
	[Id] INT NOT NULL IDENTITY(1,1),
	[SectionId] INT NOT NULL,
	[ResponseTypeId] INT NOT NULL,
	[Question] NVARCHAR(MAX) NULL,
	[QuestionLayout] NVARCHAR(MAX) NULL,
	[Order] INT NOT NULL,

)
