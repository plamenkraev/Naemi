using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NaemZZ.Core.Dto;
using NaemZZ.Core.Entities;
using NaemZZ.Core.Interfaces;
using NaemZZ.Core.Validation;
using NaemZZ.Infrastructure.Data;
using System.Extensions.Collections.Generic;

namespace NaemZZ.Infrastructure.Services;

/// <summary>Stateless persistence. No selected record, dirty flags or deletion queues.</summary>
public sealed class LandlordService(IDbContextFactory<NaemZZDbContext> factory, IMapper mapper) : ILandlordService
{
    public async Task<IReadOnlyList<LandlordListDto>> GetListAsync( string? searchText = null, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);

        var landlords = await db.Landlords
            .AsNoTracking()
            .OrderBy(x => x.NAME)
            .Select(x => new LandlordListDto
            {
                ID = x.ID,
                NAME = x.NAME,
                IsCompany = x.IsCompany,
                LegalName = x.CompanyName,
                SURNAME = x.SURNAME,
                LASTNAME = x.LASTNAME,
                CompanyId = x.FIRM_ID,
                PropertyCount = x.IMOTIs.Count
            })
            .ToListAsync(ct);

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var terms = searchText.Split(
             ' ',
             StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            landlords = landlords
                .Where(x =>
                {
                    var fullName = x.IsCompany
                        ? $"{x.CompanyName} {x.EIK} {x.MOL}"
                        : $"{x.NAME} {x.SURNAME} {x.LASTNAME}";

                    return terms.All(term =>
                        fullName.Contains(
                            term,
                            StringComparison.CurrentCultureIgnoreCase));
                })
                .ToList();
        }

        return landlords;
    }

    public async Task<(int LandlordCount, int PropertyCount, decimal PropertyAmount)> GetInfoAsync(int? companyId = null, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        
        var entity = await db.Landlords.AsNoTracking()
            .Include(x => x.IMOTIs).AsSplitQuery()
            .WhereIIf(companyId.HasValue, x=> x.FIRM_ID == companyId)
            .GroupBy(x => 1)
            .Select(g => new 
            { 
                LandlordCount = g.Count(),
                PropertyCount = g.Sum(x => x.IMOTIs.Count),
                PropertyAmount = g.Sum(x => x.IMOTIs.Sum(s=> s.PLOST)),
            })
            .SingleOrDefaultAsync(ct);

        return entity is null ? (0, 0, 0) : (entity.LandlordCount, entity.PropertyCount, entity.PropertyAmount.GetValueOrDefault());
    }

    public async Task<LandlordDto?> GetAsync(int id, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        var entity = await db.Landlords.AsNoTracking()
            .Include(x => x.IMOTIs).AsSplitQuery()
            .SingleOrDefaultAsync(x => x.ID == id, ct);
        return entity is null ? null : mapper.Map<LandlordDto>(entity);
    }

    public async Task<Landlord?> GetForContractAsync(int id, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);

        return await db.Landlords
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.Region)
            .Include(x => x.Municipality)
            .Include(x => x.Settlement)
            .Include(x => x.IMOTIs).ThenInclude(x => x.PropertyCategory)
            .Include(x => x.IMOTIs).ThenInclude(x => x.Region)
            .Include(x => x.IMOTIs).ThenInclude(x => x.Municipality)
            .Include(x => x.IMOTIs).ThenInclude(x => x.Settlement)
            .Include(x => x.Company)
            .SingleOrDefaultAsync(x => x.ID == id, ct);
    }

    private static void Validate(LandlordDto dto, IMapper mapper)
    {
        if (BulgarianValidation.ValidateLandlord(mapper.Map<Landlord>(dto)).Count != 0 ||
            dto.IMOTIs.Any(p => BulgarianValidation.ValidateProperty(mapper.Map<Property>(p)).Count != 0))
            throw new ArgumentException("Невалидни данни за наемодател или имот.");
    }

    public async Task<LandlordDto> CreateAsync(LandlordDto dto, CancellationToken ct = default)
    {
        if (dto.ID != 0) throw new ArgumentException("Expected a new landlord.", nameof(dto));
        Validate(dto, mapper);
        await using var db = await factory.CreateDbContextAsync(ct);
        if (dto.ContractTemplateId is int templateId && !await db.ContractTemplates.AnyAsync(x => x.Id == templateId, ct))
            throw new ArgumentException("Невалиден шаблон за договор.");
        var entity = mapper.Map<Landlord>(dto);
        db.Landlords.Add(entity);
        var created = new List<Property>();
        foreach (var p in dto.IMOTIs)
        {
            var child = mapper.Map<Property>(p);
            child.Landlord = entity;
            db.Properties.Add(child);
            created.Add(child);
        }
        await db.SaveChangesAsync(ct);
        dto.ID = entity.ID;
        var children = created;
        // IDs of added children are returned by EF after SaveChanges.
        for (var i = 0; i < dto.IMOTIs.Count; i++)
        {
            dto.IMOTIs[i].Id = children[i].Id;
            dto.IMOTIs[i].LandlordId = entity.ID;
        }
        return dto;
    }

    public async Task UpdateAsync(LandlordDto dto, CancellationToken ct = default)
    {
        if (dto.ID <= 0) throw new ArgumentException("Expected an existing landlord.", nameof(dto));
        Validate(dto, mapper);
        await using var db = await factory.CreateDbContextAsync(ct);
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        if (dto.ContractTemplateId is int templateId && !await db.ContractTemplates.AnyAsync(x => x.Id == templateId, ct))
            throw new ArgumentException("Невалиден шаблон за договор.");
        var entity = await db.Landlords.SingleAsync(x => x.ID == dto.ID, ct);
        mapper.Map(dto, entity);
        var stored = await db.Properties.Where(x => x.LandlordId == dto.ID).ToListAsync(ct);
        var incomingIds = dto.IMOTIs.Where(x => x.Id != 0).Select(x => x.Id).ToHashSet();
        db.Properties.RemoveRange(stored.Where(x => !incomingIds.Contains(x.Id)));
        var byId = stored.ToDictionary(x => x.Id);
        var additions = new List<(PropertyDto dto, Property entity)>();
        foreach (var property in dto.IMOTIs)
        {
            if (property.Id == 0)
            {
                var child = mapper.Map<Property>(property);
                child.Landlord = entity;
                db.Properties.Add(child);
                additions.Add((property, child));
            }
            else
            {
                if (!byId.TryGetValue(property.Id, out var child))
                    throw new InvalidOperationException("Property does not belong to the selected landlord.");
                mapper.Map(property, child);
            }
        }
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        foreach (var (source, child) in additions)
        {
            source.Id = child.Id;
            source.LandlordId = dto.ID;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0) return;
        await using var db = await factory.CreateDbContextAsync(ct);
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        var children = await db.Properties.Where(x => x.LandlordId == id).ToListAsync(ct);
        db.Properties.RemoveRange(children);
        var entity = await db.Landlords.SingleOrDefaultAsync(x => x.ID == id, ct);
        if (entity is not null) db.Landlords.Remove(entity);
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
    }
}
