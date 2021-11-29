using DataSetExplorer.DataSetBuilder.Model;
using FluentResults;
using System.Collections.Generic;

namespace DataSetExplorer
{
    public interface IDataSetAnalysisService
    {
        Result<string> FindInstancesWithAllDisagreeingAnnotations(IDictionary<string, string> projects);
        Result<string> FindInstancesRequiringAdditionalAnnotation(IDictionary<string, string> projects);
        Result<List<SmellCandidateInstances>> FindInstancesWithAllDisagreeingAnnotations(int projectId);
        Result<List<SmellCandidateInstances>> FindInstancesRequiringAdditionalAnnotation(int projectId);
        Result<string> ExportMembersFromAnnotatedClasses(IDictionary<string, string> projects, string datasetPath, string outputFolder);
    }
}
