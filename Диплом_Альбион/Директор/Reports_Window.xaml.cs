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

namespace Диплом_Альбион.Директор
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
            list_cb_вид.Add("Отчет");
            CB_Вид.ItemsSource = list_cb_вид;
        }

        public Orders_Designs orders;
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
            var путь_к_шаблону = @"C:\Users\Zakalka\source\repos\Диплом_Альбион\Диплом_Альбион\Шаблоны_заказ_дизайна\Договор физ. л..docx";

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
                document.ReplaceText("{телефон}", orders.selected_order.Сотрудники.Персональные_данные.Телефон.ToString());

                //3. Сумма
                document.ReplaceText("{сумма_заказа}", orders.selected_order.Сумма.ToString());

                document.SaveAs(filePath);
                System.Windows.MessageBox.Show("Договор успешно создан");
            }

        }

        private void GeneratePdfDogovorUrlic(string filePath) //Договор на юр лицо
        {
            var путь_к_шаблону = @"C:\Users\Zakalka\source\repos\Диплом_Альбион\Диплом_Альбион\Шаблоны_заказ_дизайна\Договор юр. л..docx";

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
                document.ReplaceText("{телефон}", orders.selected_order.Сотрудники.Персональные_данные.Телефон.ToString());

                //3. Сумма
                document.ReplaceText("{сумма_заказа}", orders.selected_order.Сумма.ToString());

                document.SaveAs(filePath);
                System.Windows.MessageBox.Show("Договор успешно создан");
            }
        }
    }
}
