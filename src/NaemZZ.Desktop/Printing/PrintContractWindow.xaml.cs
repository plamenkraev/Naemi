using System.IO;
using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Xps;
using NaemZZ.Core.Entities;

namespace NaemZZ.Desktop.Printing;

/// <summary>
/// Native WPF print-preview window for the rental contract. Ported from the legacy
/// PrintView.xaml.cs: loads one of three RTF templates (selected by the landlord's Company),
/// replaces the "&lt;&lt;n&gt;&gt;" placeholders with landlord/property data, repeats a
/// per-property block via "repeat.txt", and prints the resulting FlowDocument.
/// </summary>
public partial class PrintContractWindow : Window
{
    private static readonly string ContractsDirectory = Path.Combine(AppContext.BaseDirectory, "Contracts");

    private readonly string[] _placeholders =
    [
        "<<1>>", "<<2>>", "<<3>>", "<<4>>", "<<5>>", "<<6>>", "<<7>>", "<<8>>", "<<9>>", "<<10>>",
        "<<11>>", "<<12>>", "<<13>>", "<<&"
    ];

    private readonly Landlord _landlord;
    private List<string> _replaceText = [];
    private string _loopTemplate = "";
    private double _pageCount;

    public PrintContractWindow(Landlord landlord)
    {
        InitializeComponent();
        _landlord = landlord;
    }

    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        LoadContract();
    }

    private void LoadContract()
    {
        _replaceText =
        [
            DateTime.Today.ToString("dd/MM/yyyy"),
            $"{_landlord.NAME} {_landlord.SURNAME} {_landlord.LASTNAME}",
            BuildAddress(),
            _landlord.EGN ?? "",
            _landlord.LK ?? "",
            _landlord.LKDATE?.ToString("dd/MM/yyyy") ?? "",
            _landlord.MVR ?? "",
            _landlord.Phone ?? "",
        ];

        var repeatPath = Path.Combine(ContractsDirectory, "repeat.txt");
        _loopTemplate = File.Exists(repeatPath) ? File.ReadAllText(repeatPath) : "";

        var templateFile = _landlord.FIRM_ID switch
        {
            1 => "dogovorET.rtf",
            2 => "dogovorZP.rtf",
            3 => "dogovorHR.rtf",
            _ => null,
        };

        if (templateFile is null)
        {
            MessageBox.Show(this, "Няма избрана фирма (договор) за този наемодател.", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var templatePath = Path.Combine(ContractsDirectory, templateFile);
        if (!File.Exists(templatePath))
        {
            MessageBox.Show(this, $"Файлът с шаблона '{templateFile}' липсва.", Title, MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        using var fs = new FileStream(templatePath, FileMode.Open, FileAccess.Read);
        var range = new TextRange(RTB.Document.ContentStart, RTB.Document.ContentEnd);
        range.Load(fs, DataFormats.Rtf);
        ReplaceAll();
    }

    private string BuildAddress()
    {
        try
        {
            return $"обл.{_landlord.Region?.NAME}, общ.{_landlord.Municipality?.NAME}, {_landlord.Settlement?.T_V_M} {_landlord.Settlement?.NAME}, {_landlord.STREET}";
        }
        catch (Exception)
        {
            return _landlord.STREET ?? "";
        }
    }

    private void ReplaceAll()
    {
        var findR = new FindAndReplaceManager(RTB.Document);

        // Header placeholders <<1>>..<<8>>: landlord data (must run before any per-property
        // block is inserted, since the loop below reuses the same <<1>>..<<11>> tokens).
        for (var i = 0; i < _replaceText.Count; i++)
        {
            findR.ReplaceAll(
                _placeholders[i],
                _replaceText[i],
                FindOptions.MatchCase,
                null);
        }

        findR.ReplaceAll("<<13>>", DateTime.Today.Year.ToString(), FindOptions.MatchWholeWord,
            null);

        ClearText();

        var z = 1;
        foreach (var property in _landlord.IMOTIs)
        {
            try
            {
                var sb = new StringBuilder(_loopTemplate);
                sb.Append(_placeholders[13]).Append(z + 1).Append(">>");
                findR.ReplaceAll(_placeholders[13] + z + ">>", sb.ToString(), FindOptions.None, null);

                findR.ReplaceAll(_placeholders[0], property.PLOST?.ToString() ?? "", FindOptions.None, null);
                findR.ReplaceAll(_placeholders[1], NUM2WORD.Num2Txt(property.PLOST?.ToString() ?? "0"), FindOptions.None, null);
                findR.ReplaceAll(_placeholders[2], property.PropertyCategory?.NAME ?? "", FindOptions.None, null);
                findR.ReplaceAll(_placeholders[3], $"{property.Settlement?.T_V_M?.Trim()} {property.Settlement?.NAME}", FindOptions.None, null);
                findR.ReplaceAll(_placeholders[4], property.Municipality?.NAME ?? "", FindOptions.None, null);
                findR.ReplaceAll(_placeholders[5], property.Region?.NAME ?? "", FindOptions.None, null);

                var ekatteCode = property.Settlement?.EKATTE_CODE ?? "";
                var paddedEkatteCode = ekatteCode.PadLeft(6, '0');
                findR.ReplaceAll(_placeholders[6], paddedEkatteCode, FindOptions.None, null);

                findR.ReplaceAll(_placeholders[7], property.TERRAIN ?? "", FindOptions.None, null);

                var nomer = property.NOMER ?? "";
                var paddedNomer = nomer.PadLeft(6, '0');

                findR.ReplaceAll(_placeholders[8], paddedNomer, FindOptions.MatchWholeWord, null);
                findR.ReplaceAll(_placeholders[9], NUM2WORD.word(paddedNomer), FindOptions.None, null);

                findR.ReplaceAll(_placeholders[10], property.LIMITS ?? "", FindOptions.None, null);
                z++;
            }
            catch (Exception)
            {
                z++;
            }
        }

        findR.ReplaceAll(_placeholders[13] + z + ">>", "", FindOptions.MatchWholeWord, null);
        ClearText();
    }

    private void ClearText()
    {
        var findR = new FindAndReplaceManager(RTB.Document);
        for (var i = 0; i < _placeholders.Length - 1; i++)
        {
            try
            {
                findR.ReplaceAll(_placeholders[i], ".............................", FindOptions.MatchCase, null);
            }
            catch (Exception)
            {
                // Best-effort cleanup, same as the legacy implementation.
            }
        }
    }

    private void OnTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        RTB.Dispatcher.BeginInvoke(UpdatePageCount, DispatcherPriority.Background);
    }

    private void UpdatePageCount()
    {
        _pageCount = Math.Ceiling(RTB.ExtentHeight / 1010.0);
        lblPage.Text = _pageCount.ToString();
    }

    private void PrintButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var sourceRange = new TextRange(RTB.Document.ContentStart, RTB.Document.ContentEnd);
            using var stream = new MemoryStream();
            sourceRange.Save(stream, DataFormats.Xaml);

            var documentCopy = new FlowDocument();
            var copyRange = new TextRange(documentCopy.ContentStart, documentCopy.ContentEnd);
            stream.Position = 0;
            copyRange.Load(stream, DataFormats.Xaml);

            PrintDocumentImageableArea? imageableArea = null;
            var docWriter = PrintQueue.CreateXpsDocumentWriter(ref imageableArea);
            if (docWriter != null && imageableArea != null)
            {
                var paginator = ((IDocumentPaginatorSource)documentCopy).DocumentPaginator;
                paginator.PageSize = new Size(imageableArea.MediaSizeWidth, imageableArea.MediaSizeHeight);
                var pagePadding = new Thickness(72.0);
                documentCopy.PagePadding = new Thickness(
                    Math.Max(imageableArea.OriginWidth, pagePadding.Left),
                    Math.Max(imageableArea.OriginHeight, pagePadding.Top),
                    Math.Max(imageableArea.MediaSizeWidth - (imageableArea.OriginWidth + imageableArea.ExtentWidth), pagePadding.Right),
                    Math.Max(imageableArea.MediaSizeHeight - (imageableArea.OriginHeight + imageableArea.ExtentHeight), pagePadding.Bottom));
                documentCopy.ColumnWidth = double.PositiveInfinity;
                docWriter.Write(paginator);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();
}