using System;
using System.IO;
using System.IO.Compression;
using AutoMapper;
using DataSetExplorer.Core.CommunityDetection.Model;
using DataSetExplorer.Core.DataSets;
using DataSetExplorer.Core.CleanCodeAnalysis;
using DataSetExplorer.Core.DataSets.Model;
using DataSetExplorer.UI.Controllers.Dataset.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DataSetExplorer.UI.Controllers.Dataset
{
    [Route("api/projects/")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly string _gitClonePath;

        private readonly IMapper _mapper;
        private readonly IDataSetCreationService _dataSetCreationService;
        private readonly IProjectService _projectService;
        private readonly IGraphInstanceService _graphInstanceService;
        private readonly ICleanCodeAnalysisService _cleanCodeAnalysisService;

        public ProjectController(IMapper mapper, IDataSetCreationService creationService, IConfiguration configuration,
            IProjectService projectService, IGraphInstanceService graphInstanceService, ICleanCodeAnalysisService cleanCodeAnalysisService)
        {
            _mapper = mapper;
            _dataSetCreationService = creationService;
            _projectService = projectService;
            _graphInstanceService = graphInstanceService;
            _cleanCodeAnalysisService = cleanCodeAnalysisService;
            _gitClonePath = configuration.GetValue<string>("Workspace:GitClonePath");
        }
        
        [HttpGet]
        [Route("{id}")]
        public IActionResult GetDataSetProject([FromRoute] int id)
        {
            var result = _dataSetCreationService.GetDataSetProject(id);
            if (result.IsFailed) return BadRequest(new { message = result.Reasons[0].Message });
            return Ok(result.Value);
        }
        
        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteDataSetProject([FromRoute] int id)
        {
            var result = _dataSetCreationService.DeleteDataSetProject(id);
            if (result.IsFailed) return BadRequest(new { message = result.Reasons[0].Message });
            return Ok(result.Value);
        }
        
        [HttpPut]
        public IActionResult UpdateDataSetProject([FromBody] ProjectUpdateDTO projectDto)
        {
            var project = _mapper.Map<DataSetProject>(projectDto);
            var result = _dataSetCreationService.UpdateDataSetProject(project);
            if (result.IsFailed) return BadRequest(new { message = result.Reasons[0].Message });
            return Ok(result.Value);
        }

        [HttpPost]
        [Route("community-detection")]
        public IActionResult GetCommunities([FromBody] Graph Graph)
        {
            return Ok(_dataSetCreationService.ExportCommunities(Graph).Value);
        }

        [HttpGet]
        [Route("{id}/graph")]
        public IActionResult GetProjectWithGraphInstances([FromRoute] int id)
        {
            var result = _projectService.GetProjectWithGraphInstances(id);
            if (result.IsFailed) return BadRequest(new { message = result.Reasons[0].Message });
            return Ok(result.Value);
        }

        [HttpGet]
        [Route("{projectId}/instances/{instanceId}/graph")]
        public IActionResult GetGraphNeighboursInstances([FromRoute] int projectId, [FromRoute] int instanceId)
        {
            var result = _graphInstanceService.GetGraphInstanceWithRelatedInstances(projectId, instanceId);
            if (result.IsFailed) return BadRequest(new { message = result.Reasons[0].Message });
            return Ok(result.Value);
        }

        [HttpGet]
        [Route("{projectId}/instances/{instanceCodeSnippetId}/graph-extended")]
        public IActionResult GetGraphNeighboursInstances([FromRoute] int projectId, [FromRoute] string instanceCodeSnippetId)
        {
            var result = _graphInstanceService.GetGraphInstanceWithRelatedInstances(projectId, instanceCodeSnippetId);
            if (result.IsFailed) return BadRequest(new { message = result.Reasons[0].Message });
            return Ok(result.Value);
        }

        [HttpPost]
        [Route("{id}/export-clean-code-analysis")]
        public IActionResult ExportCleanCodeAnalysis([FromRoute] int id, [FromBody] CleanCodeAnalysisDTO analysisExportOptions)
        {
            try
            {
                var result = _cleanCodeAnalysisService.ExportProjectAnalysis(id, analysisExportOptions);
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
