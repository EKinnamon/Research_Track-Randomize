CREATE TABLE [dbo].[TestSectionMarkers]
(
	[Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_TestSectionMarkers_Id] DEFAULT NEWID(),
	[TestId] UNIQUEIDENTIFIER NOT NULL,
	[SectionId] INT NOT NULL,
	[Started] DATETIME NOT NULL,
	[Completed] DATETIME NULL

	CONSTRAINT [PK_TestSectionMarkers_Id] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_TestSectionMarkers_TestId] FOREIGN KEY (TestId) REFERENCES [dbo].[Tests] ([Id]) ON DELETE CASCADE,
	CONSTRAINT [FK_TestSectionMarkers_SectionId] FOREIGN KEY (SectionId) REFERENCES [dbo].[Sections] ([Id]), 
    CONSTRAINT [UK_TestSectionMarkers_TestId_SectionId] UNIQUE ([TestId], [SectionId])
)
GO
CREATE INDEX [IX_TestSectionMarkers_TestId] ON [dbo].[TestSectionMarkers] ([TestId])
GO
CREATE INDEX [IX_TestSectionMarkers_SectionId] ON [dbo].[TestSectionMarkers] ([SectionId])
