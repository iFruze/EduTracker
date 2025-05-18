using AngleSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.IO;
using Microsoft.Win32;
using System.Security.Cryptography;
using Microsoft.Office.Interop.Word;
using System.Net.NetworkInformation;
using System.Diagnostics;
using WpfApp2.Classes;
using System.Reflection;

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для BaseWindow.xaml
    /// </summary>
    public partial class BaseWindow : System.Windows.Window
    {
        int id;
        DateTime reportDate = new DateTime(1920, 12, 31);
        string fileName = "";
        Teachers teacher;
        BindingList<WeekSubjects> weekSubjects;
        BindingList<AllSubjectHours> allSubjectHours;
        BindingList<AllSubjectHours> filterSubjectHours = new BindingList<AllSubjectHours>();
        public delegate void GetAllHourForSubject(int t);
        int allHoursOfSubject = -1;
        List<string> filesInDirectory;
        bool sourceSave = true;
        int indexOfFile = -1;
        DateTime week = DateTime.Now;
        public BaseWindow()
        {
            InitializeComponent();
        }
        public BaseWindow(int teacherId)
        {
            InitializeComponent();
            id = teacherId;
            teacher = TeachersRepository.GetTeacherById(id);
            CreateTeachFolder(id);
        }

        private void SavePassButton_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Вы точно хотите изменить свой пароль?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                TeachReg validator = new TeachReg(teacher.login, this.NewPassBox.Text, this.NewPassBox.Text);
                if (validator.ValidatePasswordLength())
                {
                    string newPass = PasswordHasher.HashFunc(this.NewPassBox.Text).ToString();
                    if (TeachersRepository.ChangePassword(teacher, newPass))
                    {
                        MessageBox.Show("Новый пароль сохранён.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Во время сохранения пароля возникла ошибка. Повторите попытку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Длина пароля должна составлять минимум 6 символов.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Вы действительно хотите сохранить иземенения?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var button = sender as Button;
                var hour = button?.DataContext as AllSubjectHours;
                var selectedHour = HoursRepository.GetHourById(hour.Id);
                try
                {
                    Dates date = DatesRepository.GetDateById(selectedHour.dateId);
                    Subjects subject = SubjectsRepository.GetSubjectById(selectedHour.subjectId);
                    if (DateTime.TryParse(hour.Date, out DateTime dt) && hour.Subject.Length > 0)
                    {
                        if(DatesRepository.ChangeDate(date.id, dt) && SubjectsRepository.ChangeNameSubject(subject.id, hour.Subject))
                        {
                            MessageBox.Show("Данные успешно изменены.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                            CreateAllHoursTable();
                        }
                        else
                        {
                            MessageBox.Show("При сохранении изменений произошла ошибка.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Данные введены неверно.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                        UpdateHoursGrid();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void Base_Load(object sender, RoutedEventArgs e)
        {
            this.LoginBox.Text = teacher.login;
            this.UrlBox.Text = teacher.url;
            List<string> years = new List<string>();
            for(int i = DateTime.Now.Year; i >= DateTime.Now.Year-5; i--)
            {
                years.Add(i.ToString());
            }
            YearBox.ItemsSource = years;
            List<string>months = new List<string>(new string[] {"Январь", "Февраль" , "Март" , "Апрель" , "Май" , "Июнь", "Июль", "Август", "Сентябрь", "Ноябрь", "Декабрь" });
            MonthBox.ItemsSource = months;
            List<Hours> hours = HoursRepository.GetTeacherHours(teacher.id);//TeachHoursEntities2.GetContext().Hours.Where(h => h.teacherId == teacher.id).ToList();
            allSubjectHours = new BindingList<AllSubjectHours>();
            foreach(var h in hours)
            {
                int id = h.id;
                string subject = SubjectsRepository.GetSubjectById(h.subjectId).name;
                DateTime date = DatesRepository.GetDateById(h.dateId).date;
                allSubjectHours.Add(new AllSubjectHours(id, date, subject));
            }
            CreateAllHoursTable();
            this.HoursGrid.ItemsSource = allSubjectHours;
            while(week.DayOfWeek != DayOfWeek.Monday)
            {
                week = week.AddDays(-1);
            }
            DateTime week1 = week;
            DateTime date1 = DateTime.Now;
            DateTime date2 = DateTime.Now;
            for(int i = 1; i < 7; i++, week1 = week1.AddDays(1))
            {
                if (i == 1)
                {
                    var column = new DataGridTextColumn
                    {
                        Header = $"Понедельник\n{week1.ToShortDateString()}",
                        Binding = new Binding("Monday")
                    };
                    this.SourceGrid.Columns.Add(column);
                    date1 = week1;
                }
                if (i == 2)
                {
                    var column = new DataGridTextColumn
                    {
                        Header = $"Вторник\n{week1.ToShortDateString()}",
                        Binding = new Binding("Tuesday")
                    };
                    this.SourceGrid.Columns.Add(column);
                }
                if (i == 3)
                {
                    var column = new DataGridTextColumn
                    {
                        Header = $"Среда\n{week1.ToShortDateString()}",
                        Binding = new Binding("Wednesday")
                    };
                    this.SourceGrid.Columns.Add(column);
                }
                if (i == 4)
                {
                    var column = new DataGridTextColumn
                    {
                        Header = $"Четверг\n{week1.ToShortDateString()}",
                        Binding = new Binding("Thursday")
                    };
                    this.SourceGrid.Columns.Add(column);
                }
                if (i == 5)
                {
                    var column = new DataGridTextColumn
                    {
                        Header = $"Пятница\n{week1.ToShortDateString()}",
                        Binding = new Binding("Friday")
                    };
                    this.SourceGrid.Columns.Add(column);
                }
                if (i == 6)
                {
                    var column = new DataGridTextColumn
                    {
                        Header = $"Суббота\n{week1.ToShortDateString()}",
                        Binding = new Binding("Saturday")
                    };
                    this.SourceGrid.Columns.Add(column);
                    date2 = week1;
                }
            }
            var teachLogin = teacher.login;
            this.WeekName.Content = $"Неделя с {date1.ToShortDateString()} - {date2.ToShortDateString()}";
            fileName = $"Неделя с {date1.ToShortDateString()} - {date2.ToShortDateString()}.json";
            string projectDir1 = AppDomain.CurrentDomain.BaseDirectory;
            string dir1 = System.IO.Path.Combine(projectDir1, $"Расписание\\{teachLogin}");
            filesInDirectory = Directory.GetFiles(dir1).OrderBy(file => File.GetCreationTime(file)).ToList();
            if(filesInDirectory.Count > 0)
            {
                indexOfFile = filesInDirectory.Count - 1;
                try
                {
                    string json = File.ReadAllText(filesInDirectory[indexOfFile]);
                    weekSubjects = JsonConvert.DeserializeObject<BindingList<WeekSubjects>>(json);
                    FileInfo fileInfo = new FileInfo(filesInDirectory[indexOfFile]);
                    this.FileName.Content = $"Файл от {File.GetLastWriteTime(fileInfo.FullName).ToShortDateString()}";
                    SourceGrid.ItemsSource = weekSubjects;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
                }
            }
            List<Hours> hs = TeachHoursEntities2.GetContext().Hours.Where(h => h.teacherId == teacher.id).ToList();
            allSubjectHours = new BindingList<AllSubjectHours>();
            UpdateHoursGrid();
        }

        private void SaveUrlButton_Click(object sender, RoutedEventArgs e)
        {

            if (MessageBox.Show("Вы точно хотите изменить ссылку для расписания?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (this.UrlBox.Text.Length == 0)
                {
                    MessageBox.Show("Поле для расписания пустое.");
                }
                else
                {
                    if(TeachersRepository.ChangeUrl(teacher, this.UrlBox.Text))
                    {
                        MessageBox.Show("Новая ссылка сохранена.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("При сохранении новой ссылки произошла ошибка.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        static bool IsInternetAvailable()
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = ping.Send("8.8.8.8", 3000); // Используется адрес DNS-сервера Google для проверки подключения
                    return reply.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }
        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            if(IsInternetAvailable())
            {
                try
                {
                    var url = this.teacher.url;
                    if (url!=null && !url.Contains("teacher"))
                    {
                        throw new Exception();
                    }
                    var monday = await HtmlParser.GetDayOfWeek(url, $"tbody tr div.pair.lw_1:not(.removed) div.subject", $"tbody tr div.pair.lw_1:not(.removed) div.group span.group-span a");
                    var tuesday =  await HtmlParser.GetDayOfWeek(url, $"tbody tr div.pair.lw_2:not(.removed) div.subject", $"tbody tr div.pair.lw_2:not(.removed) div.group span.group-span a");
                    var wednesday = await HtmlParser.GetDayOfWeek(url, $"tbody tr div.pair.lw_3:not(.removed) div.subject", $"tbody tr div.pair.lw_3:not(.removed) div.group span.group-span a");
                    var thursday = await HtmlParser.GetDayOfWeek(url, $"tbody tr div.pair.lw_4:not(.removed) div.subject", $"tbody tr div.pair.lw_4:not(.removed) div.group span.group-span a");
                    var friday = await HtmlParser.GetDayOfWeek(url, $"tbody tr div.pair.lw_5:not(.removed) div.subject", $"tbody tr div.pair.lw_5:not(.removed) div.group span.group-span a");
                    var satuday = await HtmlParser.GetDayOfWeek(url, $"tbody tr div.pair.lw_6:not(.removed) div.subject", $"tbody tr div.pair.lw_6:not(.removed) div.group span.group-span a");
                    
                    if(monday == null || tuesday == null || wednesday == null || thursday == null || friday == null || satuday == null)
                    {
                        throw new Exception();
                    }

                    int maxValue = new int[] { monday.Count, tuesday.Count, wednesday.Count, thursday.Count, friday.Count, satuday.Count }.Max();
                    int indMon = 0, indTues = 0, indWed = 0, indThurs = 0, indFrid = 0, indSat = 0;
                    weekSubjects = new BindingList<WeekSubjects>();
                    for (int i = 0; i < maxValue; i++)
                    {
                        string mon, tues, wed, thurs, frid, sat;
                        if (indMon > monday.Count - 1)
                        {
                            mon = "";
                        }
                        else
                        {
                            mon = monday[indMon++];
                        }
                        if (indTues > tuesday.Count - 1)
                        {
                            tues = "";
                        }
                        else
                        {
                            tues = tuesday[indTues++];
                        }
                        if (indWed > wednesday.Count - 1)
                        {
                            wed = "";
                        }
                        else
                        {
                            wed = wednesday[indWed++];
                        }
                        if (indThurs > thursday.Count - 1)
                        {
                            thurs = "";
                        }
                        else
                        {
                            thurs = thursday[indThurs++];
                        }
                        if (indFrid > friday.Count - 1)
                        {
                            frid = "";
                        }
                        else
                        {
                            frid = friday[indFrid++];
                        }
                        if (indSat > satuday.Count - 1)
                        {
                            sat = "";
                        }
                        else
                        {
                            sat = satuday[indSat++];
                        }
                        weekSubjects.Add(new WeekSubjects(mon, tues, wed, thurs, frid, sat));
                    }
                    SourceGrid.Columns.Clear();
                    SourceGrid.ItemsSource = null;
                    SourceGrid.Items.Clear();
                    week = DateTime.Now;
                    //CreateWeekGrid(week);
                    while (week.DayOfWeek != DayOfWeek.Monday)
                    {
                        week = week.AddDays(-1);
                    }
                    DateTime week1 = week;
                    DateTime date1 = DateTime.Now;
                    DateTime date2 = DateTime.Now;
                    for (int i = 1; i < 7; i++, week1 = week1.AddDays(1))
                    {
                        if (i == 1)
                        {
                            var column = new DataGridTextColumn
                            {
                                Header = $"Понедельник\n{week1.ToShortDateString()}",
                                Binding = new Binding("Monday")
                            };
                            this.SourceGrid.Columns.Add(column);
                            date1 = week1;
                        }
                        if (i == 2)
                        {
                            var column = new DataGridTextColumn
                            {
                                Header = $"Вторник\n{week1.ToShortDateString()}",
                                Binding = new Binding("Tuesday")
                            };
                            this.SourceGrid.Columns.Add(column);
                        }
                        if (i == 3)
                        {
                            var column = new DataGridTextColumn
                            {
                                Header = $"Среда\n{week1.ToShortDateString()}",
                                Binding = new Binding("Wednesday")
                            };
                            this.SourceGrid.Columns.Add(column);
                        }
                        if (i == 4)
                        {
                            var column = new DataGridTextColumn
                            {
                                Header = $"Четверг\n{week1.ToShortDateString()}",
                                Binding = new Binding("Thursday")
                            };
                            this.SourceGrid.Columns.Add(column);
                        }
                        if (i == 5)
                        {
                            var column = new DataGridTextColumn
                            {
                                Header = $"Пятница\n{week1.ToShortDateString()}",
                                Binding = new Binding("Friday")
                            };
                            this.SourceGrid.Columns.Add(column);
                        }
                        if (i == 6)
                        {
                            var column = new DataGridTextColumn
                            {
                                Header = $"Суббота\n{week1.ToShortDateString()}",
                                Binding = new Binding("Saturday")
                            };
                            this.SourceGrid.Columns.Add(column);
                            date2 = week1;

                        }
                    }
                    indexOfFile = filesInDirectory.Count - 1;
                    SourceGrid.ItemsSource = weekSubjects;
                    sourceSave = false;
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Ошибка соединения с сайтом колледжа.\nПроверьте корректность ссылки.\nСсылка должна указывать на раписание преподавателя.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Отсутствует интернет-соединение.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            if(weekSubjects != null && weekSubjects.Count > 0)
            {
                var teachLogin = teacher.login;
                string projectDir = AppDomain.CurrentDomain.BaseDirectory;
                string dir = System.IO.Path.Combine(projectDir, $"Расписание\\{teachLogin}");
                string file = fileName;
                string json = JsonConvert.SerializeObject(weekSubjects, Formatting.Indented);
                File.WriteAllText($"{dir}\\{file}", json);
                MessageBox.Show("Раписание успешно сохранено." , "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                string projectDir1 = AppDomain.CurrentDomain.BaseDirectory;
                string dir1 = System.IO.Path.Combine(projectDir1, $"Расписание\\{teachLogin}");
                filesInDirectory = Directory.GetFiles(dir1).OrderBy(file1 => File.GetCreationTime(file1)).ToList();
                indexOfFile = filesInDirectory.Count - 1;
                string json1 = File.ReadAllText(filesInDirectory[indexOfFile]);
                weekSubjects = JsonConvert.DeserializeObject<BindingList<WeekSubjects>>(json1);
                FileInfo fileinfo = new FileInfo(filesInDirectory[indexOfFile]);
                this.FileName.Content = $"Файл от {File.GetLastWriteTime(fileinfo.FullName).ToShortDateString()}";
                week = DateTime.Now;
                SourceGrid.Columns.Clear();
                SourceGrid.ItemsSource = null;
                SourceGrid.Items.Clear();
                week = DateTime.Now;
                while (week.DayOfWeek != DayOfWeek.Monday)
                {
                    week = week.AddDays(-1);
                }
                DateTime week1 = week;
                DateTime date1 = DateTime.Now;
                DateTime date2 = DateTime.Now;
                for (int i = 1; i < 7; i++, week1 = week1.AddDays(1))
                {
                    if (i == 1)
                    {
                        var column = new DataGridTextColumn
                        {
                            Header = $"Понедельник\n{week1.ToShortDateString()}",
                            Binding = new Binding("Monday")
                        };
                        this.SourceGrid.Columns.Add(column);
                        date1 = week1;
                    }
                    if (i == 2)
                    {
                        var column = new DataGridTextColumn
                        {
                            Header = $"Вторник\n{week1.ToShortDateString()}",
                            Binding = new Binding("Tuesday")
                        };
                        this.SourceGrid.Columns.Add(column);
                    }
                    if (i == 3)
                    {
                        var column = new DataGridTextColumn
                        {
                            Header = $"Среда\n{week1.ToShortDateString()}",
                            Binding = new Binding("Wednesday")
                        };
                        this.SourceGrid.Columns.Add(column);
                    }
                    if (i == 4)
                    {
                        var column = new DataGridTextColumn
                        {
                            Header = $"Четверг\n{week1.ToShortDateString()}",
                            Binding = new Binding("Thursday")
                        };
                        this.SourceGrid.Columns.Add(column);
                    }
                    if (i == 5)
                    {
                        var column = new DataGridTextColumn
                        {
                            Header = $"Пятница\n{week1.ToShortDateString()}",
                            Binding = new Binding("Friday")
                        };
                        this.SourceGrid.Columns.Add(column);
                    }
                    if (i == 6)
                    {
                        var column = new DataGridTextColumn
                        {
                            Header = $"Суббота\n{week1.ToShortDateString()}",
                            Binding = new Binding("Saturday")
                        };
                        this.SourceGrid.Columns.Add(column);
                        date2 = week1;

                    }
                }
                this.WeekName.Content = $"Неделя с {date1.ToShortDateString()} - {date2.ToShortDateString()}";
                SourceGrid.ItemsSource = weekSubjects;
                sourceSave = true;
            }
            else
            {
                MessageBox.Show("Сначала загрузите расписание с сайта колледжа.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        void AddSubjectsToDB(List<string> day, DateTime date)
        {
            for (int i = 0; i < day.Count; i++)
            {
                Dates dayDate = new Dates();
                dayDate.date = date;
                DatesRepository.Add(dayDate);
                Subjects subject = new Subjects();
                subject.name = day[i];
                subject.teacherId = teacher.id;
                SubjectsRepository.Add(subject);
                Hours hour = new Hours();
                hour.dateId = dayDate.id;
                hour.subjectId = subject.id;
                hour.teacherId = teacher.id;
                HoursRepository.Add(hour);
            }
        }
        private void SaveDB_Click(object sender, RoutedEventArgs e)
        {
            if (weekSubjects != null && weekSubjects.Count > 0)
            {
                DateTime date = DateTime.Now;
                while(date.DayOfWeek != DayOfWeek.Monday)
                {
                    date = date.AddDays(-1);
                }
                var monday = (from i in weekSubjects
                          where i.Monday != "" && !i.Monday.Trim().ToLower().Contains("Урок снят".Trim().ToLower())
                          select i.Monday).ToList<string>();
                var tuesday = (from i in weekSubjects
                               where i.Tuesday != "" && !i.Tuesday.Trim().ToLower().Contains("Урок снят".Trim().ToLower())
                               select i.Tuesday).ToList<string>();
                var wednesday = (from i in weekSubjects
                                 where i.Wednesday != "" && !i.Wednesday.Trim().ToLower().Contains("Урок снят".Trim().ToLower())
                                 select i.Wednesday).ToList<string>();
                var thursday = (from i in weekSubjects 
                                where i.Thursday != "" && !i.Thursday.Trim().ToLower().Contains("Урок снят".Trim().ToLower()) 
                                select i.Thursday).ToList<string>();
                var friday = (from i in weekSubjects
                              where i.Friday != "" && !i.Friday.Trim().ToLower().Contains("Урок снят".Trim().ToLower())
                              select i.Friday).ToList<string>();
                var saturday = (from i in weekSubjects
                                where i.Saturday != "" && !i.Saturday.Trim().ToLower().Contains("Урок снят".Trim().ToLower())
                                select i.Saturday).ToList<string>();
                try
                {
                    AddSubjectsToDB(monday, date);
                    date = date.AddDays(1);
                    AddSubjectsToDB(tuesday, date);
                    date = date.AddDays(1);
                    AddSubjectsToDB(wednesday, date);
                    date = date.AddDays(1); 
                    AddSubjectsToDB(thursday, date);
                    date = date.AddDays(1);
                    AddSubjectsToDB(friday, date);
                    date = date.AddDays(1);
                    AddSubjectsToDB(saturday, date);
                    MessageBox.Show("Расписание успешно сохранено.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    UpdateHoursGrid();
                    CreateAllHoursTable();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Сначала загрузите расписание с сайта колледжа.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var removeHours = this.HoursGrid.SelectedItems.Cast<AllSubjectHours>().ToList();
            if(MessageBox.Show($"Вы действительно хотите удалить {removeHours.Count} часов?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    List<Hours> remove = (from i in removeHours
                                          select HoursRepository.GetHourById(i.Id)).ToList();
                    List<Dates> removeDates = (from i in remove
                                               select DatesRepository.GetDateById(i.dateId)).ToList();
                    List<Subjects> removeSubjects = (from i in remove
                                                     select SubjectsRepository.GetSubjectById(i.subjectId)).ToList();
                    HoursRepository.DeleteRange(remove);
                    DatesRepository.DeleteRange(removeDates);
                    SubjectsRepository.DeleteRange(removeSubjects);
                    foreach(var hour in removeHours)
                    {
                        allSubjectHours.Remove(hour);
                    }
                    UpdateHoursGrid();
                    MessageBox.Show("Часы успешно удалены.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    CreateAllHoursTable();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);    
                }
            }
        }
        public void UpdateHoursGrid()
        {
            List<Hours> hours = HoursRepository.GetTeacherHours(teacher.id);
            allSubjectHours.Clear();
            foreach (var h in hours)
            {
                int id = h.id;
                string subject = SubjectsRepository.GetSubjectById(h.subjectId).name;
                DateTime date = DatesRepository.GetDateById(h.dateId).date;
                allSubjectHours.Add(new AllSubjectHours(id, date, subject));
            }
            HoursGrid.ItemsSource = allSubjectHours;
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
            this.ViewSubjects.ItemsSource = uniqSubjects;
            this.GroupsBox.ItemsSource = uniqGroups;
        }
        private void ButtonAddHour_Click(object sender, RoutedEventArgs e)
        {
            AddHourWindow page = new AddHourWindow(teacher.id);
            if(page.ShowDialog() == true)
            {
                UpdateHoursGrid();
                CreateAllHoursTable();
            }
        }
        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            if(MonthBox.SelectedItem != null && YearBox.SelectedItem != null)
            {
                switch (MonthBox.SelectedItem.ToString())
                {
                    case "Январь":
                        reportDate = new DateTime(int.Parse(YearBox.SelectedItem.ToString()), 1, 1);
                        break;
                    case "Февраль":
                        reportDate = new DateTime(int.Parse(YearBox.SelectedItem.ToString()), 2, 1);
                        break;
                    case "Март":
                        reportDate = new DateTime(int.Parse(YearBox.SelectedItem.ToString()), 3, 1);
                        break;
                    case "Апрель":
                        reportDate = new DateTime(int.Parse(YearBox.SelectedItem.ToString()), 4, 1);
                        break;
                    case "Май":
                        reportDate = new DateTime(int.Parse(YearBox.SelectedItem.ToString()), 5, 1);
                        break;
                    case "Июнь":
                        reportDate = new DateTime(int.Parse(YearBox.SelectedItem.ToString()), 6, 1);
                        break;
                    case "Июль":
                        reportDate = new DateTime(int.Parse(YearBox.SelectedItem.ToString()), 7, 1);
                        break;
                    case "Август":
                        reportDate = new DateTime(int.Parse(YearBox.SelectedItem.ToString()), 8, 1);
                        break;
                    case "Сентябрь":
                        reportDate = new DateTime(int.Parse(YearBox.SelectedItem.ToString()), 9, 1);
                        break;
                    case "Октябрь":
                        reportDate = new DateTime(int.Parse(YearBox.SelectedItem.ToString()), 10, 1);
                        break;
                    case "Ноябрь":
                        reportDate = new DateTime(int.Parse(YearBox.SelectedItem.ToString()), 11, 1);
                        break;
                    case "Декабрь":
                        reportDate = new DateTime(int.Parse(YearBox.SelectedItem.ToString()), 12, 1);
                        break;
                }
                List<string> subjectsInGroups = new List<string>();
                List<string> datesOfSubjects = new List<string>();
                foreach (var s in allSubjectHours)
                {
                    DateTime data = DateTime.Parse(s.Date);
                    if (!subjectsInGroups.Contains(s.Subject) && data.Month == reportDate.Month && data.Year == reportDate.Year)
                    {
                        subjectsInGroups.Add(s.Subject);
                    }
                    if (!datesOfSubjects.Contains(s.Date) && DateTime.Parse(s.Date).Year == reportDate.Year)
                    {
                        datesOfSubjects.Add(s.Date);
                    }
                }
                List<List<string>> resultReport = new List<List<string>>();

                for(int i = 0; i < subjectsInGroups.Count; i++)
                {
                    resultReport.Add(new List<string>());
                    resultReport[i].Add(subjectsInGroups[i]);
                    int countOfHours = 0;
                    for(int j = 0; j < datesOfSubjects.Count; j++)
                    {
                        foreach(var s in allSubjectHours)
                        {
                            if(s.Subject == subjectsInGroups[i] && s.Date.CompareTo(datesOfSubjects[j])==0)
                            {
                                countOfHours++;
                            }
                        }
                        if(countOfHours > 0)
                        {
                            resultReport[i].Add($"{datesOfSubjects[j]} - {countOfHours}");
                            countOfHours = 0;
                        }
                    }
                }
                if(resultReport.Count > 0)
                {
                    WordReport reporter = new WordReport();
                    reporter.GenerateReport(id, resultReport, MonthBox.SelectedItem.ToString(), YearBox.SelectedItem.ToString());
                    CreateAllHoursTable();
                    MessageBox.Show("Отчёт успешно сохранён!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Предметов на данный месяц и год не найдено.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите месяц и год для формирования отчёта.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void GetHour(int t)
        {
            this.allHoursOfSubject = t;
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            filterSubjectHours = allSubjectHours;
            if(ViewSubjects.SelectedItems.Count > 0)
            {
                var tempList = new BindingList<AllSubjectHours>();
                foreach(var t in ViewSubjects.SelectedItems)
                {
                    foreach(var item in filterSubjectHours)
                    {
                        if (item.Subject.Contains(t.ToString()))
                        {
                            tempList.Add(item);
                        }
                    }
                }
                filterSubjectHours = tempList;
            }
            if(GroupsBox.SelectedItem != null)
            {
                var tempList = new BindingList<AllSubjectHours>();
                var group = GroupsBox.SelectedItem.ToString();
                foreach (var item in filterSubjectHours)
                {
                    if (item.Subject.Contains(group))
                    {
                        tempList.Add(item);
                    }
                }
                filterSubjectHours = tempList;
            }
            if(PickerStart.SelectedDate != null)
            {
                var tempList = new BindingList<AllSubjectHours>();
                var start = PickerStart.SelectedDate;
                foreach(var sbj in filterSubjectHours)
                {
                    DateTime.TryParse(sbj.Date, out DateTime date);
                    if(date >= start)
                    {
                        tempList.Add(sbj);
                    }
                }
                filterSubjectHours = tempList;
            }
            if(PickerEnd.SelectedDate != null)
            {
                var tempList = new BindingList<AllSubjectHours>();
                var end = PickerEnd.SelectedDate;
                foreach (var sbj in filterSubjectHours)
                {
                    DateTime.TryParse(sbj.Date, out DateTime date);
                    if (date <= end)
                    {
                        tempList.Add(sbj);
                    }
                }
                filterSubjectHours = tempList;
            }
            if(filterSubjectHours.Count > 0)
            {
                HoursGrid.ItemsSource = filterSubjectHours;
            }
            else
            {
                MessageBox.Show("По вашему запросу ничего не найдено.");
                ViewSubjects.SelectedItems.Clear();
                GroupsBox.SelectedItem = null;
                PickerStart.SelectedDate = null;
                PickerEnd.SelectedDate = null;
                UpdateHoursGrid();
            }
        }

        private void SubjectTitle_Click(object sender, MouseButtonEventArgs e)
        {
            ViewSubjects.SelectedItems.Clear();
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            if(indexOfFile <= 0)
            {
                Prev.IsEnabled = false;
            }
            else
            {
                if(sourceSave == false)
                {
                    if (MessageBox.Show($"Расписание не сохранено, перелистывание приведёт к потере данных.\nСохранить раписание?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        if (weekSubjects != null && weekSubjects.Count > 0)
                        {
                            var teachLogin = teacher.login;
                            string projectDir = AppDomain.CurrentDomain.BaseDirectory;
                            string dir = System.IO.Path.Combine(projectDir, $"Расписание\\{teachLogin}");
                            string file = fileName;
                            string json = JsonConvert.SerializeObject(weekSubjects, Formatting.Indented);
                            File.WriteAllText($"{dir}\\{file}", json);
                            MessageBox.Show("Файл успешно сохранён.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                            string projectDir1 = AppDomain.CurrentDomain.BaseDirectory;
                            string dir1 = System.IO.Path.Combine(projectDir1, $"Расписание\\{teachLogin}");
                            filesInDirectory = Directory.GetFiles(dir1).OrderBy(file1 => File.GetCreationTime(file1)).ToList();
                            indexOfFile = filesInDirectory.Count - 1;
                            string json1 = File.ReadAllText(filesInDirectory[indexOfFile]);
                            weekSubjects = JsonConvert.DeserializeObject<BindingList<WeekSubjects>>(json1);
                            FileInfo fileinfo = new FileInfo(filesInDirectory[indexOfFile]);
                            this.FileName.Content = $"Файл от {File.GetLastWriteTime(fileinfo.FullName).ToShortDateString()}";
                            SourceGrid.Columns.Clear();
                            SourceGrid.ItemsSource = null;
                            SourceGrid.Items.Clear();
                            week = DateTime.Now;
                            while (week.DayOfWeek != DayOfWeek.Monday)
                            {
                                week = week.AddDays(-1);
                            }
                            DateTime week1 = week;
                            DateTime date1 = DateTime.Now;
                            DateTime date2 = DateTime.Now;
                            for (int i = 1; i < 7; i++, week1 = week1.AddDays(1))
                            {
                                if (i == 1)
                                {
                                    var column = new DataGridTextColumn
                                    {
                                        Header = $"Понедельник\n{week1.ToShortDateString()}",
                                        Binding = new Binding("Monday")
                                    };
                                    this.SourceGrid.Columns.Add(column);
                                    date1 = week1;
                                }
                                if (i == 2)
                                {
                                    var column = new DataGridTextColumn
                                    {
                                        Header = $"Вторник\n{week1.ToShortDateString()}",
                                        Binding = new Binding("Tuesday")
                                    };
                                    this.SourceGrid.Columns.Add(column);
                                }
                                if (i == 3)
                                {
                                    var column = new DataGridTextColumn
                                    {
                                        Header = $"Среда\n{week1.ToShortDateString()}",
                                        Binding = new Binding("Wednesday")
                                    };
                                    this.SourceGrid.Columns.Add(column);
                                }
                                if (i == 4)
                                {
                                    var column = new DataGridTextColumn
                                    {
                                        Header = $"Четверг\n{week1.ToShortDateString()}",
                                        Binding = new Binding("Thursday")
                                    };
                                    this.SourceGrid.Columns.Add(column);
                                }
                                if (i == 5)
                                {
                                    var column = new DataGridTextColumn
                                    {
                                        Header = $"Пятница\n{week1.ToShortDateString()}",
                                        Binding = new Binding("Friday")
                                    };
                                    this.SourceGrid.Columns.Add(column);
                                }
                                if (i == 6)
                                {
                                    var column = new DataGridTextColumn
                                    {
                                        Header = $"Суббота\n{week1.ToShortDateString()}",
                                        Binding = new Binding("Saturday")
                                    };
                                    this.SourceGrid.Columns.Add(column);
                                    date2 = week1;
                                }
                            }
                            this.WeekName.Content = $"Неделя с {date1.ToShortDateString()} - {date2.ToShortDateString()}";
                            SourceGrid.ItemsSource = weekSubjects;
                            sourceSave = true;
                            
                        }
                        else
                        {
                            MessageBox.Show("Сначала откройте или загрузите расписание.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        sourceSave = true;
                    }
                }
                else
                {
                    Next.IsEnabled = true;
                    indexOfFile--;
                    try
                    {
                        string json = File.ReadAllText(filesInDirectory[indexOfFile]);
                        weekSubjects = JsonConvert.DeserializeObject<BindingList<WeekSubjects>>(json);
                        FileInfo fileinfo = new FileInfo(filesInDirectory[indexOfFile]);
                        this.FileName.Content = $"Файл от {File.GetLastWriteTime(fileinfo.FullName).ToShortDateString()}";
                        SourceGrid.Columns.Clear();
                        SourceGrid.ItemsSource = null;
                        SourceGrid.Items.Clear(); week = week.AddDays(-7);
                        DateTime week1 = week;
                        DateTime date1 = DateTime.Now;
                        DateTime date2 = DateTime.Now;
                        for (int i = 1; i < 7; i++, week1 = week1.AddDays(1))
                        {
                            if (i == 1)
                            {
                                var column = new DataGridTextColumn
                                {
                                    Header = $"Понедельник\n{week1.ToShortDateString()}",
                                    Binding = new Binding("Monday")
                                };
                                this.SourceGrid.Columns.Add(column);
                                date1 = week1;
                            }
                            if (i == 2)
                            {
                                var column = new DataGridTextColumn
                                {
                                    Header = $"Вторник\n{week1.ToShortDateString()}",
                                    Binding = new Binding("Tuesday")
                                };
                                this.SourceGrid.Columns.Add(column);
                            }
                            if (i == 3)
                            {
                                var column = new DataGridTextColumn
                                {
                                    Header = $"Среда\n{week1.ToShortDateString()}",
                                    Binding = new Binding("Wednesday")
                                };
                                this.SourceGrid.Columns.Add(column);
                            }
                            if (i == 4)
                            {
                                var column = new DataGridTextColumn
                                {
                                    Header = $"Четверг\n{week1.ToShortDateString()}",
                                    Binding = new Binding("Thursday")
                                };
                                this.SourceGrid.Columns.Add(column);
                            }
                            if (i == 5)
                            {
                                var column = new DataGridTextColumn
                                {
                                    Header = $"Пятница\n{week1.ToShortDateString()}",
                                    Binding = new Binding("Friday")
                                };
                                this.SourceGrid.Columns.Add(column);
                            }
                            if (i == 6)
                            {
                                var column = new DataGridTextColumn
                                {
                                    Header = $"Суббота\n{week1.ToShortDateString()}",
                                    Binding = new Binding("Saturday")
                                };
                                this.SourceGrid.Columns.Add(column);
                                date2 = week1;

                            }
                        }
                        this.WeekName.Content = $"Неделя с {date1.ToShortDateString()} - {date2.ToShortDateString()}";
                        SourceGrid.ItemsSource = weekSubjects;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if(indexOfFile >= filesInDirectory.Count - 1)
            {
                Next.IsEnabled = false;
            }
            else
            {
                if (sourceSave == false)
                {
                    if (MessageBox.Show($"Расписание не сохранено, перелистывание приведёт к потере данных.\nСохранить раписание?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        if (weekSubjects != null && weekSubjects.Count > 0)
                        {
                            var teachLogin = teacher.login;
                            string projectDir = AppDomain.CurrentDomain.BaseDirectory;
                            string dir = System.IO.Path.Combine(projectDir, $"Расписание\\{teachLogin}");
                            string file = fileName;
                            string json = JsonConvert.SerializeObject(weekSubjects, Formatting.Indented);
                            File.WriteAllText($"{dir}\\{file}", json);
                            MessageBox.Show("Файл успешно сохранён.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                            string projectDir1 = AppDomain.CurrentDomain.BaseDirectory;
                            string dir1 = System.IO.Path.Combine(projectDir1, $"Расписание\\{teachLogin}");
                            filesInDirectory = Directory.GetFiles(dir1).OrderBy(file1 => File.GetCreationTime(file1)).ToList();
                            indexOfFile = filesInDirectory.Count-1;
                            string json1 = File.ReadAllText(filesInDirectory[indexOfFile]);
                            weekSubjects = JsonConvert.DeserializeObject<BindingList<WeekSubjects>>(json1);
                            FileInfo fileinfo = new FileInfo(filesInDirectory[indexOfFile]);
                            this.FileName.Content = $"Файл от {File.GetLastWriteTime(fileinfo.FullName).ToShortDateString()}";
                            SourceGrid.Columns.Clear();
                            SourceGrid.ItemsSource = null;
                            SourceGrid.Items.Clear(); week = DateTime.Now;
                            while(week.DayOfWeek != DayOfWeek.Monday)
                            {
                                week = week.AddDays(-1);
                            }
                            DateTime week1 = week;
                            DateTime date1 = DateTime.Now;
                            DateTime date2 = DateTime.Now;
                            for (int i = 1; i < 7; i++, week1 = week1.AddDays(1))
                            {
                                if (i == 1)
                                {
                                    var column = new DataGridTextColumn
                                    {
                                        Header = $"Понедельник\n{week1.ToShortDateString()}",
                                        Binding = new Binding("Monday")
                                    };
                                    this.SourceGrid.Columns.Add(column);
                                    date1 = week1;
                                }
                                if (i == 2)
                                {
                                    var column = new DataGridTextColumn
                                    {
                                        Header = $"Вторник\n{week1.ToShortDateString()}",
                                        Binding = new Binding("Tuesday")
                                    };
                                    this.SourceGrid.Columns.Add(column);
                                }
                                if (i == 3)
                                {
                                    var column = new DataGridTextColumn
                                    {
                                        Header = $"Среда\n{week1.ToShortDateString()}",
                                        Binding = new Binding("Wednesday")
                                    };
                                    this.SourceGrid.Columns.Add(column);
                                }
                                if (i == 4)
                                {
                                    var column = new DataGridTextColumn
                                    {
                                        Header = $"Четверг\n{week1.ToShortDateString()}",
                                        Binding = new Binding("Thursday")
                                    };
                                    this.SourceGrid.Columns.Add(column);
                                }
                                if (i == 5)
                                {
                                    var column = new DataGridTextColumn
                                    {
                                        Header = $"Пятница\n{week1.ToShortDateString()}",
                                        Binding = new Binding("Friday")
                                    };
                                    this.SourceGrid.Columns.Add(column);
                                }
                                if (i == 6)
                                {
                                    var column = new DataGridTextColumn
                                    {
                                        Header = $"Суббота\n{week1.ToShortDateString()}",
                                        Binding = new Binding("Saturday")
                                    };
                                    this.SourceGrid.Columns.Add(column);
                                    date2 = week1;

                                }
                            }
                            this.WeekName.Content = $"Неделя с {date1.ToShortDateString()} - {date2.ToShortDateString()}";
                            SourceGrid.ItemsSource = weekSubjects;
                            sourceSave = true;
                            
                        }
                        else
                        {
                            MessageBox.Show("Сначала загрузите расписание с сайте колледжа.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        sourceSave = true;
                    }
                }
                else
                {
                    Prev.IsEnabled = true;
                    indexOfFile++;
                    try
                    {
                        string json = File.ReadAllText(filesInDirectory[indexOfFile]);
                        weekSubjects = JsonConvert.DeserializeObject<BindingList<WeekSubjects>>(json);
                        FileInfo fileinfo = new FileInfo(filesInDirectory[indexOfFile]);
                        this.FileName.Content = $"Файл от {File.GetLastWriteTime(fileinfo.FullName).ToShortDateString()}";
                        SourceGrid.Columns.Clear();
                        SourceGrid.ItemsSource = null;
                        SourceGrid.Items.Clear(); week = week.AddDays(7);
                        DateTime week1 = week;
                        DateTime date1 = DateTime.Now;
                        DateTime date2 = DateTime.Now;
                        for (int i = 1; i < 7; i++, week1 = week1.AddDays(1))
                        {
                            if (i == 1)
                            {
                                var column = new DataGridTextColumn
                                {
                                    Header = $"Понедельник\n{week1.ToShortDateString()}",
                                    Binding = new Binding("Monday")
                                };
                                this.SourceGrid.Columns.Add(column);
                                date1 = week1;

                            }
                            if (i == 2)
                            {
                                var column = new DataGridTextColumn
                                {
                                    Header = $"Вторник\n{week1.ToShortDateString()}",
                                    Binding = new Binding("Tuesday")
                                };
                                this.SourceGrid.Columns.Add(column);
                            }
                            if (i == 3)
                            {
                                var column = new DataGridTextColumn
                                {
                                    Header = $"Среда\n{week1.ToShortDateString()}",
                                    Binding = new Binding("Wednesday")
                                };
                                this.SourceGrid.Columns.Add(column);
                            }
                            if (i == 4)
                            {
                                var column = new DataGridTextColumn
                                {
                                    Header = $"Четверг\n{week1.ToShortDateString()}",
                                    Binding = new Binding("Thursday")
                                };
                                this.SourceGrid.Columns.Add(column);
                            }
                            if (i == 5)
                            {
                                var column = new DataGridTextColumn
                                {
                                    Header = $"Пятница\n{week1.ToShortDateString()}",
                                    Binding = new Binding("Friday")
                                };
                                this.SourceGrid.Columns.Add(column);
                            }
                            if (i == 6)
                            {
                                var column = new DataGridTextColumn
                                {
                                    Header = $"Суббота\n{week1.ToShortDateString()}",
                                    Binding = new Binding("Saturday")
                                };
                                this.SourceGrid.Columns.Add(column);
                                date2 = week1;
                            }
                        }
                        this.WeekName.Content = $"Неделя с {date1.ToShortDateString()} - {date2.ToShortDateString()}";
                        SourceGrid.ItemsSource = weekSubjects;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            ViewSubjects.SelectedItems.Clear();
            GroupsBox.SelectedItem = null;
            PickerStart.SelectedDate = null;
            PickerEnd.SelectedDate = null;
            UpdateHoursGrid();
        }
        private void CreateAllHoursTable()
        {
            var allHours = AllHoursRepository.GetAllHoursOfCurrentTeacher(teacher.id);
            List<string> subs = new List<string>();
            var currentSubs = allHours.Select(s => s.subjectName.ToString()).ToList();
            if(allSubjectHours.Count > 0)
            {
                foreach (var sub in allSubjectHours)
                {
                    if (!subs.Contains(sub.Subject.ToString()))
                    {
                        subs.Add(sub.Subject.ToString());
                    }
                }
                foreach (var sub in subs)
                {
                    if (!currentSubs.Contains(sub.ToString()))
                    {
                        currentSubs.Add(sub.ToString());
                        AllHoursOfSubjects window = new AllHoursOfSubjects(sub);
                        window.temp = GetHour;
                        window.ShowDialog();

                        var h = new AllHours();
                        h.subjectName = sub;
                        h.countHours = allHoursOfSubject;
                        h.teacherId = id;
                        AllHoursRepository.Add(h);
                    }
                }
                allHours = AllHoursRepository.GetAllHoursOfCurrentTeacher(teacher.id);
                currentSubs = allHours.Select(s => s.subjectName.ToString()).ToList();
                foreach (var sub in currentSubs)
                {
                    if (!subs.Contains(sub))
                    {
                        var sb = AllHoursRepository.GetTeacherHourByName(id, sub);
                        AllHoursRepository.Delete(sb);
                    }
                }
            }
            ResultGrid.ItemsSource = AllHoursRepository.GetAllHoursOfCurrentTeacher(id);
        }
        public void CreateTeachFolder(int id)
        {
            var teachLogin = teacher.login;
            string projectDir = AppDomain.CurrentDomain.BaseDirectory;
            string dir = System.IO.Path.Combine(projectDir, $"Расписание\\{teachLogin}");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
        private void ChangeHour_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var hour = button?.DataContext as AllHours;
                var subj = AllHoursRepository.GetHourById(hour.id);
                if (hour.countHours < 0)
                {
                    MessageBox.Show("Общее кол-во часов не может быть меньше 0", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    AllHoursRepository.ChangeCountOfHours(subj, hour.countHours);
                    MessageBox.Show("Изменения сохранены!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    CreateAllHoursTable();
                }
            }
            catch(Exception)
            {
                MessageBox.Show("Введите не корректные данные.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Close(object sender, CancelEventArgs e)
        {
            if(sourceSave == false)
            {
                if (MessageBox.Show($"Расписание не сохранено, перелистывание приведёт к потере данных.\nСохранить раписание?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (weekSubjects != null && weekSubjects.Count > 0)
                    {
                        var teachLogin = teacher.login;
                        string projectDir = AppDomain.CurrentDomain.BaseDirectory;
                        string dir = System.IO.Path.Combine(projectDir, $"Расписание\\{teachLogin}");
                        string file = fileName;
                        string json = JsonConvert.SerializeObject(weekSubjects, Formatting.Indented);
                        File.WriteAllText($"{dir}\\{file}", json);
                        MessageBox.Show("Файл успешно сохранён.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        private void Spravka_Click(object sender, RoutedEventArgs e)
        {
            string helpPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "spravka.chm");
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(helpPath) { UseShellExecute = true });
        }
    }
}