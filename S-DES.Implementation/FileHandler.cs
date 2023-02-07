using System.Diagnostics;
using System.IO;
using System.Text;

namespace S_DES.Implementation
{
    public static class FileHandler
    {

        private const string ProgramName = "notepad.exe";
        
        public static void WriteToFile(string fileName, string text)
        {
            using var writer = new StreamWriter(fileName);
            writer.Write(text, Encoding.UTF8);
        }

        public static void OpenFile(string fileName)
        {
            Process.Start(ProgramName, fileName);
        }

        public static string ReadFromFile(string fileName)
        {
            using var reader = new StreamReader(fileName);
            return reader.ReadToEnd();
        }
    }
}