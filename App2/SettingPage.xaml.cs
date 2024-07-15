using App2;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Core;
using WinRT.Interop;
using static BrowserLauncherApp.SettingsPage;
namespace BrowserLauncherApp
{

    public class Website
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public sealed partial class SettingsPage : Page
    {
        private const string DataFilePath = "websites.json";

        public ObservableCollection<Website> Websites { get; set; }
        public SettingsPage()
        {
            this.InitializeComponent();
            LoadWebsites();


            // ��������������
            WebsiteListView.ItemsSource = Websites;
        
        // ��ȡ����ʾ����
        DisplaySettings();

            //FilePathTextBlock.Text = "No file selected";



        }
        private void LoadWebsites()
        {
            try
            {
                if (File.Exists(DataFilePath))
                {
                    string json = File.ReadAllText(DataFilePath);
                    Websites = JsonSerializer.Deserialize<ObservableCollection<Website>>(json);
                }
                else
                {
                    Websites = new ObservableCollection<Website>
                    {
                        new Website { Name = "ѧϰͨ", Url = "https://passport2.chaoxing.com/" },
                        new Website { Name = "�����", Url = "https://www.yuketang.cn/web" },
                        new Website { Name = "����ѧ", Url = "https://www.zjooc.cn/course?type=0" }
                    };
                    string json = JsonSerializer.Serialize(Websites);
                    File.WriteAllText(DataFilePath, json);
                }
            }
            catch (Exception ex)
            {
                
            }
        }
    
        private async void EdgeSelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ��ʼ���ļ�ѡ����
                var picker = new FileOpenPicker
                {
                    ViewMode = PickerViewMode.Thumbnail,
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary
                };
                picker.FileTypeFilter.Add(".exe");


                // ��ȡ���ھ������ʼ���ļ�ѡ����
                var hwnd = App.GetWindowHandle();
                InitializeWithWindow.Initialize(picker, hwnd);

                // �첽ѡ���ļ�
                var file = await picker.PickSingleFileAsync();

                if (file != null)
                {
                    EdgebPath.Text = file.Path;
                    SaveSettings("edgePath", EdgebPath.Text);

                }
            }
            catch (System.Runtime.InteropServices.COMException comEx)
            {
                EdgebPath.Text = "Error: " + comEx.Message;
                System.Diagnostics.Debug.WriteLine($"COMException: {comEx.Message}");
                System.Diagnostics.Debug.WriteLine(comEx.StackTrace);
            }
            catch (Exception ex)
            {
                EdgebPath.Text = "Error: " + ex.Message;
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        private async void EdgedSelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ��ʼ���ļ�ѡ����
                var picker = new FolderPicker
                {
                    ViewMode = PickerViewMode.Thumbnail,
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary
                };
                picker.FileTypeFilter.Add(".exe");


                // ��ȡ���ھ������ʼ���ļ�ѡ����
                var hwnd = App.GetWindowHandle();
                InitializeWithWindow.Initialize(picker, hwnd);

                // �첽ѡ���ļ�
                var file = await picker.PickSingleFolderAsync();

                if (file != null)
                {
                    EdgedbPath.Text = file.Path;
                    SaveSettings("edgedPath", EdgedbPath.Text);

                }
            }
            catch (System.Runtime.InteropServices.COMException comEx)
            {
                EdgedbPath.Text = "Error: " + comEx.Message;
                System.Diagnostics.Debug.WriteLine($"COMException: {comEx.Message}");
                System.Diagnostics.Debug.WriteLine(comEx.StackTrace);
            }
            catch (Exception ex)
            {
                EdgedbPath.Text = "Error: " + ex.Message;
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        private async void ChromeSelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ��ʼ���ļ�ѡ����
                var picker = new FileOpenPicker
                {
                    ViewMode = PickerViewMode.Thumbnail,
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary
                };
                picker.FileTypeFilter.Add(".exe");


                // ��ȡ���ھ������ʼ���ļ�ѡ����
                var hwnd = App.GetWindowHandle();
                InitializeWithWindow.Initialize(picker, hwnd);

                // �첽ѡ���ļ�
                var file = await picker.PickSingleFileAsync();

                if (file != null)
                {
                    ChromebPath.Text = file.Path;
                    SaveSettings("ChromePath", ChromebPath.Text);
                }
            }
            catch (System.Runtime.InteropServices.COMException comEx)
            {
                ChromebPath.Text = "Error: " + comEx.Message;
                System.Diagnostics.Debug.WriteLine($"COMException: {comEx.Message}");
                System.Diagnostics.Debug.WriteLine(comEx.StackTrace);
            }
            catch (Exception ex)
            {
                ChromebPath.Text = "Error: " + ex.Message;
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        private async void ChromedSelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ��ʼ���ļ�ѡ����
                var picker = new FolderPicker
                {
                    ViewMode = PickerViewMode.Thumbnail,
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary
                };



                // ��ȡ���ھ������ʼ���ļ�ѡ����
                var hwnd = App.GetWindowHandle();
                InitializeWithWindow.Initialize(picker, hwnd);

                // �첽ѡ���ļ�
                var file = await picker.PickSingleFolderAsync();

                if (file != null)
                {
                    ChromedbPath.Text = file.Path;
                    SaveSettings("ChromedPath", ChromedbPath.Text);
                }
            }
            catch (System.Runtime.InteropServices.COMException comEx)
            {
                ChromedbPath.Text = "Error: " + comEx.Message;
                System.Diagnostics.Debug.WriteLine($"COMException: {comEx.Message}");
                System.Diagnostics.Debug.WriteLine(comEx.StackTrace);
            }
            catch (Exception ex)
            {
                ChromedbPath.Text = "Error: " + ex.Message;
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        /*
        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings("edgePath", edgePath.Text);
            SaveSettings("chromePath", chromePath.Text);
        

        private void LoadSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            DisplaySettings();
        }}*/

        private void SaveSettings(string key, string value)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values[key] = value;
        }

        private string LoadSettings(string key)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values.ContainsKey(key))
            {
                return localSettings.Values[key] as string;
            }
            else
            {
                return null;
            }
        }

        private void DisplaySettings()
        {

            ChromebPath.Text = LoadSettings("chromePath") ?? "No username saved.";
            EdgebPath.Text = LoadSettings("edgePath") ?? "No username saved.";
            ChromedbPath.Text = LoadSettings("chromedPath") ?? "No username saved.";
            EdgedbPath.Text = LoadSettings("edgedPath") ?? "No username saved.";
            

        }


        /*Chrome·�����ı�����ı���*/
        private void ChromePath_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ChromebPath.Visibility = Visibility.Collapsed;  // ����TextBlock
            ChromexPath.Visibility = Visibility.Visible;   // ��ʾTextBox
            ChromexPath.Focus(FocusState.Programmatic);    // ��TextBox��ý���
        }
        private void ChromexPath_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                SaveSettings("ChromePath", ChromebPath.Text);
                ChromebPath.Text = ChromexPath.Text;
                ChromexPath.Visibility = Visibility.Collapsed; // ����TextBox
                ChromebPath.Visibility = Visibility.Visible;   // ��ʾTextBlock
            }
        }
        private void ChromePath_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveSettings("ChromePath", ChromebPath.Text);
                ChromebPath.Text = ChromexPath.Text;
            ChromebPath.Visibility = Visibility.Visible;    // ��ʾTextBlock
            ChromexPath.Visibility = Visibility.Collapsed; // ����TextBox
        }

        /*����*/
        private void ChromePathOP_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ChromePathOP.Foreground = new SolidColorBrush(Colors.Gray);

        }
        private void ChromePathOP_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ChromePathOP.Foreground = new SolidColorBrush(Colors.Black);

        }
        private async void ChromePathOP_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                // ��ȡ�ļ�·��
                string filePath = ChromebPath.Text;

                // ����ļ�·����Ϊ��
                if (!string.IsNullOrEmpty(filePath))
                {
                    // ��ȡ�ļ���Ϣ
                    StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);
                    // ��ȡ�ļ����ڵ�Ŀ¼
                    StorageFolder folder = await file.GetParentAsync();

                    // ʹ�� FolderLauncherOptions ��Ŀ¼����ָ����ͼģʽΪ�б�
                    var options = new FolderLauncherOptions { DesiredRemainingView = (Windows.UI.ViewManagement.ViewSizePreference)PickerViewMode.List };
                    await Launcher.LaunchFolderAsync(folder, options);
                }
            }
            catch (FileNotFoundException)
            {
                ChromebPath.Text = "Error: File not found";
            }
            catch (Exception ex)
            {
                ChromebPath.Text = "Error: " + ex.Message;
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        /*Chrome���ݣ�����*/
        private void ChromedPathOP_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ChromedPathOP.Foreground = new SolidColorBrush(Colors.Gray);
            
        }
        private void ChromedPathOP_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ChromedPathOP.Foreground = new SolidColorBrush(Colors.Black);
            
        }
        private async void ChromedPathOP_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                // ��ȡ�ļ���·��
                string folderPath = ChromedbPath.Text;

                // ����Դ��������ָ��Ŀ¼
                if (!string.IsNullOrEmpty(folderPath))
                {
                    StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
                    var options = new FolderLauncherOptions { DesiredRemainingView = (Windows.UI.ViewManagement.ViewSizePreference)PickerViewMode.List };
                    await Launcher.LaunchFolderAsync(folder, options);
                }
            }
            catch (FileNotFoundException)
            {
                ChromedbPath.Text = "Error: Folder not found";
            }
            catch (Exception ex)
            {
                ChromedbPath.Text = "Error: " + ex.Message;
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        /*�ı����ı���*/
        private void ChromedPath_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            
            ChromedbPath.Visibility = Visibility.Collapsed;  // ����TextBlock
            ChromedxPath.Visibility = Visibility.Visible;   // ��ʾTextBox
            ChromedxPath.Focus(FocusState.Programmatic);    // ��TextBox��ý���
        }
        private void ChromedxPath_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                SaveSettings("ChromedPath", ChromedbPath.Text);
                ChromebPath.Text = ChromexPath.Text;
                ChromexPath.Visibility = Visibility.Collapsed; // ����TextBox
                ChromebPath.Visibility = Visibility.Visible;   // ��ʾTextBlock
            }
        }
        private void ChromedPath_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveSettings("ChromedPath", ChromedbPath.Text);
            ChromedbPath.Text = ChromedxPath.Text;
            ChromedbPath.Visibility = Visibility.Visible;    // ��ʾTextBlock
            ChromedxPath.Visibility = Visibility.Collapsed; // ����TextBox
        }



        /*Edge·�����ı�����ı���*/
        private void EdgePath_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            EdgebPath.Visibility = Visibility.Collapsed;  // ����TextBlock
            EdgexPath.Visibility = Visibility.Visible;   // ��ʾTextBox
            EdgexPath.Focus(FocusState.Programmatic);    // ��TextBox��ý���
        }
        private void EdgexPath_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                SaveSettings("edgePath", EdgebPath.Text);
                EdgebPath.Text = EdgexPath.Text;
                EdgexPath.Visibility = Visibility.Collapsed; // ����TextBox
                EdgebPath.Visibility = Visibility.Visible;   // ��ʾTextBlock
            }
        }
        private void EdgePath_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveSettings("edgePath", EdgebPath.Text);
            EdgebPath.Text = EdgexPath.Text;
            EdgebPath.Visibility = Visibility.Visible;    // ��ʾTextBlock
            EdgexPath.Visibility = Visibility.Collapsed; // ����TextBox
        }

        /*����*/
        private void EdgePathOP_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            EdgePathOP.Foreground = new SolidColorBrush(Colors.Gray);

        }
        private void EdgePathOP_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            EdgePathOP.Foreground = new SolidColorBrush(Colors.Black);

        }
        private async void EdgePathOP_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                // ��ȡ�ļ�·��
                string filePath = EdgebPath.Text;

                // ����ļ�·����Ϊ��
                if (!string.IsNullOrEmpty(filePath))
                {
                    // ��ȡ�ļ���Ϣ
                    StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);
                    // ��ȡ�ļ����ڵ�Ŀ¼
                    StorageFolder folder = await file.GetParentAsync();

                    // ʹ�� FolderLauncherOptions ��Ŀ¼����ָ����ͼģʽΪ�б�
                    var options = new FolderLauncherOptions { DesiredRemainingView = (Windows.UI.ViewManagement.ViewSizePreference)PickerViewMode.List };
                    await Launcher.LaunchFolderAsync(folder, options);
                }
            }
            catch (FileNotFoundException)
            {
                ChromebPath.Text = "Error: File not found";
            }
            catch (Exception ex)
            {
                EdgebPath.Text = "Error: " + ex.Message;
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        /*Edge���ݣ�����*/
        private void EdgedbPathOP_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            EdgedbPathOP.Foreground = new SolidColorBrush(Colors.Gray);

        }
        private void EdgedbPathOP_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            EdgedbPathOP.Foreground = new SolidColorBrush(Colors.Black);

        }
        private async void EdgedbPathOP_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                // ��ȡ�ļ���·��
                string folderPath = EdgedbPath.Text;

                // ����Դ��������ָ��Ŀ¼
                if (!string.IsNullOrEmpty(folderPath))
                {
                    StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
                    var options = new FolderLauncherOptions { DesiredRemainingView = (Windows.UI.ViewManagement.ViewSizePreference)PickerViewMode.List };
                    await Launcher.LaunchFolderAsync(folder, options);
                }
            }
            catch (FileNotFoundException)
            {
                ChromedbPath.Text = "Error: Folder not found";
            }
            catch (Exception ex)
            {
                ChromedbPath.Text = "Error: " + ex.Message;
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }
        /*�ı����ı���*/
        private void EdgedPath_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            EdgedbPath.Visibility = Visibility.Collapsed;  // ����TextBlock
            EdgedxPath.Visibility = Visibility.Visible;   // ��ʾTextBox
            EdgedxPath.Focus(FocusState.Programmatic);    // ��TextBox��ý���
        }
        private void EdgedxPath_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                SaveSettings("edgedPath", EdgedbPath.Text);
                EdgedbPath.Text = EdgexPath.Text;
                EdgedxPath.Visibility = Visibility.Collapsed; // ����TextBox
                EdgedbPath.Visibility = Visibility.Visible;   // ��ʾTextBlock
            }
        }
        private void EdgedPath_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveSettings("edgedPath", EdgedbPath.Text);
            EdgedbPath.Text = EdgedxPath.Text;
            EdgedbPath.Visibility = Visibility.Visible;    // ��ʾTextBlock
            EdgedxPath.Visibility = Visibility.Collapsed; // ����TextBox
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Websites.Insert(0, new Website
            {
                Name = "New Website",

                Url = "www.example.com",


            });
            SaveWebsites();

        }
    

        private void WebsiteNameEdit(object sender, RoutedEventArgs e)
        {
            TextBlock WebsiteNameBlock = sender as TextBlock;
            TextBox WebsiteNameBox = FindVisualChild<TextBox>(WebsiteNameBlock.Parent as UIElement, "WebsiteNameBox");
            WebsiteNameBlock.Visibility = Visibility.Collapsed;

            WebsiteNameBox.Visibility = Visibility.Visible;
            WebsiteNameBox.Focus(FocusState.Programmatic);

        }

        private void WebsiteNameEditFinished(object sender, RoutedEventArgs e)
        {
            TextBox WebsiteNameBox = sender as TextBox;
            TextBlock WebsiteNameBlock = FindVisualChild<TextBlock>(WebsiteNameBox.Parent as UIElement, "WebsiteNameBlock");
            WebsiteNameBlock.Visibility = Visibility.Visible;
            WebsiteNameBox.Visibility = Visibility.Collapsed;
            WebsiteNameBlock.Text = WebsiteNameBox.Text;
            SaveWebsites();
        }

        private void UrlPathEdit(object sender, RoutedEventArgs e)
        {
            TextBlock UrlPathBlock = sender as TextBlock;
            TextBox UrlPathBox = FindVisualChild<TextBox>(UrlPathBlock.Parent as UIElement, "UrlPathBox");
            UrlPathBlock.Visibility = Visibility.Collapsed;
            UrlPathBox.Visibility = Visibility.Visible;
            UrlPathBox.Focus(FocusState.Programmatic);

        }

        private void UrlPathEditFinished(object sender, RoutedEventArgs e)
        {
            TextBox UrlPathBox = sender as TextBox;
            TextBlock UrlPathBlock = FindVisualChild<TextBlock>(UrlPathBox.Parent as UIElement, "UrlPathBlock");
            UrlPathBlock.Visibility = Visibility.Visible;
            UrlPathBox.Visibility = Visibility.Collapsed;
            UrlPathBlock.Text = UrlPathBox.Text;
            SaveWebsites();
        }

        private T FindVisualChild<T>(DependencyObject parent, string childName) where T : FrameworkElement
        {
            if (parent == null) return null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T && ((T)child).Name == childName)
                {
                    return (T)child;
                }

                var childOfChild = FindVisualChild<T>(child, childName);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }

            return null;
        }
        
        private void SaveWebsites()
        {
            try
            {
                string json = JsonSerializer.Serialize(Websites);
                File.WriteAllText(DataFilePath, json);
                
            }
            catch (Exception ex)
            {
                
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            // �ҵ����������ģ�Ҫɾ������Ŀ��
            var menuFlyoutItem = sender as MenuFlyoutItem;
            if (menuFlyoutItem != null)
            {
                var itemToDelete = menuFlyoutItem.DataContext as Website;
                if (itemToDelete != null)
                {
                    Websites.Remove(itemToDelete);
                }
                SaveWebsites();
            }
        }

        private void StackPanel_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (sender is StackPanel stackPanel)
            {
                stackPanel.Background = new SolidColorBrush(Colors.LightGray); // �޸�Ϊ����Ҫ����ɫ
            }
        }

        private void StackPanel_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (sender is StackPanel stackPanel)
            {
                stackPanel.Background = new SolidColorBrush(Colors.WhiteSmoke);
            }
        }




    }

}




