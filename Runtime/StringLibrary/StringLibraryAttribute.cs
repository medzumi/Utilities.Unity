using UnityEngine;

namespace Utilities.StringLibrary
{
    public class StringLibraryAttribute : PropertyAttribute
    {
        public string RegexReplace;
        public string Replace;

        public StringLibraryAttribute(string replace= null, string regexReplace = null)
        {
            Replace = string.IsNullOrWhiteSpace(replace) ? string.Empty : replace;
            RegexReplace = string.IsNullOrWhiteSpace(regexReplace) ? string.Empty : regexReplace;
        }
    }
}