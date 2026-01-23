using DataSetExplorer.Core.AnnotationSchema.Model;
using DataSetExplorer.Core.DataSets.Model;
using DataSetExplorer.Core.DataSets.Repository;
using DataSetExplorer.UI.Controllers.Dataset.DTOs;
using FluentResults;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace DataSetExplorer.Core.DataSets
{
    public class InstanceService : IInstanceService
    {
        private readonly IInstanceRepository _instanceRepository;
        private readonly IDataSetCreationService _dataSetCreationService;
        private readonly IAnnotationRepository _annotationRepository;
        private readonly IConfiguration _configuration;

        public InstanceService(IInstanceRepository instanceRepository, IDataSetCreationService dataSetCreationService,
            IAnnotationRepository annotationRepository, IConfiguration configuration)
        {
            _instanceRepository = instanceRepository;
            _dataSetCreationService = dataSetCreationService;
            _annotationRepository = annotationRepository;
            _configuration = configuration;
        }

        public Result<Dictionary<string, List<Instance>>> GetInstancesWithIdentifiersByDatasetId(int datasetId)
        {
            var instances = _instanceRepository.GetInstancesWithIdentifiersByDatasetId(datasetId);
            if (instances == default) return Result.Fail($"Dataset with id: {datasetId} does not exist.");
            return Result.Ok(instances);
        }

        public Result<Dictionary<string, List<Instance>>> GetInstancesWithIdentifiersByProjectId(int projectId)
        {
            var instances = _instanceRepository.GetInstancesWithIdentifiersByProjectId(projectId);
            if (instances == default) return Result.Fail($"Project with id: {projectId} does not exist.");
            return Result.Ok(instances);
        }

        public Result<Dictionary<string, List<Instance>>> GetInstancesByDatasetId(int datasetId)
        {
            var instances = _instanceRepository.GetInstancesByDatasetId(datasetId);
            if (instances == default) return Result.Fail($"Dataset with id: {datasetId} does not exist.");
            return Result.Ok(instances);
        }

        public Result<Dictionary<string, List<Instance>>> GetInstancesByProjectId(int projectId)
        {
            var instances = _instanceRepository.GetInstancesByProjectId(projectId);
            if (instances == default) return Result.Fail($"Project with id: {projectId} does not exist.");
            return Result.Ok(instances);
        }

        public Result<InstanceDTO> GetInstanceWithRelatedInstances(int id)
        {
            var instance = _instanceRepository.GetInstanceWithRelatedInstances(id);
            if (instance == default) return Result.Fail($"Instance with id: {id} does not exist.");
            return Result.Ok(instance);
        }

        public Result<Instance> GetInstanceWithAnnotations(int id)
        {
            var instance = _instanceRepository.GetInstanceWithAnnotations(id);
            if (instance == default) return Result.Fail($"Instance with id: {id} does not exist.");
            return Result.Ok(instance);
        }

        public Result<List<Instance>> GetInstancesForSmell(string codeSmellName)
        {
            List<Instance> instances = new List<Instance>();
            var datasets = _dataSetCreationService.GetDataSetsByCodeSmell(codeSmellName).Value;
            foreach (var dataset in datasets)
            {
                foreach (var project in dataset.Projects)
                {
                    foreach (var candidate in project.CandidateInstances)
                    {
                        if (!candidate.CodeSmell.Name.Equals(codeSmellName)) continue;
                        instances.AddRange(candidate.Instances);
                    }
                }
            }
            return Result.Ok(instances);
        }

        public Result<List<SmellCandidateInstances>> DeleteCandidateInstancesForSmell(CodeSmellDefinition codeSmellDefinition)
        {
            var codeSmells = _annotationRepository.GetCodeSmellsByDefinition(codeSmellDefinition);
            var deletedCandidates = _instanceRepository.DeleteCandidateInstancesBySmell(codeSmells);
            _annotationRepository.DeleteCodeSmells(codeSmells);
            return Result.Ok(deletedCandidates);
        }

        public string GetFileFromGit(string url)
        {
            try
            {
                // Parse GitHub URL: https://github.com/{owner}/{repo}/tree/{commit-hash}/{file-path}#L{start}-L{end}
                var urlWithoutFragment = url.Split('#')[0]; // Remove line number fragment
                var parts = urlWithoutFragment.Split("https://github.com/");
                if (parts.Length < 2) return "Invalid GitHub URL format";

                var pathParts = parts[1].Split("/tree/");
                if (pathParts.Length < 2) return "Invalid GitHub URL format - missing /tree/";

                var repoParts = pathParts[0].Split('/');
                if (repoParts.Length < 2) return "Invalid GitHub URL format - missing owner/repo";

                var owner = repoParts[0];
                var repo = repoParts[1];

                var commitAndPath = pathParts[1].Split(new[] { '/' }, 2);
                if (commitAndPath.Length < 2) return "Invalid GitHub URL format - missing commit/path";

                var commitHash = commitAndPath[0];
                var filePath = commitAndPath[1];

                // Try raw.githubusercontent.com first (works for public repos)
                string rawUrl = $"https://raw.githubusercontent.com/{owner}/{repo}/{commitHash}/{filePath}";

                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    using (HttpResponseMessage response = client.GetAsync(rawUrl).Result)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            return response.Content.ReadAsStringAsync().Result;
                        }
                    }
                }

                // If raw URL fails (private repo), try GitHub API with authentication
                var gitToken = _configuration.GetValue<string>("GitCredentials:Token");
                if (!string.IsNullOrEmpty(gitToken))
                {
                    string apiUrl = $"https://api.github.com/repos/{owner}/{repo}/contents/{filePath}?ref={commitHash}";

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", gitToken);
                        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("DataSetExplorer", "1.0"));
                        client.Timeout = TimeSpan.FromSeconds(10);

                        using (HttpResponseMessage response = client.GetAsync(apiUrl).Result)
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var jsonContent = response.Content.ReadAsStringAsync().Result;
                                var json = JObject.Parse(jsonContent);

                                // GitHub API returns base64-encoded content
                                var base64Content = json["content"]?.ToString();
                                if (!string.IsNullOrEmpty(base64Content))
                                {
                                    // Remove whitespace/newlines from base64 string
                                    base64Content = base64Content.Replace("\n", "").Replace("\r", "");
                                    var bytes = Convert.FromBase64String(base64Content);
                                    return Encoding.UTF8.GetString(bytes);
                                }
                            }
                            else
                            {
                                return $"Failed to fetch file from GitHub API. Status: {response.StatusCode}. " +
                                       $"Ensure the repository is accessible and the commit exists.";
                            }
                        }
                    }
                }

                return "Failed to fetch file from GitHub. For private repositories, ensure Git credentials are configured in .env file.";
            }
            catch (Exception ex)
            {
                return $"Error fetching file from GitHub: {ex.Message}";
            }
        }
    }
}