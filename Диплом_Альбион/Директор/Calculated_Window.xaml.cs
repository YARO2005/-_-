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
    /// Логика взаимодействия для Calculated_Window.xaml
    /// </summary>
    public partial class Calculated_Window : Window
    {
        public Calculated_Window()
        {
            InitializeComponent();
        }

        public Orders_Designs od;

        private void Button_Result_Click(object sender, RoutedEventArgs e)
        {
            if (TB_Колво.Text == "" || TB_Цена.Text == "")
            {
                MessageBox.Show("Не все поля были заполнены", "Ошибка");
            }
            else
            {
                TB_Итог.Text = (Convert.ToDecimal(TB_Цена.Text) * Convert.ToDecimal(TB_Колво.Text)).ToString();
                od.TB_Сумма.Text = TB_Итог.Text;
            }    
        }
    }
}
