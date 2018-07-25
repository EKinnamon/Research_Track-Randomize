CREATE TABLE [dbo].[TestResponses] (
    [TestId] UNIQUEIDENTIFIER NOT NULL,
	[SectionId] INT NOT NULL,
    [PageId] INT NOT NULL,
    [Response] NVARCHAR(MAX),
    [Created] DATETIME NOT NULL,
    [Modified] DATETIME NULL,
    CONSTRAINT [PK_TestResponses_TestId_SectionId_PageId] PRIMARY KEY ([TestId], [SectionId], [PageId]),
    CONSTRAINT [FK_TestResponses_TestId] FOREIGN KEY ([TestId]) REFERENCES [dbo].[Tests] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TestResponses_PageId] FOREIGN KEY ([PageId]) REFERENCES [dbo].[Pages] ([Id]),
    CONSTRAINT [FK_TestResponses_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [dbo].[Sections] ([Id])
)
GO
CREATE INDEX [IX_TestResponses_TestId] ON [dbo].[TestResponses]([TestId])
GO
CREATE INDEX [IX_TestResponses_SectionId] ON [dbo].[TestResponses]([SectionId])
GO
CREATE INDEX [IX_TestResponses_PageId] ON [dbo].[TestResponses]([PageId])