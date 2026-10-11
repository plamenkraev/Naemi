using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NaemZZ.Core.Dto;
using NaemZZ.Core.Entities;
using NaemZZ.Core.Interfaces;
using NaemZZ.Core.Validation;
using NaemZZ.Infrastructure.Data;

namespace NaemZZ.Infrastructure.Services;

public sealed class LandlordService(IDbContextFactory<NaemZZDbContext> factory, IMapper mapper) : ILandlordService
{
    private List<LandlordDto> _landlords = [];
    private readonly HashSet<int> _deletedLandlords = [];
    private readonly HashSet<int> _deletedProperties = [];
    public IReadOnlyList<LandlordDto> Landlords => _landlords;

    public async Task LoadAsync(string? searchText = null, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        var query = db.NAEMODATELIs.AsNoTracking().Include(x => x.IMOTIs).AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var pattern = searchText.ToLower();
            query = query.Where(x => x.NAME != null && x.NAME.ToLower().Contains(pattern));
        }
        _landlords = mapper.Map<List<LandlordDto>>(await query.OrderBy(x => x.NAME).AsSplitQuery().ToListAsync(ct));
        _deletedLandlords.Clear();
        _deletedProperties.Clear();
    }

    public async Task<NAEMODATELI?> GetForContractAsync(int id, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        return await db.NAEMODATELIs.AsNoTracking()
            .Include(x => x.IMOTIs).ThenInclude(x => x.CATEGORI)
            .Include(x => x.IMOTIs).ThenInclude(x => x.EK_OBL)
            .Include(x => x.IMOTIs).ThenInclude(x => x.EK_OBST)
            .Include(x => x.IMOTIs).ThenInclude(x => x.EKATTE)
            .Include(x => x.FIRM)
            .SingleOrDefaultAsync(x => x.ID == id, ct);
    }

    public LandlordDto AddNew()
    {
        var item = new LandlordDto { NAME = "[нов]" };
        _landlords.Add(item);
        return item;
    }

    public void Delete(LandlordDto item)
    {
        if (item.ID != 0) _deletedLandlords.Add(item.ID);
        _landlords.Remove(item);
    }

    public PropertyDto AddProperty(LandlordDto owner)
    {
        var item = new PropertyDto();
        owner.IMOTIs.Add(item);
        return item;
    }

    public void DeleteProperty(LandlordDto owner, PropertyDto item)
    {
        if (item.imotiID != 0) _deletedProperties.Add(item.imotiID);
        owner.IMOTIs.Remove(item);
    }

    public void RestorePropertyDeletions(IEnumerable<int> ids)
    {
        foreach (var id in ids) _deletedProperties.Remove(id);
    }

    public async Task<SaveResult> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var landlord in _landlords)
        {
            if (BulgarianValidation.ValidateLandlord(mapper.Map<NAEMODATELI>(landlord)).Count > 0)
                return SaveResult.Invalid(landlord);
            foreach (var property in landlord.IMOTIs)
                if (BulgarianValidation.ValidateProperty(mapper.Map<IMOTI>(property)).Count > 0)
                    return SaveResult.Invalid(landlord);
        }

        try
        {
            await using var db = await factory.CreateDbContextAsync(ct);
            await using var transaction = await db.Database.BeginTransactionAsync(ct);
            if (_deletedLandlords.Count > 0)
            {
                var removed = await db.NAEMODATELIs.Where(x => _deletedLandlords.Contains(x.ID)).ToListAsync(ct);
                db.NAEMODATELIs.RemoveRange(removed);
            }
            if (_deletedProperties.Count > 0)
            {
                var removed = await db.IMOTIs.Where(x => _deletedProperties.Contains(x.imotiID)).ToListAsync(ct);
                db.IMOTIs.RemoveRange(removed);
            }
            foreach (var dto in _landlords)
            {
                NAEMODATELI entity;
                if (dto.ID == 0)
                {
                    entity = mapper.Map<NAEMODATELI>(dto);
                    db.NAEMODATELIs.Add(entity);
                }
                else
                {
                    entity = await db.NAEMODATELIs.SingleAsync(x => x.ID == dto.ID, ct);
                    mapper.Map(dto, entity);
                }
                foreach (var property in dto.IMOTIs)
                {
                    if (property.imotiID == 0)
                    {
                        var added = mapper.Map<IMOTI>(property);
                        added.NAEMODATELI = entity;
                        db.IMOTIs.Add(added);
                    }
                    else
                    {
                        var stored = await db.IMOTIs.SingleAsync(x => x.imotiID == property.imotiID, ct);
                        mapper.Map(property, stored);
                    }
                }
            }
            await db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            await LoadAsync(null, ct);
            return SaveResult.Ok();
        }
        catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
        {
            return SaveResult.Failed(ex.InnerException?.Message ?? ex.Message);
        }
    }
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
