using Microsoft.EntityFrameworkCore;
using NaemZZ.Core.Entities;
using NaemZZ.Infrastructure.Configurations;

namespace NaemZZ.Infrastructure.Data;

public class NaemZZDbContext : DbContext
{
    public NaemZZDbContext(DbContextOptions<NaemZZDbContext> options) : base(options)
    {
    }

    public DbSet<Landlord> NAEMODATELIs => Set<Landlord>();

    public DbSet<Property> IMOTIs => Set<Property>();

    public DbSet<ContractTemplate> ContractTemplates => Set<ContractTemplate>();

    public DbSet<Company> Company => Set<Company>();

    public DbSet<Region> Region => Set<Region>();

    public DbSet<Municipality> Municipality => Set<Municipality>();

    public DbSet<Settlement> Settlement => Set<Settlement>();

    public DbSet<PropertyCategory> PropertyCategory => Set<PropertyCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new NaemodateliConfiguration());
        modelBuilder.ApplyConfiguration(new ImotiConfiguration());
        modelBuilder.ApplyConfiguration(new FirmConfiguration());
        modelBuilder.Entity<ContractTemplate>(b => { b.ToTable("ContractTemplates"); b.HasKey(x => x.Id); b.Property(x => x.Name).HasMaxLength(160).IsRequired(); b.Property(x => x.LegacyRtfFile).HasMaxLength(128); });
        modelBuilder.ApplyConfiguration(new EkOblConfiguration());
        modelBuilder.ApplyConfiguration(new EkObstConfiguration());
        modelBuilder.ApplyConfiguration(new EkatteConfiguration());
        modelBuilder.ApplyConfiguration(new CategoriConfiguration());
    }
}
