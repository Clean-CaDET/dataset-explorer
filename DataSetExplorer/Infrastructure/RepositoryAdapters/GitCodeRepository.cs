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

            // Determine if URL is SSH or HTTPS
            var gitUrl = url;
            var isSsh = url.StartsWith("git@") || url.StartsWith("ssh://");

            if (isSsh)
            {
                gitUrl = url;
            }
            else if (url.StartsWith("https://") || url.StartsWith("http://"))
            {
                if (!string.IsNullOrEmpty(gitUser) && !string.IsNullOrEmpty(gitToken))
                {
                    var urlParts = url.Replace("https://", "").Replace("http://", "");
                    gitUrl = $"https://{gitUser}:{gitToken}@{urlParts}";
                }
                else
                {
                    gitUrl = url;
                }
            }

            var processInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = $"clone {gitUrl} \"{projectPath}\"",
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

                if (process.ExitCode != 0)
                {
                    throw new System.Exception($"Git clone failed: {error}");
                }
            }
        }

        public void CheckoutCommit(string commitHash, string projectPath)
        {
            // Use native git command to avoid LibGit2Sharp permission issues
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

            // Always clone repository regardless of environment
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
