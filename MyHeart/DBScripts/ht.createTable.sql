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

alter table [dbo].[ht_heartInfo] add heartLevel int default((0));
GO

ALTER TABLE dbo.ht_heartInfo ADD isPrivate INT DEFAULT (0)
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

--用户信息表
CREATE TABLE [dbo].[ht_userInfo]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[loginName] [varchar] (50) COLLATE Chinese_PRC_CI_AS NULL,
[password] [varchar] (50) COLLATE Chinese_PRC_CI_AS NULL,
[nickName] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL,
[useEmail] [int] NULL,
[addDate] [datetime] NULL CONSTRAINT [DF_ht_user_info_add_date] DEFAULT (getdate()),
[status] [int] NULL CONSTRAINT [DF_ht_user_info_status] DEFAULT ((0))
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ht_userInfo] ADD CONSTRAINT [PK_ht_user_info] PRIMARY KEY CLUSTERED  ([id]) ON [PRIMARY]
GO

ALTER TABLE dbo.[ht_userInfo] ADD nickNamePy varchar(2000) null;
GO

--用户参与信息表
CREATE TABLE [ht_userJoin]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[heartId] [int] NULL,
[pubLoginName] [varchar] (50) COLLATE Chinese_PRC_CI_AS NULL,
[joinerNickName] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL,
[joinerLoginName] [varchar] (50) COLLATE Chinese_PRC_CI_AS NULL,
[isRead] [varchar] (10) COLLATE Chinese_PRC_CI_AS NULL CONSTRAINT [DF_ht_userJoin_isRead] DEFAULT ('false')
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ht_userJoin] ADD CONSTRAINT [PK_ht_userJoin] PRIMARY KEY CLUSTERED  ([id]) ON [PRIMARY]
GO



