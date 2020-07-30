using Azure;
using System;
using System.Globalization;
using Azure.AI.TextAnalytics;
using TextAnalytics.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TextAnalytics
{
    public class KeyPhraseExtract
    {
        public List<RawTextAnalytics> RawTextAnalyticsList { get; private set; } = new List<RawTextAnalytics>();
        private static readonly AzureKeyCredential credentials = new AzureKeyCredential("36c308179f1d41dab49be14b3c9fc967");
        private static readonly Uri endpoint = new Uri("https://ttbanalyticsapp.cognitiveservices.azure.com/");
        private string testType = string.Empty;

       public  List<RawTextAnalytics> ExtractEntities(IList<string> lines, string textType)
        {
            testType = textType;
            IList<string> preprossedlines = PreprossedTextFileLines(lines);
            ExtractKeyPhrase(preprossedlines);
            UpdateStackRank();
            return RawTextAnalyticsList;

        }

        private IList<string> PreprossedTextFileLines(IList<string> lines) {

            var allLines = new List<string>();
            foreach (string line in lines)
            {
                string[] linesSplitWithDot = Regex.Split(line, @"\.");
                foreach (var oneLine in linesSplitWithDot)
                {
                    var oneLineWithDot = oneLine + ".";
                    allLines.Add(oneLineWithDot);
                }
            }

            return allLines;
        }
        public void ExtractKeyPhrase(IList<string> lines)
        {
            var rawTextAnalyticsList = new List<RawTextAnalytics>();
            Console.WriteLine("Extracting entity:- ");
            var client = new TextAnalyticsClient(endpoint, credentials);
            int lineNumber = 0;
            foreach (var line in lines)
            {
                Console.Write(".");
                var rawTextAnalytics = new RawTextAnalytics();
                if (line == string.Empty)
                    continue;
                rawTextAnalytics.Line = line;
                rawTextAnalytics.LineHash = line.GetHashCode();
                rawTextAnalytics.LineNumber = ++lineNumber;
                rawTextAnalytics.entities =  KeyPhraseExtractionPerLine(client, line);
                rawTextAnalyticsList.Add(rawTextAnalytics);
            }

            this.RawTextAnalyticsList = rawTextAnalyticsList;
        }

        /// <summary>
        /// This is our main logic to give stack rank. Further refinement could be 
        /// Current logic is based on entity count.
        /// TODo:-
        /// 1. Give weightage to entities (e.g. DateTime could have highest weightage. EntityCategory class defines it. Person;Address;Quantity;IPAddress;Email;PhoneNumber;DateTime;Url;Product;Event;Organization;Location;PersonType;Skill;
        /// 2. Can we consise each line further by removing brackets and extra information 
        /// 3. We should account for confindance score for each entity.
        /// </summary>
        public void UpdateStackRank()
        {
            SentenseConfidencePostProcessor.Process(RawTextAnalyticsList, testType);
        }

        /// <summary>
        /// Todo: Translate text from english to Hindi.
        /// </summary>
        /// <param name="percentage"></param>
        public List<RawTextAnalytics> CreateSummary(string percentage)
        {
            List<RawTextAnalytics> summaryTextAnalyticsList = new List<RawTextAnalytics>();
            foreach (var line in RawTextAnalyticsList)
            {
                Console.Write(".");
                if (line.NormalizeConfidenceScore >= (100 - int.Parse(percentage)))
                {
                    summaryTextAnalyticsList.Add(line);
                }
            }

            // sorting ascending order based on line number
            summaryTextAnalyticsList.Sort(delegate (RawTextAnalytics x, RawTextAnalytics y)
            {
                return x.LineNumber > y.LineNumber ? 1 : -1;

            });

            return summaryTextAnalyticsList;
        }
        private CategorizedEntityCollection KeyPhraseExtractionPerLine(TextAnalyticsClient client, string line)
        {
            var response = client.RecognizeEntities(line);

            return response.Value;
        }
    }
}
