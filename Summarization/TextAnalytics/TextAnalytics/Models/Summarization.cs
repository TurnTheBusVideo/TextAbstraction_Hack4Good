using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TextAnalytics.Models;
using translate_sample;

namespace TextAnalytics
{
    public class Summarization
    {
        private const string subscriptionKey = "96911584f87544f7bb8e700580d3223c";

        /// <summary>
        /// Stores list of supported language which can be used for translation
        /// </summary>
        public List<SupportedLanguage>  supportedLanguages = new List<SupportedLanguage>();

        /// <summary>
        /// Stores summaried text in English
        /// </summary>
        public List<RawTextAnalytics> summaryTextAnalyticsList = new List<RawTextAnalytics>();

        /// <summary>
        /// Stores the detected langauge for the text, it would be moslty ENU for now
        /// </summary>
        public string detectedLangauge { get; set; }

        /// <summary>
        /// Stores the target translation language, it should be some value from supportedLanguages.Code
        /// </summary>
        public string targetTranslationLanguage { get; set; }

        /// <summary>
        /// Stores the percentage of summarization needed
        /// </summary>
        public string percentage { get; set; }

        /// <summary>
        /// Stores all the input line delimited by dots.
        /// </summary>
        public List<string> allInputLineDelimitedByDot = new List<string>();

        public List<string> translatedText = new List<string>();

        public async Task<string> TranslateTextRequest(string inputText, string targetLanguage = "hi")
        {
            /*
             * The code for your call to the translation service will be added to this
             * function in the next few sections.
             */
            object[] body = new object[] { new { Text = inputText } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // In the next few sections you'll add code to construct the request.
                // Build the request.
                // Set the method to Post.
                request.Method = HttpMethod.Post;
                // Construct the URI and add headers.
                request.RequestUri = new Uri(string.Format("https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&to={0}", targetLanguage));
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                request.Headers.Add("Ocp-Apim-Subscription-Region", "westeurope");

                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                // Read response as a string.
                string result = await response.Content.ReadAsStringAsync();
                // Deserialize the response using the classes created earlier.
                TranslationResult[] deserializedOutput = JsonConvert.DeserializeObject<TranslationResult[]>(result);
                // Iterate over the deserialized results.
                foreach (TranslationResult o in deserializedOutput)
                {
                    // Print the detected input language and confidence score.
                    Console.WriteLine("Detected input language: {0}\nConfidence score: {1}\n", o.DetectedLanguage.Language, o.DetectedLanguage.Score);
                    // Iterate over the results and print each translation.
                    foreach (Translation t in o.Translations)
                    {
                        Console.WriteLine("Translated to {0}: {1}", t.To, t.Text);
                        return t.Text;
                    }
                }
                return null;
            }
        }

        public List<SupportedLanguage> GetSupportedAzureTranslationLanguages()
        {
            var supportedLanguages = new List<SupportedLanguage>();

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Set the method to GET
                request.Method = HttpMethod.Get;
                // Construct the full URI
                request.RequestUri = new Uri("https://api.cognitive.microsofttranslator.com/languages?api-version=3.0");
                // Send request, get response
                var response = client.SendAsync(request).Result;

                var jsonResponse = response.Content.ReadAsStringAsync().Result;

                var allTypes = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);
                var translation = allTypes["translation"];

                foreach (var lan in JsonConvert.DeserializeObject<Dictionary<string, object>>(translation.ToString()))
                {
                    var language = JsonConvert.DeserializeObject<Dictionary<string, object>>(lan.Value.ToString());
                    var langCode = lan.Key;

                    supportedLanguages.Add(new SupportedLanguage() { Code = langCode, DisplayName = language["name"].ToString() });
                }
                
                return supportedLanguages;
            }
        }

        public List<string> TranslateSummary()
        {
            foreach(var line in summaryTextAnalyticsList)
            {
                translatedText.Add(TranslateTextRequest(line.Line, targetTranslationLanguage).Result);
            }

            return translatedText;
        }

        public void PrintSummary(Summarization summarization)
        {
            Console.WriteLine("=====================");
            Console.WriteLine($"Total line count {summarization.summaryTextAnalyticsList.Count}");
            Console.WriteLine($"Summart line count {summarization.summaryTextAnalyticsList.Count}");

            Console.WriteLine("Summar of the Text");
            foreach (var summaryLine in summarization.summaryTextAnalyticsList)
            {
                Console.WriteLine(summaryLine.Line);
            }
            Console.WriteLine("=====================");
        }

        public void PrintTranslatedSummary(Summarization summarization)
        {
            Console.WriteLine("=====================");
            Console.WriteLine($"Total line count {summarization.summaryTextAnalyticsList.Count}");
            Console.WriteLine($"Summart line count {summarization.translatedText.Count}");

            Console.WriteLine("Summar of the Text");
            foreach (var summaryLine in summarization.translatedText)
            {
                Console.WriteLine(summaryLine);
            }
            Console.WriteLine("=====================");
        }
    }
}
