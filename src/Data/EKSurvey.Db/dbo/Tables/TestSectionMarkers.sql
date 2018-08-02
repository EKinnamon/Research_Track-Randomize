CREATE TABLE [dbo].[TestSectionMarkers]
(
	[TestId] UNIQUEIDENTIFIER NOT NULL,
	[SectionId] INT NOT NULL,
	[IsSelected] BIT NULL,
	[Started] DATETIME NOT NULL,
	[Completed] DATETIME NULL

	CONSTRAINT [PK_TestSectionMarkers_TestId_SectionId] PRIMARY KEY ([TestId], [SectionId]),
	CONSTRAINT [FK_TestSectionMarkers_TestId] FOREIGN KEY (TestId) REFERENCES [dbo].[Tests] ([Id]) ON DELETE CASCADE,
	CONSTRAINT [FK_TestSectionMarkers_SectionId] FOREIGN KEY (SectionId) REFERENCES [dbo].[Sections] ([Id])
)
