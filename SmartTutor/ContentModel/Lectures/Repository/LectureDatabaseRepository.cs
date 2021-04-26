﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SmartTutor.Database;

namespace SmartTutor.ContentModel.Lectures.Repository
{
    public class LectureDatabaseRepository : ILectureRepository
    {
        private readonly SmartTutorContext _dbContext;

        public LectureDatabaseRepository(SmartTutorContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Lecture> GetLectures()
        {
            return _dbContext.Lectures.Include(l => l.KnowledgeNodes).ToList();
        }

        public List<KnowledgeNode> GetKnowledgeNodes(int id)
        {
            var lecture = _dbContext.Lectures.Where(l => l.Id == id).Include(l => l.KnowledgeNodes).FirstOrDefault();
            return lecture?.KnowledgeNodes;
        }

        public KnowledgeNode GetKnowledgeNodeWithSummaries(int id)
        {
            return _dbContext.KnowledgeNodes.Where(n => n.Id == id).Include(n => n.LearningObjectSummaries).FirstOrDefault();
        }
    }
}