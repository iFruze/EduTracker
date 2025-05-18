using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Classes
{
    internal class TeachAuth
    {
        string login;
        string hashPassword;
        public  TeachAuth() { }
        public  TeachAuth(string login, string hashPas) 
        { 
            this.login = login;
            this.hashPassword = hashPas;
        }
        public bool ValidateLogin(Dictionary<string, string> teachers) => teachers.Keys.Contains(login);
        public int ValidatePassword(List<Teachers> teachers)
        {
            int? teacherId = teachers.FirstOrDefault(teach => teach.login == login && teach.password == hashPassword)?.id;
            if (teacherId == null)
            {
                return -1;
            }
            return teacherId.Value;
        }
    }
}
