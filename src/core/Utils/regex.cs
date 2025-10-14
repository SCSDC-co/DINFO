using System.Text.RegularExpressions;

namespace dinfo.core.Utils.RegularExpressions;

public static partial class RegexHelpers
{
    [GeneratedRegex(@"^\s*//")]
    public static partial Regex SlashComment();

    [GeneratedRegex(@"^\s*#")]
    public static partial Regex HashComment();

    [GeneratedRegex(@"^\s*/\*")]
    public static partial Regex MultiLineCommentStart();

    [GeneratedRegex(@"\s*\*/")]
    public static partial Regex MultiLineCommentEnd();

    [GeneratedRegex(@"^\s*<!--")]
    public static partial Regex MultiLineMarkupStart();

    [GeneratedRegex(@"\s*-->$")]
    public static partial Regex MultiLineMarkupEnd();

    [GeneratedRegex(@"^\s*--")]
    public static partial Regex DashComment();

    [GeneratedRegex(@"^\s*;")]
    public static partial Regex SemicolonComment();
}
