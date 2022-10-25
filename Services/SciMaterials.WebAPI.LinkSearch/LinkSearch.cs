using System.Text.RegularExpressions;

using SciMaterials.Contracts.WebAPI.LinkSearch;

namespace SciMaterials.WebAPI.LinkSearch;

public class LinkSearch : ILinkSearch
{
    public string ParsingLink(string text)
    {
        return Regex.Replace(text,
            @"((http|ftp|https)://([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?)",
            "\"$1\"");
    }
}