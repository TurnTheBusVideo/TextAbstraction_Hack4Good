using Azure.AI.TextAnalytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAnalytics.Models
{
    public class RawTextAnalytics
    {
        public string Line;

        public int LineHash { get; set; }
        
        public int LineNumber { get; set; }

        public CategorizedEntityCollection entities { get; set; }

        public double SentenseConfidenceScore { get; set; }

        public double NormalizeConfidenceScore { get; set; }
    }
}
