using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    internal class AllSubjectHours
    {
        public int Id { get ; set; }
        public string Date { get; set; }
        public string Subject {  get; set; }
        public AllSubjectHours(int id, DateTime date, string subject)
        {
            this.Id = id;
            this.Date = date.ToShortDateString();
            this.Subject = subject;
        }
    }
}
