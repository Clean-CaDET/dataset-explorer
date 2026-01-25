using System;
using System.Diagnostics;
using System.IO;
using LibGit2Sharp;

namespace DataSetExplorer.Infrastructure.RepositoryAdapters
{
    public class GitCodeRepository : ICodeRepository
    {
        public void CloneRepository(string url, string projectPath, string gitUser, string gitToken)
        {
            if (Directory.Exists(projectPath)) DeleteDirectory(projectPath);
            Directory.CreateDirectory(projectPath);

            if (!string.IsNullOrEmpty(gitUser) && !string.IsNullOrEmpty(gitToken))
            {
                var urlParts = url.Replace("https://", "").Replace("http://", "");
                url = $"https://{gitUser}:{gitToken}@{urlParts}";
            }
            else
            {
                var urlParts = url.Replace("https://github.com/", "").Replace("http://github.com/", "").Replace("github.com/", "");
                url = $"git@github.com:{urlParts}";
            }

            var processInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = $"clone {url} \"{projectPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processInfo))
            {
                process.WaitForExit();
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();

                // Log output for debugging
                Console.WriteLine($"[GitClone] Command: git clone {url}");
                Console.WriteLine($"[GitClone] Exit code: {process.ExitCode}");
                Console.WriteLine($"[GitClone] Output: {output}");
                Console.WriteLine($"[GitClone] Error: {error}");

                if (process.ExitCode != 0)
                {
                    throw new System.Exception($"Git clone failed: {error}");
                }
            }
        }

        public void CheckoutCommit(string commitHash, string projectPath)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = $"checkout {commitHash}",
                WorkingDirectory = projectPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processInfo))
            {
                process.WaitForExit();
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();

                // Log output for debugging
                Console.WriteLine($"[GitCheckout] Command: git checkout {commitHash}");
                Console.WriteLine($"[GitCheckout] Exit code: {process.ExitCode}");
                Console.WriteLine($"[GitCheckout] Output: {output}");
                Console.WriteLine($"[GitCheckout] Error: {error}");

                if (process.ExitCode != 0)
                {
                    throw new System.Exception($"Git checkout failed: {error}");
                }
            }
        }

        public void SetupRepository(string urlWithCommitHash, string projectPath, string gitUser, string gitToken)
        {
            var urlParts = urlWithCommitHash.Split("/tree/");
            var projectUrl = urlParts[0] + ".git";

            CloneRepository(projectUrl, projectPath, gitUser, gitToken);

            var commitHash = urlParts[1];
            CheckoutCommit(commitHash, projectPath);
        }

        private static void DeleteDirectory(string directory)
        {
            foreach (var subDirectory in Directory.EnumerateDirectories(directory))
            {
                DeleteDirectory(subDirectory);
            }

            foreach (var fileName in Directory.EnumerateFiles(directory))
            {
                var fileInfo = new FileInfo(fileName)
                {
                    Attributes = FileAttributes.Normal
                };
                fileInfo.Delete();
            }

            Directory.Delete(directory);
        }
    }
}
