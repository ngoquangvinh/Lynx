using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LynxUI_Main.Custom_Controls
{
    public partial class MenuListControl : UserControl
    {
        public event EventHandler<MenuItems> MenuItemClicked;

        public ViewModel ViewModelInstance { get; set; }

        public MenuListControl()
        {
            InitializeComponent();

            ViewModelInstance = new ViewModel();
            DataContext = ViewModelInstance;
        }

        private void MenuList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var removedItem in e.RemovedItems)
            {
                if (MenuList.ItemContainerGenerator.ContainerFromItem(removedItem) is ListBoxItem oldItem)
                {
                    oldItem.ApplyTemplate();
                    var oldIndicator = oldItem.Template.FindName("MenuIndicator", oldItem) as Border;
                    var oldIcon = FindVisualChild<Path>(oldItem);

                    if (oldIndicator != null) oldIndicator.Visibility = Visibility.Collapsed;
                    if (oldIcon != null) oldIcon.Fill = (Brush)new BrushConverter().ConvertFrom("#686D83");
                }
            }

            foreach (var addedItem in e.AddedItems)
            {
                if (MenuList.ItemContainerGenerator.ContainerFromItem(addedItem) is ListBoxItem newItem)
                {
                    newItem.ApplyTemplate();
                    var newIndicator = newItem.Template?.FindName("MenuIndicator", newItem) as Border;
                    var newIcon = FindVisualChild<Path>(newItem);

                    if (newIndicator != null) newIndicator.Visibility = Visibility.Visible;
                    if (newIcon != null) newIcon.Fill = Brushes.White;

                    if (addedItem is MenuItems menuItem)
                    {
                        MenuItemClicked?.Invoke(this, menuItem);
                    }
                }
            }
        }


        private void MenuItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is ListBoxItem item)
            {
                if (!item.IsSelected)
                {
                    var indicator = item.Template?.FindName("MenuIndicator", item) as Border;
                    var icon = FindVisualChild<Path>(item);

                    if (indicator != null) indicator.Visibility = Visibility.Visible;
                    if (icon != null) icon.Fill = Brushes.White;
                }
            }
        }

        private void MenuItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is ListBoxItem item)
            {
                if (!item.IsSelected)
                {
                    var indicator = item.Template?.FindName("MenuIndicator", item) as Border;
                    var icon = FindVisualChild<Path>(item);

                    if (indicator != null) indicator.Visibility = Visibility.Collapsed;
                    if (icon != null) icon.Fill = (Brush)new BrushConverter().ConvertFrom("#686D83");
                }
            }
        }

        private T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                    return typedChild;

                T childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }

        private void Settings_MouseEnter(object sender, MouseEventArgs e)
        {
            SettingsIndicator.Visibility = Visibility.Visible;
            SettingsIcon.Fill = Brushes.White;
        }

        private void Settings_MouseLeave(object sender, MouseEventArgs e)
        {
            SettingsIndicator.Visibility = Visibility.Collapsed;
            SettingsIcon.Fill = (Brush)new BrushConverter().ConvertFrom("#686D83");
        }

        private void Settings_MouseClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Settings!");
        }

    }
}
