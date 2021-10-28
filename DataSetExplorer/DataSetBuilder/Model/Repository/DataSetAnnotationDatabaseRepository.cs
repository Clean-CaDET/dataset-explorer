﻿using DataSetExplorer.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataSetExplorer.DataSetBuilder.Model.Repository
{
    public class DataSetAnnotationDatabaseRepository : IDataSetAnnotationRepository
    {
        private readonly DataSetExplorerContext _dbContext;

        public DataSetAnnotationDatabaseRepository(DataSetExplorerContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Annotation GetDataSetAnnotation(int id)
        {
            return _dbContext.DataSetAnnotations
                .Include(a => a.Annotator)
                .Include(a => a.ApplicableHeuristics)
                .FirstOrDefault(a => a.Id == id);
        }
        public Annotator GetAnnotator(int id)
        {
            return _dbContext.Annotators.FirstOrDefault(a => a.Id == id);
        }

        public CodeSmell GetCodeSmell(string name)
        {
            return _dbContext.CodeSmells.FirstOrDefault(s => s.Name.Equals(name));
        }

        public void Update(Annotation annotation)
        {
            _dbContext.Update(annotation);
            _dbContext.SaveChanges();
        }
    }
}
