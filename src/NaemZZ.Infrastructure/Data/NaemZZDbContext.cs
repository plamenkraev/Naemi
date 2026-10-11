using Microsoft.EntityFrameworkCore;
using NaemZZ.Core.Entities;
using NaemZZ.Infrastructure.Configurations;

namespace NaemZZ.Infrastructure.Data;

public class NaemZZDbContext : DbContext
{
    public NaemZZDbContext(DbContextOptions<NaemZZDbContext> options) : base(options)
    {
    }

    public DbSet<NAEMODATELI> NAEMODATELIs => Set<NAEMODATELI>();

    public DbSet<IMOTI> IMOTIs => Set<IMOTI>();

    public DbSet<ContractTemplate> ContractTemplates => Set<ContractTemplate>();

    public DbSet<FIRM> FIRM => Set<FIRM>();

    public DbSet<EK_OBL> EK_OBL => Set<EK_OBL>();

    public DbSet<EK_OBST> EK_OBST => Set<EK_OBST>();

    public DbSet<EKATTE> EKATTE => Set<EKATTE>();

    public DbSet<CATEGORI> CATEGORI => Set<CATEGORI>();

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
