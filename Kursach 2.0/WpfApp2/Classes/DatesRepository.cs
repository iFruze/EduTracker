using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Classes
{
    internal static class DatesRepository
    {
        public static Dates GetDateById(int id) => TeachHoursEntities2.GetContext().Dates.FirstOrDefault(d => d.id == id);
        public static bool ChangeDate(int id, DateTime newDate)
        {
            bool result = false;
            Dates date = GetDateById(id);
            if (date != null)
            {
                date.date = newDate;
                TeachHoursEntities2.GetContext().SaveChanges();
                result = true;
            }
            return result;
        }
        public static void Add(Dates date)
        {
            TeachHoursEntities2.GetContext().Dates.Add(date);
            TeachHoursEntities2.GetContext().SaveChanges();
        }
        public static void Delete(Dates date)
        {
            TeachHoursEntities2.GetContext().Dates.Remove(date);
            TeachHoursEntities2.GetContext().SaveChanges();
        }
        public static void DeleteRange(List<Dates> dates)
        {
            TeachHoursEntities2.GetContext().Dates.RemoveRange(dates);
            TeachHoursEntities2.GetContext().SaveChanges();
        }
    }
}
