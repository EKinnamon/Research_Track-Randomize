﻿CREATE TABLE [dbo].[Pages] (
    [Id] INT NOT NULL IDENTITY,
    [SectionId] INT NOT NULL,
    [Order] INT NOT NULL,
    [Range] INT,
    [Text] NVARCHAR(MAX),
    [True] NVARCHAR(MAX),
    [False] NVARCHAR(MAX),
    [Discriminator] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_dbo.Pages] PRIMARY KEY ([Id])
)
CREATE INDEX [IX_SectionId] ON [dbo].[Pages]([SectionId])
CREATE TABLE [dbo].[Sections] (
    [Id] INT NOT NULL IDENTITY,
    [SurveyId] INT NOT NULL,
    [Name] NVARCHAR(256),
    [Order] INT NOT NULL,
    CONSTRAINT [PK_dbo.Sections] PRIMARY KEY ([Id])
)
CREATE INDEX [IX_SurveyId] ON [dbo].[Sections]([SurveyId])
CREATE TABLE [dbo].[Surveys] (
    [Id] INT NOT NULL IDENTITY,
    [Name] NVARCHAR(256) NOT NULL,
    [Version] NVARCHAR(64) NOT NULL,
    [IsActive] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.Surveys] PRIMARY KEY ([Id])
)
CREATE TABLE [dbo].[TestResponses] (
    [TestId] UNIQUEIDENTIFIER NOT NULL,
    [SectionId] INT NOT NULL,
    [PageId] INT NOT NULL,
    [Response] NVARCHAR(MAX),
    [Created] DATETIME NOT NULL,
    [Modified] DATETIME,
    CONSTRAINT [PK_dbo.TestResponses] PRIMARY KEY ([TestId], [SectionId], [PageId])
)
CREATE INDEX [IX_TestId] ON [dbo].[TestResponses]([TestId])
CREATE INDEX [IX_SectionId] ON [dbo].[TestResponses]([SectionId])
CREATE INDEX [IX_PageId] ON [dbo].[TestResponses]([PageId])
CREATE TABLE [dbo].[Tests] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [UserId] NVARCHAR(MAX),
    [SurveyId] INT NOT NULL,
    [Started] DATETIME NOT NULL,
    [Modified] DATETIME,
    [Completed] DATETIME,
    CONSTRAINT [PK_dbo.Tests] PRIMARY KEY ([Id])
)
CREATE INDEX [IX_SurveyId] ON [dbo].[Tests]([SurveyId])
ALTER TABLE [dbo].[Pages] ADD CONSTRAINT [FK_dbo.Pages_dbo.Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [dbo].[Sections] ([Id]) ON DELETE CASCADE
ALTER TABLE [dbo].[Sections] ADD CONSTRAINT [FK_dbo.Sections_dbo.Surveys_SurveyId] FOREIGN KEY ([SurveyId]) REFERENCES [dbo].[Surveys] ([Id]) ON DELETE CASCADE
ALTER TABLE [dbo].[TestResponses] ADD CONSTRAINT [FK_dbo.TestResponses_dbo.Pages_PageId] FOREIGN KEY ([PageId]) REFERENCES [dbo].[Pages] ([Id]) ON DELETE CASCADE
ALTER TABLE [dbo].[TestResponses] ADD CONSTRAINT [FK_dbo.TestResponses_dbo.Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [dbo].[Sections] ([Id]) ON DELETE CASCADE
ALTER TABLE [dbo].[TestResponses] ADD CONSTRAINT [FK_dbo.TestResponses_dbo.Tests_TestId] FOREIGN KEY ([TestId]) REFERENCES [dbo].[Tests] ([Id]) ON DELETE CASCADE
ALTER TABLE [dbo].[Tests] ADD CONSTRAINT [FK_dbo.Tests_dbo.Surveys_SurveyId] FOREIGN KEY ([SurveyId]) REFERENCES [dbo].[Surveys] ([Id]) ON DELETE CASCADE
CREATE TABLE [dbo].[__MigrationHistory] (
    [MigrationId] NVARCHAR(150) NOT NULL,
    [ContextKey] NVARCHAR(300) NOT NULL,
    [Model] [varbinary](MAX) NOT NULL,
    [ProductVersion] NVARCHAR(32) NOT NULL,
    CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY ([MigrationId], [ContextKey])
)
INSERT [dbo].[__MigrationHistory]([MigrationId], [ContextKey], [Model], [ProductVersion])
VALUES (N'201807251930145_Initial', N'EKSurvey.Data.Migrations.Configuration',  0x1F8B0800000000000400ED5D6D6FE33612FE7EC0FD07419FDA436AE7A51BEC05768BACB3E9056D36DB38E9DDB78096688738BDB812152438DC2FBB0FF793EE2F94D42B5F25CAB225EDE250601153E47038F30C391CCDA8FFFBCF7F673FBEFA9EF502A31885C1DC3E991CDB160C9CD045C1666E2778FDDD7BFBC71FFEFCA7D947D77FB57E2BFA9DD17E646410CFED678CB717D369EC3C431FC4131F395118876B3C71427F0ADC707A7A7CFCD7E9C9C914121236A16559B3FB24C0C887E90FF27311060EDCE20478B7A10BBD386F274F962955EB13F061BC050E9CDB1F7F5E26D10B7C9B5C010C6CEBD2438030B184DEDAB64010841860C2E2C5630C97380A83CD724B1A80F7F0B685A4DF1A7831CC59BFA8BA9BAEE2F894AE625A0D2C4839498C43BF25C193B35C2C5371F84EC2B54BB1A522F5B71E7CA5CB4EA537B76F5F9CBF61DF235221BAB52D71CA8B8517D1DE44966F3186FEE4EF7035216326DCB8238B7F7A5462E2DDE474723E393EB21689879308CE0398E0087847D6E764E521E767F8F610FE1306F3B393D5FAECFDBB73E09E9D7F0FCFDED9D69465FC2341067E63F8FE0C36B08EDD12108B3082930C4093940A82F1848E3EB2547D2AD609EAE97FCDAC0789E711C8AD62F2D4C1731B470964A54ED8277DB906D2F4390AB730C26FF7709D2FE9C6E5174D3A4DC581E530664CB6E09B009F9DDAD627C20C5879B00435239C2526EBFC0906300218BA9F01C6300A280D984A579A5D986B091D4AA679CA7A3277910BA396243E8117B44917A1E6C9B6EEA1973E8F9FD13633FC49FEEC892A3BB6ADEB28F4EF43AF1A943D787A00D10662C250A87ABA0C93C811189A4D2B38D682B4E46E579CE604F60BD5AF119AA970BA2293FE5B502836C45BF0FA0B0C36F8796E9FBE3B272842AFD02D5A72AA8F01226723CC2DBF4FF8E7D0DE1DFC05BCD5E02F4CC3D81A5335A8F9491F3DE5F4798EF847B2410ACFBB9964CEE2CE16993EFBBF41F6634A2D0D38177FCDC4E7DF1F62DE9BF89260F3A55CF18790001704DDCF358D6D9BDB9264DF1A5BDBC996AE23081FE02BFE358171B7734EA4B467DFEC0388610E092ABFCC77ACF72FA973DBCDC72C28F4B0965A7052B996164187082E7F2D468DB1700F820DEC0E048ECCD0924B996973549B9F42542A0ED54C378CF174869617873479EF257FB6F69F8C25FA40865E5372DD3128911A5CAEA958F62A577992EB6CCC40DA2382BE87F1961C491DAC81A532B887469991BD346557E6626DD09BE26517F7AF602893E44F09725BFB397B8A00142BE842A342CB81ED621141EA0117F35C911F0FC86FEF231200A23552116A73D953FA832CEE9FB25E9547283D947C42B947EB7B5F4D1486A35E76D4F0575C3F6B592C3AB5E59212696631EBA5E18FFEA8672EEDD1C9ABCE18E8B2070EBEF775BB9DEEB4353DC630AA481C6C37D84FA089B86D51AF7B8AB8A7A5AF01941CEC21D4931949FE9C37A4A7E2FEA930A1E2591BE3B98CE3D0412903FC5694C7BDF8C57C0C5CAB3608563965E546754B4C046D895190D9E7F6892D82FF2EB8825494D6A593BDE15980D801AE2C49B202D78C9D32FE25DE58795EFE224D410C1046343E03BC05D10831691460D95A51E0A02DF0EA44210C323472BACA92BCF8E40A6E6140A347756B3699B7CE5B9A96B308C26F12CD6CCA40A9016142F445AB545D288641596E24FD804C17666D44FDBE90A616481F58532FDD086DE5863F08D864D74EA7DE1A3FAF52307FCF6AD2721D71C34D6AAF58DE09755AD67BC09D56272673ABEF7943E0AED8168CD021B9F07B469FE8FDF77C6877C7A0B080BE6128E8E74B38715987B20E239277C943AF2DE44487B4DF837B67A0096CF7842F41F65FDAD19AE2C368FFE143007BDEDCB8E841037CC7813625EB7DEF69AC4E4CE6D6C666F78FBBECDE4AC6603202461CF4AF56B43D7D6922857B1E6398477C62654659467709319F09525D93B5EF3A1583AB37C9D2F8F24C6D2291AE484921DF2B1B08B00A5591E1EDCB80988E883498D19C2492227F86E9A34CB01181D4146A281966852FC1B12942C050C9F9140D9D5F9BC9BAC5DC02C5CAEB2EC046576076F5056EEA16AFB9B91A0871070928A2E9B20C1AEE65863733660102FC6BA4A1BF8B1D000EEAE87D833C94F705F31B4377A9887784C301A58C73AA45A2F45A1BFD564100CD0B173DD566FBEA0A856C236DC081EC5419BA55DD11C039524DF2AC114611782E0FF0F2D96C9A5521E40DB3A9A65C61760BB65B146C98F285BCC55A66B50B8BEF96ED33FBFD8CC6D4E1F65FD1DD2867C261447604E1297D11E0C26B14C598964DAC003D5917AE2F7513DD15CDE15BCCC67A24B2BE8A23B9E84DFF2ED2E8E8DF77EB6FB86A8ED485F956E1D7E504AEC9CA7CEA18A6298BC21E280FB368F908F040A478FFB408BDC40FF4FEA97E3473456689D4DC9CF5B4F20C61964EDE24D3984D0529481EAD24F0361AE11521E5FB8D462704982E4AEF76BF012F81AAD44496CE158A9D08F9280084BDC1C55AA61A8E579C5536E478C5C867208E4696CAD17966214B206F32D387906D395EA508898EE3D64A96BFC88ECF5ACC7422E6748E572972AEE4C8F5922666707A51A46AD451C8931B5912D7AA040B9D6E15D9A5BDAAB7785E3959462E58CD15DDD880D57739038468471EC8032BA3C99C03A68D31EB2965B52B2C95AC659C1EDCAED0C8EF851D90A1BCDA9A004333F030B8E8AECDB2AA882552369AD3A9AA84B8D594ADA3C1067FE7EE70CEB021DBF638A91FAE3FC4B3D83E7F8CABE3FD7A2AFBBCCF152FF25942BA97FB7A2A55EC9BF31A3511F13A4A6546334BA86C34A753651FB284AAD651E1B92B8E77C46F5FBB5C91FECA5228DA863843CB0C578E50D178488C69515FE5BC72B8AF9A7BC6AB145A14BB94B3972146219438CBC37ACD9F4791E27C5917DB22427A416E1AE3CBBEE0911DEDBF7B0B0F91F5561D6E4180D604D05962B87D7A7CFC5EF8CCCA783E79328D63D7538445C5CF87F0BAEA21B31D5189365656772DCA4967915E28DF042E7C9DDBFF4A475D5837FF782A071E59A9777A611D5BFFEEF6398F74EAB6953C6C95A540C024259FAD3A0C5E40E43C83E81B1FBC7EDB9A105366D78910574AD7899270B1E42972E50D27A7EFF7569CAA4CF7FB62CD43A8D830B68E7C5C27E3C8AE1A356A4B3F84D00E132D0DAEED9731BE0EADB715FC2E9F9750D2A51F98E8F8F58815DA9B4EF557A8AFA2883509D0EF09442948883F1819197646A293590F7CE4F2F5B3A67367A33A4D2C16DD763ADB84C25A97FCC0FB28825311EA589ADEFFA62823BB533D6427450D7A820AA5923DA344815AB174B21BDC3A16300E58B398BDE4EAB546B18FBC62CD6BCAF11545B42B431CB0ECD01CA97BAD33ECB186F58BAB76A82D243432EC0381854FF2DEB9BA6227D8185BFE1E2B17BEB242C0E14FA52101D4E329D51A43439F5626257C439F54594E73EBA2C1919F519A9757E33EA01ACBF1FAAD8E1BC7FE62ACC9BE3797C1CBE9E43476517F72B19CB6562E7B35462E76AB906838BBD0A9CB693465747555742AE2DA9214758D5D4D899D92BAA600A4AEFEAEB1FC4E35516DA586723A6D819E8EBC4CB697DA3D5501516D290A8BAF2A5B7294C5798A622193424413918CA1F6CE4813353BBC2E1D69B49575BB42B6CF65772E9ADB0DBAEC965325E88CB430AE91D1836AAF45D99B9C83428E77E67FE443DC8B186D2A1234B93AC80059112DFADC04EBB0F02F048E8A2E625C1762E09253FF32C2680D1C4C1E3B308ED3AFFFE5C9DB1FFD15746F82BB046F134C960CFD95C72185FA2975F3A7B57D3CCFB3BB6DBABFEE6309844D44A3C877C18704796EC9B722435D47823A40F9DB53AA4B4CDFA26EDE4A4A9FC2C090502EBED26F7B80FED623C4E2BB6009E82BCAF6BC3DC6F017B801CE5B914AA427D2AC085EECB32B043611F0E39C46359EFC241876FDD71FFE005FD9CE74CF6A0000 , N'6.2.0-61023')

