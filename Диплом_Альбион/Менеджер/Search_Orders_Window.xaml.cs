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
using Диплом_Альбион.Менеджер;
using static Диплом_Альбион.Менеджер.Products;

namespace Диплом_Альбион.Менеджер
{
    /// <summary>
    /// Логика взаимодействия для Search_Products_Window.xaml
    /// </summary>
    public partial class Search_Orders_Window : Window
    {
        public Search_Orders_Window()
        {
            InitializeComponent();
            CB_status.ItemsSource = Entities.GetContext().Статус.Select(p => p.Название).ToList();
        }

        public Orders orders;

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (CB_status.SelectedItem != null)
            {
                if (CheckBox_status.IsChecked == true && CheckBox_number.IsChecked == false)
                {
                    orders.DG_Orders.ItemsSource = Entities.GetContext().Заказ.Include("Клиенты").Include("Сотрудники").Include("Статус").Where(p => p.Статус.Название.Contains(CB_status.Text)).Where(p => p.Виды_заказов.Название == "Товар").ToList();
                }
            }
                if (CheckBox_status.IsChecked == false && CheckBox_number.IsChecked == true)
                {
                    orders.DG_Orders.ItemsSource = Entities.GetContext().Заказ.Include("Клиенты").Include("Сотрудники").Include("Статус").Where(p => p.Клиенты.Персональные_данные.Телефон.Contains(TB_number.Text)).Where(p => p.Виды_заказов.Название == "Товар").ToList();
                }

            if (CB_status.SelectedItem != null)
            {
                if (CheckBox_status.IsChecked == true && CheckBox_number.IsChecked == true)
                {
                    orders.DG_Orders.ItemsSource = Entities.GetContext().Заказ.Include("Клиенты").Include("Сотрудники").Include("Статус").Where(p => p.Статус.Название.Contains(CB_status.Text) && p.Клиенты.Персональные_данные.Телефон.Contains(TB_number.Text)).Where(p => p.Виды_заказов.Название == "Товар").ToList();
                }
            }

                if (CheckBox_status.IsChecked == false && CheckBox_number.IsChecked == false)
                {
                    orders.DG_Orders.ItemsSource = Entities.GetContext().Заказ.Include("Клиенты").Include("Сотрудники").Include("Статус").Where(p => p.Виды_заказов.Название == "Товар").ToList();
                }
            
        }
    }
}
