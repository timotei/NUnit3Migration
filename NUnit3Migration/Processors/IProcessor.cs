using Microsoft.CodeAnalysis.Editing;

namespace NUnit3Migration.Processors
{
    public interface IProcessor
    {
        void Process(DocumentEditor editor);
    }
}
