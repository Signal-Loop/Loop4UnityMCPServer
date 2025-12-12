using System.IO;

namespace LoopMcpServer.Editor.Installer
{
    public interface IFileSystem
    {
        bool DirectoryExists(string path);
        bool FileExists(string path);
        void CreateDirectory(string path);
        void CopyFile(string source, string dest, bool overwrite);
        string[] GetFiles(string path);
        string[] GetDirectories(string path);
        string GetFileName(string path);
    }

    public class EditorFileSystem : IFileSystem
    {
        public bool DirectoryExists(string path) => Directory.Exists(path);
        public bool FileExists(string path) => File.Exists(path);
        public void CreateDirectory(string path) => Directory.CreateDirectory(path);
        public void CopyFile(string source, string dest, bool overwrite) => File.Copy(source, dest, overwrite);
        public string[] GetFiles(string path) => Directory.GetFiles(path);
        public string[] GetDirectories(string path) => Directory.GetDirectories(path);
        public string GetFileName(string path) => Path.GetFileName(path);
    }
}