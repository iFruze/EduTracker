using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp2.Interfaces;

namespace WpfApp2.Classes
{
    internal class TeachReg
    {
        string login;
        string password;
        string repeatPassword;
        public TeachReg(string login, string password, string repeatPassword)
        {
            this.login = login;
            this.password = password;
            this.repeatPassword = repeatPassword;
        }
        public TeachReg() { }
        public bool ValidatePasswords() => password == repeatPassword ? true : false;
        public bool ValidatePasswordLength() => password.Length >= 6 ? true : false;
        public bool ValidateLogin(Dictionary<string, string> teachers) => !teachers.Keys.Contains(login);
    }
}
