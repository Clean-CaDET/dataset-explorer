using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using AutoMapper;
using DataSetExplorer.Core.Annotations.Model;
using DataSetExplorer.Core.DataSets;
using DataSetExplorer.Core.DataSets.Model;
using DataSetExplorer.Core.DataSetSerializer;
using DataSetExplorer.Core.CleanCodeAnalysis;
using DataSetExplorer.UI.Controllers.Dataset.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DataSetExplorer.UI.Controllers.Dataset
{
    [Route("api/datasets/")]
    [ApiController]
    public class DataSetController : ControllerBase
    {
        private readonly string _gitClonePath;

        private readonly IMapper _mapper;
        private readonly IDataSetCreationService _dataSetCreationService;
        private readonly IDataSetExportationService _dataSetExportationService;
        private readonly ICleanCodeAnalysisService _cleanCodeAnalysisService;

        public DataSetController(IMapper mapper, IDataSetCreationService creationService, IConfiguration configuration,
            IDataSetExportationService exportationService, ICleanCodeAnalysisService cleanCodeAnalysisService)
        {
            _mapper = mapper;
            _dataSetCreationService = creationService;
            _dataSetExportationService = exportationService;
            _cleanCodeAnalysisService = cleanCodeAnalysisService;   
            _gitClonePath = configuration.GetValue<string>("Workspace:GitClonePath");
        }

        [HttpPut]
        public IActionResult UpdateDataSet([FromBody] DatasetDTO datasetDto)
        {
            var dataset = _mapper.Map<DataSet>(datasetDto);
            var result = _dataSetCreationService.UpdateDataSet(dataset);
            if (result.IsFailed) return BadRequest(new { message = result.Reasons[0].Message });
            return Ok(result.Value);
        }

        [HttpPost]
        [Route("export-draft")]
        public IActionResult ExportDraftDataSet([FromBody] DraftDataSetExportDTO dataSetDTO)
        {
            var result = _dataSetExportationService.ExportDraft(dataSetDTO);
            if (result.IsFailed) return BadRequest(new { message = result.Reasons[0].Message });

            var exportPath = result.Value;

            // Create ZIP file from export directory
            var zipPath = CreateZipFromDirectory(exportPath);
            var zipFileName = Path.GetFileName(zipPath);

            // Return ZIP file for download
            var fileBytes = System.IO.File.ReadAllBytes(zipPath);

            // Clean up temporary ZIP and export directory
            System.IO.File.Delete(zipPath);
            Directory.Delete(exportPath, true);

            return File(fileBytes, "application/zip", zipFileName);
        }

        [HttpPost]
        [Route("{id}/export-complete")]
        public IActionResult ExportCompleteDataSet([FromRoute] int id, [FromForm] CompleteDataSetExportDTO dataSetDTO)
        {
            try
            {
                var result = _dataSetExportationService.ExportComplete(id, dataSetDTO);
                if (result.IsFailed) return BadRequest(new { message = result.Reasons[0].Message });

                var exportPath = result.Value;

                // Create ZIP file from export directory
                var zipPath = CreateZipFromDirectory(exportPath);
                var zipFileName = Path.GetFileName(zipPath);

                // Return ZIP file for download
                var fileBytes = System.IO.File.ReadAllBytes(zipPath);

                // Clean up temporary ZIP and export directory
                System.IO.File.Delete(zipPath);
                Directory.Delete(exportPath, true);

                return File(fileBytes, "application/zip", zipFileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpPost]
        [Route("{id}/export-clean-code-analysis")]
        public IActionResult ExportCleanCodeAnalysis([FromRoute] int id, [FromBody] CleanCodeAnalysisDTO dataSetDTO)
        {
            try
            {
                var result = _cleanCodeAnalysisService.ExportDatasetAnalysis(id, dataSetDTO);
                if (result.IsFailed) return BadRequest(new { message = result.Reasons[0].Message });

                var exportPath = result.Value;

                // Create ZIP file from export directory
                var zipPath = CreateZipFromDirectory(exportPath);
                var zipFileName = Path.GetFileName(zipPath);

                // Return ZIP file for download
                var fileBytes = System.IO.File.ReadAllBytes(zipPath);

                // Clean up temporary ZIP and export directory
                System.IO.File.Delete(zipPath);
                Directory.Delete(exportPath, true);

                return File(fileBytes, "application/zip", zipFileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("{name}")]
        public IActionResult CreateDataSet([FromBody] List<CodeSmellDTO> codeSmells, [FromRoute] string name)
        {
            var smells = new List<CodeSmell>();
            foreach (var codeSmell in codeSmells) smells.Add(_mapper.Map<CodeSmell>(codeSmell));

            var result = _dataSetCreationService.CreateEmptyDataSet(name, smells);
            return Ok(result.Value);
        }

        [HttpPost]
        [Route("{id}/projects")]
        public IActionResult CreateDataSetProject([FromBody] ProjectCreationDTO data, [FromRoute] int id)
        {
            var dataSetProject = _mapper.Map<DataSetProject>(data.Project);
            var smellFilters = _mapper.Map<List<SmellFilter>>(data.SmellFilters);

            var result = _dataSetCreationService.AddProjectToDataSet(id, _gitClonePath, dataSetProject, smellFilters, data.BuildSettings);
            if (result.IsFailed) return BadRequest(new { message = result.Reasons[0].Message });
            return Accepted(result.Value);
        }


        [HttpPost]
        [Route("{id}/importProjects")]
        public IActionResult ImportProjects([FromForm] IFormFile file, [FromForm] string smellFilters, [FromRoute] int id)
        {
            // Manually deserialize smellFilters JSON string from FormData
            SmellFilterDTO[] smellFilterDTOs = null;
            if (!string.IsNullOrEmpty(smellFilters))
            {
                smellFilterDTOs = JsonConvert.DeserializeObject<SmellFilterDTO[]>(smellFilters);
            }

            var mappedSmellFilters = _mapper.Map<List<SmellFilter>>(smellFilterDTOs);
            var result = _dataSetCreationService.ImportProjectsToDataSet(id, _gitClonePath, file, mappedSmellFilters);
            if (result.IsFailed) return BadRequest(new { message = result.Reasons[0].Message });
            return Accepted(result.Value);
        }

        [HttpGet]
        [Route("{id}/code-smells")]
        public IActionResult GetDataSetCodeSmells([FromRoute] int id)
        {
            var result = _dataSetCreationService.GetDataSetCodeSmells(id);
            if (result.IsFailed) return BadRequest(new { message = result.Reasons[0].Message });
            return Ok(result.Value);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetDataSet([FromRoute] int id)
        {
            var result = _dataSetCreationService.GetDataSet(id);
            if (result.IsFailed) return BadRequest(new { message = result.Reasons[0].Message });
            return Ok(result.Value);
        }

        [HttpGet]
        public IActionResult GetAllDataSets()
        {
            var result = _dataSetCreationService.GetAllDataSets();
            return Ok(result.Value);
        }
        
        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteDataSet([FromRoute] int id)
        {
            var result = _dataSetCreationService.DeleteDataSet(id);
            if (result.IsFailed) return BadRequest(new { message = result.Reasons[0].Message });
            return Ok(result.Value);
        }

        private string CreateZipFromDirectory(string directoryPath)
        {
            // Create a temporary ZIP file path
            var zipFileName = Path.GetFileName(directoryPath.TrimEnd(Path.DirectorySeparatorChar)) + ".zip";
            var zipPath = Path.Combine(Path.GetTempPath(), zipFileName);

            // Delete existing ZIP if it exists
            if (System.IO.File.Exists(zipPath))
            {
                System.IO.File.Delete(zipPath);
            }

            // Create ZIP from directory
            ZipFile.CreateFromDirectory(directoryPath, zipPath, CompressionLevel.Fastest, false);

            return zipPath;
        }
    }
}
