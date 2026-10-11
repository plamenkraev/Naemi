using NaemZZ.Core.Dto;
using NaemZZ.Core.Entities;
namespace NaemZZ.Core.Interfaces;
public interface ILandlordService : IAsyncDisposable
{
    IReadOnlyList<LandlordDto> Landlords { get; }
    Task LoadAsync(string? searchText = null, CancellationToken ct = default);
    LandlordDto AddNew();
    Task<NAEMODATELI?> GetForContractAsync(int id, CancellationToken ct = default);
    void Delete(LandlordDto landlord);
    PropertyDto AddProperty(LandlordDto landlord);
    void DeleteProperty(LandlordDto landlord, PropertyDto property);
    void RestorePropertyDeletions(IEnumerable<int> ids);
    Task<SaveResult> SaveChangesAsync(CancellationToken ct = default);
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
