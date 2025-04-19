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

    }
}
