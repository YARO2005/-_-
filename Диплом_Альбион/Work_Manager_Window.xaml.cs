using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Диплом_Альбион.Директор;
using Диплом_Альбион.Менеджер;

namespace Диплом_Альбион
{
    /// <summary>
    /// Логика взаимодействия для Work_Manager_Window.xaml
    /// </summary>
    public partial class Work_Manager_Window : Window
    {
        public Work_Manager_Window()
        {
            InitializeComponent();
            products.work_manager_window = this;
            orders.work_manager_window = this;
            customers.work_manager_window = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Button_Products_Click(sender, e);
        }

        private void Hot_Button_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainWindow.role_id != 4)
            {
                Button_To_Between.IsEnabled = false;
                Button_To_Between.Visibility = Visibility.Hidden;
            }
            if (MainWindow.role_id == 4)
            {
                Button_To_Between.IsEnabled = true;
                Button_To_Between.Visibility = Visibility.Visible;
            }
        }

        public Work_Between_Window wbw;
        public Products products = new Products();
        public Customers customers = new Customers();
        public Orders orders = new Orders();

        private void Button_Products_Click(object sender, RoutedEventArgs e)
        {
            products.work_manager_window = this;
            MainFrame.Navigate(products);
            ClearHistory();
        }

        private void Button_Orders_Click(object sender, RoutedEventArgs e)
        {
            orders.work_manager_window = this;
            MainFrame.Navigate(orders);
            ClearHistory();
        }

        private void Button_Customers_Click(object sender, RoutedEventArgs e)
        {
            customers.work_manager_window = this;
            MainFrame.Navigate(customers);
            ClearHistory();
        }

        private void Button_To_Between_Click(object sender, RoutedEventArgs e)
        {
            wbw.Show();
            this.Hide();
        }

        public void ClearHistory()
        {
            while (MainFrame.CanGoBack)
            {
                MainFrame.RemoveBackEntry();
            }
        }
    }
}
