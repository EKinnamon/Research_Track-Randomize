﻿CREATE TABLE [dbo].[Sections]
(
	[Id] INT NOT NULL IDENTITY(1,1),
	[Name] NVARCHAR(256) NOT NULL,
	[TypeId] INT NOT NULL

	CONSTRAINT [PK_Sections_Id] PRIMARY KEY ([Id])
)
