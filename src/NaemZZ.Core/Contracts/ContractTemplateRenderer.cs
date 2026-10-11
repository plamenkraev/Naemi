using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using NaemZZ.Core.Dto;
using NaemZZ.Core.Entities;

namespace NaemZZ.Core.Contracts;

/// <summary>Renders trusted HTML templates; values are always HTML-encoded.</summary>
public static partial class ContractTemplateRenderer
{
    [GeneratedRegex(@"\{\{#Properties\}\}(.*?)\{\{/Properties\}\}", RegexOptions.Singleline)]
    private static partial Regex PropertiesBlock();

    [GeneratedRegex(@"\{\{([A-Za-z][A-Za-z0-9.]*)\}\}")]
    private static partial Regex Field();

    public static string Render(string html, FirmDto? firm, NAEMODATELI landlord, DateTime? date = null)
    {
        var today = date ?? DateTime.Today;
        var bg = CultureInfo.GetCultureInfo("bg-BG");
        var fields = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
        {
            ["Firm.Name"] = firm?.firmNAME,
            ["Firm.Address"] = firm?.firmADDRESS,
            ["Firm.Bulstat"] = firm?.BULSTAT,
            ["Firm.EIK"] = firm?.BULSTAT,
            ["Firm.Representative"] = firm?.firmMOL,
            ["Landlord.FullName"] = $"{landlord.NAME} {landlord.SURNAME} {landlord.LASTNAME}".Trim(),
            ["Landlord.EGN"] = landlord.EGN,
            ["Landlord.Address"] = landlord.STREET,
            ["Landlord.Phone"] = landlord.Phone,
            ["Landlord.IDCard"] = landlord.LK,
            ["Landlord.IDCardDate"] = landlord.LKDATE?.ToString("dd.MM.yyyy", bg),
            ["Landlord.Issuer"] = landlord.MVR,
            ["Contract.Date"] = today.ToString("dd.MM.yyyy", bg),
            ["Contract.Year"] = today.Year.ToString(CultureInfo.InvariantCulture)
        };

        string PropertiesHtml() => string.Join("\n", landlord.IMOTIs.Select(p =>
        {
            var values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
            {
                ["Number"] = p.NOMER,
                ["Area"] = p.PLOST?.ToString("N3", bg),
                ["Category"] = p.CATEGORI?.NAME,
                ["Terrain"] = p.TERRAIN,
                ["Limits"] = p.LIMITS,
                ["Region"] = p.EK_OBL?.NAME,
                ["Municipality"] = p.EK_OBST?.NAME,
                ["Settlement"] = p.EKATTE?.NAME,
                ["EkatteCode"] = p.EKATTE?.EKATTE_CODE
            };
            return "<p class=\"contract-property\">" +
                "НИВА с площ от " + Enc(values["Area"]) + " дка, " +
                Enc(values["Category"]) + " категория, в землището на " +
                Enc(values["Settlement"]) + ", общ. " + Enc(values["Municipality"]) +
                ", обл. " + Enc(values["Region"]) + ", ЕКАТТЕ " +
                Enc(values["EkatteCode"]) + ", местност " + Enc(values["Terrain"]) +
                ", имот № " + Enc(values["Number"]) + ", при граници: " +
                Enc(values["Limits"]) + ".</p>";
        }));

        // Expand legacy repeat blocks before HTML parsing. The repeat block is a
        // template-language construct, not an HTML element.
        html = PropertiesBlock().Replace(html, m => string.Concat(landlord.IMOTIs.Select(p =>
        {
            var values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
            {
                ["Number"] = p.NOMER, ["Area"] = p.PLOST?.ToString("N3", bg),
                ["Category"] = p.CATEGORI?.NAME, ["Terrain"] = p.TERRAIN,
                ["Limits"] = p.LIMITS, ["Region"] = p.EK_OBL?.NAME,
                ["Municipality"] = p.EK_OBST?.NAME, ["Settlement"] = p.EKATTE?.NAME,
                ["EkatteCode"] = p.EKATTE?.EKATTE_CODE
            };
            return ReplaceFields(m.Groups[1].Value, values);
        })));

        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        ExpandPropertyLists(doc, PropertiesHtml());
        // Replace ordinary scalar tokens after structural substitutions.
        return ReplaceFields(doc.DocumentNode.InnerHtml, fields);
    }

    private static void ExpandPropertyLists(HtmlDocument doc, string propertiesHtml)
    {
        var markerPattern = new Regex(@"\{\{\s*Properties\.List\s*\}\}", RegexOptions.IgnoreCase);
        // Prefer replacing a complete block-level paragraph. This is important:
        // putting <p> elements inside another <p> breaks pagination and spacing.
        var paragraphs = doc.DocumentNode.Descendants("p").ToList();
        foreach (var paragraph in paragraphs)
        {
            if (paragraph.ParentNode is null || !markerPattern.IsMatch(paragraph.InnerText))
                continue;

            var text = WebUtility.HtmlDecode(paragraph.InnerText).Trim();
            if (markerPattern.IsMatch(text) && markerPattern.Replace(text, "").Trim().Length == 0)
            {
                var replacement = HtmlNode.CreateNode("<div class=\"contract-properties\">" + propertiesHtml + "</div>");
                paragraph.ParentNode.ReplaceChild(replacement, paragraph);
            }
        }

        // Standalone tokens outside a paragraph (or within other text) are also
        // supported. Replace text nodes, never raw HTML, to avoid malformed DOM.
        var nodes = doc.DocumentNode.DescendantsAndSelf()
            .Where(n => n.NodeType == HtmlNodeType.Text && markerPattern.IsMatch(n.InnerHtml))
            .ToList();
        foreach (var node in nodes)
        {
            if (node.ParentNode is null) continue;
            var chunks = markerPattern.Split(node.InnerHtml);
            var matches = markerPattern.Matches(node.InnerHtml);
            for (var i = 0; i < chunks.Length; i++)
            {
                if (chunks[i].Length > 0)
                    node.ParentNode.InsertBefore(doc.CreateTextNode(chunks[i]), node);
                if (i < matches.Count)
                    node.ParentNode.InsertBefore(HtmlNode.CreateNode("<div class=\"contract-properties\">" + propertiesHtml + "</div>"), node);
            }
            node.Remove();
        }
    }

    private static string Enc(string? value) => WebUtility.HtmlEncode(value ?? "");
    private static string ReplaceFields(string source, IReadOnlyDictionary<string, string?> fields) =>
        Field().Replace(source, m => fields.TryGetValue(m.Groups[1].Value, out var value) ? Enc(value) : m.Value);
}
