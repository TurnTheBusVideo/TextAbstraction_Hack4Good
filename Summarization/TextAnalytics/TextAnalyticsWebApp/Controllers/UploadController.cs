using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TextAnalytics;
using TextAnalytics.Models;

namespace TextAnalyticsWebApp.Controllers
{
    public class UploadController : Controller
    {
        // GET: Upload  
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult UploadFile()
        {
            var summarization = new Summarization();
            summarization.supportedLanguages = summarization.GetSupportedAzureTranslationLanguages();
            ViewBag.supportedLanguages = ToSelectList(summarization.supportedLanguages);
            return View();
        }

        [NonAction]
        public SelectList ToSelectList(List<SupportedLanguage> table)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (var row in table)
            {
                list.Add(new SelectListItem()
                {
                    Text = row.DisplayName,
                    Value = row.Code
                });
            }

            return new SelectList(list, "Value", "Text");
        }
        [HttpPost]
        public FileStreamResult DownloadFile(string fileContent)
        {

            var byteArray =  Encoding.Unicode.GetBytes(string.IsNullOrEmpty(fileContent)? string.Empty:fileContent);
            var stream = new MemoryStream(byteArray);

            return File(stream, System.Net.Mime.MediaTypeNames.Application.Octet, "summary.txt");
        }
            [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file, string abstractPercent = "20", string targetLanguage = "hi", string textType = "Default")
        {
            try
            {
                var summarization = new Summarization();
                summarization.supportedLanguages = summarization.GetSupportedAzureTranslationLanguages();
                ViewBag.supportedLanguages = ToSelectList(summarization.supportedLanguages);

                if (file?.ContentLength > 0 || file?.ContentLength < 22528)
                {
                    if (!string.IsNullOrEmpty(file.FileName))
                    {
                        string extension = Path.GetExtension(file.FileName);
                        if (!".txt".Equals(extension?.ToLower()))
                        { 
                            ViewBag.Message = "Please upload text file with extension '.txt'";
                            return View();
                        }
                        
                        var lines = new List<string>();
                        using (StreamReader reader = new StreamReader(file.InputStream))
                        {
                            do
                            {
                                string textLine = reader.ReadLine();
                                if (!string.IsNullOrEmpty(textLine))
                                    lines.Add(textLine);
                            } while (reader.Peek() != -1);
                        }
                        var keyPhraseExtract = new KeyPhraseExtract();
                        List<RawTextAnalytics> result = keyPhraseExtract.ExtractEntities(lines, textType);
                        ViewBag.Message = "File Analyzed Successfully!!";
                        ViewBag.logFileContent = String.Join("",lines);
                        ViewBag.AnalyzedFileContent = result;

                        summarization.allInputLineDelimitedByDot = lines;
                        summarization.summaryTextAnalyticsList = keyPhraseExtract.CreateSummary(abstractPercent);
                        summarization.targetTranslationLanguage = targetLanguage; // ToDo Use summarization.supportedLanguages to take input
                        summarization.translatedText = summarization.TranslateSummary();

                        ViewBag.SummarizedContent = string.Join("",summarization.translatedText);
                    }
                }
                else
                {
                    ViewBag.Message = "File content is too less(<0) or too long(>22528).";
                }
                return View();
            }
            catch(Exception ex)
            {
                ViewBag.Message = "File upload failed!!";
                return View();
            }
        }
    }
}