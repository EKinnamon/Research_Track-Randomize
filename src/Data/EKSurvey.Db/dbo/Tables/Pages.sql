CREATE TABLE [dbo].[Pages] (
    [Id] INT NOT NULL IDENTITY(1,1),
    [SectionId] INT NOT NULL,
    [Order] INT NOT NULL,
    [Range] INT NULL,
    [IsHtml] BIT NULL,
	[Text] NVARCHAR(MAX) NULL,
    [True] NVARCHAR(MAX) NULL,
    [False] NVARCHAR(MAX) NULL,
    [Discriminator] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_Pages_Id] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_Pages_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [dbo].[Sections] ([Id]) ON DELETE CASCADE
)
GO
CREATE INDEX [IX_Pages_SectionId] ON [dbo].[Pages]([SectionId])