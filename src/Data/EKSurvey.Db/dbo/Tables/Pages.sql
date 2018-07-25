CREATE TABLE [dbo].[Pages] (
    [Id] INT NOT NULL IDENTITY(1,1),
    [SectionId] INT NOT NULL,
    [Order] INT NOT NULL,
    [Range] INT,
    [Text] NVARCHAR(MAX),
    [True] NVARCHAR(MAX),
    [False] NVARCHAR(MAX),
    [Discriminator] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_Pages_Id] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_Pages_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [dbo].[Sections] ([Id]) ON DELETE CASCADE
)
GO
CREATE INDEX [IX_Pages_SectionId] ON [dbo].[Pages]([SectionId])