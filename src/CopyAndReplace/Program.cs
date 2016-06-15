namespace CopyAndReplace
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;

    class Program
    {
        private readonly static IEnumerable<string> IgnoreFolders = ConfigurationManager.AppSettings["IgnoreFolders"].Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        private readonly static IEnumerable<string> IgnoreFiles = ConfigurationManager.AppSettings["IgnoreFiles"].Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        private static int _FilesAndFoldersTouched;
        private static CopyAndReplaceArgs _Args;

        // /source:"c:\myFolder\CopyMeXXX" /target:c:\myFolder /text:XXX /replace:YYYYY 
        static int Main(string[] args)
        {

            try
            {
                _Args = new CopyAndReplaceArgs(args);
                if (!_Args.IsValid)
                {
                    Console.WriteLine(_Args.ToString());
                    Console.WriteLine(_Args.Usage());
                    return -1;
                }

                Directory.CreateDirectory(_Args.FullTargetPath);
                CopyFolder(new DirectoryInfo(_Args.SourceFolder), new DirectoryInfo(_Args.FullTargetPath));
                UpdateFolder(_Args.FullTargetPath);

                Console.WriteLine($"Completed copy and replace at: {_Args.FullTargetPath}");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(_Args.ToString());
                Console.WriteLine(ex.ToString());
                return -1;
            }
        }

        public static void UpdateFolder(string path)
        {
            Action<Action> RunWithoutException = method =>
            {
                try { method(); }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            };

            foreach (var file in Directory.EnumerateFiles(path))
            {
                var fileName = Path.GetFileName(file);
                if (IgnoreFiles.Any(p => fileName.Contains(p)))
                    continue;

                // Rename Contents
                RunWithoutException(() =>
                {
                    var contents = File.ReadAllText(file);
                    if (contents.Contains(_Args.SourceText))
                    {
                        _FilesAndFoldersTouched++;
                        contents = contents.Replace(_Args.SourceText, _Args.ReplacementText);
                        File.WriteAllText(file, contents);
                    }
                });

                RunWithoutException(() =>
                {
                    if (fileName.Replace(_Args.SourceText, _Args.ReplacementText) != fileName)
                    {
                        _FilesAndFoldersTouched++;
                        File.Move(file, Path.Combine(path, fileName.Replace(_Args.SourceText, _Args.ReplacementText)));
                    }
                });
            }

            foreach (var folder in Directory.EnumerateDirectories(path))
            {
                var folderName = Path.GetFileName(folder);
                if (IgnoreFolders.Any(p => folderName.Contains(p)))
                    continue;

                UpdateFolder(folder);
                RunWithoutException(() =>
                {
                    if (folderName.Replace(_Args.SourceText, _Args.ReplacementText) != folderName)
                    {
                        _FilesAndFoldersTouched++;
                        Directory.Move(folder, Path.Combine(Path.GetDirectoryName(folder), folderName.Replace(_Args.SourceText, _Args.ReplacementText)));
                    }
                });
            }

        }

        public static void CopyFolder(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyFolder(dir, target.CreateSubdirectory(dir.Name));

            foreach (FileInfo file in source.GetFiles())
                file.CopyTo(Path.Combine(target.FullName, file.Name));
        }
    }
}