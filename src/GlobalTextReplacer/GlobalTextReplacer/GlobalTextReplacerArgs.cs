namespace CopyAndReplace
{
    using System.IO;
    using System.Linq;
    using System.Text;


    // /source:"c:\myFolder\CopyMeXXX" /target:c:\myFolder /text:XXX /replace:YYYYY 
    public class GlobalTextReplacerArgs : ArgsParser
    {
        public string SourceFolder { get { return PrefixedParams["source"]; } }
        public string TargetFolder { get { return PrefixedParams["target"]; } }
        public string SourceText { get { return PrefixedParams["text"]; } }
        public string ReplacementText { get { return PrefixedParams["replace"]; } }
        public string FullTargetPath { get { return Path.Combine(TargetFolder, SourceFolder.Replace(SourceText, ReplacementText)); } }

        public GlobalTextReplacerArgs(string[] args) : base(args) { }

        public override string Usage()
        {
            var retVal = new StringBuilder();
            retVal.AppendLine("// ******************** Args ******************");
            retVal.AppendLine("//");
            retVal.AppendLine($"\t/source: [source folder]");
            retVal.AppendLine($"\t/target: [target folder (output folder will be taget + replace)]");
            retVal.AppendLine($"\t/text: [text to be replaced]");
            retVal.AppendLine($"\t/replace: [replacement text]");
            retVal.AppendLine("//");
            retVal.AppendLine("// *******************************************");
            return retVal.ToString();
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
