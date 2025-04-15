using System;
using System.Collections.Generic;
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
            string log = this.RegLogText_Box.Text;
            string password1 = this.FirstRegPasText_Box.Password;
            string password2 = this.SecondRegPasText_Box.Password;
            if(password1.CompareTo(password2) == 0)
            {
                if (password2.Length <= 5)
                {
                    MessageBox.Show("Длина пароля должна составлять минимум 6 символов.");
                }
                else
                {
                    var hash = HashFunc(password2, 11, 9);
                    var teachers = TeachHoursEntities2.GetContext().Teachers.ToDictionary(teach => teach.login, teach => teach.password);
                    if (teachers.Keys.Contains(log))
                    {
                        MessageBox.Show("Данный логин уже занят.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        Teachers teach = new Teachers();
                        teach.login = log;
                        teach.password = hash.ToString();
                        TeachHoursEntities2.GetContext().Teachers.Add(teach);
                        try
                        {
                            TeachHoursEntities2.GetContext().SaveChanges();
                            MessageBox.Show("Вы зарегистрированы!");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Пароли не совпадают.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public ulong HashFunc(string str, int key, int count)
        {
            ulong size = (ulong)Math.Pow(10, count);
            ulong hash_code, t_hash = 0;
            for(int i = 0; i < str.Length; i++)
            {
                t_hash += (ulong)Math.Pow(key, i) * (ulong)str[i];
                t_hash %= size;
            }
            hash_code = t_hash % size;
            return hash_code;
        }
        
        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string login = this.AuthLogText_Box.Text;
                string password = this.AuthPasText_Box.Password;
                var hash = HashFunc(password, 11, 9);
                var teachers = TeachHoursEntities2.GetContext().Teachers.ToDictionary(teach => teach.login, teach => teach.password);
                if (teachers.Keys.Contains(login))
                {
                    if (teachers.TryGetValue(login, out string pass))
                    {
                        if (pass.CompareTo(hash.ToString()) == 0)
                        {
                            var teach = TeachHoursEntities2.GetContext().Teachers.SingleOrDefault(t => t.login.CompareTo(login) == 0);
                            if (teach != null)
                            {
                                int id = teach.id;
                                BaseWindow page = new BaseWindow(id);
                                page.Show();
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("При регистрации возникла непредвиденая ошибка.\nПовторите попытку.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Неверный пароль.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Произошла непредвиденная ошибка.");
                    }
                }
                else
                {
                    MessageBox.Show("Неверный логин.");
                }
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
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
    }
    
}

/*
Логин: TestUser
Пароль: UserTest230225
*/