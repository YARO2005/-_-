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
    public partial class Search_Products_Window : Window
    {
        public Search_Products_Window()
        {
            InitializeComponent();
        }

        public Products products;

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBox_name.IsChecked == true && CheckBox_manufacturer.IsChecked == false)
            {
                products.DG_Products.ItemsSource = Entities.GetContext().Товары.Where(p=>p.Название.Contains(TB_name.Text)).ToList();
            }
            if (CheckBox_name.IsChecked == false && CheckBox_manufacturer.IsChecked == true)
            {
                products.DG_Products.ItemsSource = Entities.GetContext().Товары.Where(p => p.Производитель.Название.Contains(TB_manufacturer.Text)).ToList();
            }
            if (CheckBox_name.IsChecked == true && CheckBox_manufacturer.IsChecked == true)
            {
                products.DG_Products.ItemsSource = Entities.GetContext().Товары.Where(p => p.Производитель.Название.Contains(TB_manufacturer.Text) && p.Название.Contains(TB_name.Text)).ToList();
            }
            if (CheckBox_name.IsChecked == false && CheckBox_manufacturer.IsChecked == false)
            {
                products.DG_Products.ItemsSource = Entities.GetContext().Товары.ToList();
            }
        }
    }
}
