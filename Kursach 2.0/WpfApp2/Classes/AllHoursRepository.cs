using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Classes
{
    internal static class AllHoursRepository
    {
        public static List<AllHours> GetAllHoursOfCurrentTeacher(int currentTeacherId) => TeachHoursEntities2.GetContext().AllHours.Where(t => t.teacherId == currentTeacherId).ToList();
        public static AllHours GetTeacherHourByName(int teachId, string sbjName) => TeachHoursEntities2.GetContext().AllHours.FirstOrDefault(s => s.subjectName == sbjName && s.teacherId == teachId);
        public static AllHours GetHourById(int id) => TeachHoursEntities2.GetContext().AllHours.FirstOrDefault(h => h.id == id);
        public static void SubtractHours(AllHours hour, int hours)
        {
            hour.countHours -= hours;
            if (hour.countHours < 0)
            {
                hour.countHours = 0;
            }
            TeachHoursEntities2.GetContext().SaveChanges();
        }
        public static void Add(AllHours hour)
        {
            TeachHoursEntities2.GetContext().AllHours.Add(hour);
            TeachHoursEntities2.GetContext().SaveChanges();
        }
        public static void Delete(AllHours hour)
        {
            TeachHoursEntities2.GetContext().AllHours.Remove(hour);
            TeachHoursEntities2.GetContext().SaveChanges();
        }
        public static void ChangeCountOfHours(AllHours hour, int countOfHours)
        {
            hour.countHours = countOfHours;
            TeachHoursEntities2.GetContext().SaveChanges();
        }
    }
}
