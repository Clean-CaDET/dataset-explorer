using System;
using System.Collections.Generic;
using System.IO;
using DataSetExplorer.Core.Annotations.Model;
using DataSetExplorer.Core.Auth;
using DataSetExplorer.Core.DataSets;
using DataSetExplorer.Core.DataSets.Model;
using DataSetExplorer.Core.DataSets.Repository;
using DataSetExplorer.UI.Controllers.Dataset.DTOs;
using FluentResults;

namespace DataSetExplorer.Core.DataSetSerializer
{
    class DataSetExportationService : IDataSetExportationService
    {
        private readonly FullDataSetFactory _fullDataSetFactory;
        private readonly IDraftDataSetExportationService _draftDataSetExportationService;
        private readonly ICompleteDataSetExportationService _completeDataSetExportationService;
        private readonly IDataSetRepository _dataSetRepository;
        private readonly IAuthService _authService;

        public DataSetExportationService(FullDataSetFactory fullDataSetFactory, IDraftDataSetExportationService draftDataSetExportationService, 
            IDataSetRepository dataSetRepository, ICompleteDataSetExportationService completeDataSetExportationService,
            IAuthService authService)
        {
            _fullDataSetFactory = fullDataSetFactory;
            _dataSetRepository = dataSetRepository;
            _draftDataSetExportationService = draftDataSetExportationService;
            _completeDataSetExportationService = completeDataSetExportationService;
            _authService = authService;
        }

        public Result<string> ExportDraft(DraftDataSetExportDTO dataSetDTO)
        {
            var dataSet = GetDataSetForExport(dataSetDTO.Id).Value;
            var exportPath = _draftDataSetExportationService.Export(dataSetDTO.AnnotatorId, dataSet);
            return Result.Ok(exportPath);
        }

        public Result<string> ExportComplete(int datasetId, CompleteDataSetExportDTO dataSetDTO)
        {
            if (dataSetDTO.DraftDatasetFiles == null || dataSetDTO.DraftDatasetFiles.Count == 0)
                return Result.Fail("No draft dataset files were uploaded.");

            // Save uploaded files temporarily and get their paths
            var tempFolder = Path.Combine("/app/uploads", "temp_complete_export_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);

            var filePaths = new List<string>();
            try
            {
                foreach (var file in dataSetDTO.DraftDatasetFiles)
                {
                    if (file == null || file.Length == 0) continue;

                    if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                        return Result.Fail($"File '{file.FileName}' is not a valid Excel file. Only .xlsx files are supported.");

                    var tempFilePath = Path.Combine(tempFolder, file.FileName);
                    using (var stream = new FileStream(tempFilePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    filePaths.Add(tempFilePath);
                }

                if (filePaths.Count == 0)
                    return Result.Fail("No valid draft dataset files were uploaded.");

                // Calculate export path with Final_ prefix and incremental naming
                var dataSet = GetDataSetForExport(datasetId).Value;
                var sanitizedDataSetName = SanitizeFolderName(dataSet.Name);
                var folderName = $"Final_{sanitizedDataSetName}";
                var outputPath = GetUniqueExportPath("/app/exports", folderName);
                outputPath = EndPathWithSeparator(outputPath);

                var result = this.Export(datasetId, filePaths.ToArray(), outputPath);

                // Clean up temporary files
                Directory.Delete(tempFolder, true);

                return result;
            }
            catch (Exception ex)
            {
                // Clean up on error
                if (Directory.Exists(tempFolder))
                    Directory.Delete(tempFolder, true);
                throw;
            }
        }

        private Result<DataSet> GetDataSetForExport(int id)
        {
            var dataSet = _dataSetRepository.GetDataSetForExport(id);
            if (dataSet == default) return Result.Fail($"DataSet with id: {id} does not exist.");
            return Result.Ok(dataSet);
        }

        public Result<string> Export(int datasetId, string[] annotationsFilesPaths, string outputPath)
        {
            List<Annotator> annotators = _authService.GetAll().Value;
            try
            {
                var instancesGroupedBySmells = _fullDataSetFactory.GetAnnotatedInstancesGroupedBySmells(datasetId, annotators, annotationsFilesPaths);
                foreach (var codeSmellGroup in instancesGroupedBySmells)
                {
                    _completeDataSetExportationService.Export(outputPath, codeSmellGroup.Instances, codeSmellGroup.CodeSmell.Name, "DataSet_" + codeSmellGroup.CodeSmell.Name);
                }
                return Result.Ok("Data set exported: " + outputPath);
            } catch (IOException e)
            {
                return Result.Fail(e.ToString());
            }
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
            if (!Directory.Exists(fullPath)) return fullPath;

            int counter = 1;
            string numberedPath;
            do
            {
                numberedPath = Path.Combine(basePath, $"{folderName}({counter})");
                counter++;
            } while (Directory.Exists(numberedPath));

            return numberedPath;
        }

        private string EndPathWithSeparator(string folderPath)
        {
            if (!folderPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                folderPath += Path.DirectorySeparatorChar;
            }
            return folderPath;
        }
    }
}
