namespace CopyAndReplace
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Text.RegularExpressions;

    public abstract class ArgsParser
    {
        private StringDictionary _prefixedParams = new StringDictionary();
        private List<string> _anonParams = new List<string>();

        public ArgsParser(string[] args)
        {
            Regex spliter = new Regex(@"^([/-]|--){1}(?<name>\w+)([:=])?(?<value>.+)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            char[] trimChars = { '"', '\'' };
            Match part;

            foreach (string arg in args)
            {
                part = spliter.Match(arg);
                if (part.Success)
                    _prefixedParams[part.Groups["name"].Value] = part.Groups["value"].Value.Trim(trimChars);
                else
                    _anonParams.Add(arg);
            }
        }

        public bool IsValid { get { return CheckParams(); } }

        protected StringDictionary PrefixedParams { get { return _prefixedParams; } }

        protected List<string> AnonParams { get { return _anonParams; } set { _anonParams = value; } }

        public abstract string Usage();

        protected abstract bool CheckParams();
    }
}
