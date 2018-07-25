CREATE TABLE [dbo].[TestResponses] (
    [TestId] [uniqueidentifier] NOT NULL,
    [SectionId] [int] NOT NULL,
    [PageId] [int] NOT NULL,
    [Response] [nvarchar](max),
    [Created] [datetime] NOT NULL,
    [Modified] [datetime],
    CONSTRAINT [PK_dbo.TestResponses] PRIMARY KEY ([TestId], [SectionId], [PageId])
)
GO
ALTER TABLE [dbo].[TestResponses] ADD CONSTRAINT [FK_dbo.TestResponses_dbo.Pages_PageId] FOREIGN KEY ([PageId]) REFERENCES [dbo].[Pages] ([Id]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TestResponses] ADD CONSTRAINT [FK_dbo.TestResponses_dbo.Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [dbo].[Sections] ([Id]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TestResponses] ADD CONSTRAINT [FK_dbo.TestResponses_dbo.Tests_TestId] FOREIGN KEY ([TestId]) REFERENCES [dbo].[Tests] ([Id]) ON DELETE CASCADE
GO
CREATE INDEX [IX_TestId] ON [dbo].[TestResponses]([TestId])
GO
CREATE INDEX [IX_SectionId] ON [dbo].[TestResponses]([SectionId])
GO
CREATE INDEX [IX_PageId] ON [dbo].[TestResponses]([PageId])