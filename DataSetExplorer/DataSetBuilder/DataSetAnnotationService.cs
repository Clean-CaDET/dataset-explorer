﻿using DataSetExplorer.DataSetBuilder.Model;

using DataSetExplorer.DataSetBuilder.Model.Repository;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataSetExplorer.DataSetBuilder
{
    public class DataSetAnnotationService : IDataSetAnnotationService
    {
        private readonly IDataSetInstanceRepository _dataSetInstanceRepository;
        private readonly IDataSetAnnotationRepository _dataSetAnnotationRepository;

        public DataSetAnnotationService(IDataSetInstanceRepository dataSetInstanceRepository, IDataSetAnnotationRepository dataSetAnnotationRepository)
        {
            _dataSetInstanceRepository = dataSetInstanceRepository;
            _dataSetAnnotationRepository = dataSetAnnotationRepository;
        }

        public Result<DataSetAnnotation> AddDataSetAnnotation(DataSetAnnotation annotation, int dataSetInstanceId, int annotatorId)
        {
            var instance = _dataSetInstanceRepository.GetDataSetInstance(dataSetInstanceId);
            if (instance == default) return Result.Fail<DataSetAnnotation>($"DataSetInstance with id: {dataSetInstanceId} does not exist.");
            var annotator = _dataSetAnnotationRepository.GetAnnotator(annotatorId);
            if (annotator == default) return Result.Fail<DataSetAnnotation>($"Annotator with id: {annotatorId} does not exist.");
            annotation.Annotator = annotator;
            instance.AddAnnotation(annotation);
            _dataSetInstanceRepository.Update(instance);
            return Result.Ok(annotation);
        }

        public Result<DataSetAnnotation> UpdateAnnotation(DataSetAnnotation changed, int annotationId, int annotatorId)
        {
            var annotation = _dataSetAnnotationRepository.GetDataSetAnnotation(annotationId);
            if (annotation == default) return Result.Fail<DataSetAnnotation>($"DataSetAnnotation with id: {annotationId} does not exist.");
            if (annotation.Annotator.Id != annotatorId) return Result.Fail<DataSetAnnotation>($"Only creator can update annotation.");
            annotation.Update(changed);
            _dataSetAnnotationRepository.Update(annotation);
            return Result.Ok(annotation);
        }
    }
}
