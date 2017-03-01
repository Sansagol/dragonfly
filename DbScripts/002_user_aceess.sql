/* ---------------------------------------------------- */
/*  Generated by Enterprise Architect Version 13.0 		*/
/*  Created On : 24-���-2017 21:44:10 				*/
/*  DBMS       : SQL Server 2012 						*/
/* ---------------------------------------------------- */

/* Drop Foreign Key Constraints */

IF EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = object_id(N'[FK_User_Access_User]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1) 
ALTER TABLE [User_Access] DROP CONSTRAINT [FK_User_Access_User]
GO

/* Drop Tables */

IF EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = object_id(N'[User_Access]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1) 
DROP TABLE [User_Access]
GO

/* Create Tables */

CREATE TABLE [User_Access]
(
	[ID_User_Access] numeric(18) NOT NULL IDENTITY (1, 1),
	[Access_Token] nvarchar(128) NOT NULL,    -- ����� ������� ������������ � �������
	[Date_Creation] datetime2(7) NOT NULL,    -- ���� �������� ������
	[Date_Expiration] datetime2(7) NOT NULL,    -- ����, �� ������� ��������� �����.
	[ID_User] numeric(18) NOT NULL    -- ������������, ��� �������� ������������ �����.
)
GO

/* Create Primary Keys, Indexes, Uniques, Checks */

ALTER TABLE [User_Access] 
 ADD CONSTRAINT [PK_User_Access]
	PRIMARY KEY CLUSTERED ([ID_User_Access] ASC)
GO

ALTER TABLE [User_Access] 
 ADD CONSTRAINT [UK_Access_Token] UNIQUE NONCLUSTERED ([Access_Token] ASC)
GO

CREATE NONCLUSTERED INDEX [IXFK_User_Access_User] 
 ON [User_Access] ([ID_User] ASC)
GO

/* Create Foreign Key Constraints */

ALTER TABLE [User_Access] ADD CONSTRAINT [FK_User_Access_User]
	FOREIGN KEY ([ID_User]) REFERENCES [User] ([ID_User]) ON DELETE Cascade ON UPDATE Cascade
GO

/* Create Table Comments */


if exists (select * from ::fn_listextendedproperty ('MS_Description', 'SCHEMA', 'dbo', 'table', 'User_Access', NULL, NULL)) 
begin 
  EXEC sys.sp_updateextendedproperty 'MS_Description', '������� ������� ������������� � �������. ������ ���� ������� ��� ���������� �������������.', 'SCHEMA', 'dbo', 'table', 'User_Access' 
end 
else 
begin 
  EXEC sys.sp_addextendedproperty 'MS_Description', '������� ������� ������������� � �������. ������ ���� ������� ��� ���������� �������������.', 'SCHEMA', 'dbo', 'table', 'User_Access' 
end

EXEC sp_addextendedproperty 'MS_Description', '����� ������� ������������ � �������', 'Schema', [dbo], 'table', [User_Access], 'column', [Access_Token]
GO

EXEC sp_addextendedproperty 'MS_Description', '���� �������� ������', 'Schema', [dbo], 'table', [User_Access], 'column', [Date_Creation]
GO

EXEC sp_addextendedproperty 'MS_Description', '����, �� ������� ��������� �����.', 'Schema', [dbo], 'table', [User_Access], 'column', [Date_Expiration]
GO

EXEC sp_addextendedproperty 'MS_Description', '������������, ��� �������� ������������ �����.', 'Schema', [dbo], 'table', [User_Access], 'column', [ID_User]
GO