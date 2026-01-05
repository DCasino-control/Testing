using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Interfaces
{
    public interface IExportable
    {
        void ExportToPDF(string filePath);
        void ExportToExcel(string filePath);
    }
}

