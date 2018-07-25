CREATE TABLE [dbo].[Surveys] (
    [Id] [int] NOT NULL IDENTITY,
    [Name] [nvarchar](max),
    [Version] [nvarchar](max),
    [IsActive] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.Surveys] PRIMARY KEY ([Id])
)