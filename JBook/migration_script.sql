IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250606073507_mssql.local_migration_773'
)
BEGIN
    CREATE TABLE [Books] (
        [bookId] int NOT NULL IDENTITY,
        [title] nvarchar(max) NULL,
        [author] nvarchar(max) NULL,
        [publisher] nvarchar(max) NULL,
        [category] nvarchar(max) NULL,
        [description] nvarchar(max) NULL,
        [ISBN] nvarchar(max) NULL,
        [Url] nvarchar(max) NULL,
        CONSTRAINT [PK_Books] PRIMARY KEY ([bookId])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250606073507_mssql.local_migration_773'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'bookId', N'ISBN', N'Url', N'author', N'category', N'description', N'publisher', N'title') AND [object_id] = OBJECT_ID(N'[Books]'))
        SET IDENTITY_INSERT [Books] ON;
    EXEC(N'INSERT INTO [Books] ([bookId], [ISBN], [Url], [author], [category], [description], [publisher], [title])
    VALUES (1, N''9787552220094'', NULL, N''Wu Sun'', NULL, NULL, NULL, N''Sun Zi''''s Art of War''),
    (2, N''9787530221532'', NULL, N''Hua Yu'', NULL, NULL, NULL, N''Alive'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'bookId', N'ISBN', N'Url', N'author', N'category', N'description', N'publisher', N'title') AND [object_id] = OBJECT_ID(N'[Books]'))
        SET IDENTITY_INSERT [Books] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250606073507_mssql.local_migration_773'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250606073507_mssql.local_migration_773', N'9.0.5');
END;

COMMIT;
GO

