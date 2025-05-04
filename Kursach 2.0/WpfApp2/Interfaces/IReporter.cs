using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Interfaces
{
    internal interface IReporter
    {
        void GenerateReport(int teacherId, List<List<string>> resultReport, string month, string year);
    }
}
