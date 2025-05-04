using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    internal static class SubjectsRepository
    {
        public static Subjects GetSubjectById(int id) => TeachHoursEntities2.GetContext().Subjects.FirstOrDefault(s => s.id == id);
        public static bool ChangeNameSubject(int id, string newName)
        {
            bool result = false;
            Subjects subject = GetSubjectById(id);
            if (subject != null)
            {
                subject.name = newName;
                TeachHoursEntities2.GetContext().SaveChanges();
                result = true;
            }
            return result;
        }
        public static void Add(Subjects subject)
        {
            TeachHoursEntities2.GetContext().Subjects.Add(subject);
            TeachHoursEntities2.GetContext().SaveChanges();
        }
        public static void Delete(Subjects subject)
        {
            TeachHoursEntities2.GetContext().Subjects.Remove(subject);
            TeachHoursEntities2.GetContext().SaveChanges();
        }
        public static void DeleteRange(List<Subjects> subjects)
        {
            TeachHoursEntities2.GetContext().Subjects.RemoveRange(subjects);
            TeachHoursEntities2.GetContext().SaveChanges();
        }
    }
}
