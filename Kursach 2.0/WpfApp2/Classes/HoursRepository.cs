using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Classes
{
    internal static class HoursRepository
    {
        public static Hours GetHourById(int id) => TeachHoursEntities2.GetContext().Hours.FirstOrDefault(h => h.id == id);
        public static List<Hours> GetTeacherHours(int teachId) => TeachHoursEntities2.GetContext().Hours.Where(h => h.teacherId == teachId).ToList();
        public static void Add(Hours hour)
        {
            TeachHoursEntities2.GetContext().Hours.Add(hour);
            TeachHoursEntities2.GetContext().SaveChanges();
        }
        public static void Delete(Hours hour)
        {
            TeachHoursEntities2.GetContext().Hours.Remove(hour);
            TeachHoursEntities2.GetContext().SaveChanges();
        }
        public static void DeleteRange(List<Hours> hours)
        {
            TeachHoursEntities2.GetContext().Hours.RemoveRange(hours);
            TeachHoursEntities2.GetContext().SaveChanges();
        }
    }
}
