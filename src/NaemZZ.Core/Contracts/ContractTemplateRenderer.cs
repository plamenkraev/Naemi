using HtmlAgilityPack;
using NaemZZ.Core.Dto;
using NaemZZ.Core.Entities;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace NaemZZ.Core.Contracts;

/// <summary>Renders trusted HTML templates; values are always HTML-encoded.</summary>
public static partial class ContractTemplateRenderer
{
    [GeneratedRegex(@"\{\{#Properties\}\}(.*?)\{\{/Properties\}\}", RegexOptions.Singleline)]
    private static partial Regex PropertiesBlock();

    [GeneratedRegex(@"\{\{([A-Za-z][A-Za-z0-9.]*)\}\}")]
    private static partial Regex Field();

    public static string Render(string html, FirmDto? firm, Landlord landlord, DateTime? date = null)
    {
        var today = date ?? DateTime.Today;
        var bg = CultureInfo.GetCultureInfo("en-US");
        var fields = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
        {
            ["Firm.Name"] = firm?.Name,
            ["Firm.Address"] = firm?.Address,
            ["Firm.Bulstat"] = firm?.BULSTAT,
            ["Firm.EIK"] = firm?.BULSTAT,
            ["Firm.Representative"] = firm?.Representative,
            ["Landlord.IsCompany"] = landlord.IsCompany ? "true" : "false",
            ["Landlord.CompanyName"] = landlord.CompanyName,
            ["Landlord.EIK"] = landlord.EIK,
            ["Landlord.MOL"] = landlord.MOL,
            ["Landlord.DisplayName"] = landlord.IsCompany ? landlord.CompanyName : $"{landlord.NAME} {landlord.SURNAME} {landlord.LASTNAME}".Trim(),
            ["Landlord.FullName"] = $"{landlord.NAME} {landlord.SURNAME} {landlord.LASTNAME}".Trim(),
            ["Landlord.EGN"] = landlord.EGN,
            ["Landlord.Address"] = FormatLandlordAddress(landlord),
            ["Landlord.Phone"] = landlord.Phone,
            ["Landlord.Email"] = landlord.Email,
            ["Landlord.IDCard"] = landlord.LK,
            ["Landlord.IDCardDate"] = landlord.LKDATE?.ToString("dd.MM.yyyy", bg),
            ["Landlord.Issuer"] = landlord.MVR,
            ["Contract.Date"] = today.ToString("dd.MM.yyyy", bg),
            ["Contract.Year"] = today.Year.ToString(CultureInfo.InvariantCulture),
            ["Contract.Price"] = (firm?.Price ?? landlord.Price)?.ToString("N2", bg),
            ["Contract.Total"] = (landlord.IMOTIs.Sum(i => i.PLOST.GetValueOrDefault()) * (firm?.Price ?? landlord.Price))?.ToString("N2", bg),
            ["Contract.TotalArea"] = landlord.IMOTIs.Sum(i => i.PLOST.GetValueOrDefault()).ToString("N3", bg),
        };

        string FormatLandlordAddress(Landlord landlord)
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(landlord.Region?.NAME))
                parts.Add($"обл. {landlord.Region.NAME}");

            if (!string.IsNullOrWhiteSpace(landlord.Municipality?.NAME))
                parts.Add($"общ. {landlord.Municipality.NAME}");

            if (!string.IsNullOrWhiteSpace(landlord.Settlement?.NAME))
            {
                parts.Add($"{landlord.Settlement.T_V_M} {landlord.Settlement.NAME}".Trim());
            }

            if (!string.IsNullOrWhiteSpace(landlord.STREET))
            {
                parts.Add(landlord.STREET);
            }

            return string.Join(", ", parts);
        }

        string PropertiesHtml() => string.Join("\n", landlord.IMOTIs.Select(p =>
            {
                var values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Number"] = p.NOMER,
                    ["Area"] = p.PLOST?.ToString("N3", bg),
                    ["Category"] = p.PropertyCategory?.NAME,
                    ["Terrain"] = p.TERRAIN,
                    ["Limits"] = p.LIMITS,
                    ["Region"] = p.Region?.NAME,
                    ["Municipality"] = p.Municipality?.NAME,
                    ["Settlement"] = p.Settlement?.NAME,
                    ["EkatteCode"] = p.Settlement?.EKATTE_CODE
                };
                return "<p class=\"contract-property\">" +
                    "НИВА с площ от " + Enc(values["Area"]) + " дка, " +
                    Enc(values["Category"]) + " категория, в землището на " +
                    Enc(values["Settlement"]) + ", общ. " + Enc(values["Municipality"]) +
                    ", обл. " + Enc(values["Region"]) + ", ЕКАТТЕ " +
                    Enc(values["EkatteCode"]) + ", местност " + Enc(values["Terrain"]) +
                   ", имот № " + Enc(values["Number"]) +
                    (!string.IsNullOrWhiteSpace(values["Limits"])
                        ? ", при граници: " + Enc(values["Limits"])
                        : "") +
                    ".</p>";
            }));

        // Expand legacy repeat blocks before HTML parsing. The repeat block is a
        // template-language construct, not an HTML element.
        html = PropertiesBlock().Replace(html, m => string.Concat(landlord.IMOTIs.Select(p =>
        {
            var values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
            {
                ["Number"] = p.NOMER,
                ["Area"] = p.PLOST?.ToString("N3", bg),
                ["Category"] = p.PropertyCategory?.NAME,
                ["Terrain"] = p.TERRAIN,
                ["Limits"] = p.LIMITS,
                ["Region"] = p.Region?.NAME,
                ["Municipality"] = p.Municipality?.NAME,
                ["Settlement"] = p.Settlement?.NAME,
                ["EkatteCode"] = p.Settlement?.EKATTE_CODE
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