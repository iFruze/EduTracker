using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp2.Classes;

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            TeachReg validator = new TeachReg(this.RegLogText_Box.Text, this.FirstRegPasText_Box.Password, this.SecondRegPasText_Box.Password);
            if (validator.ValidatePasswords())
            {
                if(validator.ValidatePasswordLength())
                {
                    var teachers = TeachersRepository.GetTeachersDictionary();
                    if (validator.ValidateLogin(teachers))
                    {
                        var login = this.RegLogText_Box.Text;
                        var hashPas = PasswordHasher.HashFunc(this.FirstRegPasText_Box.Password);
                        if(TeachersRepository.Save(login, hashPas.ToString()))
                        {
                            MessageBox.Show("Вы зарегистрированы!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Во время регитсрации произршла непредвиденная ошибка. Повторите попытку.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Данный логин уже занят.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Длина пароля должна составлять минимум 6 символов.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Пароли не совпадают.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            var hashPas = PasswordHasher.HashFunc(this.AuthPasText_Box.Password);
            TeachAuth validator = new TeachAuth(this.AuthLogText_Box.Text, hashPas.ToString());
            var teachersDict = TeachersRepository.GetTeachersDictionary();
            var teachersList = TeachersRepository.GetTeachersList();
            if (validator.ValidateLogin(teachersDict))
            {
                int teachId = validator.ValidatePassword(teachersList);
                if (teachId > 0)
                {
                    BaseWindow page = new BaseWindow(teachId);
                    page.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный пароль.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Неверный логин.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void AuthPasTextBox_Click(object sender, MouseButtonEventArgs e)
        {
            this.AuthPasText_Box.Password = "";
        }
        private void AuthLogTextBox_Click(object sender, MouseButtonEventArgs e)
        {
            this.AuthLogText_Box.Text = "";
        }
        private void RegLog_Click(object sender, MouseButtonEventArgs e)
        {
            this.RegLogText_Box.Text = "";
        }
        private void RegPas1_Click(object sender, MouseButtonEventArgs e)
        {
            this.FirstRegPasText_Box.Password = "";
        }
        private void RegPas2_Click(object sender, MouseButtonEventArgs e)
        {
            this.SecondRegPasText_Box.Password = "";
        }

        private void Spravka_Click(object sender, RoutedEventArgs e)
        {
            string helpPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "spravka.chm");
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(helpPath) { UseShellExecute = true });
        }
    }
}
