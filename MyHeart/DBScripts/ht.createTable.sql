﻿/*重新创建Heart数据库表*/

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



SELECT top 6 NewID() as random, Convert(varchar(10),[pub_date],120) as pubdate,[writer],[content]
FROM [heart].[dbo].[ht_message]
where writer='juejue'
ORDER BY random desc

select * from ht_heartInfo
where pubName='w'
 order by heartId desc

select (
	select count(*) from ht_heartInfo
	where pubName='w'
) as allcount,
(
	select count(*) from ht_heartInfo
	where pubName='w' and station=1
) as okcount


select * from ht_message where pub_date='2014-01-01'

update ht_userInfo set nickName='格格' where id=1

se