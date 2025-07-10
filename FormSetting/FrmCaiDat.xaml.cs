using System;
using System.IO;
using System.Windows;

namespace LynxUI_Main.FormSetting
{
    public partial class FrmCaiDat : Window
    {
        public FrmCaiDat()
        {
            InitializeComponent();
        }

        private void btnDangXuat_Click(object sender, RoutedEventArgs e)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string tokenFile = Path.Combine(appDataPath, "lynx_token.txt");

            if (File.Exists(tokenFile))
            {
                try
                {
                    File.Delete(tokenFile);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể xóa file đăng nhập: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            MessageBox.Show("Bạn đã đăng xuất!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

            // Đóng MainWindow và mở lại LoginWindow
            var loginWindow = new LynxUI_Main.ViewLogin.LoginWindow();
            loginWindow.Show();

            // Đóng tất cả cửa sổ chính (bao gồm cả form cài đặt)
            foreach (Window win in Application.Current.Windows)
            {
                if (win != loginWindow)
                    win.Close();
            }
        }

    }
}
