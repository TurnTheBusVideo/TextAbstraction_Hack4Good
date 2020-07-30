using Azure;
using System;
using System.Globalization;
using Azure.AI.TextAnalytics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace TextAnalytics
{
    class Program
    {
        static void Main(string[] args)
        {
            var summarization = new Summarization();

            Console.WriteLine("Enter the input file (e.g. D:\\hack\\Summarization\\Panipath.txt): ");
            // string file = Console.ReadLine();
            string inputFile = @".\TestData.txt";

            Console.WriteLine("Enter pecentage of abstraction (e.g. 10):");
            string percentage = Console.ReadLine();

            Console.Write("Enter the target language of translation(e.g. 'hi'):");
            var targetLanguage = Console.ReadLine();
            string[] lines = System.IO.File.ReadAllLines(inputFile);

            var allLines = new List<string>();

            foreach(string line in lines)
            {
                string[] linesSplitWithDot = Regex.Split(line, @"\.");
                foreach(var oneLine in linesSplitWithDot)
                {
                    var oneLineWithDot = oneLine+ ".";
                    allLines.Add(oneLineWithDot);
                }
            }

            var keyPhaseExtractor = new KeyPhraseExtract();
            keyPhaseExtractor.ExtractKeyPhrase(allLines);
            keyPhaseExtractor.UpdateStackRank();

            summarization.allInputLineDelimitedByDot = allLines;
            summarization.supportedLanguages = summarization.GetSupportedAzureTranslationLanguages();
            summarization.summaryTextAnalyticsList = keyPhaseExtractor.CreateSummary(percentage);
            summarization.targetTranslationLanguage = targetLanguage;
            summarization.translatedText = summarization.TranslateSummary();

            summarization.PrintSummary(summarization);
            summarization.PrintTranslatedSummary(summarization);

            Console.Write("Press any key to exit.");
            Console.ReadKey();
        }

        
    }
}
