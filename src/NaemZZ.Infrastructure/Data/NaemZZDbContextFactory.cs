using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NaemZZ.Infrastructure.Data;

/// <summary>
/// Lets EF Core design-time tools (`dotnet ef migrations add ...`) create a DbContext
/// without needing the full Desktop host/DI container. Uses a local throwaway file so
/// `dotnet ef` never touches a real customer database.
/// </summary>
public class NaemZZDbContextFactory : IDesignTimeDbContextFactory<NaemZZDbContext>
{
    public NaemZZDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<NaemZZDbContext>();
        builder.UseSqlite("Data Source=naemzz.design.db");
        return new NaemZZDbContext(builder.Options);
    }
}
