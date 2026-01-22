using CodeModel.CaDETModel.CodeItems;
using DataSetExplorer.Core.CleanCodeAnalysis.Model;
using DataSetExplorer.Core.DataSets;
using DataSetExplorer.Core.DataSets.Model;
using DataSetExplorer.Core.DataSets.Repository;
using DataSetExplorer.UI.Controllers.Dataset.DTOs;
using FluentResults;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace DataSetExplorer.Core.CleanCodeAnalysis
{
    public class CleanCodeAnalysisService : ICleanCodeAnalysisService
    {
        private readonly string _cleanNamesAnalysisTemplatePath = "./Core/DataSetSerializer/Template/Clean_Names_Analysis_Template.xlsx";
        private readonly string _cleanFunctionsAnalysisTemplatePath = "./Core/DataSetSerializer/Template/Clean_Functions_Analysis_Template.xlsx";
        private readonly string _cleanClassesAnalysisTemplatePath = "./Core/DataSetSerializer/Template/Clean_Classes_Analysis_Template.xlsx";
        private string _exportPath;
        private ExcelPackage _excelFile;
        private ExcelWorksheet _sheet;
        private readonly IInstanceService _instanceService;
        private readonly IDataSetRepository _dataSetRepository;
        private readonly IProjectRepository _projectRepository;

        public CleanCodeAnalysisService(IInstanceService instanceService, IDataSetRepository dataSetRepository, IProjectRepository projectRepository)
        {
            _instanceService = instanceService;
            _dataSetRepository = dataSetRepository;
            _projectRepository = projectRepository;
        }

        public Result<string> ExportDatasetAnalysis(int datasetId, CleanCodeAnalysisDTO analysisOptions)
        {
            string datasetFolderPath = CreateDatasetExportFolder(datasetId);

            foreach (var option in analysisOptions.CleanCodeOptions)
            {
                if (option.Equals("Clean names"))
                    ExportCleanNamesAnalysis(_instanceService.GetInstancesWithIdentifiersByDatasetId(datasetId).Value, datasetFolderPath);
                if (option.Equals("Clean functions"))
                    ExportCleanFunctionsAnalysis(_instanceService.GetInstancesByDatasetId(datasetId).Value, datasetFolderPath);
                if (option.Equals("Clean classes"))
                    ExportCleanClassesAnalysis(_instanceService.GetInstancesByDatasetId(datasetId).Value, datasetFolderPath);
            }
            return Result.Ok(datasetFolderPath);
        }

        private string CreateDatasetExportFolder(int datasetId)
        {
            string datasetFolderPath = GetExportPathForDataset(datasetId);
            Directory.CreateDirectory(datasetFolderPath);
            return datasetFolderPath;
        }

        private string GetExportPathForDataset(int datasetId)
        {
            var dataset = _dataSetRepository.GetDataSetWithProjectsAndCodeSmells(datasetId);
            var datasetName = SanitizeFolderName(dataset.Name);

            var datasetFolderPath = GetUniqueExportPath("/app/exports", datasetName);
            datasetFolderPath = EndPathWithSeparator(datasetFolderPath);

            return datasetFolderPath;
        }

        private static string EndPathWithSeparator(string folderPath)
        {
            if (!folderPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                folderPath += Path.DirectorySeparatorChar;
            }

            return folderPath;
        }

        public Result<string> ExportProjectAnalysis(int projectId, CleanCodeAnalysisDTO analysisOptions)
        {
            string projectFolderPath = CreateProjectExportFolder(projectId);

            foreach (var option in analysisOptions.CleanCodeOptions)
            {
                if (option.Equals("Clean names"))
                    ExportCleanNamesAnalysis(_instanceService.GetInstancesWithIdentifiersByProjectId(projectId).Value, projectFolderPath);
                if (option.Equals("Clean functions"))
                    ExportCleanFunctionsAnalysis(_instanceService.GetInstancesByProjectId(projectId).Value, projectFolderPath);
                if (option.Equals("Clean classes"))
                    ExportCleanClassesAnalysis(_instanceService.GetInstancesByProjectId(projectId).Value, projectFolderPath);
            }
            return Result.Ok(projectFolderPath);
        }

        private string CreateProjectExportFolder(int projectId)
        {
            var datasetFolderPath = GetExportPathForProject(projectId);
            Directory.CreateDirectory(datasetFolderPath);

            var projectFolderPath = GetUniqueExportPath(datasetFolderPath, GetProjectName(projectId));
            projectFolderPath = EndPathWithSeparator(projectFolderPath);

            Directory.CreateDirectory(projectFolderPath);
            return projectFolderPath;
        }

        private string GetProjectName(int projectId)
        {
            var project = _projectRepository.Get(projectId);
            var projectName = SanitizeFolderName(project.Name);
            return projectName;
        }

        private string GetExportPathForProject(int projectId)
        {
            var datasetId = _projectRepository.GetDatasetIdByProjectId(projectId);
            var datasetName = datasetId.HasValue
                ? SanitizeFolderName(_dataSetRepository.GetDataSetWithProjectsAndCodeSmells(datasetId.Value).Name)
                : "StandaloneProjects";

            return $"/app/exports/{datasetName}/";
        }

        private void ExportCleanNamesAnalysis(Dictionary<string, List<Instance>> projectsAndInstances, string basePath)
        {
            if (projectsAndInstances == default) return;

            foreach (var projectAndInstances in projectsAndInstances)
            {
                var projectName = SanitizeFolderName(projectAndInstances.Key);
                var projectFolderPath = $"{basePath}{projectName}/";
                Directory.CreateDirectory(projectFolderPath);

                _exportPath = projectFolderPath;

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                _excelFile = new ExcelPackage(new FileInfo(_cleanNamesAnalysisTemplatePath));
                _sheet = _excelFile.Workbook.Worksheets[0];

                RemoveFunctions(projectAndInstances.Value);
                var instancesAndIdentifiers = GetInstancesAndIdentifiers(projectAndInstances.Value);
                PopulateCleanNamesTemplate(projectAndInstances.Value, instancesAndIdentifiers);
                Serialize("CleanNames");
            }
        }

        private Dictionary<int, Dictionary<string, List<IdentifierType>>> GetInstancesAndIdentifiers(List<Instance> instances)
        {
            Dictionary<int, Dictionary<string, List<IdentifierType>>> instanceIdentifiers = new Dictionary<int, Dictionary<string, List<IdentifierType>>>();    
            foreach (var instance in instances)
            {
                Dictionary<string, List<IdentifierType>> identifierTypes = new Dictionary<string, List<IdentifierType>>();
                instance.Identifiers.ForEach(ident => AddTypeForIdentifier(ident, identifierTypes));
                instanceIdentifiers.Add(instance.Id, identifierTypes);
            }
            return instanceIdentifiers;
        }

        private void AddTypeForIdentifier(Identifier identifier, Dictionary<string, List<IdentifierType>> identifierTypes)
        {
            if (identifierTypes.ContainsKey(identifier.Name)) identifierTypes[identifier.Name].Add(identifier.Type);
            else
            {
                identifierTypes.Add(identifier.Name, new List<IdentifierType>());
                identifierTypes[identifier.Name].Add(identifier.Type);
            }
        }

        private void RemoveFunctions(List<Instance> instances)
        {
            instances.RemoveAll(i => i.Type.Equals(SnippetType.Function));
        }

        private void PopulateCleanNamesTemplate(List<Instance> instances, Dictionary<int, Dictionary<string, List<IdentifierType>>> instancesAndIdentifiers)
        {
            var identifierCount = 0;
            for (var i = 0; i < instances.Count; i++)
            {
                var row = 3 + i + identifierCount;
                _sheet.Cells[row, 1].Value = instances[i].CodeSnippetId;
                _sheet.Cells[row, 2].Value = instances[i].Link;

                var identifiersAndTypes = instancesAndIdentifiers[instances[i].Id];
                var mainIdentifier = instances[i].Identifiers.Find(i => i.Type.Equals(IdentifierType.Class));
                PopulateIdentifiers(row, mainIdentifier, identifiersAndTypes);
                identifierCount += identifiersAndTypes.Count;
            }
        }

        private void PopulateIdentifiers(int row, Identifier mainIdentifier, Dictionary<string, List<IdentifierType>> identifiersAndTypes)
        {
            var j = 0;

            _sheet.Cells[row + j, 3].Value = mainIdentifier.Name;
            _sheet.Cells[row + j, 4].Value = mainIdentifier.Type.ToString();
            j++;

            foreach (var identifierAndType in identifiersAndTypes)
            {
                if (identifierAndType.Key.Equals(mainIdentifier.Name)) continue;
                _sheet.Cells[row + j, 3].Value = identifierAndType.Key;
                PopulateIdentifierTypes(row, j, identifierAndType.Value);
                j++;
            }
        }

        private void PopulateIdentifierTypes(int row, int j, List<IdentifierType> types)
        {
            Dictionary<string, int> typeOccurrence = CountTypeOccurrence(types);
            _sheet.Cells[row + j, 4].Value = "";
            foreach (var occurrence in typeOccurrence)
            {
                _sheet.Cells[row + j, 4].Value += occurrence.Value + "x " + occurrence.Key + "; ";
            }
        }

        private static Dictionary<string, int> CountTypeOccurrence(List<IdentifierType> types)
        {
            return types.GroupBy(t => t.ToString()).ToDictionary(t => t.Key, t => t.Count());
        }

        private void ExportCleanFunctionsAnalysis(Dictionary<string, List<Instance>> instances, string basePath)
        {
            if (instances == default) return;

            foreach (var projectName in instances.Keys)
            {
                var sanitizedProjectName = SanitizeFolderName(projectName);
                var projectFolderPath = $"{basePath}{sanitizedProjectName}/";
                Directory.CreateDirectory(projectFolderPath);

                _exportPath = projectFolderPath;

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                _excelFile = new ExcelPackage(new FileInfo(_cleanFunctionsAnalysisTemplatePath));
                _sheet = _excelFile.Workbook.Worksheets[0];
                PopulateCleanFunctionsTemplate(instances[projectName]);
                Serialize("CleanFunctions");
            }
        }

        private void PopulateCleanFunctionsTemplate(List<Instance> instances)
        {
            instances = FilterInstances(instances);
            for (var i = 0; i < instances.Count; i++)
            {
                var row = 5 + i;
                PopulateInstanceInfo(row, instances[i]);

                PopulateMetrics(row, 3, CaDETMetric.MELOC, 15, 25, instances[i]);
                SetConditionalFormatting(new ExcelAddress(5, 3, instances.Count + 4, 3), _sheet.Cells[3, 3].Value, Color.Red);
                SetConditionalFormatting(new ExcelAddress(5, 3, instances.Count + 4, 3), _sheet.Cells[2, 3].Value, Color.Yellow);

                PopulateMetrics(row, 4, CaDETMetric.CYCLO_SWITCH, 5, 12, instances[i]);
                SetConditionalFormatting(new ExcelAddress(5, 4, instances.Count + 4, 4), _sheet.Cells[3, 4].Value, Color.Red);
                SetConditionalFormatting(new ExcelAddress(5, 4, instances.Count + 4, 4), _sheet.Cells[2, 4].Value, Color.Yellow);
                
                PopulateMetrics(row, 5, CaDETMetric.MNOL, 2, 4, instances[i]);
                SetConditionalFormatting(new ExcelAddress(5, 5, instances.Count + 4, 5), _sheet.Cells[3, 5].Value, Color.Red);
                SetConditionalFormatting(new ExcelAddress(5, 5, instances.Count + 4, 5), _sheet.Cells[2, 5].Value, Color.Yellow);

                PopulateMetrics(row, 6, CaDETMetric.NOP, 3, 6, instances[i]);
                SetConditionalFormatting(new ExcelAddress(5, 6, instances.Count + 4, 6), _sheet.Cells[3, 6].Value, Color.Red);
                SetConditionalFormatting(new ExcelAddress(5, 6, instances.Count + 4, 6), _sheet.Cells[2, 6].Value, Color.Yellow);
                
                PopulateMetrics(row, 7, CaDETMetric.MNOC, 2, 4, instances[i]);
                SetConditionalFormatting(new ExcelAddress(5, 7, instances.Count + 4, 7), _sheet.Cells[3, 7].Value, Color.Red);
                SetConditionalFormatting(new ExcelAddress(5, 7, instances.Count + 4, 7), _sheet.Cells[2, 7].Value, Color.Yellow);
            }
        }

        private List<Instance> FilterInstances(List<Instance> instances)
        {
            instances.RemoveAll(i => i.Type.Equals(SnippetType.Class));
            return RemoveConstructors(instances);
        }

        private List<Instance> RemoveConstructors(List<Instance> instances)
        {
            var instancesWithoutConstructors = new List<Instance>();
            foreach (Instance instance in instances)
            {
                var snippetPartsWithoutParams = instance.CodeSnippetId.Split("(")[0];
                var snippetParts = snippetPartsWithoutParams.Split(".");
                var methodName = snippetParts.Last();
                var className = snippetParts[snippetParts.Length - 2];
                if (!methodName.Equals(className)) instancesWithoutConstructors.Add(instance);
            }
            return instancesWithoutConstructors;
        }

        private void ExportCleanClassesAnalysis(Dictionary<string, List<Instance>> instances, string basePath)
        {
            if (instances == default) return;

            foreach (var projectName in instances.Keys)
            {
                var sanitizedProjectName = SanitizeFolderName(projectName);
                var projectFolderPath = $"{basePath}{sanitizedProjectName}/";
                Directory.CreateDirectory(projectFolderPath);

                _exportPath = projectFolderPath;

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                _excelFile = new ExcelPackage(new FileInfo(_cleanClassesAnalysisTemplatePath));
                _sheet = _excelFile.Workbook.Worksheets[0];
                PopulateCleanClassesTemplate(instances[projectName]);
                Serialize("CleanClasses");
            }
        }

        private void PopulateCleanClassesTemplate(List<Instance> instances)
        {
            instances.RemoveAll(i => i.Type.Equals(SnippetType.Function));
            for (var i = 0; i < instances.Count; i++)
            {
                var row = 5 + i;
                PopulateInstanceInfo(row, instances[i]);

                PopulateMetrics(row, 3, CaDETMetric.CLOC, 80, 150, instances[i]);
                SetConditionalFormatting(new ExcelAddress(5, 3, instances.Count + 4, 3), _sheet.Cells[3, 3].Value, Color.Red);
                SetConditionalFormatting(new ExcelAddress(5, 3, instances.Count + 4, 3), _sheet.Cells[2, 3].Value, Color.Yellow);

                PopulateMetrics(row, 4, CaDETMetric.WMC, 15, 25, instances[i]);
                SetConditionalFormatting(new ExcelAddress(5, 4, instances.Count + 4, 4), _sheet.Cells[3, 4].Value, Color.Red);
                SetConditionalFormatting(new ExcelAddress(5, 4, instances.Count + 4, 4), _sheet.Cells[2, 4].Value, Color.Yellow);

                PopulateMetrics(row, 5, CaDETMetric.NAD, 10, 15, instances[i]);
                SetConditionalFormatting(new ExcelAddress(5, 5, instances.Count + 4, 5), _sheet.Cells[3, 5].Value, Color.Red);
                SetConditionalFormatting(new ExcelAddress(5, 5, instances.Count + 4, 5), _sheet.Cells[2, 5].Value, Color.Yellow);

                PopulateMetrics(row, 6, CaDETMetric.NMD, 12, 16, instances[i]);
                SetConditionalFormatting(new ExcelAddress(5, 6, instances.Count + 4, 6), _sheet.Cells[3, 6].Value, Color.Red);
                SetConditionalFormatting(new ExcelAddress(5, 6, instances.Count + 4, 6), _sheet.Cells[2, 6].Value, Color.Yellow);

                PopulateMetrics(row, 7, CaDETMetric.CBO, 8, 12, instances[i]);
                SetConditionalFormatting(new ExcelAddress(5, 7, instances.Count + 4, 7), _sheet.Cells[3, 7].Value, Color.Red);
                SetConditionalFormatting(new ExcelAddress(5, 7, instances.Count + 4, 7), _sheet.Cells[2, 7].Value, Color.Yellow);

                PopulateMetrics(row, 8, CaDETMetric.DIT, 3, 5, instances[i]);
                SetConditionalFormatting(new ExcelAddress(5, 8, instances.Count + 4, 8), _sheet.Cells[3, 8].Value, Color.Red);
                SetConditionalFormatting(new ExcelAddress(5, 8, instances.Count + 4, 8), _sheet.Cells[2, 8].Value, Color.Yellow);
            }
        }

        private void PopulateInstanceInfo(int row, Instance instance)
        {
            _sheet.Cells[row, 1].Value = instance.CodeSnippetId;
            _sheet.Cells[row, 2].Value = instance.Link;
        }

        private void PopulateMetrics(int row, int column, CaDETMetric metric, int suspiciousValue, int criticalValue, Instance instance)
        {
            _sheet.Cells[2, column].Value = suspiciousValue;
            _sheet.Cells[3, column].Value = criticalValue;
            _sheet.Cells[4, column].Value = metric;
            _sheet.Cells[row, column].Value = instance.MetricFeatures[metric];
        }

        private void SetConditionalFormatting(ExcelAddress excelAddress, object formula, Color color)
        {
            var cf = _sheet.ConditionalFormatting.AddGreaterThan(excelAddress);
            cf.Formula = formula.ToString();
            cf.Style.Fill.BackgroundColor.Color = color;
        }

        private void Serialize(string fileName)
        {
            var filePath = _exportPath + fileName + ".xlsx";
            _excelFile.SaveAs(new FileInfo(filePath));
        }

        private string SanitizeFolderName(string name)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = string.Join("_", name.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
            return sanitized.Trim();
        }

        private string GetUniqueExportPath(string basePath, string folderName)
        {
            var fullPath = Path.Combine(basePath, folderName);

            // If folder doesn't exist, use it
            if (!Directory.Exists(fullPath))
            {
                return fullPath;
            }

            // Find the next available number
            int counter = 1;
            string numberedPath;
            do
            {
                numberedPath = Path.Combine(basePath, $"{folderName}({counter})");
                counter++;
            } while (Directory.Exists(numberedPath));

            return numberedPath;
        }
    }
}
