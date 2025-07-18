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

namespace Диплом_Альбион
{
    /// <summary>
    /// Логика взаимодействия для Work_Manager_Window.xaml
    /// </summary>
    public partial class Work_Director_Window : Window
    {
        public Work_Director_Window()
        {
            InitializeComponent();
            MainFrame.Navigate(users);
            ClearHistory();
            users.work_director_window = this;
            orders_designs.work_director_window = this;
        }

        public Work_Between_Window wbw;
        public Users users = new Users();
        public Stats stats = new Stats();
        public Orders_Designs orders_designs = new Orders_Designs();

        private void Button_Users_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(users);
            ClearHistory();
        }

        private void Button_Stats_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(stats);
            ClearHistory();
        }

        private void Button_Orders_Designs_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(orders_designs);
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
