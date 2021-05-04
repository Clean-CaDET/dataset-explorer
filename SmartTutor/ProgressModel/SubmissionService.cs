﻿using SmartTutor.ContentModel.LearningObjects.ArrangeTasks;
using SmartTutor.ContentModel.LearningObjects.Challenges;
using SmartTutor.ContentModel.LearningObjects.Challenges.FunctionalityTester;
using SmartTutor.ContentModel.LearningObjects.Questions;
using SmartTutor.ContentModel.LearningObjects.Repository;
using SmartTutor.LearnerModel.Workspaces.Repository;
using SmartTutor.ProgressModel.Submissions;
using SmartTutor.ProgressModel.Submissions.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartTutor.ProgressModel
{
    public class SubmissionService : ISubmissionService
    {
        private readonly ILearningObjectRepository _learningObjectRepository;
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IWorkspaceRepository _workspaceRepository;

        public SubmissionService(ILearningObjectRepository learningObjectRepository, ISubmissionRepository submissionRepository,
            IWorkspaceRepository workspaceRepository)
        {
            _learningObjectRepository = learningObjectRepository;
            _submissionRepository = submissionRepository;
            _workspaceRepository = workspaceRepository;
        }

        public ChallengeEvaluation EvaluateChallenge(ChallengeSubmission submission)
        {
            Challenge challenge = _learningObjectRepository.GetChallenge(submission.ChallengeId);
            if (challenge == null) return null;

            string workspacePath = _workspaceRepository.GetById(submission.LearnerId).Path;
            var evaluation = challenge.CheckChallengeFulfillment(submission.SourceCode, new FileFunctionalityTester(workspacePath));

            if (evaluation.ChallengeCompleted) submission.MarkCorrect();
            _submissionRepository.SaveChallengeSubmission(submission);

            //TODO: Tie in with Instructor and handle learnerId to get suitable LO for LO summaries.
            evaluation.ApplicableLOs =
                _learningObjectRepository.GetFirstLearningObjectsForSummaries(
                    evaluation.ApplicableHints.GetDistinctLearningObjectSummaries());
            evaluation.SolutionLO = _learningObjectRepository.GetLearningObjectForSummary(challenge.Solution.Id);

            return evaluation;
        }

        public List<AnswerEvaluation> EvaluateAnswers(QuestionSubmission submission)
        {
            var question = _learningObjectRepository.GetQuestion(submission.QuestionId);
            var evaluations = question.EvaluateAnswers(submission.SubmittedAnswerIds);

            if (evaluations.Select(a => a.SubmissionWasCorrect).All(c => c)) submission.MarkCorrect();
            _submissionRepository.SaveQuestionSubmission(submission);

            return evaluations;
        }

        public List<ArrangeTaskContainerEvaluation> EvaluateArrangeTask(ArrangeTaskSubmission submission)
        {
            var arrangeTask = _learningObjectRepository.GetArrangeTask(submission.ArrangeTaskId);
            var evaluations = arrangeTask.EvaluateSubmission(submission.Containers);
            if (evaluations == null) throw new InvalidOperationException("Invalid submission of arrange task.");

            if (evaluations.Select(e => e.SubmissionWasCorrect).All(c => c)) submission.MarkCorrect();
            _submissionRepository.SaveArrangeTaskSubmission(submission);

            return evaluations;
        }
    }
}
