using LynxUI_Main.Services;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;

namespace LynxUI_Main
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            StartupAsync();
        }

        private async void StartupAsync()
        {
            string? savedToken = TokenStorage.LoadToken();
            if (!string.IsNullOrEmpty(savedToken))
            {
                var api = new ApiService();
                int? userId = await api.GetCurrentUserIdAsync();
                var user = await api.GetUserByIdAsync(userId.Value);

                if (userId.HasValue)
                {
                    var mainWindow = new LynxUI_Main.MainWindow(userId.Value, user.DisplayName);
                    MainWindow = mainWindow;
                    mainWindow.Show();
                }
                else
                {
                    // Token không hợp lệ, xóa và hiện LoginWindow
                    TokenStorage.ClearToken();
                    ShowLogin();
                }
            }
            else
            {
                ShowLogin();
            }
        }
        private void ShowLogin()
        {
            var login = new ViewLogin.LoginWindow();
            MainWindow = login;
            login.Show();
        }

        private async Task<bool> CheckTokenValid(string token)
        {
            try
            {
                using (var http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);

                    // Gửi request đến một API yêu cầu [Authorize]
                    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(8)); // timeout 8s
                    var response = await http.GetAsync("https://localhost:7031/api/auth/me", cts.Token);
                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
