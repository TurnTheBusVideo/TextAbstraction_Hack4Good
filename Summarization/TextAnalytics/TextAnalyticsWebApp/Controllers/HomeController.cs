using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TextAnalytics;
using TextAnalytics.Models;

namespace TextAnalyticsWebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TextEntityLinking(string inputFile)
        {
            //var response = AnalyticsClient.RecognizeLinkedEntities(
            //    "Microsoft was founded by Bill Gates and Paul Allen on April 4, 1975, " +
            //    "to develop and sell BASIC interpreters for the Altair 8800. " +
            //    "During his career at Microsoft, Gates held the positions of chairman, " +
            //    "chief executive officer, president and chief software architect, " +
            //    "while also being the largest individual shareholder until May 2014.");
            string[] lines = System.IO.File.ReadAllLines(inputFile);
            var keyPhraseExtract = new KeyPhraseExtract();
            //It seems that this path is not used.
            List<RawTextAnalytics> result = keyPhraseExtract.ExtractEntities(lines, "Default");
            return PartialView(result);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}