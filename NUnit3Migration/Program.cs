using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Text;
using NUnit3Migration.Processors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NUnit3Migration
{
    public class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Need first argument a path to the directory in which to search recursively .cs files");
                Environment.Exit(1);
            }
            new Program().Run(args[0]).Wait();
        }

        private readonly List<IProcessor> _syntaxNodeProcessors = new List<IProcessor>
        {
            new TestCaseAttributeProcessor(),
            new AssertProcessor(),
            new ExpectedExceptionAttributeProcessor(),
            new TestCaseSourceAttributeProcessor()
        };

        private async Task Run(string path)
        {
            foreach (var fsEntry in Directory.EnumerateFileSystemEntries(path))
            {
                if (Directory.Exists(fsEntry))
                {
                    await Run(fsEntry);
                }
                else if (".cs" == Path.GetExtension(fsEntry))
                {
                    Console.WriteLine($"Processing {fsEntry} ...");
                    var originalStr = File.ReadAllText(fsEntry);
                    var encoding = GetEncoding(fsEntry);
                    File.WriteAllText(fsEntry, await Process(_syntaxNodeProcessors, originalStr), encoding);
                }
            }
        }


        private static Encoding GetEncoding(string filePath)
        {
            return FileEncodindDetector.GetEncoding(filePath);
        }

        public static async Task<string> Process(IEnumerable<IProcessor> processors, string inputSource)
        {
            try
            {
                var workspace = new AdhocWorkspace();

                string projName = "NewProject";
                var projectId = ProjectId.CreateNewId();
                var versionStamp = VersionStamp.Create();
                var projectInfo = ProjectInfo.Create(projectId, versionStamp, projName, projName, LanguageNames.CSharp);
                var newProject = workspace.AddProject(projectInfo);
                var document = workspace.AddDocument(newProject.Id, "NewFile.cs", SourceText.From(inputSource));
                DocumentEditor editor = await DocumentEditor.CreateAsync(document);

                foreach (var processor in processors)
                {
                    processor.Process(editor);
                }

                return (await editor.GetChangedDocument().GetTextAsync()).ToString();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return inputSource;
            }
        }
    }
}
