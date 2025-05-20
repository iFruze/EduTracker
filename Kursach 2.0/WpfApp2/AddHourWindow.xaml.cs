using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp2.Classes;

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для AddHourWindow.xaml
    /// </summary>
    public partial class AddHourWindow : Window
    {
        public AddHourWindow()
        {
            InitializeComponent();
        }
        
        public AddHourWindow(int teachId)
        {
            InitializeComponent();
            this.id = teachId;
            List<Hours> hours = HoursRepository.GetTeacherHours(id);
            foreach (var h in hours)
            {
                int id = h.id;
                string subject = SubjectsRepository.GetSubjectById(h.subjectId).name;
                DateTime date = DatesRepository.GetDateById(h.dateId).date;
                allSubjectHours.Add(new AllSubjectHours(id, date, subject));
            }
            List<string> uniqGroups = new List<string>();
            List<string> uniqSubjects = new List<string>();
            foreach (var s in allSubjectHours)
            {
                var temp = s.Subject.Split(' ');
                if (!uniqGroups.Contains(temp[0]))
                {
                    uniqGroups.Add(temp[0]);
                }
                if (!uniqSubjects.Contains(temp[1]))
                {
                    uniqSubjects.Add(temp[1]);
                }
            }
            NameBox.ItemsSource = uniqSubjects;
            GroupBox.ItemsSource = uniqGroups;
        }
        int id;
        BindingList<AllSubjectHours> allSubjectHours = new BindingList<AllSubjectHours>();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> errors = new List<string>();
                if (!Regex.IsMatch(GroupBox.Text, @"[А-Я]{1}-[0-9]{3}"))
                {
                    errors.Add("Некорретное название группы.\nНазвание должно соответствовать шаблону \"А-000\"");
                }
                if(NameBox.Text.Length == 0 || NameBox.Text.Trim().ToLower().Contains("Урок снят".Trim().ToLower()))
                {
                    errors.Add("Некорретное название предмета.\nНазвание предмета не может быть пустым.\nПредмет не может называться \"Урок снят\"");
                }
                if(datePicker.SelectedDate == null)
                {
                    errors.Add("Некорректная дата.");
                }
                if(!(int.TryParse(DateBox.Text, out int count) == true && count >= 1 && count <= 200))
                {
                    errors.Add("Некорректное количество часов. Часы должны находиться в диапазоне от 1 до 200.");
                }
                if(errors.Count > 0)
                {
                    StringBuilder sb = new StringBuilder("");
                    foreach (var t in errors)
                    {
                        sb.AppendLine(t);
                    }
                    MessageBox.Show(sb.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    DateTime.TryParse(datePicker.SelectedDate.Value.ToString(), out DateTime date);
                    for (int i = 0; i < count; i++)
                    {
                        Hours hour = new Hours();
                        Subjects subjects = new Subjects();
                        subjects.name = $"{GroupBox.Text.Trim()} {NameBox.Text.Trim()}";
                        subjects.teacherId = this.id;
                        SubjectsRepository.Add(subjects);
                        Dates dates = new Dates();
                        dates.date = date;
                        DatesRepository.Add(dates);
                        hour.subjectId = subjects.id;
                        hour.teacherId = this.id;
                        hour.dateId = dates.id;
                        HoursRepository.Add(hour);
                    }
                    MessageBox.Show("Час успешно добавлен.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Date_Click(object sender, MouseButtonEventArgs e)
        {
            DateBox.Text = "";
        }
    }
}
