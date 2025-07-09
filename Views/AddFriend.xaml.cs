using LynxUI_Main.ViewModels;
using System.Windows;

namespace LynxUI_Main.Views
{
    public partial class AddFriend : Window
    {

        public AddFriend(int currentUserId)
        {
            InitializeComponent();

            var vm = new AddFriendViewModel(currentUserId);
            DataContext = vm;
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
