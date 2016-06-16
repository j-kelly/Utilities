namespace CopyAndReplace
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;

    // /source:"c:\myFolder\CopyMeXXX" /target:c:\myFolder /text:XXX /replace:YYYYY 
    public class CopyAndReplaceArgs : ArgsParser
    {
        // anon
        public bool Verbose { get { return AnonParams.Contains("verbose"); } }
        public bool AllowSpaces { get { return AnonParams.Contains("allowSpaces"); } }

        // param
        public string SourceFolder { get { return Path.GetFullPath(PrefixedParams["source"]); } }
        public string TargetFolder { get { return Path.GetFullPath(PrefixedParams["target"]); } }
        public string SourceText { get { return PrefixedParams["text"]; } }
        public string ReplacementText { get { return AllowSpaces ? PrefixedParams["replace"] : PrefixedParams["replace"].Replace(" ", "_"); } }
        public string FullTargetPath
        {
            get
            {
                var dirName = Path.GetFileName(SourceFolder);
                dirName = Path.Combine(TargetFolder, dirName.Replace(SourceText, ReplacementText));
                return dirName;
            }
        }

        public CopyAndReplaceArgs(string[] args) : base(args) { }

        public override string Usage()
        {
            var retVal = new StringBuilder();
            retVal.AppendLine("// ******************** Args ******************");
            retVal.AppendLine("//");
            retVal.AppendLine($"\t/source: [source folder]");
            retVal.AppendLine($"\t/target: [target folder (output folder will be taget + replace)]");
            retVal.AppendLine($"\t/text: [text to be replaced]");
            retVal.AppendLine($"\t/replace: [replacement text]");
            retVal.AppendLine($"\tverbose");
            retVal.AppendLine($"\tallowSpaces");
            retVal.AppendLine("//");
            retVal.AppendLine("// *******************************************");
            return retVal.ToString();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"SourceFolder: {SourceFolder}");
            sb.AppendLine($"TargetFolder: {TargetFolder}");
            sb.AppendLine($"SourceText: {SourceText}");
            sb.AppendLine($"ReplacementText: {ReplacementText}");
            sb.AppendLine($"FullTargetPath: {FullTargetPath}");

            Console.WriteLine(sb.ToString());
            return sb.ToString();
        }

        protected override bool CheckParams()
        {
            var invalid = false;
            invalid = AnonParams.Count() != 0 || invalid;
            invalid = PrefixedParams.Count != 4 || invalid;
            invalid = string.IsNullOrWhiteSpace(SourceFolder) || invalid;
            invalid = string.IsNullOrWhiteSpace(TargetFolder) || invalid;
            invalid = string.IsNullOrWhiteSpace(SourceText) || invalid;
            invalid = string.IsNullOrWhiteSpace(ReplacementText) || invalid;
            invalid = !Directory.Exists(SourceFolder) || invalid;
            invalid = Directory.Exists(FullTargetPath) || invalid;
            return !invalid;
        }
    }
}
