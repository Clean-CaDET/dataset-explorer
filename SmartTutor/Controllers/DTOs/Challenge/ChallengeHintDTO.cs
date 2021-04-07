﻿using SmartTutor.Controllers.DTOs.Lecture;
using System.Collections.Generic;

namespace SmartTutor.Controllers.DTOs.Challenge
{
    public class ChallengeHintDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public LearningObjectDTO LearningObject { get; set; }
        public List<string> ApplicableToCodeSnippets { get; set; }
    }
}
