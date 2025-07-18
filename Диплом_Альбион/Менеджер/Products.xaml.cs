using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
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
    public partial class Products : Page
    {
        public Products()
        {
            InitializeComponent();
        }

        bool process_add = false;
        bool process_edit = false;
        Search_Products_Window search_window;
        Товары selected_product;
        public Work_Manager_Window work_manager_window;

        public Товар_Заказ товар_заказ;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DG_Products.ItemsSource = Entities.GetContext().Товары.Include("Производитель").Include("Категории").ToList();
            ComboBox_Производитель.ItemsSource = Entities.GetContext().Производитель.Select(p => p.Название).ToList();
            ComboBox_Категория.ItemsSource = Entities.GetContext().Категории.Select(p => p.Название).ToList();
            if (work_manager_window != null)
            {
                if (work_manager_window.orders.process_add == true || work_manager_window.orders.process_edit == true)
                {
                    Button_Cancel_Order.IsEnabled = true;
                    Button_Select.IsEnabled = true;
                }
                else
                {
                    Button_Cancel_Order.IsEnabled = false;
                    Button_Select.IsEnabled = false;
                }

            }
            else
            {
                Button_Cancel_Order.IsEnabled = false;
                Button_Select.IsEnabled = false;
            }
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            if (process_add == false && process_edit == false)
            {
                work_manager_window.Hot_Button.IsEnabled = false;
                process_add = true;
                TB_Название.Text = "";
                TB_Цена.Text = "";
                ComboBox_Производитель.SelectedItem = null;
                ComboBox_Категория.SelectedItem = null;
                TB_Описание.Text = "";
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
                if (selected_product != null)
                {
                    MessageBoxResult result = MessageBox.Show("Удалить выбранную запись?", "Подтверждение", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            Entities.GetContext().Товары.Remove(selected_product);
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
                if (selected_product != null)
                {
                    work_manager_window.Hot_Button.IsEnabled = false;
                    process_edit = true;
                    TB_Название.Text = selected_product.Название;
                    TB_Цена.Text = selected_product.Цена.ToString();
                    ComboBox_Производитель.SelectedItem = selected_product.Производитель.Название;
                    ComboBox_Категория.SelectedItem = selected_product.Категории.Название;
                    TB_Описание.Text = selected_product.Описание;
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
                search_window = new Search_Products_Window();
                search_window.Show();
                search_window.products = this;
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

        private void Button_Select_Click(object sender, RoutedEventArgs e)
        {
            if (selected_product != null)
            {
                var result = MessageBox.Show($"Выбрать выделенный товар: {selected_product.Название}, цена: {selected_product.Цена}?", "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    work_manager_window.Hot_Button.IsEnabled = true;
                    Grid_Add_Edit.IsEnabled = false;
                    process_add = false;
                    process_edit = false;

                    товар_заказ = new Товар_Заказ
                    {
                        ID_Заказа = work_manager_window.orders.selected_order.ID_Заказа,
                        ID_Товара = selected_product.ID_Товара,
                        Количество = 1
                    };
                    Entities.GetContext().Товар_Заказ.Add(товар_заказ);
                    Entities.GetContext().SaveChanges();

                    if (Entities.GetContext().Товар_Заказ.Where(p => p.ID_Товара == selected_product.ID_Товара && p.ID_Заказа == товар_заказ.ID_Заказа).Count() < 2)
                    {
                        work_manager_window.MainFrame.Navigate(work_manager_window.orders);
                        work_manager_window.orders.Page_Loaded(sender, e);
                        work_manager_window.ClearHistory();
                    }
                    else
                    {
                        Entities.GetContext().Товар_Заказ.Remove(товар_заказ);
                        Entities.GetContext().SaveChanges();
                        var заказДругоеКолво = Entities.GetContext().Товар_Заказ.Where(p => p.ID_Товара == selected_product.ID_Товара && p.ID_Заказа == товар_заказ.ID_Заказа).FirstOrDefault();
                        заказДругоеКолво.Количество++;
                        Entities.GetContext().SaveChanges();


                        work_manager_window.MainFrame.Navigate(work_manager_window.orders);
                        work_manager_window.orders.Page_Loaded(sender, e);
                        work_manager_window.ClearHistory();
                    }
                }
            }
            else { MessageBox.Show("Вы не выбрали запись!", "Ошибка"); }
        }

        private void Button_Cancel_Order_Click(object sender, RoutedEventArgs e)
        {
            work_manager_window.Hot_Button.IsEnabled = true;
            Grid_Add_Edit.IsEnabled = false;
            process_add = false;
            process_edit = false;
            work_manager_window.MainFrame.Navigate(work_manager_window.orders);
            work_manager_window.ClearHistory();
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
            if (TB_Название.Text == "" || TB_Цена.Text == "" || ComboBox_Производитель.SelectionBoxItem.ToString() == "" 
                || ComboBox_Категория.SelectionBoxItem.ToString() == "")
            {
                MessageBox.Show("Не все поля былы заполнены!", "Ошибка");
            }
            else if (Regex_Название.IsMatch(TB_Название.Text) &&
                Regex_Цена.IsMatch(TB_Цена.Text) &&
                ComboBox_Производитель.SelectionBoxItem.ToString() != null &&
                ComboBox_Категория.SelectionBoxItem.ToString() != null)
            {
                try
                {
                    Товары товар = new Товары
                    {
                        Название = TB_Название.Text,
                        Цена = Convert.ToDecimal(TB_Цена.Text),
                        ID_Производителя = Entities.GetContext().Производитель.Where(p => p.Название.Contains(ComboBox_Производитель.
                        SelectionBoxItem.ToString())).Select(p => p.ID_Производителя).First(),
                        ID_Категории = Entities.GetContext().Категории.Where(p => p.Название.Contains(ComboBox_Категория.
                        SelectionBoxItem.ToString())).Select(p => p.ID_Категории).First(),
                        Описание = TB_Описание.Text
                    };

                    Entities.GetContext().Товары.Add(товар);
                    Entities.GetContext().SaveChanges();
                    MessageBox.Show("Новая запись добавлена.", "Подтверждение");

                    if (work_manager_window.orders.process_add == true || work_manager_window.orders.process_edit == true) { }
                    else { work_manager_window.Hot_Button.IsEnabled = true; }
                    process_add = false;
                    Grid_Add_Edit.IsEnabled = false;
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
            if (TB_Название.Text == "" || TB_Цена.Text == "" || ComboBox_Производитель.SelectionBoxItem.ToString() == "" || ComboBox_Категория.SelectionBoxItem.ToString() == "")
            {
                MessageBox.Show("Не все поля былы заполнены!", "Ошибка");
            }
            else if (Regex_Название.IsMatch(TB_Название.Text) &&
                Regex_Цена.IsMatch(TB_Цена.Text) &&
                ComboBox_Производитель.SelectionBoxItem.ToString() != null &&
                ComboBox_Категория.SelectionBoxItem.ToString() != null)
            {
                if (selected_product != null)
                {
                    try
                    {
                        Товары товар = new Товары
                        {
                            ID_Товара = selected_product.ID_Товара,
                            Название = TB_Название.Text,
                            Цена = Convert.ToDecimal(TB_Цена.Text),
                            ID_Производителя = Entities.GetContext().Производитель.Where(p => p.Название.Contains(ComboBox_Производитель.SelectionBoxItem.ToString())).Select(p => p.ID_Производителя).First(),
                            ID_Категории = Entities.GetContext().Категории.Where(p => p.Название.Contains(ComboBox_Категория.SelectionBoxItem.ToString())).Select(p => p.ID_Категории).First(),
                            Описание = TB_Описание.Text
                        };

                        Entities.GetContext().Товары.AddOrUpdate(товар);
                        Entities.GetContext().SaveChanges();
                        MessageBox.Show("Выбранная запись изменена.", "Подтверждение");

                        if (work_manager_window.orders.process_add == true || work_manager_window.orders.process_edit == true) { }
                        else { work_manager_window.Hot_Button.IsEnabled = true; }
                        process_edit = false;
                        Grid_Add_Edit.IsEnabled = false;
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

        Regex Regex_Название = new Regex(@"^[A-z,А-я,\s,0-9]{3,50}$");
        Regex Regex_Цена = new Regex(@"^[0-9,]{2,9}$");


        private void TB_Название_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TB_Название.Text == "")
            {
                mark_название.Text = "<";
                mark_название.Foreground = Brushes.Gray;
            }
            else
            {
                if (Regex_Название.IsMatch(TB_Название.Text))
                {
                    mark_название.Text = "*";
                    mark_название.Foreground = Brushes.Green;

                }
                else
                {
                    mark_название.Text = "!";
                    mark_название.Foreground = Brushes.Red;
                }
            }
        }

        private void TB_Цена_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TB_Цена.Text == "")
            {
                mark_цена.Text = "<";
                mark_цена.Foreground = Brushes.Gray;
            }
            else
            {
                if (Regex_Цена.IsMatch(TB_Цена.Text))
                {
                    mark_цена.Text = "*";
                    mark_цена.Foreground = Brushes.Green;

                }
                else
                {
                    mark_цена.Text = "!";
                    mark_цена.Foreground = Brushes.Red;
                }
            }
        }

        private void ComboBox_Производитель_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox_Производитель.SelectionBoxItem.ToString() == "")
            {
                mark_производитель.Text = "<";
                mark_производитель.Foreground = Brushes.Gray;
            }
            else
            {
                mark_производитель.Text = "*";
                mark_производитель.Foreground = Brushes.Green;
            }
        }

        private void ComboBox_Категория_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox_Категория.SelectionBoxItem.ToString() == "")
            {
                mark_категория.Text = "<";
                mark_категория.Foreground = Brushes.Gray;
            }
            else
            {
                mark_категория.Text = "*";
                mark_категория.Foreground = Brushes.Green;
            }
        }

        private void DG_Products_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                selected_product = (Товары)DG_Products.SelectedItem;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}, {ex.Source}, {ex.InnerException}");
            }
        }

        private void Grid_Add_Edit_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ComboBox_Производитель != null)
            {
                if (ComboBox_Производитель.SelectionBoxItem.ToString() == "")
                {
                    mark_производитель.Text = "<";
                    mark_производитель.Foreground = Brushes.Gray;
                }
                else
                {
                    mark_производитель.Text = "*";
                    mark_производитель.Foreground = Brushes.Green;
                }

                if (ComboBox_Категория.SelectionBoxItem.ToString() == "")
                {
                    mark_категория.Text = "<";
                    mark_категория.Foreground = Brushes.Gray;
                }
                else
                {
                    mark_категория.Text = "*";
                    mark_категория.Foreground = Brushes.Green;
                }
            }
        }
    }
}
