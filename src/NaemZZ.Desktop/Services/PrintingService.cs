using System.Windows;
using NaemZZ.Core.Entities;
using NaemZZ.Core.Interfaces;
using NaemZZ.Desktop.Printing;

namespace NaemZZ.Desktop.Services;

/// <summary>
/// Native WPF implementation of <see cref="IPrintingService"/>: shows the
/// <see cref="PrintContractWindow"/> modally over the main window.
/// </summary>
public class PrintingService : IPrintingService
{
    public Task ShowPrintPreviewAsync(Landlord landlord)
    {
        var window = new PrintContractWindow(landlord)
        {
            Owner = Application.Current.MainWindow
        };
        window.ShowDialog();
        return Task.CompletedTask;
    }
}
