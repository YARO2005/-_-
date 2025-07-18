using System;
using System.Collections.Generic;
using System.IO;
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
using Диплом_Альбион.Менеджер;
using Microsoft.Win32;
using static System.Windows.Forms.DialogResult;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Windows.Forms;
using Xceed.Words.NET;
using Xceed.Document.NET;

namespace Диплом_Альбион.Менеджер
{
    /// <summary>
    /// Логика взаимодействия для Search_Products_Window.xaml
    /// </summary>
    public partial class Reports_Window : Window
    {
        public Reports_Window()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> list_cb_вид = new List<string>();
            list_cb_вид.Add("Договор Физ.л.");
            list_cb_вид.Add("Договор Юр.л.");
            list_cb_вид.Add("Спецификация");
            list_cb_вид.Add("Отчет");
            CB_Вид.ItemsSource = list_cb_вид;
        }

        public Orders orders;
        Regex Regex_Название = new Regex(@"^[0-9А-яA-z_-]{2,30}$");
        string selectedFilePath = null;

        private void button_указать_путь_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                selectedFilePath = dialog.SelectedPath;
            }
            if (selectedFilePath != null)
            {
                TB_Путь.Text = selectedFilePath;
            }
        }

        private void Button_Report_Click(object sender, RoutedEventArgs e)
        {
            if (orders.selected_order == null) { System.Windows.MessageBox.Show("Не выбран заказ, для которого создается договор"); }
            else if (TB_Название.Text == "" || CB_Вид.SelectedItem == null || TB_Путь.Text == null)
            { System.Windows.MessageBox.Show("Не все поля были заполнены"); }
            else if (!Regex_Название.IsMatch(TB_Название.Text))
            {
                System.Windows.MessageBox.Show("Название файла не соответсвует правилам", "Ошибка");
            }
            else
            {
                if (selectedFilePath == null) { System.Windows.MessageBox.Show("Вы не выбрали путь", "Ошибка"); }
                else
                {
                    var path = $@"{selectedFilePath}\{TB_Название.Text}";
                    switch (CB_Вид.SelectedItem)
                    {
                        case "Договор Физ.л.":
                            GeneratePdfDogovorFizlic(path);
                            break;

                        case "Договор Юр.л.":
                            GeneratePdfDogovorUrlic(path);
                            break;

                        case "Спецификация":
                            GeneratePdfSpecification(path);
                            break;

                        case "Отчет":
                            GeneratePdfReport(path);
                            break;
                    }
                }
            }
        }

        private void GeneratePdfReport(string filePath) //Отчет
        {
            
        }

        private void GeneratePdfDogovorFizlic(string filePath) //Договор на физ лицо
        {
            var путь_к_шаблону = @"C:\Users\Zakalka\source\repos\Диплом_Альбион\Диплом_Альбион\Шаблоны_заказ_товара\Договор физ. л..docx";

            if (!File.Exists(путь_к_шаблону))
            {
                System.Windows.MessageBox.Show("Шаблон не найден!");
                return;
            }

            using (var document = DocX.Load(путь_к_шаблону))
            {
                document.ReplaceText("{номер_заказа}", orders.selected_order.ID_Заказа.ToString());

                //титульник дата
                document.ReplaceText("{день}", orders.selected_order.ДатаОформления.Date.Day.ToString());
                document.ReplaceText("{месяц}", orders.selected_order.ДатаОформления.Date.Month.ToString());
                document.ReplaceText("{год}", orders.selected_order.ДатаОформления.Date.Year.ToString());

                // фио заказчика 
                document.ReplaceText("{фамилия}", orders.selected_order.Клиенты.Персональные_данные.Фамилия.ToString());
                document.ReplaceText("{имя}", orders.selected_order.Клиенты.Персональные_данные.Имя.ToString());
                document.ReplaceText("{отчество}", orders.selected_order.Клиенты.Персональные_данные.Отчество.ToString());
                document.ReplaceText("{серия}", orders.selected_order.Клиенты.Персональные_данные.Серия.ToString());
                document.ReplaceText("{номер}", orders.selected_order.Сотрудники.Персональные_данные.Номер.ToString());

                //1. Сумма
                document.ReplaceText("{сумма_заказа}", orders.selected_order.Сумма.ToString());

                //3. дата+5 дней и+15
                var date_5 = orders.selected_order.ДатаОформления;
                date_5 = date_5.AddDays(5);
                document.ReplaceText("{день_5}", date_5.Day.ToString());
                document.ReplaceText("{месяц_5}", date_5.Month.ToString());
                document.ReplaceText("{год_5}", date_5.Year.ToString());
                var date_15 = orders.selected_order.ДатаОформления;
                date_15 = date_15.AddDays(15);
                document.ReplaceText("{день_15}", date_15.Day.ToString());
                document.ReplaceText("{месяц_15}", date_15.Month.ToString());
                document.ReplaceText("{год_15}", date_15.Year.ToString());

                //5 день +3
                var date_3 = orders.selected_order.ДатаОформления;
                date_3 = date_3.AddDays(3);
                document.ReplaceText("{день_3}", date_3.Day.ToString());
                document.ReplaceText("{месяц_3}", date_3.Month.ToString());
                document.ReplaceText("{год_3}", date_3.Year.ToString());

                document.SaveAs(filePath);
                System.Windows.MessageBox.Show("Договор успешно создан");
                GeneratePdfSpecification(filePath);
            }

        }

        private void GeneratePdfDogovorUrlic(string filePath) //Договор на юр лицо
        {
            var путь_к_шаблону = @"C:\Users\Zakalka\source\repos\Диплом_Альбион\Диплом_Альбион\Шаблоны_заказ_товара\Договор  юр. л..docx";

            if (!File.Exists(путь_к_шаблону))
            {
                System.Windows.MessageBox.Show("Шаблон не найден!");
                return;
            }

            using (var document = DocX.Load(путь_к_шаблону))
            {
                document.ReplaceText("{номер_заказа}", orders.selected_order.ID_Заказа.ToString());

                //титульник дата
                document.ReplaceText("{день}", orders.selected_order.ДатаОформления.Date.Day.ToString());
                document.ReplaceText("{месяц}", orders.selected_order.ДатаОформления.Date.Month.ToString());
                document.ReplaceText("{год}", orders.selected_order.ДатаОформления.Date.Year.ToString());

                // фио заказчика 
                document.ReplaceText("{фамилия}", orders.selected_order.Клиенты.Персональные_данные.Фамилия.ToString());
                document.ReplaceText("{имя}", orders.selected_order.Клиенты.Персональные_данные.Имя.ToString());
                document.ReplaceText("{отчество}", orders.selected_order.Клиенты.Персональные_данные.Отчество.ToString());

                //1. Сумма
                document.ReplaceText("{сумма_заказа}", orders.selected_order.Сумма.ToString());

                //3. дата+5 дней и+15
                var date_5 = orders.selected_order.ДатаОформления;
                date_5 = date_5.AddDays(5);
                document.ReplaceText("{день_5}", date_5.Day.ToString());
                document.ReplaceText("{месяц_5}", date_5.Month.ToString());
                document.ReplaceText("{год_5}", date_5.Year.ToString());
                var date_15 = orders.selected_order.ДатаОформления;
                date_15 = date_15.AddDays(15);
                document.ReplaceText("{день_15}", date_15.Day.ToString());
                document.ReplaceText("{месяц_15}", date_15.Month.ToString());
                document.ReplaceText("{год_15}", date_15.Year.ToString());

                //5 день +3
                var date_3 = orders.selected_order.ДатаОформления;
                date_3 = date_3.AddDays(3);
                document.ReplaceText("{день_3}", date_3.Day.ToString());
                document.ReplaceText("{месяц_3}", date_3.Month.ToString());
                document.ReplaceText("{год_3}", date_3.Year.ToString());

                document.SaveAs(filePath);
                System.Windows.MessageBox.Show("Договор успешно создан");
                GeneratePdfSpecification(filePath);
            }
        }

        private void GeneratePdfSpecification(string filePath) //Спецификация к договору
        {
            var путь_к_шаблону = @"C:\Users\Zakalka\source\repos\Диплом_Альбион\Диплом_Альбион\Шаблоны_заказ_товара\Спецификация.docx";

            if (!File.Exists(путь_к_шаблону))
            {
                System.Windows.MessageBox.Show("Шаблон не найден!");
                return;
            }

            using (var document = DocX.Load(путь_к_шаблону))
            {
                filePath = $@"{filePath}_Спецификация_№{orders.selected_order.ID_Заказа}_{orders.selected_order.ДатаОформления.Day.ToString()}.{orders.selected_order.ДатаОформления.Date.Month.ToString()}.{orders.selected_order.ДатаОформления.Date.Year.ToString()}";

                document.ReplaceText("{номер_заказа}", orders.selected_order.ID_Заказа.ToString());
                document.ReplaceText("{день}", orders.selected_order.ДатаОформления.Date.Day.ToString());
                document.ReplaceText("{месяц}", orders.selected_order.ДатаОформления.Date.Month.ToString());
                document.ReplaceText("{год}", orders.selected_order.ДатаОформления.Date.Year.ToString());
                document.ReplaceText("{сумма_заказа}", orders.selected_order.Сумма.ToString());
                document.ReplaceText("{фамилия}", orders.selected_order.Клиенты.Персональные_данные.Фамилия.ToString());
                document.ReplaceText("{имя}", orders.selected_order.Клиенты.Персональные_данные.Имя.ToString());
                document.ReplaceText("{отчество}", orders.selected_order.Клиенты.Персональные_данные.Отчество.ToString());

                // Создаем таблицу
                var products_count = Entities.GetContext().Товар_Заказ.Where(p => p.ID_Заказа == orders.selected_order.ID_Заказа).Select(p=>p.ID_Товара).ToList().Count() ;
                Xceed.Document.NET.Table table = document.AddTable(products_count + 1, 3); // +1 для заголовков
                table.Alignment = Alignment.center;
                table.Design = TableDesign.TableGrid;

                // Заголовки таблицы
                table.Rows[0].Cells[0].Paragraphs[0].Append("Название").Bold().FontSize(10).Font("Times New Roman");
                table.Rows[0].Cells[1].Paragraphs[0].Append("Количество").Bold().FontSize(10).Font("Times New Roman");
                table.Rows[0].Cells[2].Paragraphs[0].Append("Цена * шт.").Bold().FontSize(10).Font("Times New Roman");

                var products = Entities.GetContext().Товар_Заказ.Where(p => p.ID_Заказа == orders.selected_order.ID_Заказа).ToList();
                // Добавляем товары
                for (int i = 0; i < products_count; i++)
                {
                    var товар = products[i];
                    table.Rows[i + 1].Cells[0].Paragraphs[0].Append(товар.Товары.Название).FontSize(10).Font("Times New Roman");
                    table.Rows[i + 1].Cells[1].Paragraphs[0].Append(товар.Количество.ToString()).FontSize(10).Font("Times New Roman");
                    table.Rows[i + 1].Cells[2].Paragraphs[0].Append(товар.Товары.Цена.ToString()).FontSize(10).Font("Times New Roman");
                }

                // Вставляем таблицу после удалённой метки
                foreach (var paragraph in document.Paragraphs)
                {
                    if (paragraph.Text.Contains("{таблица}"))
                    {
                        paragraph.InsertTableAfterSelf(table);
                        break;
                    }
                }
                document.ReplaceText("{таблица}", "");

                // Сохраняем документ
                document.SaveAs(filePath);
            }

            System.Windows.MessageBox.Show("Спецификация успешно создана!");
        }
    }
}
