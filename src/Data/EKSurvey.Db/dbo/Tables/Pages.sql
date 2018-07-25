CREATE TABLE [dbo].[Pages] (
    [Id] [int] NOT NULL IDENTITY,
    [SectionId] [int] NOT NULL,
    [Order] [int] NOT NULL,
    [Range] [int],
    [Text] [nvarchar](max),
    [True] [nvarchar](max),
    [False] [nvarchar](max),
    [Discriminator] [nvarchar](128) NOT NULL,
    CONSTRAINT [PK_Pages_Id] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_Pages_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [dbo].[Sections] ([Id]) ON DELETE CASCADE
)
GO
CREATE INDEX [IX_Pages_SectionId] ON [dbo].[Pages]([SectionId])
