using System.IO;
using System.Linq;
using System.Text;

namespace NUnit3Migration
{
    static class FileEncodindDetector
    {
        private static readonly byte[] Utf8Bom = new UTF8Encoding(true).GetPreamble();

        public static Encoding GetEncoding(string filePath)
        {
            Encoding encoding;
            using (var reader = new StreamReader(File.OpenRead(filePath), true))
            {
                reader.Peek();
                encoding = reader.CurrentEncoding;

                //Just UTF8 for now, we need to check if BOM exists, to not have files changed just for BOM
                if (!(encoding is UTF8Encoding))
                {
                    return encoding;
                }
            }

            return IsMissingBOM(filePath) ? new UTF8Encoding(false) : encoding;
        }

        private static bool IsMissingBOM(string filePath)
        {
            using (var binaryReader = new FileStream(filePath, FileMode.Open))
            {
                var buffer = new byte[Utf8Bom.Length];
                int foundLength = binaryReader.Read(buffer, 0, buffer.Length);

                return !(foundLength == Utf8Bom.Length && Utf8Bom.Where((v, i) => v != buffer[i]).Any());
            }
        }
    }
}
