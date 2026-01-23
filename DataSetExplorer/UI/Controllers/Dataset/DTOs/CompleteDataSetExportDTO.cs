using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace DataSetExplorer.UI.Controllers.Dataset.DTOs
{
    public class CompleteDataSetExportDTO
    {
        public List<IFormFile> DraftDatasetFiles { get; set; }
    }
}
