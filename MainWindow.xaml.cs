using LynxUI_Main.Helpers;
using LynxUI_Main.ViewModels;
using LynxUI_Main.ViewLogin;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace LynxUI_Main
{
    public partial class MainWindow : Window
    {
        private readonly ChatListViewModel _chatListViewModel;
        private readonly ConversationViewModel _conversationViewModel;

        public MainWindow()
        {
            InitializeComponent();

            // ViewModels initialization
            _chatListViewModel = new ChatListViewModel();
            _conversationViewModel = new ConversationViewModel();

            ChatListControl.DataContext = _chatListViewModel;
            ConversationControl.DataContext = _conversationViewModel;

            _chatListViewModel.PropertyChanged += ChatViewModel_PropertyChanged;
        }

        private void ChatViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ChatListViewModel.SelectedChat))
            {
                _conversationViewModel.ActiveChat = _chatListViewModel.SelectedChat;
            }
        }
        public void ToggleSidebar()
        {
            double from = SidebarColumn.ActualWidth;
            double to = isSidebarOpen ? 0 : 310;

            var animation = new GridLengthAnimation
            {
                From = new GridLength(from),
                To = new GridLength(to),
                Duration = TimeSpan.FromSeconds(0.3)
            };

            SidebarColumn.BeginAnimation(ColumnDefinition.WidthProperty, animation);
            isSidebarOpen = !isSidebarOpen;
        }

        private bool isSidebarOpen = false;

        private void BtnToggleSidebar_Click(object sender, RoutedEventArgs e)
        {
            var animation = new GridLengthAnimation
            {
                From = SidebarColumn.Width,
                To = isSidebarOpen ? new GridLength(0) : new GridLength(310),
                Duration = TimeSpan.FromSeconds(0.2)
            };

            SidebarColumn.BeginAnimation(ColumnDefinition.WidthProperty, animation);
            isSidebarOpen = !isSidebarOpen;
        }
    }
}
