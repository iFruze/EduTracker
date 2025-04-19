using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.Interfaces;

namespace WpfApp2.Classes
{
    internal static class TeachersRepository
    {
        public static Dictionary<string, string> GetTeachersDictionary() => TeachHoursEntities2.GetContext().Teachers.ToDictionary(teach => teach.login, teach => teach.password);
        public static bool Save(string login, string hashPassword)
        {
            Teachers teacher = new Teachers();
            teacher.login = login;
            teacher.password = hashPassword;
            try
            {
                TeachHoursEntities2.GetContext().Teachers.Add(teacher);
                TeachHoursEntities2.GetContext().SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static Teachers GetTeacherById(int id) => TeachHoursEntities2.GetContext().Teachers.FirstOrDefault(t => t.id == id);
        public static List<Teachers> GetTeachersList() => TeachHoursEntities2.GetContext().Teachers.ToList();
        public static bool ChangePassword(int id, string password)
        {
            try
            {
                Teachers teacher = GetTeacherById(id);
                teacher.password = password;
                TeachHoursEntities2.GetContext().SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
