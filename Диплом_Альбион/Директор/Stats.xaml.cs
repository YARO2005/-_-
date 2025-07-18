using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Диплом_Альбион;

namespace Диплом_Альбион.Директор
{
    /// <summary>
    /// Логика взаимодействия для Products.xaml
    /// </summary>
    public partial class Stats : Page
    {
        public Stats()
        {
            InitializeComponent();
        }

        public Персональные_данные selected_user;
        public Work_Director_Window work_director_window;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CB_Товары.ItemsSource = Entities.GetContext().Товары.Select(p=>p.Название).ToList();
            CB_Сотрудник.ItemsSource = Entities.GetContext().Персональные_данные.Where(p => p.Роли.Название != "Клиент").Select(p => p.Фамилия + " " + p.Имя + " " + p.Отчество + " " + p.Телефон).ToList(); ;
        }

        public string GetHash(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            return Convert.ToBase64String(hash);
        }

        private void Button_Общие_продажи_Click(object sender, RoutedEventArgs e)
        {
            if (DP_From.SelectedDate == null || DP_To.SelectedDate == null)
            { MessageBox.Show("Не задан промежуток времени"); }
            else if (DP_From.SelectedDate > DP_To.SelectedDate)
            { MessageBox.Show("Начальная дата не может быть позже конечной"); }
            else
            {
                var воз = Entities.GetContext().Заказ.Where(p => p.ДатаОформления >= DP_From.SelectedDate && p.ДатаОформления <= DP_To.SelectedDate).FirstOrDefault();
                if (воз != null)
                {
                    decimal итог = Entities.GetContext().Заказ.Where(p => p.ДатаОформления >= DP_From.SelectedDate && p.ДатаОформления <= DP_To.SelectedDate).Select(p => p.Сумма).Sum();
                    TB_Итог.Text = $"{итог} руб.";
                }
                else { TB_Итог.Text = $"{0} руб."; }
            }
        }

        private void Button_Колво_продаж_по_товару_Click(object sender, RoutedEventArgs e)
        {
            if (DP_From1.SelectedDate == null || DP_To1.SelectedDate == null || CB_Товары.SelectedItem == null)
            { MessageBox.Show("Не все поля были заполнены"); }
            else if (DP_From1.SelectedDate > DP_To1.SelectedDate)
            { MessageBox.Show("Начальная дата не может быть позже конечной"); }
            else
            {
                var id_товара = Entities.GetContext().Товары.Where(p=>p.Название == CB_Товары.SelectedItem.ToString()).FirstOrDefault();
                var проверка = Entities.GetContext().Товар_Заказ.Where(p => p.ID_Товара == id_товара.ID_Товара && (p.Заказ.ДатаОформления >= DP_From1.SelectedDate && p.Заказ.ДатаОформления <= DP_To1.SelectedDate)).FirstOrDefault();
                if (проверка != null)
                {
                    var итог = Entities.GetContext().Товар_Заказ.Where(p => p.ID_Товара == id_товара.ID_Товара && (p.Заказ.ДатаОформления >= DP_From1.SelectedDate && p.Заказ.ДатаОформления <= DP_To1.SelectedDate)).Select(p => p.Товары.Цена).Sum();
                    var кол_во = Entities.GetContext().Товар_Заказ.Where(p => p.ID_Товара == id_товара.ID_Товара && (p.Заказ.ДатаОформления >= DP_From1.SelectedDate && p.Заказ.ДатаОформления <= DP_To1.SelectedDate)).Select(p => p.Товары.Цена).Count(); 
                    TB_Итог1.Text = $"{кол_во}шт.*{Math.Round(id_товара.Цена, 2)}, {Math.Round(итог, 2)} руб.";
                }
                else { TB_Итог1.Text = $"{0}шт., {0} руб."; }
            }
        }

        class Товары_для_рейтинга
        {
            public int ID_Товара { get; set; }
            public string Название { get; set; }
            public decimal Цена { get; set; }
            public string Производитель { get; set; }
            public string Категория { get; set; }
            public string Описание { get; set; }
            public int Кол_во {get; set;}
        }

        private void Button_Рейтинг(object sender, RoutedEventArgs e)
        {
            if (DP_From2.SelectedDate == null || DP_To2.SelectedDate == null)
            { MessageBox.Show("Не все поля были заполнены"); }
            else if (DP_From2.SelectedDate > DP_To2.SelectedDate)
            { MessageBox.Show("Начальная дата не может быть позже конечной"); }
            else
            {
                var заказы = Entities.GetContext().Заказ.Where(p => p.ДатаОформления >= DP_From2.SelectedDate && p.ДатаОформления <= DP_To2.SelectedDate).ToList();
                List<Товар_Заказ> товзак = new List<Товар_Заказ>();
                List<Товары> товары = new List<Товары>();
                List<Товары_для_рейтинга> товрейт = new List<Товары_для_рейтинга>();
                if (заказы.FirstOrDefault() != null)
                {
                    foreach (var i in заказы)
                    { товзак.AddRange(Entities.GetContext().Товар_Заказ.Where(p => p.ID_Заказа == i.ID_Заказа).ToList()); }
                    foreach (var i in товзак)
                    { товары.Add(Entities.GetContext().Товары.Where(p=>p.ID_Товара == i.ID_Товара).FirstOrDefault()); }
                    foreach (var i in товары)
                    {
                        if (товрейт.Where(p=>p.ID_Товара == i.ID_Товара).FirstOrDefault() == null)
                        {
                            Товары_для_рейтинга тдр = new Товары_для_рейтинга
                            {
                                ID_Товара = i.ID_Товара,
                                Название = i.Название,
                                Цена = i.Цена,
                                Производитель = i.Производитель.Название,
                                Категория = i.Категории.Название,
                                Описание = i.Описание,
                                Кол_во = 1
                            };
                            товрейт.Add(тдр);
                        }
                        else
                        {
                            var тов = товрейт.Where(p => p.ID_Товара == i.ID_Товара).FirstOrDefault();
                            тов.Кол_во ++;
                        }
                    }
                    DG_Итог2.ItemsSource = товрейт.ToList();
                }
                else { DG_Итог2.ItemsSource = null; MessageBox.Show("Для таких дат не было найдено заказов"); }
            }
        }

        private void Button_Заказы_по_Сотруднику_Click(object sender, RoutedEventArgs e)
        {
            if (DP_From3.SelectedDate == null || DP_To3.SelectedDate == null || CB_Сотрудник.SelectedItem == null)
            { MessageBox.Show("Не все поля были заполнены"); }
            else if (DP_From3.SelectedDate > DP_To3.SelectedDate)
            { MessageBox.Show("Начальная дата не может быть позже конечной"); }
            else
            {
                string[] nsp = CB_Сотрудник.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); //переменная массив, где содержится ФИО
                var фамилия = nsp[0]; var имя = nsp[1]; var отчество = nsp[2]; var телефон = nsp[3];
                var ПерсДанные = Entities.GetContext().Персональные_данные.Where(p => p.Фамилия.Contains(фамилия) && p.Имя.Contains(имя) && p.Отчество.Contains(отчество) && p.Телефон.Contains(телефон)).Select(p => p.ID_ПерсДанных).FirstOrDefault();
                var Сотрудник = Entities.GetContext().Сотрудники.Where(p=>p.ID_ПерсДанных == ПерсДанные).FirstOrDefault();
                var заказы = Entities.GetContext().Заказ.Where(p => p.ID_Сотрудника == Сотрудник.ID_Сотрудника && (p.ДатаОформления >= DP_From3.SelectedDate && p.ДатаОформления <= DP_To3.SelectedDate)).ToList();
                if (заказы.FirstOrDefault() != null)
                {
                    DG_Orders_Worker.ItemsSource = заказы;
                }
                else { DG_Orders_Worker.ItemsSource = null; MessageBox.Show("Для таких дат не было найдено заказов"); }
            }
        }
    }
}
