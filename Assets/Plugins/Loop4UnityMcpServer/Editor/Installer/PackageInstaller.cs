using UnityEngine;
using System.IO;

namespace LoopMcpServer.Editor.Installer
{
    public class PackageInstaller
    {
        private readonly IFileSystem _fileSystem;

        public PackageInstaller(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public bool Install(string sourcePath, string targetPath)
        {
            if (!_fileSystem.DirectoryExists(sourcePath))
            {
                Debug.LogError($"[PackageInstaller] Source directory not found: {sourcePath}");
                return false;
            }

            if (_fileSystem.DirectoryExists(targetPath))
            {
                Debug.LogWarning($"[PackageInstaller] Target already exists, skipping install: {targetPath}");
                return false;
            }

            try
            {
                CopyDirectoryRecursive(sourcePath, targetPath);
                Debug.Log($"[PackageInstaller] Successfully installed assets to: {targetPath}");
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[PackageInstaller] Failed to install assets. Error: {ex.Message}");
                return false;
            }
        }

        private void CopyDirectoryRecursive(string sourceDir, string targetDir)
        {
            _fileSystem.CreateDirectory(targetDir);

            foreach (var file in _fileSystem.GetFiles(sourceDir))
            {

                if (file.EndsWith(".meta")) continue;

                string fileName = _fileSystem.GetFileName(file);
                string destFile = NormalizePath(Path.Combine(targetDir, fileName));

                _fileSystem.CopyFile(file, destFile, true);
            }

            foreach (var directory in _fileSystem.GetDirectories(sourceDir))
            {
                string directoryName = _fileSystem.GetFileName(directory);
                string destDir = NormalizePath(Path.Combine(targetDir, directoryName));

                CopyDirectoryRecursive(directory, destDir);
            }
        }

        // Normalize to forward slashes so tests and Unity paths stay consistent across platforms.
        private static string NormalizePath(string path) => path.Replace("\\", "/");
    }
}