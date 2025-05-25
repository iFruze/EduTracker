using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для AllHoursOfSubjects.xaml
    /// </summary>
    public partial class AllHoursOfSubjects : Window
    {
        public AllHoursOfSubjects()
        {
            InitializeComponent();
        }
        public AllHoursOfSubjects(string title)
        {
            InitializeComponent();
            this.titleSubject = title;
        }
        string titleSubject;
        public BaseWindow.GetAllHourForSubject temp;

        private void Window_Load(object sender, RoutedEventArgs e)
        {
            this.Label1.Content = $"Количество часов по предмету\n{this.titleSubject}:";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int t = Convert.ToInt32(this.TextBox.Text);
                if (t <= 0 || t > 1200)
                {
                    throw new Exception();
                }
                temp(t);
                this.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Некорректные данные.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Box_Click(object sender, MouseButtonEventArgs e)
        {
            this.TextBox.Text = "";
        }
    }
}
