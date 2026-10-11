using Microsoft.EntityFrameworkCore;
using AutoMapper;
using NaemZZ.Core.Dto;
using NaemZZ.Core.Entities;
using NaemZZ.Core.Interfaces;
using NaemZZ.Infrastructure.Data;

namespace NaemZZ.Infrastructure.Services;
public sealed class FirmManagementService(IDbContextFactory<NaemZZDbContext> factory, ILookupService lookups, IMapper mapper) : IFirmManagementService
{
    public async Task<IReadOnlyList<FirmDto>> ListAsync(CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);

        var companies = await db.Companies
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

        var statistics = await db.Landlords
            .AsNoTracking()
            .Where(x => x.FIRM_ID != null)
            .GroupBy(x => x.FIRM_ID)
            .Select(g => new
            {
                CompanyId = g.Key,
                LandlordCount = g.Count(),
                PropertyCount = g.Sum(x => x.IMOTIs.Count),
                PropertyAmount = g.Sum(x => x.IMOTIs.Sum(p => p.PLOST))
            })
            .ToDictionaryAsync(x => x.CompanyId, ct);

        var result = mapper.Map<List<FirmDto>>(companies);

        foreach (var firm in result)
        {
            if (statistics.TryGetValue(firm.Id, out var info))
            {
                firm.LandlordCount = info.LandlordCount;
                firm.PropertyCount = info.PropertyCount;
                firm.PropertyAmount = info.PropertyAmount.GetValueOrDefault();
            }
        }

        return result;
    }

    public async Task<FirmDto> SaveAsync(FirmDto firm, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(firm.Name))
        {
            throw new ArgumentException("Името на фирмата е задължително.");
        }

        await using var db = await factory.CreateDbContextAsync(ct);

        if (firm.ContractTemplateId is int templateId && !await db.ContractTemplates.AnyAsync(x => x.Id == templateId, ct))
        {
            throw new ArgumentException("Невалиден шаблон.");
        }

        if (firm.Id == 0)
        {
            var created = mapper.Map<Company>(firm);
            db.Companies.Add(created);
            await db.SaveChangesAsync(ct);
            firm.Id = created.Id;
        }
        else
        {
            var stored = await db.Companies.SingleAsync(x => x.Id == firm.Id, ct);
            mapper.Map(firm, stored);
        }
        await db.SaveChangesAsync(ct);
        lookups.InvalidateCache();

        return firm;
    }
    public async Task<IReadOnlyList<ContractTemplate>> ListTemplatesAsync(CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        return await db.ContractTemplates.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);
    }
    public async Task<ContractTemplate> SaveTemplateAsync(ContractTemplate template, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(template.Name))
        {
            throw new ArgumentException("Името на шаблона е задължително.");
        }

        await using var db = await factory.CreateDbContextAsync(ct);

        if (template.Id == 0)
        {
            db.ContractTemplates.Add(template);
        }
        else
        {
            var stored = await db.ContractTemplates.SingleAsync(x => x.Id == template.Id, ct);
            stored.Name = template.Name; stored.Html = template.Html; stored.LegacyRtfFile = template.LegacyRtfFile;
        }
        await db.SaveChangesAsync(ct);
        return template;
    }

    public async Task DeleteTemplateAsync(int id, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        await using var transaction = await db.Database.BeginTransactionAsync(ct);

        await db.Companies.Where(x => x.ContractTemplateId == id)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.ContractTemplateId, (int?)null), ct);

        var template = await db.ContractTemplates.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (template is not null)
        {
            db.ContractTemplates.Remove(template);
            await db.SaveChangesAsync(ct);
        }
        await transaction.CommitAsync(ct);
        lookups.InvalidateCache();
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        await using var transaction = await db.Database.BeginTransactionAsync(ct);

        await db.Landlords
            .Where(x => x.FIRM_ID == id)
            .ExecuteUpdateAsync(
                s => s.SetProperty(x => x.FIRM_ID, (int?)null), ct);

        var company = await db.Companies
            .SingleOrDefaultAsync(x => x.Id == id, ct);

        if (company is not null)
        {
            db.Companies.Remove(company);
            await db.SaveChangesAsync(ct);
        }

        await transaction.CommitAsync(ct);
        lookups.InvalidateCache();
    }
}
