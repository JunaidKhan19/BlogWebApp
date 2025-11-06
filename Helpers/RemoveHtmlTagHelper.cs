using System.Text.RegularExpressions;

namespace BlogWebApplication.Helpers
{
    public class RemoveHtmlTagHelper
    {
        public static string RemoveHtmlTag(string tag)
        {
            return Regex.Replace(tag, "<.*?>|&.*?;",string.Empty);
        }
    }
}
