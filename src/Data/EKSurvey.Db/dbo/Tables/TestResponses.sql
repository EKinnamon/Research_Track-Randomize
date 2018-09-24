CREATE TABLE [dbo].[TestResponses] (
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_TestResponses_Id] DEFAULT NEWID(),
    [TestId] UNIQUEIDENTIFIER NOT NULL,
    [PageId] INT NOT NULL,
    [Response] NVARCHAR(MAX),
    [Created] DATETIME NOT NULL,
    [Modified] DATETIME NULL,
    [Responded] DATETIME NULL,
    CONSTRAINT [PK_TestResponses_Id] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TestResponses_TestId] FOREIGN KEY ([TestId]) REFERENCES [dbo].[Tests] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TestResponses_PageId] FOREIGN KEY ([PageId]) REFERENCES [dbo].[Pages] ([Id]), 
    CONSTRAINT [UK_TestResponses_TestId_SectionId_PageId] UNIQUE ([TestId], [PageId]) 
)
GO
CREATE INDEX [IX_TestResponses_TestId] ON [dbo].[TestResponses]([TestId])
GO
CREATE INDEX [IX_TestResponses_PageId] ON [dbo].[TestResponses]([PageId])