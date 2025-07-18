using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
using Диплом_Альбион;

namespace Диплом_Альбион.Директор
{
    /// <summary>
    /// Логика взаимодействия для Products.xaml
    /// </summary>
    public partial class Users : Page
    {
        public Users()
        {
            InitializeComponent();
        }

        bool process_add = false;
        bool process_edit = false;
        Search_Users_Window search_window;
        public Персональные_данные selected_user;
        public Work_Director_Window work_director_window;

        public string GetHash(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            return Convert.ToBase64String(hash);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DG_Users.ItemsSource = Entities.GetContext().Персональные_данные.Include("Роли").Include("Пользователи").ToList();
            CB_Роль.ItemsSource = Entities.GetContext().Роли.Select(p=>p.Название).ToList();

            if (work_director_window.orders_designs.process_add == false)
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
                work_director_window.Hot_Button.IsEnabled = false;
                process_add = true;
                TB_Фамилия.Text = "";
                TB_Имя.Text = "";
                TB_Отчество.Text = "";
                TB_Серия.Text = "";
                TB_Номер.Text = "";
                TB_Почта.Text = "";
                TB_Телефон.Text = "";
                TB_Логин.Text = "";
                TB_Пароль.Text = "";
                CB_Роль.SelectedItem = null;
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
                if (selected_user != null)
                {
                    MessageBoxResult result = MessageBox.Show("Удалить выбранную запись?", "Подтверждение", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            if (Entities.GetContext().Сотрудники.Where(p=>p.ID_ПерсДанных == selected_user.ID_ПерсДанных).FirstOrDefault() != null)
                            {
                                var сотрудник = Entities.GetContext().Сотрудники.Where(p => p.ID_ПерсДанных == selected_user.ID_ПерсДанных).FirstOrDefault();
                                Entities.GetContext().Сотрудники.Remove(сотрудник);
                            }
                            if (Entities.GetContext().Клиенты.Where(p => p.ID_ПерсДанных == selected_user.ID_ПерсДанных).FirstOrDefault() != null)
                            {
                                var клиент = Entities.GetContext().Клиенты.Where(p => p.ID_ПерсДанных == selected_user.ID_ПерсДанных).FirstOrDefault();
                                Entities.GetContext().Клиенты.Remove(клиент);
                            }
                            Entities.GetContext().Пользователи.Remove(selected_user.Пользователи);
                            Entities.GetContext().Персональные_данные.Remove(selected_user);
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
                if (selected_user != null)
                {
                    if (selected_user.ID_Пользователя != null)
                    {
                        work_director_window.Hot_Button.IsEnabled = false;
                        process_edit = true;
                        TB_Фамилия.Text = selected_user.Фамилия;
                        TB_Имя.Text = selected_user.Имя;
                        TB_Отчество.Text = selected_user.Отчество;
                        TB_Серия.Text = selected_user.Серия;
                        TB_Номер.Text = selected_user.Номер;
                        TB_Почта.Text = selected_user.Эл_почта;
                        TB_Телефон.Text = selected_user.Телефон;
                        TB_Логин.Text = selected_user.Пользователи.Логин;
                        TB_Пароль.Text = "";
                        CB_Роль.SelectedItem = Entities.GetContext().Роли.Where(p => p.ID_Роли == selected_user.ID_Роли).Select(p => p.Название).FirstOrDefault();
                        Grid_Add_Edit.IsEnabled = true;
                    }
                    else
                    {
                        work_director_window.Hot_Button.IsEnabled = false;
                        process_edit = true;
                        TB_Фамилия.Text = selected_user.Фамилия;
                        TB_Имя.Text = selected_user.Имя;
                        TB_Отчество.Text = selected_user.Отчество;
                        TB_Серия.Text = selected_user.Серия;
                        TB_Номер.Text = selected_user.Номер;
                        TB_Почта.Text = selected_user.Эл_почта;
                        TB_Телефон.Text = selected_user.Телефон;
                        TB_Логин.Text = "";
                        TB_Пароль.Text = "";
                        CB_Роль.SelectedItem = Entities.GetContext().Роли.Where(p => p.ID_Роли == selected_user.ID_Роли).Select(p => p.Название).FirstOrDefault();
                        Grid_Add_Edit.IsEnabled = true;
                    }
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
                search_window = new Search_Users_Window();
                search_window.Show();
                search_window.users = this;
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
            if (CB_LogPas.IsChecked == false && (TB_Фамилия.Text == "" || TB_Имя.Text == "" || TB_Отчество.Text == "" || TB_Серия.Text == "" || TB_Номер.Text == "" || TB_Почта.Text == "" || TB_Телефон.Text == "" || TB_Логин.Text == "" || TB_Пароль.Text == "" || CB_Роль.SelectedItem == null))
            {
                MessageBox.Show("Не все поля былы заполнены!", "Ошибка");
            }
            else if (CB_LogPas.IsChecked == true && (TB_Фамилия.Text == "" || TB_Имя.Text == "" || TB_Отчество.Text == "" || TB_Серия.Text == "" || TB_Номер.Text == "" || TB_Почта.Text == "" || TB_Телефон.Text == "" || CB_Роль.SelectedItem == null))
            { MessageBox.Show("Не все поля былы заполнены!", "Ошибка"); }
            else if (CB_LogPas.IsChecked == false && (!Regex_Фамилия.IsMatch(TB_Фамилия.Text) &&
                !Regex_Имя.IsMatch(TB_Имя.Text) &&
                !Regex_Отчество.IsMatch(TB_Отчество.Text) &&
                !Regex_Серия.IsMatch(TB_Серия.Text) &&
                !Regex_Номер.IsMatch(TB_Номер.Text) &&
                !Regex_Почта.IsMatch(TB_Почта.Text) &&
                !Regex_Телефон.IsMatch(TB_Телефон.Text) &&
                !Regex_Логин.IsMatch(TB_Логин.Text) &&
                !Regex_Пароль.IsMatch(TB_Пароль.Text)))
            { MessageBox.Show("Поля заполнены неверно!", "Ошибка"); }
            else if (CB_LogPas.IsChecked == true && (!Regex_Фамилия.IsMatch(TB_Фамилия.Text) &&
                !Regex_Имя.IsMatch(TB_Имя.Text) &&
                !Regex_Отчество.IsMatch(TB_Отчество.Text) &&
                !Regex_Серия.IsMatch(TB_Серия.Text) &&
                !Regex_Номер.IsMatch(TB_Номер.Text) &&
                !Regex_Почта.IsMatch(TB_Почта.Text) &&
                !Regex_Телефон.IsMatch(TB_Телефон.Text)))
            { MessageBox.Show("Поля заполнены неверно!", "Ошибка"); }
            else
            {
                if (CB_LogPas.IsChecked == false) //Если с логином и паролем
                {
                    try
                    {
                        Пользователи польз = new Пользователи
                        {
                            Логин = TB_Логин.Text,
                            Пароль = TB_Пароль.Text
                        };

                        Персональные_данные перс_дан = new Персональные_данные
                        {
                            Фамилия = TB_Фамилия.Text,
                            Имя = TB_Имя.Text,
                            Отчество = TB_Отчество.Text,
                            Серия = TB_Серия.Text,
                            Номер = TB_Номер.Text,
                            Эл_почта = TB_Почта.Text,
                            Телефон = TB_Телефон.Text,
                            ID_Роли = Entities.GetContext().Роли.Where(p => p.Название == CB_Роль.Text).Select(p => p.ID_Роли).FirstOrDefault(),
                            ID_Пользователя = польз.ID_Пользователя
                        };

                        var Клиент = Entities.GetContext().Персональные_данные.Where(p => p.Эл_почта == перс_дан.Эл_почта || p.Телефон == перс_дан.Телефон).FirstOrDefault();
                        var Логин = Entities.GetContext().Персональные_данные.Where(p => p.Пользователи.Логин == польз.Логин).FirstOrDefault();
                        if (Логин != null)
                        {
                            MessageBox.Show($"Пользователь с таким логином уже зарегистрирован. Это {Логин.Фамилия} {Логин.Имя} {Логин.Отчество}, телефон: {Логин.Телефон}, почта: {Логин.Эл_почта}, Логин: {Логин.Пользователи.Логин}", "Ошибка");
                        }
                        else
                        {
                            if (Клиент != null)
                            {
                                MessageBox.Show($"Пользователь с такой электронной почтой или телефоном уже зарегистрирован. Это {Клиент.Фамилия} {Клиент.Имя} {Клиент.Отчество}, " +
                                    $"номер телефона: {Клиент.Телефон}, электронная почта: {Клиент.Эл_почта}.", "Ошибка");
                            }
                            else
                            {
                                Entities.GetContext().Пользователи.Add(польз);
                                Entities.GetContext().Персональные_данные.Add(перс_дан);
                                Entities.GetContext().SaveChanges();
                                MessageBox.Show("Новая запись добавлена.", "Подтверждение");

                                process_add = false;
                                work_director_window.Hot_Button.IsEnabled = true;
                                Grid_Add_Edit.IsEnabled = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{ex.Message}, {ex.Source}, {ex.InnerException}");
                    }
                }
                else //Если без логина и пароля
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
                            ID_Роли = Entities.GetContext().Роли.Where(p => p.Название == CB_Роль.Text).Select(p => p.ID_Роли).FirstOrDefault()
                        };

                        var персона = Entities.GetContext().Персональные_данные.Where(p => p.Эл_почта == перс_дан.Эл_почта || p.Телефон == перс_дан.Телефон).FirstOrDefault();

                        if (персона != null)
                        {
                            MessageBox.Show($"Пользователь с такой электронной почтой или телефоном уже зарегистрирован. Это {персона.Фамилия} {персона.Имя} {персона.Отчество}, " +
                                $"номер телефона: {персона.Телефон}, электронная почта: {персона.Эл_почта}.", "Ошибка");
                        }
                        else
                        {
                            Entities.GetContext().Персональные_данные.Add(перс_дан);

                            if (перс_дан.Роли.Название == "Клиент")
                            {
                                Клиенты клиент = new Клиенты
                                {
                                    ID_ПерсДанных = перс_дан.ID_ПерсДанных
                                };
                                Entities.GetContext().Клиенты.Add(клиент);
                            }
                            if (перс_дан.Роли.Название != "Клиент")
                            {
                                Сотрудники сотрудник = new Сотрудники
                                {
                                    ID_ПерсДанных = перс_дан.ID_ПерсДанных
                                };
                                Entities.GetContext().Сотрудники.Add(сотрудник);
                            }

                            Entities.GetContext().SaveChanges();
                            MessageBox.Show("Новая запись добавлена.", "Подтверждение");

                            process_add = false;
                            work_director_window.Hot_Button.IsEnabled = true;
                            Grid_Add_Edit.IsEnabled = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{ex.Message}, {ex.Source}, {ex.InnerException}");
                    }
                }
            }
        }

        public void Save_Edit()
        {
            if (CB_LogPas.IsChecked == false && (TB_Фамилия.Text == "" || TB_Имя.Text == "" || TB_Отчество.Text == "" || TB_Серия.Text == "" || TB_Номер.Text == "" || TB_Почта.Text == "" || TB_Телефон.Text == "" || TB_Логин.Text == "" || CB_Роль.SelectedItem == null))
            {
                MessageBox.Show("Не все поля былы заполнены!", "Ошибка");
            }
            else if (CB_LogPas.IsChecked == true && (TB_Фамилия.Text == "" || TB_Имя.Text == "" || TB_Отчество.Text == "" || TB_Серия.Text == "" || TB_Номер.Text == "" || TB_Почта.Text == "" || TB_Телефон.Text == "" || CB_Роль.SelectedItem == null))
            { MessageBox.Show("Не все поля былы заполнены!", "Ошибка"); }
            else if (CB_LogPas.IsChecked == false && (!Regex_Фамилия.IsMatch(TB_Фамилия.Text) &&
                !Regex_Имя.IsMatch(TB_Имя.Text) &&
                !Regex_Отчество.IsMatch(TB_Отчество.Text) &&
                !Regex_Серия.IsMatch(TB_Серия.Text) &&
                !Regex_Номер.IsMatch(TB_Номер.Text) &&
                !Regex_Почта.IsMatch(TB_Почта.Text) &&
                !Regex_Телефон.IsMatch(TB_Телефон.Text) &&
                !Regex_Логин.IsMatch(TB_Логин.Text) &&
                !Regex_Пароль.IsMatch(TB_Пароль.Text)))
            { MessageBox.Show("Поля заполнены неверно!", "Ошибка"); }
            else if (CB_LogPas.IsChecked == true && (!Regex_Фамилия.IsMatch(TB_Фамилия.Text) &&
                !Regex_Имя.IsMatch(TB_Имя.Text) &&
                !Regex_Отчество.IsMatch(TB_Отчество.Text) &&
                !Regex_Серия.IsMatch(TB_Серия.Text) &&
                !Regex_Номер.IsMatch(TB_Номер.Text) &&
                !Regex_Почта.IsMatch(TB_Почта.Text) &&
                !Regex_Телефон.IsMatch(TB_Телефон.Text)))
            { MessageBox.Show("Поля заполнены неверно!", "Ошибка"); }
            else
            {
                if (selected_user != null)
                {
                    if (CB_LogPas.IsChecked == false) //Если с логином и паролем
                    {
                        try
                        {
                            string пароль;
                            if (TB_Пароль.Text == "") { пароль = selected_user.Пользователи.Пароль; }
                            else { пароль = GetHash(TB_Пароль.Text); }

                            var поиск_польз = Entities.GetContext().Пользователи.Where(p => p.ID_Пользователя == selected_user.ID_Пользователя).FirstOrDefault();
                            Пользователи польз = new Пользователи();
                            if (поиск_польз == null)
                            {
                                польз.Логин = TB_Логин.Text;
                                польз.Пароль = пароль;
                            }
                            else
                            {
                                польз.ID_Пользователя = selected_user.Пользователи.ID_Пользователя;
                                польз.Логин = TB_Логин.Text;
                                польз.Пароль = пароль;
                            }

                            Персональные_данные перс_дан = new Персональные_данные
                            {
                                ID_ПерсДанных = selected_user.ID_ПерсДанных,
                                Фамилия = TB_Фамилия.Text,
                                Имя = TB_Имя.Text,
                                Отчество = TB_Отчество.Text,
                                Серия = TB_Серия.Text,
                                Номер = TB_Номер.Text,
                                Эл_почта = TB_Почта.Text,
                                Телефон = TB_Телефон.Text,
                                ID_Пользователя = selected_user.ID_Пользователя,
                                ID_Роли = selected_user.ID_Роли
                            };

                            var Клиент = Entities.GetContext().Персональные_данные.Where(p => (p.Эл_почта == перс_дан.Эл_почта && p.Эл_почта != selected_user.Эл_почта) || (p.Телефон == перс_дан.Телефон && p.Телефон != selected_user.Телефон)).FirstOrDefault();
                            if (поиск_польз == null)
                            {
                                var Логин_к = Entities.GetContext().Персональные_данные.Where(p => p.Пользователи.Логин == польз.Логин).FirstOrDefault();
                                if (Логин_к != null)
                                {
                                    MessageBox.Show($"Клиент с таким логином уже зарегистрирован. Это {Логин_к.Фамилия} {Логин_к.Имя} {Логин_к.Отчество}, телефон: {Логин_к.Телефон}, почта: {Логин_к.Эл_почта}, Логин: {Логин_к.Пользователи.Логин}", "Ошибка");
                                }
                                else if (Клиент != null)
                                {
                                    MessageBox.Show($"Клиент с такой электронной почтой или телефоном уже зарегистрирован. Это {Клиент.Фамилия} {Клиент.Имя} {Клиент.Отчество}, " +
                                        $"номер телефона: {Клиент.Телефон}, электронная почта: {Клиент.Эл_почта}.", "Ошибка");
                                }
                                else
                                {
                                    Entities.GetContext().Пользователи.Add(польз);
                                    перс_дан.ID_Пользователя = польз.ID_Пользователя;
                                    Entities.GetContext().Персональные_данные.AddOrUpdate(перс_дан);
                                    Entities.GetContext().SaveChanges();
                                    MessageBox.Show("Выбранная запись изменена.", "Подтверждение");

                                    process_edit = false;
                                    work_director_window.Hot_Button.IsEnabled = true;
                                    Grid_Add_Edit.IsEnabled = false;
                                }

                            }
                            else
                            {
                                var Логин_к = Entities.GetContext().Персональные_данные.Where(p => p.Пользователи.Логин == польз.Логин && p.Пользователи.Логин != selected_user.Пользователи.Логин).FirstOrDefault();
                                if (Логин_к != null)
                                {
                                    MessageBox.Show($"Клиент с таким логином уже зарегистрирован. Это {Логин_к.Фамилия} {Логин_к.Имя} {Логин_к.Отчество}, телефон: {Логин_к.Телефон}, почта: {Логин_к.Эл_почта}, Логин: {Логин_к.Пользователи.Логин}", "Ошибка");
                                }
                                else if (Клиент != null)
                                {
                                    MessageBox.Show($"Клиент с такой электронной почтой или телефоном уже зарегистрирован. Это {Клиент.Фамилия} {Клиент.Имя} {Клиент.Отчество}, " +
                                        $"номер телефона: {Клиент.Телефон}, электронная почта: {Клиент.Эл_почта}.", "Ошибка");
                                }
                                else
                                {
                                    Entities.GetContext().Пользователи.AddOrUpdate(польз);
                                    Entities.GetContext().Персональные_данные.AddOrUpdate(перс_дан);
                                    Entities.GetContext().SaveChanges();
                                    MessageBox.Show("Выбранная запись изменена.", "Подтверждение");

                                    process_edit = false;
                                    work_director_window.Hot_Button.IsEnabled = true;
                                    Grid_Add_Edit.IsEnabled = false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"{ex.Message}, {ex.Source}, {ex.InnerException}");
                        }
                    }
                    else //Если без логина и пароля
                    {
                        try
                        {
                            Персональные_данные перс_дан = new Персональные_данные
                            {
                                ID_ПерсДанных = selected_user.ID_ПерсДанных,
                                Фамилия = TB_Фамилия.Text,
                                Имя = TB_Имя.Text,
                                Отчество = TB_Отчество.Text,
                                Серия = TB_Серия.Text,
                                Номер = TB_Номер.Text,
                                Эл_почта = TB_Почта.Text,
                                Телефон = TB_Телефон.Text,
                                ID_Пользователя = selected_user.ID_Пользователя,
                                ID_Роли = selected_user.ID_Роли
                            };

                            var Клиент = Entities.GetContext().Персональные_данные.Where(p => (p.Эл_почта == перс_дан.Эл_почта && p.Эл_почта != selected_user.Эл_почта) || (p.Телефон == перс_дан.Телефон && p.Телефон != selected_user.Телефон)).FirstOrDefault();

                            if (Клиент != null)
                            {
                                MessageBox.Show($"Пользователь с такой электронной почтой или телефоном уже зарегистрирован. Это {Клиент.Фамилия} {Клиент.Имя} {Клиент.Отчество}, " +
                                    $"номер телефона: {Клиент.Телефон}, электронная почта: {Клиент.Эл_почта}.", "Ошибка");
                            }
                            else
                            {
                                Entities.GetContext().Персональные_данные.AddOrUpdate(перс_дан);
                                Entities.GetContext().SaveChanges();
                                MessageBox.Show("Выбранная запись изменена.", "Подтверждение");

                                process_edit = false;
                                work_director_window.Hot_Button.IsEnabled = true;
                                Grid_Add_Edit.IsEnabled = false;
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"{ex.Message}, {ex.Source}, {ex.InnerException}");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Вы не выбрали запись!", "Ошибка");
                }
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (work_director_window.users.process_add == true || work_director_window.users.process_edit == true) { }
            else { work_director_window.Hot_Button.IsEnabled = true; }
            process_add = false;
            process_edit = false;
            Grid_Add_Edit.IsEnabled = false;
        }

        private void Button_Select_Click(object sender, RoutedEventArgs e)
        {
            if (selected_user != null)
            {
                var user = Entities.GetContext().Клиенты.Where(p => p.ID_ПерсДанных == selected_user.ID_ПерсДанных).FirstOrDefault();
                if (user == null)
                {
                    MessageBox.Show("Вы выбрали не клиента!", "Ошибка");
                }
                else
                {
                    var result = MessageBox.Show($"Выбрать выделенного клиента: {selected_user.Фамилия} {selected_user.Имя} {selected_user.Отчество}, его телефон: {selected_user.Телефон}, почта: {selected_user.Эл_почта}", "Подтверждение", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        work_director_window.Hot_Button.IsEnabled = true;
                        Grid_Add_Edit.IsEnabled = false;
                        process_add = false;
                        process_edit = false;
                        work_director_window.MainFrame.Navigate(work_director_window.orders_designs);
                        work_director_window.ClearHistory();
                        work_director_window.orders_designs.Add_Order();
                    }
                }
            }
            else { MessageBox.Show("Вы не выбрали запись!", "Ошибка"); }
        }

        private void Button_Cancel_Order_Click(object sender, RoutedEventArgs e)
        {
            work_director_window.Hot_Button.IsEnabled = true;
            work_director_window.orders_designs.process_add = false;
            work_director_window.orders_designs.process_edit = false;
            Grid_Add_Edit.IsEnabled = false;
            process_add = false;
            process_edit = false;
            work_director_window.MainFrame.Navigate(work_director_window.orders_designs);
            work_director_window.ClearHistory();
        }

        private void CB_LogPas_Click(object sender, RoutedEventArgs e)
        {
            if (CB_LogPas.IsChecked == false)
            {
                SP_Login.IsEnabled = true;
                SP_Password.IsEnabled = true;
            }
            else
            {
                SP_Login.IsEnabled = false;
                SP_Password.IsEnabled = false;
            }
        }

        Regex Regex_Фамилия = new Regex(@"^[A-z,А-я,\s,-]{3,30}$");
        Regex Regex_Имя = new Regex(@"^[A-z,А-я,\s,-]{2,30}$");
        Regex Regex_Отчество = new Regex(@"^[A-z,А-я,\s,-]{2,30}$");
        Regex Regex_Серия = new Regex(@"^[0-9]{4}$");
        Regex Regex_Номер = new Regex(@"^[0-9]{6}$");
        Regex Regex_Почта = new Regex(@"^[0-9A-z]{2,25}\@[a-z]{2,7}\.[a-z]{2,3}");
        Regex Regex_Телефон = new Regex(@"^[0-9]{10,11}$");
        Regex Regex_Логин = new Regex(@"^[A-z,А-я,0-9]{3,20}$");
        Regex Regex_Пароль = new Regex(@"^[!*./&?%$#№0-9A-zА-я]{3,10}$");

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

        private void CB_Роль_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CB_Роль.Text == "")
            {
                mark_роль.Text = "<";
                mark_роль.Foreground = Brushes.Gray;
            }
            else
            {
                mark_роль.Text = "*";
                mark_роль.Foreground = Brushes.Green;
            }
        }

        private void TB_Логин_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TB_Логин.Text == "")
            {
                mark_логин.Text = "<";
                mark_логин.Foreground = Brushes.Gray;
            }
            else
            {
                if (Regex_Логин.IsMatch(TB_Логин.Text))
                {
                    mark_логин.Text = "*";
                    mark_логин.Foreground = Brushes.Green;

                }
                else
                {
                    mark_логин.Text = "!";
                    mark_логин.Foreground = Brushes.Red;
                }
            }
        }

        private void TB_Пароль_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TB_Пароль.Text == "")
            {
                mark_пароль.Text = "<";
                mark_пароль.Foreground = Brushes.Gray;
            }
            else
            {
                if (Regex_Пароль.IsMatch(TB_Пароль.Text))
                {
                    mark_пароль.Text = "*";
                    mark_пароль.Foreground = Brushes.Green;

                }
                else
                {
                    mark_пароль.Text = "!";
                    mark_пароль.Foreground = Brushes.Red;
                }
            }
        }

        private void DG_Users_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                selected_user = (Персональные_данные)DG_Users.SelectedItem;
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
