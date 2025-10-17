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
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251017220026_InitialOtpCreate'
)
BEGIN
    CREATE TABLE [OtpEntries] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWID()),
        [UserId] nvarchar(256) NOT NULL,
        [OtpCode] nvarchar(10) NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [IsUsed] bit NOT NULL DEFAULT CAST(0 AS bit),
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [RecipientEmail] nvarchar(256) NULL,
        CONSTRAINT [PK_OtpEntries] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251017220026_InitialOtpCreate'
)
BEGIN
    CREATE INDEX [IX_OtpEntries_ExpiresAt] ON [OtpEntries] ([ExpiresAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251017220026_InitialOtpCreate'
)
BEGIN
    CREATE INDEX [IX_OtpEntries_UserId_OtpCode] ON [OtpEntries] ([UserId], [OtpCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251017220026_InitialOtpCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251017220026_InitialOtpCreate', N'8.0.20');
END;
GO

COMMIT;
GO

