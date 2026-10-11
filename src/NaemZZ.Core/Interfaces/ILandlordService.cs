using NaemZZ.Core.Dto;
using NaemZZ.Core.Entities;

namespace NaemZZ.Core.Interfaces;

public interface ILandlordService
{
    Task<IReadOnlyList<LandlordListDto>> GetListAsync(string? searchText = null, CancellationToken ct = default);
    Task<LandlordDto?> GetAsync(int id, CancellationToken ct = default);
    Task<LandlordDto> CreateAsync(LandlordDto dto, CancellationToken ct = default);
    Task UpdateAsync(LandlordDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<Landlord?> GetForContractAsync(int id, CancellationToken ct = default);
}

public sealed class SaveResult
{
    public bool Success { get; init; }
    public LandlordDto? FirstInvalidLandlord { get; init; }
    public string? ErrorMessage { get; init; }
    public static SaveResult Ok() => new() { Success = true };
    public static SaveResult Invalid(LandlordDto landlord) => new() { FirstInvalidLandlord = landlord };
    public static SaveResult Failed(string message) => new() { ErrorMessage = message };
}
