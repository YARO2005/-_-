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

namespace Диплом_Альбион.Директор
{
    /// <summary>
    /// Логика взаимодействия для Search_Products_Window.xaml
    /// </summary>
    public partial class Search_Users_Window : Window
    {
        public Search_Users_Window()
        {
            InitializeComponent();
            CB_role.ItemsSource = Entities.GetContext().Роли.Select(p => p.Название).ToList();
        }

        public Users users;

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (CB_role.SelectedItem != null)
            {
                if (CheckBox_role.IsChecked == true && CheckBox_number.IsChecked == false && CheckBox_sname.IsChecked == false)
                {
                    users.DG_Users.ItemsSource = Entities.GetContext().Персональные_данные.Include("Роли").Include("Пользователи").Where(p => p.Роли.Название.Contains(CB_role.Text)).ToList();
                }
            }
                if (CheckBox_role.IsChecked == false && CheckBox_number.IsChecked == true && CheckBox_sname.IsChecked == false)
                {
                    users.DG_Users.ItemsSource = Entities.GetContext().Персональные_данные.Include("Роли").Include("Пользователи").Where(p => p.Телефон.Contains(TB_number.Text)).ToList();
                }

                if (CheckBox_role.IsChecked == false && CheckBox_number.IsChecked == false && CheckBox_sname.IsChecked == true)
                {
                    users.DG_Users.ItemsSource = Entities.GetContext().Персональные_данные.Include("Роли").Include("Пользователи").Where(p => p.Фамилия.Contains(TB_sname.Text)).ToList();
                }

            if (CB_role.SelectedItem != null)
            {
                if (CheckBox_role.IsChecked == true && CheckBox_number.IsChecked == true && CheckBox_sname.IsChecked == true)
                {
                    users.DG_Users.ItemsSource = Entities.GetContext().Персональные_данные.Include("Роли").Include("Пользователи").Where(p => p.Роли.Название.Contains(CB_role.Text) && p.Телефон.Contains(TB_number.Text) && p.Фамилия.Contains(TB_sname.Text)).ToList();
                }
            }

            if (CB_role.SelectedItem != null)
            {
                if (CheckBox_role.IsChecked == true && CheckBox_number.IsChecked == true && CheckBox_sname.IsChecked == false)
                {
                    users.DG_Users.ItemsSource = Entities.GetContext().Персональные_данные.Include("Роли").Include("Пользователи").Where(p => p.Роли.Название.Contains(CB_role.Text) && p.Телефон.Contains(TB_number.Text)).ToList();
                }
            }

            if (CB_role.SelectedItem != null)
            {
                if (CheckBox_role.IsChecked == true && CheckBox_number.IsChecked == false && CheckBox_sname.IsChecked == true)
                {
                    users.DG_Users.ItemsSource = Entities.GetContext().Персональные_данные.Include("Роли").Include("Пользователи").Where(p => p.Роли.Название.Contains(CB_role.Text) && p.Фамилия.Contains(TB_sname.Text)).ToList();
                }
            }

                if (CheckBox_role.IsChecked == false && CheckBox_number.IsChecked == true && CheckBox_sname.IsChecked == true)
                {
                    users.DG_Users.ItemsSource = Entities.GetContext().Персональные_данные.Include("Роли").Include("Пользователи").Where(p => p.Телефон.Contains(TB_number.Text) && p.Фамилия.Contains(TB_sname.Text)).ToList();
                }

                if (CheckBox_role.IsChecked == false && CheckBox_number.IsChecked == false && CheckBox_sname.IsChecked == false)
                {
                    users.DG_Users.ItemsSource = Entities.GetContext().Персональные_данные.Include("Роли").Include("Пользователи").ToList();
                }

        } 
    }
}
