using Azure.AI.TextAnalytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TextAnalytics.Models;

namespace TextAnalytics
{
    public class SentenseConfidencePostProcessor
    {
        public static void Process(List<RawTextAnalytics> sentences, string textType)
        {
            foreach(var sentence in sentences)
            {
                sentence.SentenseConfidenceScore = 0;
                foreach(var entity in sentence.entities)
                {
                    sentence.SentenseConfidenceScore += TextAnalysisConfidenceScore(entity.Category, textType) * entity.ConfidenceScore;
                }
            }

            CalculateNormalizeConfidenceScore(sentences);
        }


        private static void CalculateNormalizeConfidenceScore(List<RawTextAnalytics> sentences)
        {
            double maxConfidence = GetMaxSentenseConfidence(sentences);
            foreach(var sentence in sentences)
            {
                sentence.NormalizeConfidenceScore = (sentence.SentenseConfidenceScore / maxConfidence) * 100;
            }
        }
        
        private static double GetMaxSentenseConfidence(List<RawTextAnalytics> sentences)
        {
            double maxConfidence = 0;
            foreach (var sentence in sentences)
            {
                if(sentence.SentenseConfidenceScore > maxConfidence)
                {
                    maxConfidence = sentence.SentenseConfidenceScore;
                }
            }

            return maxConfidence;
        }

        //Score entity type in 100
        private static int TextAnalysisConfidenceScore(EntityCategory entityCategory, string textType)
        {

            switch (textType) {
                case "Sports":
                    return SportsTextAnalysisConfidenceScore(entityCategory);
                case "Science":
                    return ScienceTextAnalysisConfidenceScore(entityCategory);
                case "Energy":
                    return EnergyTextAnalysisConfidenceScore(entityCategory);
                default:
                    return DefaultTextAnalysisConfidenceScore(entityCategory);
            }
        }

        private static int SportsTextAnalysisConfidenceScore(EntityCategory entityCategory)
        {
            if (entityCategory == EntityCategory.Event)
            {
                return 100;
            }
            else if (entityCategory == EntityCategory.Person
                || entityCategory == EntityCategory.PersonType
                || entityCategory == EntityCategory.Location
                || entityCategory == EntityCategory.DateTime
                || entityCategory == EntityCategory.Skill)
            {
                return 70;
            }
            else if (entityCategory == EntityCategory.Organization)
            {
                return 60;
            }
            else
            {
                return 50;
            }
        }
        private static int DefaultTextAnalysisConfidenceScore(EntityCategory entityCategory)
        {
            if (entityCategory == EntityCategory.DateTime)
            {
                return 100;
            }
            else if (entityCategory == EntityCategory.Person
                || entityCategory == EntityCategory.Location)
            {
                return 70;
            }
            else if (entityCategory == EntityCategory.Organization)
            {
                return 60;
            }
            else
            {
                return 50;
            }
        }

        private static int ScienceTextAnalysisConfidenceScore(EntityCategory entityCategory)
        {
            if (entityCategory == EntityCategory.Skill)
            {
                return 100;
            }
            else if (entityCategory == EntityCategory.Quantity
                || entityCategory == EntityCategory.Location)
            {
                return 70;
            }
            else if (entityCategory == EntityCategory.Organization)
            {
                return 60;
            }
            else
            {
                return 50;
            }
        }

        private static int EnergyTextAnalysisConfidenceScore(EntityCategory entityCategory)
        {
            if (entityCategory == EntityCategory.Quantity)
            {
                return 100;
            }
            else if (entityCategory == EntityCategory.Person
                || entityCategory == EntityCategory.Location
                || entityCategory == EntityCategory.Product)
            {
                return 70;
            }
            else if (entityCategory == EntityCategory.Organization)
            {
                return 60;
            }
            else
            {
                return 50;
            }
        }
    }
}
