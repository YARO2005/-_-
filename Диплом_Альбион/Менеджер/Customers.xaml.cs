using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
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
using static Диплом_Альбион.Менеджер.Search_Products_Window;

namespace Диплом_Альбион.Менеджер
{
    /// <summary>
    /// Логика взаимодействия для Products.xaml
    /// </summary>
    public partial class Customers : Page
    {
        public Customers()
        {
            InitializeComponent();
        }

        bool process_add = false;
        bool process_edit = false;
        Search_Customers_Window search_window;
        public Персональные_данные selected_customer;
        public Work_Manager_Window work_manager_window;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DG_Customers.ItemsSource = Entities.GetContext().Персональные_данные.Include("Роли").Where(p=>p.Роли.ID_Роли == 1).ToList();

            if (work_manager_window.orders.process_add == false)
            {
                Button_Cancel_Order.IsEnabled = false;
                Button_Select.IsEnabled = false;
            }
            else
            {
                Button_Cancel_Order.IsEnabled = true;
                Button_Select.IsEnabled = true;
            }
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            if (process_add == false && process_edit == false)
            {
                work_manager_window.Hot_Button.IsEnabled = false;
                process_add = true;
                TB_Фамилия.Text = "";
                TB_Имя.Text = "";
                TB_Отчество.Text = "";
                TB_Серия.Text = "";
                TB_Номер.Text = "";
                TB_Почта.Text = "";
                TB_Телефон.Text = "";
                Grid_Add_Edit.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Сохраните введенные данные или сделайте отмену, прежде чем начинать новое добавление!", "Ошибка");
            }
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (process_add == false && process_edit == false)
            {
                if (selected_customer != null)
                {
                    MessageBoxResult result = MessageBox.Show("Удалить выбранную запись?", "Подтверждение", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            var клиент = Entities.GetContext().Клиенты.Where(p=>p.ID_ПерсДанных == selected_customer.ID_ПерсДанных).FirstOrDefault();
                            Entities.GetContext().Клиенты.Remove(клиент);
                            Entities.GetContext().Персональные_данные.Remove(selected_customer);
                            Entities.GetContext().SaveChanges();
                            MessageBox.Show("Запись удалена.", "Подтверждение");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"{ex.Message}, {ex.Source}, {ex.InnerException}");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Удаление отменено.", "Подтверждение");
                    }
                }
                else
                {
                    MessageBox.Show("Вы не выбрали запись!", "Ошибка");
                }
            }
            else
            {
                MessageBox.Show("Сохраните введенные данные или сделайте отмену, прежде чем удалять записи!", "Ошибка");
            }
        }

        private void Button_Edit_Click(object sender, RoutedEventArgs e)
        {
            if (process_add == false && process_edit == false)
            {
                if (selected_customer != null)
                {
                    work_manager_window.Hot_Button.IsEnabled = false;
                    process_edit = true;
                    TB_Фамилия.Text = selected_customer.Фамилия;
                    TB_Имя.Text = selected_customer.Имя;
                    TB_Отчество.Text = selected_customer.Отчество;
                    TB_Серия.Text = selected_customer.Серия;
                    TB_Номер.Text = selected_customer.Номер;
                    TB_Почта.Text = selected_customer.Эл_почта;
                    TB_Телефон.Text = selected_customer.Телефон;
                    Grid_Add_Edit.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("Вы не выбрали запись!", "Ошибка");
                }
            }
            else
            {
                MessageBox.Show("Сохраните введенные данные или сделайте отмену, прежде чем начинать редактирование!", "Ошибка");
            }
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (search_window == null || !search_window.IsLoaded)
            {
                search_window = new Search_Customers_Window();
                search_window.Show();
                search_window.customer = this;
            }
            else
            {
                MessageBox.Show("Окно уже открыто.");
            }
        }

        private void Button_Update_Click(object sender, RoutedEventArgs e)
        {
            Page_Loaded(sender, e);
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (process_add == true) //если идет процесс добавления
            {
                Save_Add();
            }
            if (process_edit == true) //если идет процесс редактирования
            {
                Save_Edit();
            }
        }

        public void Save_Add()
        {
            if (TB_Фамилия.Text == "" || TB_Имя.Text == "" || TB_Отчество.Text == "" || TB_Серия.Text == "" || TB_Номер.Text == "" || TB_Почта.Text == "" || TB_Телефон.Text == "")
            {
                MessageBox.Show("Не все поля былы заполнены!", "Ошибка");
            }
            else if (Regex_Фамилия.IsMatch(TB_Фамилия.Text) &&
                Regex_Имя.IsMatch(TB_Имя.Text) &&
                Regex_Отчество.IsMatch(TB_Отчество.Text) &&
                Regex_Серия.IsMatch(TB_Серия.Text) &&
                Regex_Номер.IsMatch(TB_Номер.Text) &&
                Regex_Почта.IsMatch(TB_Почта.Text) &&
                Regex_Телефон.IsMatch(TB_Телефон.Text))
            {
                try
                {
                    Персональные_данные перс_дан = new Персональные_данные
                    {
                        Фамилия = TB_Фамилия.Text,
                        Имя = TB_Имя.Text,
                        Отчество = TB_Отчество.Text,
                        Серия = TB_Серия.Text,
                        Номер = TB_Номер.Text,
                        Эл_почта = TB_Почта.Text,
                        Телефон = TB_Телефон.Text,
                        ID_Роли = 1
                    };

                    Персональные_данные Клиент = null;
                    try
                    {
                        Клиент = Entities.GetContext().Персональные_данные.Where(p => p.Эл_почта == перс_дан.Эл_почта || p.Телефон == перс_дан.Телефон).First();
                    }
                    catch 
                    {

                    }

                    if (Клиент != null)
                    {
                        MessageBox.Show($"Клиент с такой электронной почтой или телефоном уже зарегистрирован. Это {Клиент.Фамилия} {Клиент.Имя} {Клиент.Отчество}, " +
                            $"номер телефона: {Клиент.Телефон}, электронная почта: {Клиент.Эл_почта}.", "Ошибка");
                    }
                    else
                    {
                        Клиенты клиенты = new Клиенты
                        {
                            ID_ПерсДанных = перс_дан.ID_ПерсДанных
                        };

                        Entities.GetContext().Клиенты.Add(клиенты);
                        Entities.GetContext().Персональные_данные.Add(перс_дан);
                        Entities.GetContext().SaveChanges();
                        MessageBox.Show("Новая запись добавлена.", "Подтверждение");

                        if (work_manager_window.orders.process_add == true || work_manager_window.orders.process_edit == true) { }
                        else { work_manager_window.Hot_Button.IsEnabled = true; }
                        process_add = false;
                        Grid_Add_Edit.IsEnabled = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.Message}, {ex.Source}, {ex.InnerException}");
                }
            }
            else
            {
                MessageBox.Show("Поля заполнены неверно!", "Ошибка");
            }
        }

        public void Save_Edit()
        {
            if (TB_Фамилия.Text == "" || TB_Имя.Text == "" || TB_Отчество.Text == "" || TB_Серия.Text == "" || TB_Номер.Text == "" || TB_Почта.Text == "" || TB_Телефон.Text == "")
            {
                MessageBox.Show("Не все поля былы заполнены!", "Ошибка");
            }
            else if (Regex_Фамилия.IsMatch(TB_Фамилия.Text) &&
                Regex_Имя.IsMatch(TB_Имя.Text) &&
                Regex_Отчество.IsMatch(TB_Отчество.Text) &&
                Regex_Серия.IsMatch(TB_Серия.Text) &&
                Regex_Номер.IsMatch(TB_Номер.Text) &&
                Regex_Почта.IsMatch(TB_Почта.Text) &&
                Regex_Телефон.IsMatch(TB_Телефон.Text))
            {
                if (selected_customer != null)
                {
                    try
                    {
                        Персональные_данные перс_дан = new Персональные_данные
                        {
                            ID_ПерсДанных = selected_customer.ID_ПерсДанных,
                            Фамилия = TB_Фамилия.Text,
                            Имя = TB_Имя.Text,
                            Отчество = TB_Отчество.Text,
                            Серия = TB_Серия.Text,
                            Номер = TB_Номер.Text,
                            Эл_почта = TB_Почта.Text,
                            Телефон = TB_Телефон.Text,
                            ID_Пользователя = selected_customer.ID_Пользователя,
                            ID_Роли = selected_customer.ID_Роли
                        };

                        var Клиент = Entities.GetContext().Персональные_данные.Where(p => (p.Эл_почта == перс_дан.Эл_почта && p.Эл_почта != selected_customer.Эл_почта) || (p.Телефон == перс_дан.Телефон && p.Телефон != selected_customer.Телефон)).FirstOrDefault();
                        if (Клиент != null)
                        {
                            MessageBox.Show($"Клиент с такой электронной почтой или телефоном уже зарегистрирован. Это {Клиент.Фамилия}" +
                                $" {Клиент.Имя} {Клиент.Отчество}, " +
                                $"номер телефона: {Клиент.Телефон}, электронная почта: {Клиент.Эл_почта}.", "Ошибка");
                        }
                        else
                        {
                            Entities.GetContext().Персональные_данные.AddOrUpdate(перс_дан);
                            Entities.GetContext().SaveChanges();
                            MessageBox.Show("Выбранная запись изменена.", "Подтверждение");

                            if (work_manager_window.orders.process_add == true || work_manager_window.orders.process_edit == true) { }
                            else { work_manager_window.Hot_Button.IsEnabled = true; }
                            process_edit = false;
                            Grid_Add_Edit.IsEnabled = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{ex.Message}, {ex.Source}, {ex.InnerException}");
                    }
                }
                else
                {
                    MessageBox.Show("Вы не выбрали запись!", "Ошибка");
                }
                
            }
            else
            {
                MessageBox.Show("Поля заполнены неверно!", "Ошибка");
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (work_manager_window.orders.process_add == true || work_manager_window.orders.process_edit == true) { }
            else { work_manager_window.Hot_Button.IsEnabled = true; }
            process_add = false;
            process_edit = false;
            Grid_Add_Edit.IsEnabled = false;
        }

        private void Button_Cancel_Order_Click(object sender, RoutedEventArgs e)
        {
            work_manager_window.Hot_Button.IsEnabled = true;
            work_manager_window.orders.process_add = false;
            work_manager_window.orders.process_edit = false;
            Grid_Add_Edit.IsEnabled = false;
            process_add = false;
            process_edit = false;
            work_manager_window.MainFrame.Navigate(work_manager_window.orders);
            work_manager_window.ClearHistory();
        }

        private void Button_Select_Click(object sender, RoutedEventArgs e)
        {
            if (selected_customer != null)
            {
                var result = MessageBox.Show($"Выбрать выделенного клиента: {selected_customer.Фамилия} {selected_customer.Имя} {selected_customer.Отчество}, его телефон: {selected_customer.Телефон}, почта: {selected_customer.Эл_почта}", "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    work_manager_window.Hot_Button.IsEnabled = true;
                    Grid_Add_Edit.IsEnabled = false;
                    process_add = false;
                    process_edit = false;
                    work_manager_window.MainFrame.Navigate(work_manager_window.orders);
                    work_manager_window.ClearHistory();
                    work_manager_window.orders.Add_Order();
                }
            }
            else { MessageBox.Show("Вы не выбрали запись!","Ошибка"); }
        }

        Regex Regex_Фамилия = new Regex(@"^[A-z,А-я,\s,-]{3,30}$");
        Regex Regex_Имя = new Regex(@"^[A-z,А-я,\s,-]{2,30}$");
        Regex Regex_Отчество = new Regex(@"^[A-z,А-я,\s,-]{2,30}$");
        Regex Regex_Серия = new Regex(@"^[0-9]{4}$");
        Regex Regex_Номер = new Regex(@"^[0-9]{6}$");
        Regex Regex_Почта = new Regex(@"^[0-9A-z]{2,25}\@[a-z]{2,7}\.[a-z]{2,3}");
        Regex Regex_Телефон = new Regex(@"^[0-9]{10,11}$");

        private void TB_Фамилия_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TB_Фамилия.Text == "")
            {
                mark_фамилия.Text = "<";
                mark_фамилия.Foreground = Brushes.Gray;
            }
            else
            {
                if (Regex_Фамилия.IsMatch(TB_Фамилия.Text))
                {
                    mark_фамилия.Text = "*";
                    mark_фамилия.Foreground = Brushes.Green;

                }
                else
                {
                    mark_фамилия.Text = "!";
                    mark_фамилия.Foreground = Brushes.Red;
                }
            }
        }

        private void TB_Имя_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TB_Имя.Text == "")
            {
                mark_имя.Text = "<";
                mark_имя.Foreground = Brushes.Gray;
            }
            else
            {
                if (Regex_Имя.IsMatch(TB_Имя.Text))
                {
                    mark_имя.Text = "*";
                    mark_имя.Foreground = Brushes.Green;

                }
                else
                {
                    mark_имя.Text = "!";
                    mark_имя.Foreground = Brushes.Red;
                }
            }
        }

        private void TB_Отчество_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TB_Отчество.Text == "")
            {
                mark_отчество.Text = "<";
                mark_отчество.Foreground = Brushes.Gray;
            }
            else
            {
                if (Regex_Отчество.IsMatch(TB_Отчество.Text))
                {
                    mark_отчество.Text = "*";
                    mark_отчество.Foreground = Brushes.Green;

                }
                else
                {
                    mark_отчество.Text = "!";
                    mark_отчество.Foreground = Brushes.Red;
                }
            }
        }

        private void TB_Почта_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TB_Почта.Text == "")
            {
                mark_почта.Text = "<";
                mark_почта.Foreground = Brushes.Gray;
            }
            else
            {
                if (Regex_Почта.IsMatch(TB_Почта.Text))
                {
                    mark_почта.Text = "*";
                    mark_почта.Foreground = Brushes.Green;

                }
                else
                {
                    mark_почта.Text = "!";
                    mark_почта.Foreground = Brushes.Red;
                }
            }
        }

        private void TB_Телефон_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TB_Телефон.Text == "")
            {
                mark_телефон.Text = "<";
                mark_телефон.Foreground = Brushes.Gray;
            }
            else
            {
                if (Regex_Телефон.IsMatch(TB_Телефон.Text))
                {
                    mark_телефон.Text = "*";
                    mark_телефон.Foreground = Brushes.Green;

                }
                else
                {
                    mark_телефон.Text = "!";
                    mark_телефон.Foreground = Brushes.Red;
                }
            }
        }

        private void DG_Products_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                selected_customer = (Персональные_данные)DG_Customers.SelectedItem;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}, {ex.Source}, {ex.InnerException}");
            }
        }

        private void TB_Серия_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TB_Серия.Text == "")
            {
                mark_серия.Text = "<";
                mark_серия.Foreground = Brushes.Gray;
            }
            else
            {
                if (Regex_Серия.IsMatch(TB_Серия.Text))
                {
                    mark_серия.Text = "*";
                    mark_серия.Foreground = Brushes.Green;

                }
                else
                {
                    mark_серия.Text = "!";
                    mark_серия.Foreground = Brushes.Red;
                }
            }
        }

        private void TB_Номер_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TB_Номер.Text == "")
            {
                mark_номер.Text = "<";
                mark_номер.Foreground = Brushes.Gray;
            }
            else
            {
                if (Regex_Номер.IsMatch(TB_Номер.Text))
                {
                    mark_номер.Text = "*";
                    mark_номер.Foreground = Brushes.Green;

                }
                else
                {
                    mark_номер.Text = "!";
                    mark_номер.Foreground = Brushes.Red;
                }
            }
        }
    }
}
