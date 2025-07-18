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

namespace Диплом_Альбион.Директор
{
    /// <summary>
    /// Логика взаимодействия для Work_Between_Window.xaml
    /// </summary>
    public partial class Work_Between_Window : Window
    {
        public Work_Between_Window()
        {
            InitializeComponent();
            wdw.wbw = this;
            wmw.wbw = this;
        }

        Work_Manager_Window wmw = new Work_Manager_Window();
        Work_Director_Window wdw = new Work_Director_Window();

        private void Button_To_Manager_Click(object sender, RoutedEventArgs e)
        {
            wmw.Show();
            this.Hide();
        }

        private void Button_To_Director_Click(object sender, RoutedEventArgs e)
        {
            wdw.Show();
            this.Hide();
        }

        private void Button_Exite_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
