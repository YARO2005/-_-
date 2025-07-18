using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
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
using Диплом_Альбион.Директор;

namespace Диплом_Альбион
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Tip_Логин.Visibility = Visibility.Hidden;
            Tip_Пароль.Visibility = Visibility.Hidden;
        }

        public static int worker_id;
        public static int role_id;

        Regex Regex_Логин = new Regex(@"^[A-z,А-я,0-9]{3,20}$");
        Regex Regex_Пароль = new Regex(@"^[!*./&?%$#№0-9A-zА-я]{3,10}$");

        private void Button_Log_Click(object sender, RoutedEventArgs e)
        {
            if (TB_Логин.Text != null && PB_Пароль.Password != null)
            {
                if (Regex_Логин.IsMatch(TB_Логин.Text) == true && Regex_Пароль.IsMatch(PB_Пароль.Password))
                {
                    var пароль = GetHash(PB_Пароль.Password);
                    var user = Entities.GetContext().Персональные_данные.Where(p=>p.Пользователи.Логин == TB_Логин.Text && p.Пользователи.Пароль == пароль).FirstOrDefault();
                    if (user != null)
                    {
                        var сотрудник = Entities.GetContext().Сотрудники.Where(p=>p.ID_ПерсДанных == user.ID_ПерсДанных).FirstOrDefault();
                        worker_id = (int)сотрудник.ID_Сотрудника;
                        role_id = (int)user.ID_Роли;
                        switch (user.ID_Роли)
                        {
                            case 2:
                                Work_Manager_Window wM = new Work_Manager_Window();
                                wM.Show();
                                MessageBox.Show("Вы успешно авторизовались");
                                this.Close();
                                break;

                            case 3:
                                //Work_Designer_Window wD = new Work_Designer_Window();
                                //wD.Show();
                                //this.Close();
                                MessageBox.Show("Вы успешно авторизовались");
                                MessageBox.Show("Дизайнер");
                                break;

                            case 4:
                                Work_Between_Window wbw = new Work_Between_Window();
                                wbw.Show();
                                MessageBox.Show("Вы успешно авторизовались");
                                this.Close();
                                break;
                        }

                    }
                    else { MessageBox.Show("Пользователь не найден!", "Ошибка"); }
                }
                else { MessageBox.Show("Поля заполнены не верно!", "Ошибка"); }
            }
            else { MessageBox.Show("Не все поля были заполнены!", "Ошибка"); }
        }

        public string GetHash(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            return Convert.ToBase64String(hash);
        }

        private void TB_Логин_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TB_Логин.Text == "")
            {
                mark_логин.Text = "<";
                mark_логин.Foreground = Brushes.Gray;
                Tip_Логин.Visibility = Visibility.Hidden;
                Tip_Логин.Text = "Введите логин";
            }
            else
            {
                if (Regex_Логин.IsMatch(TB_Логин.Text))
                {
                    mark_логин.Text = "*";
                    mark_логин.Foreground = Brushes.Green;
                    Tip_Логин.Foreground = Brushes.Green;
                    Tip_Логин.Visibility = Visibility.Visible;
                    Tip_Логин.Text = "Логин соответствует шаблону";
                }
                else
                {
                    mark_логин.Text = "!";
                    mark_логин.Foreground = Brushes.Red;
                    Tip_Логин.Foreground = Brushes.Red;
                    Tip_Логин.Visibility = Visibility.Visible;
                    Tip_Логин.Text = "Разрешенные символы: кириллица, латиница и !*./&%$#0-9A-Za-zА-Яа-я. Логин должен быть не больше 20 и не меньше 3 символов";
                }
            }
        }

        private void PB_Пароль_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PB_Пароль.Password == "")
            {
                mark_пароль.Text = "<";
                mark_пароль.Foreground = Brushes.Gray;
                Tip_Пароль.Visibility = Visibility.Hidden;
                Tip_Пароль.Text = "Введите пароль";
            }
            else
            {
                if (Regex_Пароль.IsMatch(PB_Пароль.Password))
                {
                    mark_пароль.Text = "*";
                    mark_пароль.Foreground = Brushes.Green;
                    Tip_Пароль.Foreground = Brushes.Green;
                    Tip_Пароль.Visibility = Visibility.Visible;
                    Tip_Пароль.Text = "Логин соответствует шаблону";

                }
                else
                {
                    mark_пароль.Text = "!";
                    mark_пароль.Foreground = Brushes.Red;
                    Tip_Пароль.Foreground = Brushes.Red;
                    Tip_Пароль.Visibility = Visibility.Visible;
                    Tip_Пароль.Text = "Разрешенные символы: кириллица, латиница. Не больше 10 и не меньше 3 символов";
                }
            }
        }

    }
}
