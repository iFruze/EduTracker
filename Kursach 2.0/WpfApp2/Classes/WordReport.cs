using Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp2.Interfaces;

namespace WpfApp2.Classes
{
    internal class WordReport : IReporter
    {
        public void GenerateReport(int teacherId, List<List<string>> resultReport, string month, string year)
        {
            string projectDir = AppDomain.CurrentDomain.BaseDirectory;
            string dir = System.IO.Path.Combine(projectDir, "Отчёты");
            var save = new SaveFileDialog
            {
                Title = "Сохранить файл как",
                Filter = "Документы Word (*.docx)|*.docx",
                FileName = $"Отчёт.docx",
                InitialDirectory = dir
            };
            string file = $"Отчёт.docx";
            if (save.ShowDialog() == true)
            {
                file = save.FileName;
                for (int i = 0; i < resultReport.Count; i++)
                {
                    int sum = 0;
                    for (int j = 1; j < resultReport[i].Count; j++)
                    {
                        string[] temp = resultReport[i][j].Split('-');
                        if (int.TryParse(temp[1].Trim(), out int num))
                        {
                            sum += num;
                        }
                    }
                    string subName = resultReport[i][0];
                    var sbj = AllHoursRepository.GetTeacherHourByName(teacherId, subName);
                    resultReport[i].Add($"Всего часов: {sbj.countHours}");
                    resultReport[i].Add($"Вычтено: {sum}");
                    resultReport[i].Add($"Осталось: {sbj.countHours - sum}");
                    AllHoursRepository.SubtractHours(sbj, sum);
                }
                int[] arr = new int[resultReport.Count];
                for (int i = 0; i < resultReport.Count; i++)
                {
                    arr[i] = resultReport[i].Count;
                }
                int max = arr.Max();
                Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
                Microsoft.Office.Interop.Word.Document doc = word.Documents.Add();
                doc.PageSetup.PaperSize = WdPaperSize.wdPaperA3;
                doc.PageSetup.Orientation = WdOrientation.wdOrientLandscape;
                Microsoft.Office.Interop.Word.Paragraph paragraph = doc.Content.Paragraphs.Add();
                paragraph.Range.Text = $"Отчёт по отработанным часам за {month} {year} года.";
                paragraph.Range.Font.Size = 16;
                paragraph.Range.Font.Bold = 1;
                paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                paragraph.Range.InsertParagraphAfter();
                Microsoft.Office.Interop.Word.Range tableRange = doc.Content.Paragraphs.Add().Range;
                Microsoft.Office.Interop.Word.Table table = doc.Tables.Add(tableRange, max, resultReport.Count);
                table.Borders.Enable = 1;
                table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                table.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);
                table.Rows.Alignment = WdRowAlignment.wdAlignRowCenter;
                for (int col = 0; col < resultReport.Count; col++)
                {
                    for (int row = 0; row < resultReport[col].Count; row++)
                    {
                        table.Cell(row + 1, col + 1).Range.Text = resultReport[col][row];
                        table.Cell(row + 1, col + 1).Range.Font.Bold = 0;
                    }
                }
                paragraph.Range.Text = $"Дата создания: {DateTime.Now.ToShortDateString()}";
                paragraph.Range.Font.Size = 16;
                paragraph.Range.Font.Bold = 1;
                paragraph.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                paragraph.Range.InsertParagraphAfter();
                doc.SaveAs2(file);
                doc.Close();
                word.Quit();
                
            }
        }
    }
}
