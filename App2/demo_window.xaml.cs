using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using BrowserLauncherApp;
using System.Collections.ObjectModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App2
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class demo_window : Window
    {
        public demo_window()
        {
            this.InitializeComponent();
            Items = new ObservableCollection<Item>
        {
            new Item { Name = "Item 1", Description = "Description 1" },
            new Item { Name = "Item 2", Description = "Description 2" }
        };
            myListView.ItemsSource = Items;

        }
        public ObservableCollection<Item> Items { get; set; }




        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            var menuFlyoutItem = sender as MenuFlyoutItem;
            if (menuFlyoutItem != null)
            {
                var flyout = menuFlyoutItem.Parent as MenuFlyout;
                if (flyout != null)
                {
                    var listViewItem = flyout.Target as ListViewItem;
                    if (listViewItem != null)
                    {
                        var item = listViewItem.Content as Item;
                        if (item != null)
                        {
                            Items.Remove(item);
                        }
                    }
                }
            }
        }
    }
}
