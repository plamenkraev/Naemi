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
        return mapper.Map<List<FirmDto>>(await db.FIRM.AsNoTracking().OrderBy(x => x.firmNAME).ToListAsync(ct));
    }
    public async Task<FirmDto> SaveAsync(FirmDto firm, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(firm.firmNAME))
        {
            throw new ArgumentException("Името на фирмата е задължително.");
        }

        await using var db = await factory.CreateDbContextAsync(ct);

        if (firm.ContractTemplateId is int templateId && !await db.ContractTemplates.AnyAsync(x => x.Id == templateId, ct))
        {
            throw new ArgumentException("Невалиден шаблон.");
        }

        if (firm.firmID == 0)
        {
            var created = mapper.Map<FIRM>(firm);
            db.FIRM.Add(created);
            await db.SaveChangesAsync(ct);
            firm.firmID = created.firmID;
        }
        else
        {
            var stored = await db.FIRM.SingleAsync(x => x.firmID == firm.firmID, ct);
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
}
