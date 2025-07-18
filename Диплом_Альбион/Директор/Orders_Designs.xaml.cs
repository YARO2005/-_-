using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
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
using static System.Net.Mime.MediaTypeNames;
using static Диплом_Альбион.Менеджер.Search_Products_Window;

namespace Диплом_Альбион.Директор
{
    /// <summary>
    /// Логика взаимодействия для Products.xaml
    /// </summary>
    public partial class Orders_Designs : Page
    {
        public Orders_Designs()
        {
            InitializeComponent();
        }

        public bool process_add = false;
        public bool process_edit = false;
        Search_Orders_Designs_Window search_window;
        public Заказ selected_order;
        public Work_Director_Window work_director_window;
        Заказ backup_order_edit;


        public void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DG_Orders.ItemsSource = Entities.GetContext().Заказ.Include("Сотрудники").Include("Клиенты").Include("Статус").Include("Виды_заказов").Where(p=>p.Виды_заказов.Название == "Дизайн" ).ToList();
            ComboBox_Клиент.ItemsSource = Entities.GetContext().Персональные_данные.Where(p=>p.Роли.Название == "Клиент").Select(p=>p.Фамилия + " " + p.Имя + " " + p.Отчество + " " + p.Телефон).ToList();
            ComboBox_Статус.ItemsSource = Entities.GetContext().Статус.Where(p=>p.ID_Статуса == 1 || p.ID_Статуса == 2 || p.ID_Статуса == 7 || p.ID_Статуса == 8 || p.ID_Статуса == 9).Select(p=>p.Название).ToList();
            if (selected_order != null && (process_add == true || process_edit == true)) 
            {
                TB_Сумма.Text = selected_order.Сумма.ToString();
            }

            if (process_add == true || process_edit == true)
            {
                Grid_Add_Edit.IsEnabled = true;
            }
            else
            {
                Grid_Add_Edit.IsEnabled = false;
            }
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            if (process_add == false && process_edit == false)
            {
                MessageBox.Show("Для того, чтобы начать формировать заказ, выберите клиента, на которого хотите оформить заказ.", 
                    "Подтверждение", MessageBoxButton.OK);
                process_add = true;

                work_director_window.Hot_Button.IsEnabled = false;
                work_director_window.MainFrame.Navigate(work_director_window.users);
            }
            else
            {
                MessageBox.Show("Сохраните введенные данные или сделайте отмену, прежде чем начинать новое добавление!", "Ошибка");
            }
        }

        public void Add_Order()
        {
            selected_order = new Заказ()
            {
                ID_Клиента = Entities.GetContext().Клиенты.Where(p => p.ID_ПерсДанных ==
                work_director_window.users.selected_user.ID_ПерсДанных).Select(p => p.ID_Клиента).FirstOrDefault(),
                ID_Сотрудника = MainWindow.worker_id,
                ДатаОформления = DateTime.Now,
                Сумма = 0,
                ID_Статуса = 1,
                ID_ВидаЗаказа = 2, //Название вида - Дизайн
                Уведомление = false,
                Доставка = false,
            };

            Entities.GetContext().Заказ.Add(selected_order);
            Entities.GetContext().SaveChanges();

            var ПерсДанные = Entities.GetContext().Клиенты.Where(p => p.ID_Клиента == selected_order.ID_Клиента).Select(p => p.ID_ПерсДанных).FirstOrDefault();
            ComboBox_Клиент.SelectedItem = Entities.GetContext().Персональные_данные.Where(p => p.ID_ПерсДанных == ПерсДанные).
                Select(p => p.Фамилия + " " + p.Имя + " " + p.Отчество + " " + p.Телефон).FirstOrDefault();
            ComboBox_Статус.SelectedItem = Entities.GetContext().Статус.Where(p=>p.ID_Статуса == selected_order.ID_Статуса).
                Select(p=>p.Название).FirstOrDefault();
            TB_Сумма.Text = selected_order.ToString();
            DP_Дата.SelectedDate = selected_order.ДатаОформления;
            DP_Дата2.IsEnabled = false;
            ComboBox_Статус.IsEnabled = false;
            Grid_Add_Edit.IsEnabled = true;
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (process_add == false && process_edit == false)
            {
                if (selected_order != null)
                {
                    MessageBoxResult result = MessageBox.Show("Удалить выбранную запись?", "Подтверждение", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            Entities.GetContext().Заказ.Remove(selected_order);
                            Entities.GetContext().SaveChanges();
                            Page_Loaded(sender, e);
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
                if (selected_order != null)
                {
                    backup_order_edit = new Заказ
                    {
                        ID_Заказа = selected_order.ID_Заказа,
                        ID_Клиента = selected_order.ID_Клиента,
                        ID_Сотрудника = selected_order.ID_Сотрудника,
                        ID_Статуса = selected_order.ID_Статуса,
                        ДатаОформления = selected_order.ДатаОформления,
                        Сумма = selected_order.Сумма,
                        ID_ВидаЗаказа = selected_order.ID_ВидаЗаказа,
                        Уведомление = selected_order.Уведомление
                    };

                    process_edit = true;
                    DP_Дата.SelectedDate = selected_order.ДатаОформления;
                    var ПерсДанные = Entities.GetContext().Клиенты.Where(p=>p.ID_Клиента == selected_order.ID_Клиента).Select(p=>p.ID_ПерсДанных).FirstOrDefault();
                    ComboBox_Клиент.SelectedItem = Entities.GetContext().Персональные_данные.Where(p => p.ID_ПерсДанных == ПерсДанные).Select(p => p.Фамилия + " " + p.Имя + " " +
                    p.Отчество + " " + p.Телефон).FirstOrDefault();
                    ComboBox_Статус.SelectedItem = Entities.GetContext().Статус.Where(p => p.ID_Статуса == selected_order.ID_Статуса).Select(p => p.Название).FirstOrDefault();
                    TB_Сумма.Text = selected_order.Сумма.ToString();
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
                search_window = new Search_Orders_Designs_Window();
                search_window.Show();
                search_window.orders_designs = this;
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

        private void Button_Report_Click(object sender, RoutedEventArgs e)
        {
            Reports_Window reports_window = new Reports_Window();
            reports_window.orders = this;
            reports_window.Show();
        }

        private void Button_Alert_Click(object sender, RoutedEventArgs e)
        {
            var mb = MessageBox.Show("Разослать клиентами уведомление о готовности заказов?", "Подтверждение", MessageBoxButton.YesNo);
            if (mb == MessageBoxResult.Yes)
            {
                Mail_send();
            }
        }

        private void Mail_send()
        {
            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress("abad3443@gmail.com", "Альбион");
            // кому отправляем
            foreach (Заказ заказ in Entities.GetContext().Заказ.ToList())
            {
                if (заказ.ID_Статуса == 9 && заказ.Уведомление == false && заказ.ID_ВидаЗаказа == 2)
                {
                    заказ.Уведомление = true;
                    Entities.GetContext().SaveChanges();
                    MailAddress to = new MailAddress($"{заказ.Клиенты.Персональные_данные.Эл_почта}");
                    // создаем объект сообщения
                    MailMessage m = new MailMessage(from, to);
                    // тема письма
                    m.Subject = "Дизайн готов!";
                    // текст письма
                    var фио = заказ.Клиенты.Персональные_данные.Фамилия + " " + заказ.Клиенты.Персональные_данные.Имя
                        + " " + заказ.Клиенты.Персональные_данные.Отчество;
                    m.Body = $"<p style=\"text-align: center;\">Ваш заказ дизайна готов!</p>\r\n<p style=\"text-align: left;\">{фио}," +
                        $" ваш заказ дизайна спроектирован, дизайнер скоро свяжется с вами. <br />P.S. Альбион :<br />г. Кемерово, ул. Тухачевского 47.</p>";
                    // письмо представляет код html
                    m.IsBodyHtml = true;
                    // адрес smtp-сервера и порт, с которого будем отправлять письмо
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    // логин и пароль
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("abad3443@gmail.com", "fazr ydac dxkf nxtp");
                    try
                    {
                        smtp.Send(m);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при отправке сообщения: " + ex.Message);
                    }
                }
            }
            MessageBox.Show("Сообщения о готовности заказов были успешно отправлены клиентам", "Операция завершена");

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
            if (TB_Сумма.Text == "" || ComboBox_Клиент.SelectedItem == null || DP_Дата.SelectedDate == null)
            {
                MessageBox.Show("Не все поля былы заполнены!", "Ошибка");
            }
            else if (Regex_Сумма.IsMatch(TB_Сумма.Text))
            {
                try
                {
                    string[] nsp = ComboBox_Клиент.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); //переменная массив, где содержится ФИО
                    var фамилия = nsp[0]; var имя = nsp[1]; var отчество = nsp[2]; var телефон = nsp[3];
                    var ПерсДанные = Entities.GetContext().Персональные_данные.Where(p => p.Фамилия.Contains(фамилия) && p.Имя.Contains(имя) && p.Отчество.Contains(отчество) && p.Телефон.Contains(телефон)).Select(p => p.ID_ПерсДанных).First();
                    selected_order.ID_Клиента = Entities.GetContext().Клиенты.Where(p=>p.ID_ПерсДанных == ПерсДанные).Select(p=>p.ID_Клиента).FirstOrDefault();
                    selected_order.ДатаОформления = (DateTime)DP_Дата.SelectedDate;
                    selected_order.ID_Статуса = 2;
                    selected_order.Сумма = Convert.ToDecimal(TB_Сумма.Text);
                    Entities.GetContext().SaveChanges();
                    MessageBox.Show("Новая запись добавлена.", "Подтверждение");

                    process_add = false;
                    DP_Дата2.IsEnabled = true;
                    Grid_Add_Edit.IsEnabled = false;
                    ComboBox_Статус.IsEnabled = true;
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
            if (TB_Сумма.Text == "" || ComboBox_Клиент.SelectedItem == null || ComboBox_Статус.SelectedItem == null || DP_Дата.SelectedDate == null)
            {
                MessageBox.Show("Не все поля былы заполнены!", "Ошибка");
            }
            else if (Regex_Сумма.IsMatch(TB_Сумма.Text))
            {
                if (selected_order != null)
                {
                    try
                    {
                        string[] nsp = ComboBox_Клиент.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); //переменная массив, где содержится ФИО
                        var фамилия = nsp[0]; var имя = nsp[1]; var отчество = nsp[2]; var телефон = nsp[3];
                        var ПерсДанные = Entities.GetContext().Персональные_данные.Where(p => p.Фамилия.Contains(фамилия) && p.Имя.Contains(имя) && p.Отчество.Contains(отчество) && p.Телефон.Contains(телефон)).Select(p => p.ID_ПерсДанных).First();
                        selected_order.ID_Клиента = Entities.GetContext().Клиенты.Where(p => p.ID_ПерсДанных == ПерсДанные).Select(p => p.ID_Клиента).FirstOrDefault();
                        selected_order.ДатаОформления = (DateTime)DP_Дата.SelectedDate;
                        selected_order.ID_Статуса = Entities.GetContext().Статус.Where(p => p.Название.Contains(ComboBox_Статус.Text)).Select(p => p.ID_Статуса).First();
                        selected_order.Сумма = Convert.ToDecimal(TB_Сумма.Text);

                        if (DP_Дата2.SelectedDate != null)
                        {
                            if (DP_Дата2.SelectedDate < DP_Дата.SelectedDate && DP_Дата2.SelectedDate != DP_Дата.SelectedDate)
                            {
                                MessageBox.Show("Дата выполнения не может быть раньше даты оформления", "Ошибка");
                            }
                            else
                            {
                                selected_order.ДатаВыполнения = (DateTime)DP_Дата2.SelectedDate;
                                selected_order.ID_Статуса = 7;
                            }

                            if (Entities.GetContext().Статус.Where(p => p.Название.Contains(ComboBox_Статус.Text)).Select(p => p.ID_Статуса).First() == 7)
                            {
                                selected_order.ДатаВыполнения = (DateTime)DP_Дата2.SelectedDate;
                            }
                        }
                        else
                        {
                            selected_order.ДатаВыполнения = null;

                            if (Entities.GetContext().Статус.Where(p => p.Название.Contains(ComboBox_Статус.Text)).Select(p => p.ID_Статуса).First() == 7)
                            {
                                selected_order.ДатаВыполнения = DateTime.Now;
                            }
                        }

                        Entities.GetContext().SaveChanges();
                        MessageBox.Show("Выбранная запись изменена.", "Подтверждение");

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

        public void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (process_add == true)
            {
                process_add = false;
                Entities.GetContext().Заказ.Remove(selected_order);
                Entities.GetContext().SaveChanges();
            }
            if (process_edit == true)
            {
                process_edit = false;
                Entities.GetContext().Заказ.AddOrUpdate(backup_order_edit);
                Entities.GetContext().SaveChanges();

            }
            Page_Loaded(sender, e);
            Grid_Add_Edit.IsEnabled = false;
        }

        private void Button_Калькулятор_Click(object sender, RoutedEventArgs e)
        {
            Calculated_Window cw = new Calculated_Window();
            cw.od = this;
            cw.Show();
        }

        Regex Regex_Сумма = new Regex(@"^[0-9\,\.]{2,13}$");

        private void TB_Сумма_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TB_Сумма.Text == "")
            {
                mark_сумма.Text = "<";
                mark_сумма.Foreground = Brushes.Gray;
            }
            else
            {
                if (Regex_Сумма.IsMatch(TB_Сумма.Text))
                {
                    mark_сумма.Text = "*";
                    mark_сумма.Foreground = Brushes.Green;

                }
                else
                {
                    mark_сумма.Text = "!";
                    mark_сумма.Foreground = Brushes.Red;
                }
            }
        }

        private void ComboBox_Клиент_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox_Клиент.SelectionBoxItem.ToString() == "")
            {
                mark_клиент.Text = "<";
                mark_клиент.Foreground = Brushes.Gray;
            }
            else
            {
                mark_клиент.Text = "*";
                mark_клиент.Foreground = Brushes.Green;
            }
        }

        private void DP_Дата_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DP_Дата.SelectedDate == null)
            {
                mark_дата.Text = "<";
                mark_дата.Foreground = Brushes.Gray;
            }
            else
            {
                mark_дата.Text = "*";
                mark_дата.Foreground = Brushes.Green;
            }
        }

        private void DP_Дата2_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DP_Дата2.SelectedDate == null)
            {
                mark_дата2.Text = "<";
                mark_дата2.Foreground = Brushes.Gray;
            }
            else
            {
                mark_дата2.Text = "*";
                mark_дата2.Foreground = Brushes.Green;
            }
        }

        private void ComboBox_Статус_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox_Статус.SelectionBoxItem.ToString() == "")
            {
                mark_статус.Text = "<";
                mark_статус.Foreground = Brushes.Gray;
            }
            else
            {
                mark_статус.Text = "*";
                mark_статус.Foreground = Brushes.Green;
            }
        }

        private void DG_Orders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (process_add == true || process_edit == true)
            {

            }
            else
            {
                try
                {
                    selected_order = (Заказ)DG_Orders.SelectedItem;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.Message}, {ex.Source}, {ex.InnerException}");
                }
            }
        }
    }
}
