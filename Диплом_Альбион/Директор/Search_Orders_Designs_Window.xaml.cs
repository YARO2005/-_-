using System;
using System.Collections.Concurrent;
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

namespace Диплом_Альбион.Директор
{
    /// <summary>
    /// Логика взаимодействия для Search_Products_Window.xaml
    /// </summary>
    public partial class Search_Orders_Designs_Window : Window
    {
        public Search_Orders_Designs_Window()
        {
            InitializeComponent();
            CB_status.ItemsSource = Entities.GetContext().Статус.Select(p => p.Название).ToList();
        }

        public Orders_Designs orders_designs;

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (CB_status.SelectedItem != null)
            {
                if (CheckBox_status.IsChecked == true && CheckBox_number.IsChecked == false)
                {
                    orders_designs.DG_Orders.ItemsSource = Entities.GetContext().Заказ.Include("Сотрудники").
                        Include("Клиенты").Include("Статус").Include("Виды_заказов").Where(p => p.Виды_заказов.Название == "Дизайн").
                        Where(p=>p.Статус.Название.Contains(CB_status.Text)).ToList();
                }
            }
            if (CheckBox_status.IsChecked == false && CheckBox_number.IsChecked == true)
            {
                orders_designs.DG_Orders.ItemsSource = Entities.GetContext().Заказ.Include("Сотрудники").
                    Include("Клиенты").Include("Статус").Include("Виды_заказов").Where(p => p.Виды_заказов.Название == "Дизайн").
                    Where(p=>p.Клиенты.Персональные_данные.Телефон.Contains(TB_number.Text)).ToList();
            }

            if (CB_status.SelectedItem != null)
            {
                if (CheckBox_status.IsChecked == true && CheckBox_number.IsChecked == true)
                {
                    orders_designs.DG_Orders.ItemsSource = Entities.GetContext().Заказ.Include("Сотрудники").
                        Include("Клиенты").Include("Статус").Include("Виды_заказов").Where(p => p.Виды_заказов.Название == "Дизайн").
                        Where(p => p.Статус.Название.Contains(CB_status.Text) && p.Клиенты.Персональные_данные.Телефон.Contains(TB_number.Text)).ToList();
                }
            }

            if (CheckBox_status.IsChecked == false && CheckBox_number.IsChecked == false)
            {
                orders_designs.DG_Orders.ItemsSource = Entities.GetContext().Заказ.Include("Сотрудники").
                    Include("Клиенты").Include("Статус").Include("Виды_заказов").Where(p => p.Виды_заказов.Название == "Дизайн").ToList();
            }

        } 
    }
}
