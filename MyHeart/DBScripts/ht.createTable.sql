/*重新创建Heart数据库表*/

--心愿信息表
CREATE TABLE [dbo].[ht_heartInfo]
(
[heartId] [int] NOT NULL IDENTITY(1, 1),
[title] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL,
[pubId] [int] NULL,
[pubName] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL,
[participator] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL,
[contact] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL,
[bgImage] [nvarchar] (500) COLLATE Chinese_PRC_CI_AS NULL,
[content] [nvarchar] (3000) COLLATE Chinese_PRC_CI_AS NULL,
[beginDate] [datetime] NULL,
[endDate] [datetime] NULL,
[station] [int] NULL CONSTRAINT [DF_ht_heartInfo_station] DEFAULT ((0)),
[addDate] [datetime] NULL CONSTRAINT [DF_ht_heartInfo_addDate] DEFAULT (getdate())
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ht_heartInfo] ADD CONSTRAINT [PK_ht_heartInfo] PRIMARY KEY CLUSTERED  ([heartId]) ON [PRIMARY]
GO

--悄悄话信息表
CREATE TABLE [dbo].[ht_message]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[pub_date] [datetime] NULL,
[writer] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL,
[content] [nvarchar] (4000) COLLATE Chinese_PRC_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ht_message] ADD CONSTRAINT [PK_ht_message] PRIMARY KEY CLUSTERED  ([id]) ON [PRIMARY]
GO

SELECT Convert(varchar(10),[pub_date],120),[writer],[content]
FROM [heart].[dbo].[ht_message]
ORDER BY id desc
