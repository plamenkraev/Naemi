using NaemZZ.Core.Entities;
namespace NaemZZ.Core.Interfaces;
public interface IContractDocumentService
{
    Task<ContractDocument> LoadAsync(NAEMODATELI landlord, CancellationToken cancellationToken = default);
}
public sealed record ContractDocument(ContractTemplate Template, string Html);
