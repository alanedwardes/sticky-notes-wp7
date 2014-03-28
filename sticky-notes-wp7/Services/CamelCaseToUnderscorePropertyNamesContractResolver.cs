namespace StickyNotes.Services
{
    using System.Text.RegularExpressions;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Converts PropetyName to property_name,
    /// as used in the server data constructs.
    /// </summary>
    public class CamelCaseToUnderscorePropertyNamesContractResolver : DefaultContractResolver
    {
        public CamelCaseToUnderscorePropertyNamesContractResolver()
            : base(false)
        {
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            return Regex.Replace(propertyName, "([A-Z])", "_$1", RegexOptions.Compiled).Trim(new char[] { '_' });
        }
    }
}