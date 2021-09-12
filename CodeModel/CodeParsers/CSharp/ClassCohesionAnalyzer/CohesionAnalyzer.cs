﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CodeModel.CaDETModel.CodeItems;

namespace CodeModel.CodeParsers.CSharp.ClassCohesionAnalyzer
{
    public class CohesionAnalyzer
    {
        private const double MaximumLackOfCohesion = 0.4;
        private const int MaximumCohesivePartsCount = 2;
        private ICohesionMetric CohesionMetric { get; }

        public CohesionAnalyzer(ICohesionMetric cohesionMetric)
        {
            CohesionMetric = cohesionMetric;
        }

        public List<CohesiveParts> IdentifyCohesiveParts(CaDETClass parsedClass)
        {
            ResultMapper resultMapper = new ResultMapper(parsedClass);
            ClassPart classPart = new ClassPart(resultMapper);

            IEnumerable<CohesiveParts> possibleParts = GetAllPossibleParts(classPart);

            var cohesiveParts = FilterHighlyCohesiveParts(possibleParts).ToList();

            return cohesiveParts;
        }

        private IEnumerable<CohesiveParts> FilterHighlyCohesiveParts(IEnumerable<CohesiveParts> possibleParts)
        {
            return possibleParts.Where(cohesiveParts =>
                cohesiveParts.Parts.TrueForAll(part => CohesionMetric.Calculate(part) < MaximumLackOfCohesion));
        }

        private IEnumerable<CohesiveParts> GetAllPossibleParts(ClassPart classPart)
        {
            var accessesToBeRemoved = classPart.GetAccessesThatCanBeRemoved();
            var possibleParts = new List<CohesiveParts>();
            foreach (var accesses in accessesToBeRemoved)
            {
                if (possibleParts.Any(s => accesses.IsProperSupersetOf(s.AccessesToRemove))) continue;
                var parts = FindValidCohesiveParts(accesses, new HashSet<Access>(classPart.Accesses));
                if (parts is not { Count: MaximumCohesivePartsCount }) continue;
                var cohesiveParts = new CohesiveParts(accesses, parts);
                possibleParts.RemoveAll(s => s.AccessesToRemove.IsProperSupersetOf(cohesiveParts.AccessesToRemove));
                possibleParts.Add(cohesiveParts);
            }

            return possibleParts;
        }

        private List<HashSet<Access>> FindValidCohesiveParts(HashSet<Access> accessesToRemove, HashSet<Access> accesses)
        {
            List<HashSet<Access>> result = new List<HashSet<Access>>();
            accesses.ExceptWith(accessesToRemove);

            var lastGroup = new HashSet<Access>();
            var foundAccesses = FindConnectedAccesses(accesses);
            while (foundAccesses != null)
            {
                result.Add(foundAccesses);
                if (result.Count == MaximumCohesivePartsCount) break;
                lastGroup = accesses;
                accesses.ExceptWith(foundAccesses);
                foundAccesses = FindConnectedAccesses(accesses);
            }

            result.Add(lastGroup);

            return result;
        }

        private HashSet<Access> FindConnectedAccesses(IReadOnlyCollection<Access> accessesToCheck)
        {
            if (accessesToCheck.Count == 0) return null;
            var foundAccesses = SearchForConnectedAccesses(accessesToCheck);
            return foundAccesses.Count != accessesToCheck.Count ? foundAccesses : null;
        }

        private HashSet<Access> SearchForConnectedAccesses(IReadOnlyCollection<Access> accesses)
        {
            List<int> visitedMethods = new List<int>();
            List<int> visitedFields = new List<int>();
            HashSet<Access> collectedAccesses = new HashSet<Access>();
            HashSet<Access> accessesToVisit = new HashSet<Access> { accesses.First() };

            while (accessesToVisit.Count != 0)
            {
                HashSet<Access> accessesForNextIteration = new HashSet<Access>();
                foreach (var access in accessesToVisit)
                {
                    if (!visitedMethods.Contains(access.Method))
                    {
                        List<Access> accessesForMethod = accesses.Where(acs => acs.Method == access.Method).ToList();
                        accessesForMethod.ForEach(e => accessesForNextIteration.Add(e));
                        visitedMethods.Add(access.Method);
                    }

                    if (visitedFields.Contains(access.Field)) continue;
                    List<Access> accessesForField = accesses.Where(acs => acs.Field == access.Field).ToList();
                    accessesForField.ForEach(e => accessesForNextIteration.Add(e));
                    visitedFields.Add(access.Field);
                }

                foreach (var visitedAccesses in accessesToVisit)
                {
                    collectedAccesses.Add(visitedAccesses);
                }

                accessesToVisit = accessesForNextIteration;
            }

            return collectedAccesses;
        }
    }
}