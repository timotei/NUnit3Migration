using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Text;
using NUnit3Migration.Processors;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NUnit3Migration
{
    public class Program
    {
        private static void Main(string[] args)
        {
            new Program().Run().Wait();
        }

        private readonly List<IProcessor> _syntaxNodeProcessors = new List<IProcessor>
        {
            new TestCaseAttributeProcessor()
        };

        private async Task Run()
        {
            string path = @"F:\src\ullink\git\ul-trader-extension\" +
                          "desk/extension/test/legacy/ultrader.test/OrderEntryTests/ULOrderEntryFormBooksTests.cs";

            var originalStr = File.ReadAllText(path);

            File.WriteAllText(path, await Process(_syntaxNodeProcessors, originalStr));
        }

        public static async Task<string> Process(IEnumerable<IProcessor> processors, string inputSource)
        {
            var workspace = new AdhocWorkspace();

            string projName = "NewProject";
            var projectId = ProjectId.CreateNewId();
            var versionStamp = VersionStamp.Create();
            var projectInfo = ProjectInfo.Create(projectId, versionStamp, projName, projName, LanguageNames.CSharp);
            var newProject = workspace.AddProject(projectInfo);
            workspace.AddDocument(newProject.Id, "NewFile.cs", SourceText.From(inputSource));

            var document = workspace.CurrentSolution.Projects.First().Documents.First();
            DocumentEditor editor = await DocumentEditor.CreateAsync(document);

            foreach (var processor in processors)
            {
                processor.Process(editor);
            }

            return (await editor.GetChangedDocument().GetTextAsync()).ToString();
        }
    }
}
