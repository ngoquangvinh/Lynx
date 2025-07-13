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

namespace LynxUI_Main.ViewFrofile
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class EditFrofile : Window
    {
        public EditFrofile()
        {
            InitializeComponent();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Thông tin đã được lưu!");
            this.Close(); // hoặc làm gì đó
        }

    }
}
