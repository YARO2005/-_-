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
    public partial class Search_Customers_Window : Window
    {
        public Search_Customers_Window()
        {
            InitializeComponent();
        }

        public Customers customer;

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBox_sname.IsChecked == true && CheckBox_number.IsChecked == false)
            {
                customer.DG_Customers.ItemsSource = Entities.GetContext().Персональные_данные.Where(p=>p.Фамилия.Contains(TB_sname.Text) && p.ID_Роли == 1).ToList();
            }
            if (CheckBox_sname.IsChecked == false && CheckBox_number.IsChecked == true)
            {
                customer.DG_Customers.ItemsSource = Entities.GetContext().Персональные_данные.Where(p => p.Телефон.Contains(TB_number.Text) && p.ID_Роли == 1).ToList();
            }
            if (CheckBox_sname.IsChecked == true && CheckBox_number.IsChecked == true)
            {
                customer.DG_Customers.ItemsSource = Entities.GetContext().Персональные_данные.Where(p => (p.Фамилия.Contains(TB_sname.Text) && p.Телефон.Contains(TB_number.Text)) && p.ID_Роли == 1).ToList();
            }
            if (CheckBox_sname.IsChecked == false && CheckBox_number.IsChecked == false)
            {
                customer.DG_Customers.ItemsSource = Entities.GetContext().Персональные_данные.Where(p=>p.ID_Роли == 1).ToList();
            }
        }
    }
}
