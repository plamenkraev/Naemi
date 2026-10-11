using NaemZZ.Core.Entities;
using NaemZZ.Core.Dto;
namespace NaemZZ.Core.Interfaces;
public interface IFirmManagementService
{
    Task<IReadOnlyList<FirmDto>> ListAsync(CancellationToken ct = default);
    Task<FirmDto> SaveAsync(FirmDto firm, CancellationToken ct = default);
    Task<IReadOnlyList<ContractTemplate>> ListTemplatesAsync(CancellationToken ct = default);
    Task<ContractTemplate> SaveTemplateAsync(ContractTemplate template, CancellationToken ct = default);
}
