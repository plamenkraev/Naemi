using NaemZZ.Core.Contracts;
using NaemZZ.Core.Entities;
using NaemZZ.Core.Interfaces;
using System.IO;

namespace NaemZZ.Desktop.Services;

/// <summary>Loads saved HTML templates or the bundled HTML conversions of the legacy RTF contracts.</summary>
public sealed class ContractDocumentService(IFirmManagementService firms) : IContractDocumentService
{
    public async Task<ContractDocument> LoadAsync(Landlord landlord, CancellationToken cancellationToken = default)
    {
        var allFirms = await firms.ListAsync(cancellationToken);
        var firm = allFirms.FirstOrDefault(x => x.Id == landlord.FIRM_ID)
            ?? throw new InvalidOperationException("Моля, изберете фирма за наемодателя.");
        var templates = await firms.ListTemplatesAsync(cancellationToken);
        var selectedTemplateId = landlord.ContractTemplateId ?? firm.ContractTemplateId;
        var template = templates.FirstOrDefault(x => x.Id == selectedTemplateId);
        if (template is null)
        {
            var legacyFile = firm.Id switch
            {
                1 => "dogovorET", 2 => "dogovorZP", 3 => "dogovorHR", _ => null
            };
            if (legacyFile is null)
            {
                throw new InvalidOperationException("Фирмата няма избран шаблон. Изберете шаблон в редактора на фирми.");
            }

            var path = Path.Combine(AppContext.BaseDirectory, "Contracts", legacyFile + ".html");
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Липсва HTML версията на договора.", path);
            }

            template = new ContractTemplate
            {
                Name = "Договор - " + firm.Name,
                Html = await File.ReadAllTextAsync(path, cancellationToken),
                LegacyRtfFile = legacyFile + ".rtf"
            };
        }
        var rendered = ContractTemplateRenderer.Render(template.Html, firm, landlord);
        return new ContractDocument(template, rendered);
    }
}
