using HtmlAgilityPack;

namespace NaemZZ.Core.Contracts;

/// <summary>
/// One-time cleanup for HTML imported from legacy RTF/LibreOffice files.
/// Do NOT run on every TinyMCE save: that would discard user formatting.
/// </summary>
public static class ContractHtmlCleaner
{
    private static readonly HashSet<string> LegacyProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "widows", "orphans"
    };

    public static string CleanLegacyImport(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        foreach (var node in doc.DocumentNode.Descendants().ToList())
        {
            if (node.NodeType != HtmlNodeType.Element) continue;
            var style = node.GetAttributeValue("style", "");
            if (style.Length > 0)
            {
                var kept = new List<string>();
                foreach (var part in style.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    var colon = part.IndexOf(':');
                    if (colon < 0) { kept.Add(part.Trim()); continue; }
                    var property = part[..colon].Trim();
                    var value = part[(colon + 1)..].Trim();
                    if (LegacyProperties.Contains(property)) continue;
                    if (property.Equals("margin-bottom", StringComparison.OrdinalIgnoreCase) &&
                        (value.Equals("0in", StringComparison.OrdinalIgnoreCase) ||
                         value.Equals("0cm", StringComparison.OrdinalIgnoreCase))) continue;
                    if (property.Equals("line-height", StringComparison.OrdinalIgnoreCase) &&
                        value.Equals("100%", StringComparison.OrdinalIgnoreCase)) continue;
                    kept.Add(part.Trim());
                }
                if (kept.Count == 0) node.Attributes.Remove("style");
                else node.SetAttributeValue("style", string.Join("; ", kept));
            }

            // Preserve user-visible emphasis; only unwrap redundant legacy <font>
            // when no meaningful font styling remains.
            if (node.Name.Equals("font", StringComparison.OrdinalIgnoreCase) &&
                !node.Attributes.Any(a => a.Name.Equals("face", StringComparison.OrdinalIgnoreCase) ||
                                          a.Name.Equals("size", StringComparison.OrdinalIgnoreCase) ||
                                          a.Name.Equals("color", StringComparison.OrdinalIgnoreCase) ||
                                          a.Name.Equals("style", StringComparison.OrdinalIgnoreCase)))
            {
                if (node.ParentNode is null) continue;
                foreach (var child in node.ChildNodes.ToList())
                    node.ParentNode.InsertBefore(child, node);
                node.Remove();
            }
        }
        return doc.DocumentNode.InnerHtml;
    }
}
