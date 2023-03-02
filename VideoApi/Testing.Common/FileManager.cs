using System;
using System.IO;
using System.Reflection;

namespace Testing.Common
{
    public static class FileManager
    {
        public static string GetAssemblyDirectory()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        public static string CreateNewAudioFile(string originalFileName, string fileNameWithoutExtension, string path = "TestAudioFiles")
        {
            var originalFilePath = Path.Join(GetAssemblyDirectory(), path, originalFileName);
            if (!File.Exists(originalFilePath))
            {
                throw new FileNotFoundException($"Unable to find audio file with path : {originalFilePath}");
            }

            var fileWithExtension = $"{fileNameWithoutExtension}.mp4";
            var newFilePath = Path.Join(GetAssemblyDirectory(), path, fileWithExtension);
            File.Copy(originalFilePath, newFilePath, true);
            return newFilePath;
        }

        public static void RemoveLocalAudioFile(string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new FileNotFoundException($"Unable to find audio file with path : {filepath}");
            }
            File.Delete(filepath);
        }
    }
}
