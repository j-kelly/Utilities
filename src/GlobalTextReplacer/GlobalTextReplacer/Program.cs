namespace GlobalTextReplacer
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
        private readonly static string DefaultReplacement = ConfigurationManager.AppSettings["DefaultReplacement"];

        private static string _From;
        private static string _To;
        private static int _FilesAndFoldersTouched;

        static void Main(string[] args)
        {
            var path = Path.GetDirectoryName(typeof(Program).Assembly.Location);

            while (true)
            {
                Console.WriteLine($"Enter text to be replaced(press return for default: {DefaultReplacement}");
                _From = Console.ReadLine();

                if (string.IsNullOrEmpty(_From))
                    _From = DefaultReplacement;

                Console.WriteLine($"Enter text to replace with");
                _To = Console.ReadLine();

                Console.WriteLine($"Replace: '{_From}' with '{_To}' ?");
                Console.WriteLine("Type Y to continue");

                if (Console.ReadLine().ToUpper() == "Y")
                    break;

                Console.WriteLine("");
                Console.WriteLine("TRY AGAIN");
                Console.WriteLine("");
            }

            UpdateFolder(path);

            Console.WriteLine("");
            Console.WriteLine($"Completed. {_FilesAndFoldersTouched} files or folders updated.");
            Console.WriteLine("Press any key to exit");
            Console.Read();
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
                    if (contents.Contains(_From))
                    {
                        _FilesAndFoldersTouched++;
                        contents = contents.Replace(_From, _To);
                        File.WriteAllText(file, contents);
                    }
                });

                RunWithoutException(() =>
                {
                    if (fileName.Replace(_From, _To) != fileName)
                    {
                        _FilesAndFoldersTouched++;
                        File.Move(file, Path.Combine(path, fileName.Replace(_From, _To)));
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
                    if (folderName.Replace(_From, _To) != folderName)
                    {
                        _FilesAndFoldersTouched++;
                        Directory.Move(folder, Path.Combine(Path.GetDirectoryName(folder), folderName.Replace(_From, _To)));
                    }
                });
            }
        }
    }
}