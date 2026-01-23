using DataSetExplorer.Core.DataSets.Model;

namespace DataSetExplorer.Core.DataSetSerializer
{
    public interface IDraftDataSetExportationService
    {
        public string Export(int annotatorId, DataSet dataSet);
    }
}
