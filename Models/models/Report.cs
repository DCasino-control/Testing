using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Interfaces;

namespace Testing.Models.models
{
    public abstract class Report : IExportable
    {
        public string ReportTitle { get; set; }
        public DateTime GeneratedDate { get; set; }

        protected Report(string title)
        {
            ReportTitle = title;
            GeneratedDate = DateTime.Now;
        }

        // Kani kay abstract methods
        public abstract void Generate();
        public abstract void Display();

        // mao ni ang implementation sa Interface 
        public virtual void ExportToPDF(string filePath)
        {
            // Kani kay module sa pag export sa PDF
        }

        public virtual void ExportToExcel(string filePath)
        {
            // Kani kay module sa pag export sa Excel
        }
    }
}

