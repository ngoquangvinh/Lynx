using LynxUI_Main.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LynxUI_Main.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ChatListViewModel ChatListVM { get; }
        public ConversationViewModel ConversationVM { get; }
        public SignalRService SignalRService { get; }

        public ChatListItem SelectedChat
        {
            get => ChatListVM.SelectedChat;
            set
            {
                if (ChatListVM.SelectedChat != value && value != null)
                {
                    ChatListVM.SelectedChat = value;
                    ConversationVM.ActiveChat = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainViewModel(int userId, SignalRService signalRService, string displayName)
        {
            SignalRService = signalRService;
            ChatListVM = new ChatListViewModel(userId);
            ConversationVM = new ConversationViewModel(userId, signalRService, displayName);
            ChatListVM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ChatListViewModel.SelectedChat))
                {
                    ConversationVM.ActiveChat = ChatListVM.SelectedChat;
                    OnPropertyChanged(nameof(SelectedChat)); // Notify UI
                }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
