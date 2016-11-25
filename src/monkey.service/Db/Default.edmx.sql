
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 11/25/2016 11:03:48
-- Generated from EDMX file: D:\project\git\monkey\src\monkey.service\Db\Default.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [monkey];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Db_ExceptionLog_inherits_Db_BaseLog]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Db_BaseLogSet_Db_ExceptionLog] DROP CONSTRAINT [FK_Db_ExceptionLog_inherits_Db_BaseLog];
GO
IF OBJECT_ID(N'[dbo].[FK_Db_ManagerUser_inherits_Db_BaseUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Db_BaseUserSet_Db_ManagerUser] DROP CONSTRAINT [FK_Db_ManagerUser_inherits_Db_BaseUser];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Db_BaseLogSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_BaseLogSet];
GO
IF OBJECT_ID(N'[dbo].[Db_BaseLogSet_Db_ExceptionLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_BaseLogSet_Db_ExceptionLog];
GO
IF OBJECT_ID(N'[dbo].[Db_BaseUserSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_BaseUserSet];
GO
IF OBJECT_ID(N'[dbo].[Db_BaseUserSet_Db_ManagerUser]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_BaseUserSet_Db_ManagerUser];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Db_BaseUserSet'
CREATE TABLE [dbo].[Db_BaseUserSet] (
    [Id] nvarchar(50)  NOT NULL,
    [createdOn] datetime  NOT NULL,
    [lastLoginTime] datetime  NULL,
    [lastLoginIpAddress] nvarchar(50)  NULL,
    [roleNames] nvarchar(max)  NOT NULL,
    [isDeleted] bit  NOT NULL,
    [isDisabled] bit  NOT NULL
);
GO

-- Creating table 'Db_BaseLogSet'
CREATE TABLE [dbo].[Db_BaseLogSet] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [logType] tinyint  NOT NULL,
    [createdOn] datetime  NOT NULL,
    [message] nvarchar(max)  NULL
);
GO

-- Creating table 'Db_BaseUserSet_Db_ManagerUser'
CREATE TABLE [dbo].[Db_BaseUserSet_Db_ManagerUser] (
    [loginName] nvarchar(50)  NOT NULL,
    [passWord] nvarchar(50)  NOT NULL,
    [fullName] nvarchar(50)  NOT NULL,
    [mobilePhone] nvarchar(50)  NULL,
    [Id] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Db_BaseLogSet_Db_ExceptionLog'
CREATE TABLE [dbo].[Db_BaseLogSet_Db_ExceptionLog] (
    [code] nvarchar(50)  NOT NULL,
    [stackTrace] nvarchar(max)  NULL,
    [codeString] nvarchar(200)  NOT NULL,
    [Id] bigint  NOT NULL
);
GO

-- Creating table 'Db_BaseLogSet_Db_UserLog'
CREATE TABLE [dbo].[Db_BaseLogSet_Db_UserLog] (
    [code] nvarchar(50)  NOT NULL,
    [userId] nvarchar(50)  NOT NULL,
    [Id] bigint  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Db_BaseUserSet'
ALTER TABLE [dbo].[Db_BaseUserSet]
ADD CONSTRAINT [PK_Db_BaseUserSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Db_BaseLogSet'
ALTER TABLE [dbo].[Db_BaseLogSet]
ADD CONSTRAINT [PK_Db_BaseLogSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Db_BaseUserSet_Db_ManagerUser'
ALTER TABLE [dbo].[Db_BaseUserSet_Db_ManagerUser]
ADD CONSTRAINT [PK_Db_BaseUserSet_Db_ManagerUser]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Db_BaseLogSet_Db_ExceptionLog'
ALTER TABLE [dbo].[Db_BaseLogSet_Db_ExceptionLog]
ADD CONSTRAINT [PK_Db_BaseLogSet_Db_ExceptionLog]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Db_BaseLogSet_Db_UserLog'
ALTER TABLE [dbo].[Db_BaseLogSet_Db_UserLog]
ADD CONSTRAINT [PK_Db_BaseLogSet_Db_UserLog]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Id] in table 'Db_BaseUserSet_Db_ManagerUser'
ALTER TABLE [dbo].[Db_BaseUserSet_Db_ManagerUser]
ADD CONSTRAINT [FK_Db_ManagerUser_inherits_Db_BaseUser]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Db_BaseUserSet]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Db_BaseLogSet_Db_ExceptionLog'
ALTER TABLE [dbo].[Db_BaseLogSet_Db_ExceptionLog]
ADD CONSTRAINT [FK_Db_ExceptionLog_inherits_Db_BaseLog]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Db_BaseLogSet]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Db_BaseLogSet_Db_UserLog'
ALTER TABLE [dbo].[Db_BaseLogSet_Db_UserLog]
ADD CONSTRAINT [FK_Db_UserLog_inherits_Db_BaseLog]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Db_BaseLogSet]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------