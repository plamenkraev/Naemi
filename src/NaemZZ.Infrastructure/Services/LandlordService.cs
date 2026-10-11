using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NaemZZ.Core.Dto;
using NaemZZ.Core.Entities;
using NaemZZ.Core.Interfaces;
using NaemZZ.Core.Validation;
using NaemZZ.Infrastructure.Data;

namespace NaemZZ.Infrastructure.Services;

/// <summary>Stateless persistence. No selected record, dirty flags or deletion queues.</summary>
public sealed class LandlordService(IDbContextFactory<NaemZZDbContext> factory, IMapper mapper) : ILandlordService
{
    public async Task<IReadOnlyList<LandlordListDto>> GetListAsync(string? searchText = null, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        var query = db.NAEMODATELIs.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var pattern = searchText.Trim();
            query = query.Where(x => x.NAME != null && x.NAME.Contains(pattern));
        }
        return await query.OrderBy(x => x.NAME)
            .Select(x => new LandlordListDto
            {
                ID = x.ID, NAME = x.NAME, SURNAME = x.SURNAME,
                LASTNAME = x.LASTNAME, PropertyCount = x.IMOTIs.Count
            }).ToListAsync(ct);
    }

    public async Task<LandlordDto?> GetAsync(int id, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        var entity = await db.NAEMODATELIs.AsNoTracking()
            .Include(x => x.IMOTIs).AsSplitQuery()
            .SingleOrDefaultAsync(x => x.ID == id, ct);
        return entity is null ? null : mapper.Map<LandlordDto>(entity);
    }

    public async Task<Landlord?> GetForContractAsync(int id, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        return await db.NAEMODATELIs.AsNoTracking()
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
        var entity = mapper.Map<Landlord>(dto);
        db.NAEMODATELIs.Add(entity);
        var created = new List<Property>();
        foreach (var p in dto.IMOTIs)
        {
            var child = mapper.Map<Property>(p);
            child.Landlord = entity;
            db.IMOTIs.Add(child);
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
        var entity = await db.NAEMODATELIs.SingleAsync(x => x.ID == dto.ID, ct);
        mapper.Map(dto, entity);
        var stored = await db.IMOTIs.Where(x => x.LandlordId == dto.ID).ToListAsync(ct);
        var incomingIds = dto.IMOTIs.Where(x => x.Id != 0).Select(x => x.Id).ToHashSet();
        db.IMOTIs.RemoveRange(stored.Where(x => !incomingIds.Contains(x.Id)));
        var byId = stored.ToDictionary(x => x.Id);
        var additions = new List<(PropertyDto dto, Property entity)>();
        foreach (var property in dto.IMOTIs)
        {
            if (property.Id == 0)
            {
                var child = mapper.Map<Property>(property);
                child.Landlord = entity;
                db.IMOTIs.Add(child);
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
        var children = await db.IMOTIs.Where(x => x.LandlordId == id).ToListAsync(ct);
        db.IMOTIs.RemoveRange(children);
        var entity = await db.NAEMODATELIs.SingleOrDefaultAsync(x => x.ID == id, ct);
        if (entity is not null) db.NAEMODATELIs.Remove(entity);
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
    }
}
