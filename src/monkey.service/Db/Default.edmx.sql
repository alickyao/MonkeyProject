
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 05/03/2017 15:20:32
-- Generated from EDMX file: D:\project\git\MonkeyProject\src\monkey.service\Db\Default.edmx
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

IF OBJECT_ID(N'[dbo].[FK_Db_BaseUserDb_BaseUserRole]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Db_BaseUserRoleSet] DROP CONSTRAINT [FK_Db_BaseUserDb_BaseUserRole];
GO
IF OBJECT_ID(N'[dbo].[FK_Db_BaseDocDb_BaseDocFile]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Db_BaseDocFileSet] DROP CONSTRAINT [FK_Db_BaseDocDb_BaseDocFile];
GO
IF OBJECT_ID(N'[dbo].[FK_Db_WorkFlowDefinitionDb_WorkFlowSetp]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Db_WorkFlowSetpSet] DROP CONSTRAINT [FK_Db_WorkFlowDefinitionDb_WorkFlowSetp];
GO
IF OBJECT_ID(N'[dbo].[FK_Db_ManagerUser_inherits_Db_BaseUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Db_BaseUserSet_Db_ManagerUser] DROP CONSTRAINT [FK_Db_ManagerUser_inherits_Db_BaseUser];
GO
IF OBJECT_ID(N'[dbo].[FK_Db_ExceptionLog_inherits_Db_BaseLog]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Db_BaseLogSet_Db_ExceptionLog] DROP CONSTRAINT [FK_Db_ExceptionLog_inherits_Db_BaseLog];
GO
IF OBJECT_ID(N'[dbo].[FK_Db_UserLog_inherits_Db_BaseLog]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Db_BaseLogSet_Db_UserLog] DROP CONSTRAINT [FK_Db_UserLog_inherits_Db_BaseLog];
GO
IF OBJECT_ID(N'[dbo].[FK_Db_DocPic_inherits_Db_BaseDoc]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Db_BaseDocSet_Db_DocPic] DROP CONSTRAINT [FK_Db_DocPic_inherits_Db_BaseDoc];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Db_BaseUserSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_BaseUserSet];
GO
IF OBJECT_ID(N'[dbo].[Db_BaseLogSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_BaseLogSet];
GO
IF OBJECT_ID(N'[dbo].[Db_BaseUserRoleSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_BaseUserRoleSet];
GO
IF OBJECT_ID(N'[dbo].[Db_BaseTreeSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_BaseTreeSet];
GO
IF OBJECT_ID(N'[dbo].[Db_BaseDocSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_BaseDocSet];
GO
IF OBJECT_ID(N'[dbo].[Db_BaseFileSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_BaseFileSet];
GO
IF OBJECT_ID(N'[dbo].[Db_BaseDocFileSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_BaseDocFileSet];
GO
IF OBJECT_ID(N'[dbo].[Db_WorkFlowRoleSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_WorkFlowRoleSet];
GO
IF OBJECT_ID(N'[dbo].[Db_WorkFlowRoleDescriptSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_WorkFlowRoleDescriptSet];
GO
IF OBJECT_ID(N'[dbo].[Db_WorkFlowSetpSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_WorkFlowSetpSet];
GO
IF OBJECT_ID(N'[dbo].[Db_WorkFlowDefinitionSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_WorkFlowDefinitionSet];
GO
IF OBJECT_ID(N'[dbo].[Db_BaseUserSet_Db_ManagerUser]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_BaseUserSet_Db_ManagerUser];
GO
IF OBJECT_ID(N'[dbo].[Db_BaseLogSet_Db_ExceptionLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_BaseLogSet_Db_ExceptionLog];
GO
IF OBJECT_ID(N'[dbo].[Db_BaseLogSet_Db_UserLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_BaseLogSet_Db_UserLog];
GO
IF OBJECT_ID(N'[dbo].[Db_BaseDocSet_Db_DocPic]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Db_BaseDocSet_Db_DocPic];
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

-- Creating table 'Db_BaseUserRoleSet'
CREATE TABLE [dbo].[Db_BaseUserRoleSet] (
    [Id] nvarchar(50)  NOT NULL,
    [roleName] nvarchar(max)  NOT NULL,
    [Db_BaseUserId] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Db_BaseTreeSet'
CREATE TABLE [dbo].[Db_BaseTreeSet] (
    [Id] nvarchar(50)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [ParentId] nvarchar(50)  NULL,
    [CreatedOn] datetime  NOT NULL,
    [Code] nvarchar(50)  NOT NULL,
    [Seq] int  NOT NULL
);
GO

-- Creating table 'Db_BaseDocSet'
CREATE TABLE [dbo].[Db_BaseDocSet] (
    [Id] nvarchar(50)  NOT NULL,
    [DocType] int  NOT NULL,
    [Caption] nvarchar(max)  NOT NULL,
    [Code] nvarchar(50)  NULL,
    [TreeId] nvarchar(50)  NULL,
    [CreatedOn] datetime  NOT NULL,
    [Seq] int  NOT NULL,
    [IsDeleted] bit  NOT NULL,
    [IsDisabled] bit  NOT NULL
);
GO

-- Creating table 'Db_BaseFileSet'
CREATE TABLE [dbo].[Db_BaseFileSet] (
    [Id] nvarchar(50)  NOT NULL,
    [Path] nvarchar(max)  NOT NULL,
    [FileName] nvarchar(max)  NOT NULL,
    [ExName] nvarchar(50)  NULL,
    [CreatedOn] datetime  NOT NULL
);
GO

-- Creating table 'Db_BaseDocFileSet'
CREATE TABLE [dbo].[Db_BaseDocFileSet] (
    [Id] nvarchar(50)  NOT NULL,
    [FileId] nvarchar(50)  NOT NULL,
    [Seq] int  NOT NULL,
    [Caption] nvarchar(max)  NULL,
    [Descript] nvarchar(max)  NULL,
    [CreatedOn] datetime  NOT NULL,
    [Db_BaseDocId] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Db_WorkFlowRoleSet'
CREATE TABLE [dbo].[Db_WorkFlowRoleSet] (
    [Id] nvarchar(50)  NOT NULL,
    [RoleName] nvarchar(100)  NOT NULL,
    [Descript] nvarchar(max)  NULL,
    [CreatedOn] datetime  NOT NULL
);
GO

-- Creating table 'Db_WorkFlowRoleDescriptSet'
CREATE TABLE [dbo].[Db_WorkFlowRoleDescriptSet] (
    [WorkFlowRoleId] nvarchar(50)  NOT NULL,
    [UserId] nvarchar(50)  NOT NULL,
    [CreatedOn] datetime  NOT NULL
);
GO

-- Creating table 'Db_WorkFlowDefSetpSet'
CREATE TABLE [dbo].[Db_WorkFlowDefSetpSet] (
    [Id] nvarchar(50)  NOT NULL,
    [Seq] int  NOT NULL,
    [Caption] nvarchar(100)  NOT NULL,
    [WFConfirmRoleId] nvarchar(50)  NOT NULL,
    [Db_WorkFlowDefinitionId] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Db_WorkFlowDefinitionSet'
CREATE TABLE [dbo].[Db_WorkFlowDefinitionSet] (
    [Id] nvarchar(50)  NOT NULL,
    [Caption] nvarchar(100)  NOT NULL,
    [Descript] nvarchar(max)  NULL,
    [CreatedOn] datetime  NOT NULL
);
GO

-- Creating table 'Db_WorkFlowDefLineSet'
CREATE TABLE [dbo].[Db_WorkFlowDefLineSet] (
    [Id] nvarchar(50)  NOT NULL,
    [FromId] nvarchar(50)  NOT NULL,
    [ToId] nvarchar(50)  NOT NULL,
    [Caption] nvarchar(100)  NOT NULL,
    [Descript] nvarchar(max)  NULL,
    [Db_WorkFlowDefinitionId] nvarchar(50)  NOT NULL
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
    [userName] nvarchar(500)  NOT NULL,
    [fkId] nvarchar(50)  NULL,
    [fkName] nvarchar(500)  NULL,
    [Id] bigint  NOT NULL
);
GO

-- Creating table 'Db_BaseDocSet_Db_DocPic'
CREATE TABLE [dbo].[Db_BaseDocSet_Db_DocPic] (
    [Descript] nvarchar(max)  NULL,
    [Content] nvarchar(max)  NULL,
    [Id] nvarchar(50)  NOT NULL
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

-- Creating primary key on [Id] in table 'Db_BaseUserRoleSet'
ALTER TABLE [dbo].[Db_BaseUserRoleSet]
ADD CONSTRAINT [PK_Db_BaseUserRoleSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Db_BaseTreeSet'
ALTER TABLE [dbo].[Db_BaseTreeSet]
ADD CONSTRAINT [PK_Db_BaseTreeSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Db_BaseDocSet'
ALTER TABLE [dbo].[Db_BaseDocSet]
ADD CONSTRAINT [PK_Db_BaseDocSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Db_BaseFileSet'
ALTER TABLE [dbo].[Db_BaseFileSet]
ADD CONSTRAINT [PK_Db_BaseFileSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Db_BaseDocFileSet'
ALTER TABLE [dbo].[Db_BaseDocFileSet]
ADD CONSTRAINT [PK_Db_BaseDocFileSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Db_WorkFlowRoleSet'
ALTER TABLE [dbo].[Db_WorkFlowRoleSet]
ADD CONSTRAINT [PK_Db_WorkFlowRoleSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [WorkFlowRoleId], [UserId] in table 'Db_WorkFlowRoleDescriptSet'
ALTER TABLE [dbo].[Db_WorkFlowRoleDescriptSet]
ADD CONSTRAINT [PK_Db_WorkFlowRoleDescriptSet]
    PRIMARY KEY CLUSTERED ([WorkFlowRoleId], [UserId] ASC);
GO

-- Creating primary key on [Id] in table 'Db_WorkFlowDefSetpSet'
ALTER TABLE [dbo].[Db_WorkFlowDefSetpSet]
ADD CONSTRAINT [PK_Db_WorkFlowDefSetpSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Db_WorkFlowDefinitionSet'
ALTER TABLE [dbo].[Db_WorkFlowDefinitionSet]
ADD CONSTRAINT [PK_Db_WorkFlowDefinitionSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Db_WorkFlowDefLineSet'
ALTER TABLE [dbo].[Db_WorkFlowDefLineSet]
ADD CONSTRAINT [PK_Db_WorkFlowDefLineSet]
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

-- Creating primary key on [Id] in table 'Db_BaseDocSet_Db_DocPic'
ALTER TABLE [dbo].[Db_BaseDocSet_Db_DocPic]
ADD CONSTRAINT [PK_Db_BaseDocSet_Db_DocPic]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Db_BaseUserId] in table 'Db_BaseUserRoleSet'
ALTER TABLE [dbo].[Db_BaseUserRoleSet]
ADD CONSTRAINT [FK_Db_BaseUserDb_BaseUserRole]
    FOREIGN KEY ([Db_BaseUserId])
    REFERENCES [dbo].[Db_BaseUserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Db_BaseUserDb_BaseUserRole'
CREATE INDEX [IX_FK_Db_BaseUserDb_BaseUserRole]
ON [dbo].[Db_BaseUserRoleSet]
    ([Db_BaseUserId]);
GO

-- Creating foreign key on [Db_BaseDocId] in table 'Db_BaseDocFileSet'
ALTER TABLE [dbo].[Db_BaseDocFileSet]
ADD CONSTRAINT [FK_Db_BaseDocDb_BaseDocFile]
    FOREIGN KEY ([Db_BaseDocId])
    REFERENCES [dbo].[Db_BaseDocSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Db_BaseDocDb_BaseDocFile'
CREATE INDEX [IX_FK_Db_BaseDocDb_BaseDocFile]
ON [dbo].[Db_BaseDocFileSet]
    ([Db_BaseDocId]);
GO

-- Creating foreign key on [Db_WorkFlowDefinitionId] in table 'Db_WorkFlowDefSetpSet'
ALTER TABLE [dbo].[Db_WorkFlowDefSetpSet]
ADD CONSTRAINT [FK_Db_WorkFlowDefinitionDb_WorkFlowSetp]
    FOREIGN KEY ([Db_WorkFlowDefinitionId])
    REFERENCES [dbo].[Db_WorkFlowDefinitionSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Db_WorkFlowDefinitionDb_WorkFlowSetp'
CREATE INDEX [IX_FK_Db_WorkFlowDefinitionDb_WorkFlowSetp]
ON [dbo].[Db_WorkFlowDefSetpSet]
    ([Db_WorkFlowDefinitionId]);
GO

-- Creating foreign key on [Db_WorkFlowDefinitionId] in table 'Db_WorkFlowDefLineSet'
ALTER TABLE [dbo].[Db_WorkFlowDefLineSet]
ADD CONSTRAINT [FK_Db_WorkFlowDefinitionDb_WorkFlowLine]
    FOREIGN KEY ([Db_WorkFlowDefinitionId])
    REFERENCES [dbo].[Db_WorkFlowDefinitionSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Db_WorkFlowDefinitionDb_WorkFlowLine'
CREATE INDEX [IX_FK_Db_WorkFlowDefinitionDb_WorkFlowLine]
ON [dbo].[Db_WorkFlowDefLineSet]
    ([Db_WorkFlowDefinitionId]);
GO

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

-- Creating foreign key on [Id] in table 'Db_BaseDocSet_Db_DocPic'
ALTER TABLE [dbo].[Db_BaseDocSet_Db_DocPic]
ADD CONSTRAINT [FK_Db_DocPic_inherits_Db_BaseDoc]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Db_BaseDocSet]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------