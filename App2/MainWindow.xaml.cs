using Windows.UI.ApplicationSettings;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Linq;
using Microsoft.UI;
using WinRT.Interop;
using Microsoft.UI.Windowing;

namespace BrowserLauncherApp


{
    
    public sealed partial class MainWindow : Window
    {
        string curengPage = "Home";
        private const string DataFileName = "data.json";
        public MainWindow()
        {
            this.InitializeComponent();
            NavigationViewControl.ItemInvoked += OnNavigationViewItemInvoked;
           
            // Default selection and navigation to Home page
            var homeItem = NavigationViewControl.MenuItems.OfType<NavigationViewItem>().FirstOrDefault(item => item.Content.ToString() == "Home");
            if (homeItem != null)
            {
                NavigationViewControl.SelectedItem = homeItem;
                ContentFrame.Navigate(typeof(HomePage));
            }
            




        }

        private void OnNavigationViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                ContentFrame.Navigate(typeof(SettingsPage));
                curengPage = "Settings";
            }
            else if (args.InvokedItem != null)
            {
                string invokedItem = args.InvokedItem.ToString();
                if (invokedItem != curengPage)
                {
                    switch (invokedItem)
                    {
                        case "Home":
                            ContentFrame.Navigate(typeof(HomePage));
                            curengPage = "Home";
                            break;
                        case "Processes":
                            ContentFrame.Navigate(typeof(ProcessesPage));
                            curengPage = "Processes";
                            break;
                    }
                }
            }
        }
    }
}
