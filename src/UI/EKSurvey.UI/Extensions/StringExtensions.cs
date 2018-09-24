using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace EKSurvey.UI.Extensions
{
    public static class StringExtensions
    {
        private const string PlaceholderRegexPattern = @"\{\w+\}";
        private static readonly Regex PlaceholderRegex = new Regex(PlaceholderRegexPattern);

        private static object GetValue(string propertyName, object source)
        {
            var namedProperty = source
                .GetType()
                .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
            if (namedProperty == null)
                return null;

            var value = namedProperty.GetValue(source);

            return value;
        }
        
        public static string ApplyValues(this string url, object source)
        {
            if (source.GetType().IsValueType)
                return url.Replace("{value}", source.ToString());

            var placeholders = PlaceholderRegex.Matches(url);
            var modifiedUrl = url;

            foreach (var placeholder in placeholders.OfType<Match>())
            {
                var val = GetValue(placeholder.Value.Trim("{}".ToCharArray()), source);
                if (val == null)
                    continue;

                modifiedUrl = modifiedUrl.Replace(placeholder.Value, val.ToString());
            }

            return modifiedUrl;
        }
    }
}