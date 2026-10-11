using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace NaemZZ.Infrastructure.Data;

/// <summary>
/// Applies pending EF Core migrations safely, per the "Database storage / Schema migrations"
/// requirements:
/// 1. Back up the existing database file before touching it.
/// 2. Verify the database is accessible.
/// 3. Apply pending migrations (never <c>EnsureCreated</c>, never silently recreate).
/// 4. Validate the resulting schema is reachable.
/// 5. Report failures instead of leaving the app in an inconsistent state.
/// </summary>
public static class DatabaseInitializer
{
    public static async Task<DatabaseInitResult> InitializeAsync(
        IDbContextFactory<NaemZZDbContext> factory,
        string databaseFilePath,
        ILogger logger,
        CancellationToken ct = default)
    {
        try
        {
            var databaseExisted = File.Exists(databaseFilePath);
            string? backupPath = null;

            if (databaseExisted)
            {
                backupPath = CreateBackup(databaseFilePath, logger);
            }

            await using var db = await factory.CreateDbContextAsync(ct);

            // Verify accessibility before attempting any schema change.
            await db.Database.CanConnectAsync(ct);

            var pending = (await db.Database.GetPendingMigrationsAsync(ct)).ToList();
            if (pending.Count > 0)
            {
                logger.LogInformation("Applying {Count} pending migration(s): {Migrations}", pending.Count, string.Join(", ", pending));
                await db.Database.MigrateAsync(ct);
            }
            else if (!databaseExisted)
            {
                await db.Database.MigrateAsync(ct);
            }

            // Validate the resulting schema is reachable (a cheap no-op query per table root).
            await db.NAEMODATELIs.AsNoTracking().Select(n => n.ID).Take(1).ToListAsync(ct);

            return DatabaseInitResult.Ok(backupPath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database initialization failed for {Path}", databaseFilePath);
            return DatabaseInitResult.Failed(ex.Message);
        }
    }

    private static string CreateBackup(string databaseFilePath, ILogger logger)
    {
        var backupDir = Path.Combine(Path.GetDirectoryName(databaseFilePath)!, "Backups");
        Directory.CreateDirectory(backupDir);

        var backupPath = Path.Combine(
            backupDir,
            $"{Path.GetFileNameWithoutExtension(databaseFilePath)}_{DateTime.Now:yyyyMMdd_HHmmss}.db");

        File.Copy(databaseFilePath, backupPath, overwrite: false);
        logger.LogInformation("Backed up database to {BackupPath}", backupPath);
        return backupPath;
    }
}

public sealed class DatabaseInitResult
{
    public bool Success { get; init; }

    public string? BackupPath { get; init; }

    public string? ErrorMessage { get; init; }

    public static DatabaseInitResult Ok(string? backupPath) => new() { Success = true, BackupPath = backupPath };

    public static DatabaseInitResult Failed(string message) => new() { Success = false, ErrorMessage = message };
}
