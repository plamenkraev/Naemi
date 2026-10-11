using NaemZZ.Core.Entities;

namespace NaemZZ.Core.Interfaces;

/// <summary>
/// Prints the rental contract for a landlord. Implemented natively in the WPF shell
/// (System.Printing / FlowDocument), since printing is a native desktop concern and
/// Blazor Hybrid pages only need to trigger it.
/// </summary>
public interface IPrintingService
{
    /// <summary>Shows the native print-preview window for the given landlord's contract.</summary>
    Task ShowPrintPreviewAsync(NAEMODATELI landlord);
}
