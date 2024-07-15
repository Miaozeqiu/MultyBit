using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using static BrowserLauncherApp.ProcessesPage;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using BrowserLauncherApp;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using System.Diagnostics;
using Microsoft.UI.Windowing;
using System.Runtime.InteropServices;
using WinRT.Interop;
using Windows.UI.Popups;
using System.Windows;
using Microsoft.UI.Text;
using System.Text.RegularExpressions;
using Microsoft.UI.Xaml.Documents;
using IWshRuntimeLibrary;

public class DataManager
{

    
    private const string DataFileName = "data.json";

    public ObservableCollection<Item> EdgeItems { get; set; }
    public ObservableCollection<Item> ChromeItems { get; set; }

    public DataManager()
    {
        EdgeItems = new ObservableCollection<Item>();
        ChromeItems = new ObservableCollection<Item>();
    }

    // ��������
    public async Task LoadDataAsync()
    {
        
        try
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            Console.WriteLine(localFolder);
            StorageFile file = await localFolder.GetFileAsync(DataFileName);
            if (file != null)
            {
                string json = await FileIO.ReadTextAsync(file);
                var savedData = JsonSerializer.Deserialize<SavedData>(json);
                if (savedData != null)
                {
                    // �������ݵ� ObservableCollection
                    foreach (var item in savedData.EdgeItems)
                    {
                        EdgeItems.Add(item);
                    }
                    foreach (var item in savedData.ChromeItems)
                    {
                        ChromeItems.Add(item);
                    }
                }
            }
        }
        catch (FileNotFoundException)
        {
            // ����ļ������ڣ����Դ���һ���µı������ݵ��ļ�
            await SaveDataAsync();
        }
        catch (Exception ex)
        {
            // �����������ʱ���쳣
            Console.WriteLine($"Failed to load data: {ex.Message}");
        }
    }

    // ��������
    public async Task SaveDataAsync()
    {
        try
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.CreateFileAsync(DataFileName, CreationCollisionOption.ReplaceExisting);
            /*
            var options = new FolderLauncherOptions { DesiredRemainingView = (Windows.UI.ViewManagement.ViewSizePreference)PickerViewMode.List };
            await Launcher.LaunchFolderAsync(localFolder, options);*/


    SavedData dataToSave = new SavedData
            {
                EdgeItems = EdgeItems.ToList(),
                ChromeItems = ChromeItems.ToList()
            };

            string json = JsonSerializer.Serialize(dataToSave);
            await FileIO.WriteTextAsync(file, json);
        }
        catch (Exception ex)
        {
            // ����������ʱ���쳣
            Console.WriteLine($"Failed to save data: {ex.Message}");
        }
    }

    // ���ڱ���ͼ��صĸ�����
    private class SavedData
    {
        public List<Item> EdgeItems { get; set; }
        public List<Item> ChromeItems { get; set; }
    }

    public async Task ModifyEdgeItemAsyncName(int id ,string newName,string BrowserName)
    {
        Item itemToModify;
        if (BrowserName == "Chrome") {
            // ���Ҿ����ض� ID ����
            itemToModify = ChromeItems.FirstOrDefault(item => item.Id == id);
        }
        else
        {
            itemToModify = EdgeItems.FirstOrDefault(item => item.Id == id);
        }
        
        if (itemToModify != null)
        {
            // �޸��������
            itemToModify.Name = newName;
            //Console.WriteLine($"Edge item {itemName} �������Ѹ���Ϊ {newName}");

            // �����޸ĺ������
            await SaveDataAsync();
        }
        else
        {
            //Console.WriteLine($"δ�ҵ� ID Ϊ {itemName} �� Edge item");
        }
    }

    public async Task ModifyEdgeItemAsyncDis(int itemId, string newDis,string BrowserName)
    {
        Item itemToModify;
        if (BrowserName == "Chrome")
        {
            // ���Ҿ����ض� ID ����
            itemToModify = ChromeItems.FirstOrDefault(item => item.Id == itemId);
        }
        else
        {
            itemToModify = EdgeItems.FirstOrDefault(item => item.Id == itemId);
        }

        if (itemToModify != null)
        {
            // �޸��������
            itemToModify.Description = newDis;
            //Console.WriteLine($"Edge item {itemName} �������Ѹ���Ϊ {newName}");

            // �����޸ĺ������
            await SaveDataAsync();
        }
        else
        {
            //Console.WriteLine($"δ�ҵ� ID Ϊ {itemName} �� Edge item");
        }
    }

    public async Task ModifyEdgeItemAsyncPort(int itemId, int newPort, string BrowserName)
    {
        // ���Ҿ����ض� ID ����
        Item itemToModify;
        if (BrowserName == "Chrome")
        {
            // ���Ҿ����ض� ID ����
            itemToModify = ChromeItems.FirstOrDefault(item => item.Id == itemId);
        }
        else
        {
            itemToModify = EdgeItems.FirstOrDefault(item => item.Id == itemId);
        }
        if (itemToModify != null)
        {
            // �޸��������
            itemToModify.Port = newPort;
            //Console.WriteLine($"Edge item {itemName} �������Ѹ���Ϊ {newName}");

            // �����޸ĺ������
            await SaveDataAsync();
        }
        else
        {
            //Console.WriteLine($"δ�ҵ� ID Ϊ {itemName} �� Edge item");
        }
    }




    public async Task AddEdgeItemAsync(string name, string description, string BrowserName)
    {
        try
        {
            // ����һ���µ� Item ����
            Item newItem = new Item
            {
                Name = name,
                Description = description
            };

            // ������ EdgeItems ����
            if (BrowserName == "Chrome")
            {
                ChromeItems.Add(newItem);
            }
            else {
                EdgeItems.Add(newItem); 
            }
            Console.WriteLine($"�������� EdgeItems: Name={newItem.Name}, Description={newItem.Description}");

            // �����޸ĺ������
            await SaveDataAsync();
        }
        catch (Exception ex)
        {
            // �����������ʱ���쳣
            Console.WriteLine($"Failed to add edge item: {ex.Message}");
        }
    }
    public void DeleteItemById(int id, string BrowserName)
    {
        if(BrowserName == "Chrome") {
            var ChromeItem = ChromeItems.FirstOrDefault(item => item.Id == id);
            if (ChromeItem != null)
            {
                ChromeItems.Remove(ChromeItem);
            }
        }
        else
        {
            var edgeItem = EdgeItems.FirstOrDefault(item => item.Id == id);
            if (edgeItem != null)
            {
                EdgeItems.Remove(edgeItem);
            }
        }

        // �� EdgeItems �в��Ҳ�ɾ��
        
        

        // �� ChromeItems �в��Ҳ�ɾ��
        /*var chromeItem = ChromeItems.FirstOrDefault(item => item.Name == name);
        if (chromeItem != null)
        {
            ChromeItems.Remove(chromeItem);
        }*/
    }
    public bool Findid(int itemId, string BrowserName)
    {
        Item itemToModify;
        if (BrowserName == "Chrome")
        {
            // ���Ҿ����ض� ID ����
            itemToModify = ChromeItems.FirstOrDefault(item => item.Id == itemId);
        }
        else
        {
            itemToModify = EdgeItems.FirstOrDefault(item => item.Id == itemId);
        }
        if (itemToModify != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool FindName(string itemName, string BrowserName)
    {
        Item itemToModify;
        if (BrowserName == "Chrome")
        {
            // ���Ҿ����ض� ID ����
            itemToModify = ChromeItems.FirstOrDefault(item => item.Name == itemName);
        }
        else
        {
            itemToModify = EdgeItems.FirstOrDefault(item => item.Name == itemName);
        }
        if (itemToModify != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}



namespace BrowserLauncherApp



{
    public class Item : INotifyPropertyChanged
    {
        private string name;
        private string description;
        private bool isSelected;
        private bool isEditingName;
        private bool isEditingDescription;
        private bool isPortfixed;
        private int port;
        private int id;


        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                if (description != value)
                {
                    description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public bool IsEditingName
        {
            get => isEditingName;
            set
            {
                isEditingName = value;
                OnPropertyChanged(nameof(IsEditingName));
            }
        }

        public bool IsEditingDescription
        {
            get => isEditingDescription;
            set
            {
                isEditingDescription = value;
                OnPropertyChanged(nameof(IsEditingDescription));
            }
        }

        public bool IsPortfixed
        {
            get => isPortfixed;
            set
            {
                isPortfixed = value;
                OnPropertyChanged(nameof(IsPortfixed));
            }
        }
        public int Port
        {
            get => port;
            set
            {
                port = value;
                OnPropertyChanged(nameof(Port));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int Id
        {
            get => id;
            set {
                id = value;
            }
        }
    }

    


    public sealed partial class ProcessesPage : Page
    {
        public ObservableCollection<Website> Websites { get; set; }
        private const string WebsiteDataFilePath = "websites.json";
        private DataManager dataManager = new DataManager();
        public ObservableCollection<Item> Items { get; set; }
        public ObservableCollection<Item> EdgeItems { get; set; }
        public ObservableCollection<Item> ChromeItems { get; set; }
        bool EdgeOP = true;
        bool ChromeOP = false;
        string url = "";
        bool ChromeSelectAll = false; 
        bool EdgeSelectAll = false;

    public ProcessesPage()
        
        {
            this.InitializeComponent();
            LoadData();
            Websites = new ObservableCollection<Website>();
            LoadWebsites();
            InitializeComboBox();
            
        }
        
            private void LoadData()
        {

            _ = dataManager.LoadDataAsync();
            // �������ݵ� UI �ؼ��У����� ListView
            EdgeItems = dataManager.EdgeItems;
         
            ChromeItems = dataManager.ChromeItems;
            
            
            Items = EdgeItems;
            
        // �����ݰ󶨵�ListView
        myListView.ItemsSource = Items;
            
        UpdateSelectionCount();
        }

        private async void SaveData()
        {
            await dataManager.SaveDataAsync();
        }

        protected override void OnNavigatedFrom(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            
            foreach (var item in EdgeItems)
            {
                item.IsSelected = false;
            }
            foreach (var item in ChromeItems)
            {
                item.IsSelected = false;
            }
            SelectAllBox.IsChecked = false;
            SaveData();
            base.OnNavigatedFrom(e);
        }

        private void NameTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is TextBlock textBlock && textBlock.DataContext is Item item)
            {
                item.IsEditingName = true;
            }
        }

        private void NameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is Item item)
            {
                item.IsEditingName = false;
            }
        }

        private void DescriptionTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is TextBlock textBlock && textBlock.DataContext is Item item)
            {
                item.IsEditingDescription = true;
            }
        }

        private void DescriptionTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is Item item)
            {
                item.IsEditingDescription = false;
            }
        }

        private void OnPerformActionClick(object sender, RoutedEventArgs e)
        {
            var selectedItems = Items.Where(item => item.IsSelected).ToList();
            if (selectedItems.Any())
            {
                // ������ִ�ж�ѡ����Ĳ���
                foreach (var item in selectedItems)
                {
                    // ʾ�����������ѡ���������
                    System.Diagnostics.Debug.WriteLine($"Selected Item: {item.Name}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No items selected.");
            }
            UpdateSelectionCount();
        }

        private void OnSelectFiveItemsClick(object sender, RoutedEventArgs e)
        {
            // ��ȫ���ͼ���ת��
            if (myListView.ItemsSource is IEnumerable<Item> items)
            {
                var itemList = items.ToList();
                int j = 0;
                for (int i = 0; j < 5 && i < itemList.Count; i++)
                {
                    if(itemList[i].IsSelected == false) {
                        itemList[i].IsSelected = true;
                        j++;
                    }
                    
                }
            }
            else
            {
                // ����ItemsSource����Ԥ�����͵����
                // ������������־��������߼�
            }
            UpdateSelectionCount();
        }

        private void UpdateSelectionCount()
        {
            
                int selectedCount = Items.Count(item => item.IsSelected);
                selectionCountTextBlock.Text = $" {selectedCount}";
            
        }

        private void OnSelectAllItemsClick(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("�ѵ��ȫѡ��ť");
            UpdateSelectionCount();

        }

        private void ChromeOptionButtonEntered(object sender, PointerRoutedEventArgs e)
        {
            if (ChromeOP == false)
            {
                ChromeOptionButton.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }
        
        private void ChromeOptionButtonExited(object sender, PointerRoutedEventArgs e)
        {
            if(ChromeOP == false)
            { ChromeOptionButton.Foreground = new SolidColorBrush(Colors.LightGray); }

        }
        
        private void ChromeOptionButtonPressed(object sender, PointerRoutedEventArgs e)
        {
            Items = ChromeItems;
            // �����ݰ󶨵�ListView
            myListView.ItemsSource = ChromeItems;
            EdgeOptionButton.Foreground = new SolidColorBrush(Colors.LightGray);
            ChromeOptionButton.Foreground = new SolidColorBrush(Colors.Black);
            
            EdgeOP = false;
            ChromeOP = true;
            if (ChromeSelectAll == true)
            {
                SelectAllBox.IsChecked = true;
            }
            else
            {
                SelectAllBox.IsChecked = false;
            }
            UpdateSelectionCount();


        }

        private void EdgeOptionButtonEntered(object sender, PointerRoutedEventArgs e)
        {
            if (EdgeOP == false)
            {
                EdgeOptionButton.Foreground = new SolidColorBrush(Colors.Gray);
            }

        }

        private void EdgeOptionButtonExited(object sender, PointerRoutedEventArgs e)
        {
            if (EdgeOP == false) { 
            EdgeOptionButton.Foreground = new SolidColorBrush(Colors.LightGray);
            }
        }

        private void EdgeOptionButtonPressed(object sender, PointerRoutedEventArgs e)
        {
            Items = EdgeItems;
            // �����ݰ󶨵�ListView
            myListView.ItemsSource = Items;
            ChromeOptionButton.Foreground = new SolidColorBrush(Colors.LightGray);
            EdgeOptionButton.Foreground = new SolidColorBrush(Colors.Black);
            
            EdgeOP = true;
            ChromeOP = false;
            if (EdgeSelectAll == true)
            {
                SelectAllBox.IsChecked = true;
            }
            else
            {
                SelectAllBox.IsChecked = false;
            }
            UpdateSelectionCount();

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (DataContext is MyViewModel viewModel)
            {

                viewModel.IsSelected = true;
               
            }
           
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            
            
            UpdateSelectionCount();


        }
        
        private void CheckBox_Clicked(object sender, RoutedEventArgs e)
        {
            UpdateSelectionCount();
        }

        private void nameBoxPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // ��ȡ������� TextBlock
            TextBlock textBlock = sender as TextBlock;

                var properties = e.GetCurrentPoint(sender as UIElement).Properties;
                if (properties.IsRightButtonPressed)
                {
                // �Ҽ������
                // �������������߼�
            }
            else {
                if (textBlock != null)
                {
                    // �ҵ���Ӧ�� Item
                    Item item = textBlock.DataContext as Item;
                    if (item != null)
                    {
                        // ���� TextBlock����ʾ TextBox������ TextBox ��ý���
                        textBlock.Visibility = Visibility.Collapsed;
                        TextBox textBox = FindVisualChild<TextBox>(textBlock.Parent as Grid);
                        if (textBox != null)
                        {
                            textBox.Visibility = Visibility.Visible;
                            textBox.Focus(FocusState.Programmatic);
                        }
                    }
                }
            }
            

            
        }

        private void nameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // ��ȡʧȥ����� TextBox
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text != "" && !dataManager.FindName(textBox.Text, ChromeOP == true ? "Chrome" : "Edge"))
            {
                // �ҵ���Ӧ�� TextBlock
                TextBlock textBlock = FindVisualChild2<TextBlock>(textBox.Parent as UIElement, "nameBlock");
                var originalSource = e.OriginalSource as FrameworkElement;
                Item item = originalSource.DataContext as Item;
                int id = item.Id;
                if (textBlock != null)
                {
                    // ���� TextBox����ʾ TextBlock�������°󶨵� Name ����
                    textBox.Visibility = Visibility.Collapsed;
                    textBlock.Visibility = Visibility.Visible;
                    bool sus = false;

                    string dirPath;
                    if (ChromeOP == true)
                    {
                        dirPath = LoadSettings("ChromedPath");

                        // ��ȡĿ���ļ���·��
                    }
                    else
                    {
                        dirPath = LoadSettings("EdgedPath");
                    }
                    if (!string.IsNullOrEmpty(dirPath) && Directory.Exists(dirPath))
                    {
                        string oldFolderPath = Path.Combine(dirPath, textBlock.Text);
                        string newFolderPath = Path.Combine(dirPath, textBox.Text);

                        // ȷ��Ŀ���ļ��д����Ҳ��������Ƴ�ͻ
                        try
                        {
                            if (Directory.Exists(oldFolderPath) && !Directory.Exists(newFolderPath))
                            {
                                // �����������ļ���
                                Directory.Move(oldFolderPath, newFolderPath);
                                Console.WriteLine($"Folder renamed from {oldFolderPath} to {newFolderPath}.");
                                sus = true;
                            }
                            else
                            {
                                if (!Directory.Exists(oldFolderPath))
                                {
                                    Console.WriteLine($"The old folder path {oldFolderPath} does not exist.");
                                    sus = true;
                                }
                                else if (Directory.Exists(newFolderPath))
                                {
                                    Console.WriteLine($"The new folder path {newFolderPath} already exists.");
                                }
                            }
                        }
                        catch (IOException ioEx)
                        {
                            Console.WriteLine($"IOException: The folder could not be renamed. It might be in use. Details: {ioEx.Message}");
                        }
                        catch (UnauthorizedAccessException uaEx)
                        {
                            Console.WriteLine($"UnauthorizedAccessException: You do not have permission to rename this folder. Details: {uaEx.Message}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                        }


                    }

                    if (sus == true)
                    {
                        BindingExpression binding = textBox.GetBindingExpression(TextBox.TextProperty);
                        binding?.UpdateSource();
                        _ = dataManager.ModifyEdgeItemAsyncName(id, newName: textBox.Text, ChromeOP == true ? "Chrome" : "Edge");
                        _ = dataManager.SaveDataAsync();


                        textBlock.Text = textBox.Text;
                    }
                    else {
                        textBox.Text = textBlock.Text;
                    } 
                }
                


            }
            else
            {
                TextBlock textBlock = FindVisualChild2<TextBlock>(textBox.Parent as UIElement, "nameBlock");
                textBox.Text = textBlock.Text;
                textBox.Visibility = Visibility.Collapsed;
                textBlock.Visibility = Visibility.Visible;
            }
        }

        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }

        private T FindVisualChild2<T>(DependencyObject parent, string childName) where T : FrameworkElement
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

                var childOfChild = FindVisualChild2<T>(child, childName);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }

            return null;
        }

        private void myListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            // Find the ListViewItem that was right-clicked
            var listView = sender as ListView;
            var item = (e.OriginalSource as FrameworkElement)?.DataContext as Item;

            if (item != null)
            {
                // Set the selected item
                listView.SelectedItem = item;

                // Show the context menu
               
            }
        }

        private void DescriptionBoxPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // ��ȡ������� TextBlock
            TextBlock textBlock = sender as TextBlock;

            var properties = e.GetCurrentPoint(sender as UIElement).Properties;
            if (properties.IsRightButtonPressed)
            {
                // �Ҽ������
                // �������������߼���������ʾ�����Ĳ˵���
            }
            else
            {
                if (textBlock != null)
                {
                    // �ҵ���Ӧ�� Item
                    Item item = textBlock.DataContext as Item;
                    if (item != null)
                    {
                        // ���� TextBlock����ʾ TextBox������ TextBox ��ý���
                        
                        TextBox textBox = FindVisualChild2<TextBox>(textBlock.Parent as UIElement, "describeBox");
                        if (textBox != null)
                        {
                            textBlock.Visibility = Visibility.Collapsed;
                            textBox.Visibility = Visibility.Visible;
                            textBox.Focus(FocusState.Programmatic);
                        }
                    }
                }
            }
        }

        private void describeBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // ��ȡʧȥ����� TextBox
            TextBox textBox = sender as TextBox;
            var originalSource = e.OriginalSource as FrameworkElement;
            Item  item = originalSource.DataContext as Item;
            int id = item.Id;
            if (textBox != null)
            {
                // �ҵ���Ӧ�� TextBlock
                TextBlock textBlock = FindVisualChild2<TextBlock>(textBox.Parent as UIElement,"describeBlock");
                if (textBlock != null)
                {
                    // ���� TextBox����ʾ TextBlock�������°󶨵� Name ����
                    textBox.Visibility = Visibility.Collapsed;
                    textBlock.Visibility = Visibility.Visible;
                    BindingExpression binding = textBox.GetBindingExpression(TextBox.TextProperty);
                    binding?.UpdateSource();
                    _ = dataManager.ModifyEdgeItemAsyncDis(id,  textBox.Text, ChromeOP == true ? "Chrome" : "Edge");
                    _ = dataManager.SaveDataAsync();
                    textBlock.Text = textBox.Text; // ȷ�����༭���ֵ���µ��󶨵�������
                }



            }
        }

        private void LaunchItem_Click(object sender, RoutedEventArgs e)
        {
            // Handle edit menu item click
            // Example: Show a dialog or navigate to edit page
        }

        private void CreateShortcut_Click(object sender, RoutedEventArgs e)
        {
            var menuFlyoutItem = sender as MenuFlyoutItem;
            if (menuFlyoutItem != null)
            {
                // ��ȡ ContextMenu �ĸ��� StackPanel
                var stackPanel = GetParentOfType<StackPanel>(menuFlyoutItem);
                if (stackPanel != null)
                {
                    // ��ȡ StackPanel �� DataContext
                    var item = stackPanel.DataContext as Item;
                    if (item != null)
                    {
                        int Port = item.Port; // ���� item.Path ��Ŀ��·��
                        string Name= item.Name; // ���� item.Name ���û������ļ���·��

                        CreateShortcut(Port, Name);
                    }
                }
            }
           
        }

        private void CreateShortcut(int Port, string Name)
        {
            string shortcutName = Port == 0 ? $"{Name}.lnk" : $"{Name}_{Port}.lnk";
            string shortcutLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), shortcutName);

            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

            shortcut.Description = "Shortcut for Edge with custom user data directory";

            if (EdgeOP == true)
            {
                string edgePath = LoadSettings("EdgePath");
                string dataPath = LoadSettings("EdgedPath"); // ��ȡĿ��·������ Edge ��ִ���ļ���·��
                string userDataDir = Path.Combine(dataPath, Name); // ƴ�� dataPath �� Name ������·��

                shortcut.TargetPath = edgePath;
                shortcut.Arguments = $"--user-data-dir=\"{userDataDir}\"";

                if (Port != 0)
                {
                    shortcut.Arguments += $" --port={Port}";
                }
            }
            else
            {
                string chromePath = LoadSettings("ChromePath");
                string dataPath = LoadSettings("ChromedPath");
                string userDataDir = Path.Combine(dataPath, Name);

                shortcut.TargetPath = chromePath;
                shortcut.Arguments = $"--user-data-dir=\"{userDataDir}\"";

                if (Port != 0)
                {
                    shortcut.Arguments += $" --port={Port}";
                }
            }

            shortcut.Save();
        }


        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            // ��ȡ MenuFlyoutItem �ĸ���
            var menuFlyoutItem = sender as MenuFlyoutItem;
            if (menuFlyoutItem != null)
            {
                // ��ȡ ContextMenu �ĸ��� StackPanel
                var stackPanel = GetParentOfType<StackPanel>(menuFlyoutItem);
                if (stackPanel != null)
                {
                    // ��ȡ StackPanel �� DataContext
                    var item = stackPanel.DataContext as Item;
                    if (item != null)
                    {
                        bool sus = false;
                        // ��ȡ TextBlock ����
                        var nameBlock = FindChild<TextBlock>(stackPanel, "nameBlock");
                        string dataPath;
                        if (ChromeOP == true)
                        {
                            dataPath = LoadSettings("chromedPath");
                        }
                        else
                        {
                            dataPath = LoadSettings("edgedPath");
                        }
                        string folderToDelete = Path.Combine(dataPath, item.Name);
                        try
                        {
                            if (Directory.Exists(folderToDelete))
                            {
                                Directory.Delete(folderToDelete, true);
                                Console.WriteLine($"Folder {folderToDelete} has been deleted.");
                                sus = true;
                            }
                            else
                            {
                                Console.WriteLine($"Folder {folderToDelete} does not exist.");
                                sus = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred while deleting the folder: {ex.Message}");
                            
                        }



                        if (sus == true)
                        {
                            //var name = nameBlock.Text;
                            dataManager.DeleteItemById(item.Id, ChromeOP == true ? "Chrome" : "Edge");
                            _ = dataManager.SaveDataAsync();
                            
                            UpdateSelectionCount();

                            // ������������������ɾ����Ŀ
                            DeleteItem(item);
                        }
                        
                    }
                }
                
            }
        }

        private void DeleteItem(Item item)
        {
            // ��������� item ����ɾ������
            // ����Ӽ������Ƴ���Ŀ
            
        }

        private T GetParentOfType<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;

            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return GetParentOfType<T>(parentObject);
            }
        }

        private T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                T childType = child as T;
                if (childType == null)
                {
                    foundChild = FindChild<T>(child, childName);
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
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
                var flyout = stackPanel.ContextFlyout as MenuFlyout;
                if (flyout != null)
                {
                    flyout.Closed += (s, args) =>
                    {
                        stackPanel.Background = Application.Current.Resources["ApplicationPageBackgroundThemeBrush"] as SolidColorBrush;
                    };
                }
                // ����Ҽ��˵�û�д򿪣���ָ���ʼ��ɫ
                if (flyout == null || !flyout.IsOpen)
                {
                    stackPanel.Background = Application.Current.Resources["ApplicationPageBackgroundThemeBrush"] as SolidColorBrush;
                }
            }
        }



        private void portBoxPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;

            var properties = e.GetCurrentPoint(sender as UIElement).Properties;
            if (properties.IsRightButtonPressed)
            {
                // �Ҽ������
                // �������������߼���������ʾ�����Ĳ˵���
            }
            else
            {
                if (textBlock != null)
                {
                    // �ҵ���Ӧ�� Item
                    Item item = textBlock.DataContext as Item;
                    if (item != null)
                    {
                        // ���� TextBlock����ʾ TextBox������ TextBox ��ý���

                        TextBox textBox = FindVisualChild2<TextBox>(textBlock.Parent as UIElement, "portBox");
                        if (textBox != null)
                        {
                            textBlock.Visibility = Visibility.Collapsed;
                            textBox.Visibility = Visibility.Visible;
                            textBox.Focus(FocusState.Programmatic);
                        }
                    }
                }
            }
        }


        private bool IsNumeric(string text)
        {
            Regex regex = new Regex(@"^\d+$"); // ֻ�������ֵ�������ʽ
            return regex.IsMatch(text);
        }
        private void portBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // ��ȡʧȥ����� TextBox
            TextBox textBox = sender as TextBox;
            var originalSource = e.OriginalSource as FrameworkElement;
            Item item = originalSource.DataContext as Item;
            int id = item.Id;
            if (textBox != null && IsNumeric(textBox.Text))
            {
                // �ҵ���Ӧ�� TextBlock
                TextBlock textBlock = FindVisualChild2<TextBlock>(textBox.Parent as UIElement, "portBlock");
                if (int.TryParse(textBox.Text, out int newPort))
                {
                    // ���� TextBox����ʾ TextBlock�������°󶨵� Name ����
                    textBox.Visibility = Visibility.Collapsed;
                    textBlock.Visibility = Visibility.Visible;
                    BindingExpression binding = textBox.GetBindingExpression(TextBox.TextProperty);
                    binding?.UpdateSource();
                    _ = dataManager.ModifyEdgeItemAsyncPort(id, newPort, ChromeOP == true ? "Chrome" : "Edge");
                    _ = dataManager.SaveDataAsync();
                    textBlock.Text = textBox.Text; // ȷ�����༭���ֵ���µ��󶨵�������
                }
            }
            else
            {
                TextBlock textBlock = FindVisualChild2<TextBlock>(textBox.Parent as UIElement, "portBlock");
                textBox.Visibility = Visibility.Collapsed;
                textBlock.Visibility = Visibility.Visible;
                textBox.Text = textBlock.Text;
            }
        }

       private void onAddClicked(object sender, RoutedEventArgs e)
        {
            
            int id = 0;
            for (int i = 1; i <= 99999; i++)
            {
                if (!dataManager.Findid(i, ChromeOP == true ? "Chrome" : "Edge"))
                {
                    id = i;
                    break;
                }
            }
            Items.Insert(0, new Item
            {
                Name = "Item " + id.ToString(),
                Port = 0,
                Id = id,
                Description = "Description",


            }) ;
            
            
        }

        private void OnLaunchItemsClick(object sender, RoutedEventArgs e)
        {
            var selectedItems = Items.Where(item => item.IsSelected).ToList();
            
            foreach (var selectedItem in selectedItems)
            {
                string itemName = selectedItem.Name;
                int itemPort = selectedItem.Port;
                string browser;
                if(ChromeOP == true)
                {
                    browser = "Chrome";
                }
                else
                {
                    browser = "Edge";
                }
                if(itemPort == 0)
                {
                    _ = LaunchItem(itemPort, itemName, browser, withPort: false,url);
                }
                else
                {
                    _ = LaunchItem(itemPort, itemName, browser, withPort: true,url);
                }
                        
                
            }
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

        public async Task LaunchItem(int port, string folderName, string browser, bool withPort = true, string url = null)
        {
            string chromeExe = LoadSettings("chromePath") ?? "No username saved.";
            string edgeExe = LoadSettings("edgePath") ?? "No username saved.";
            string exePath = browser == "Chrome" ? chromeExe : edgeExe;

            
            //string userDataDir = $@"C:\Users\{Environment.UserName}\AppData\Local\{browser}\{folderName}";

            string baseDir = browser == "Chrome" ? LoadSettings("chromedPath") ?? "No username saved." : LoadSettings("edgedPath") ?? "No username saved.";

            string defDir = Path.Combine(baseDir, "def");
            string userDataDir;

            if (!string.IsNullOrEmpty(folderName))
            {
                userDataDir = Path.Combine(baseDir, folderName);

                if (!Directory.Exists(userDataDir))
                {
                    Directory.CreateDirectory(userDataDir);

                    // ���� def �ļ������ݵ��´������ļ�����
                    CopyDirectory(defDir, userDataDir);
                }
            }
            else
            {
                // ��� folderName Ϊ�գ�ֱ��ʹ�� def �ļ���·��
                userDataDir = defDir;
            }

            string arguments;
            if (withPort)
            {
                arguments = $"--remote-debugging-port={port} --user-data-dir=\"{userDataDir}\"";
            }
            else
            {
                arguments = $"--user-data-dir=\"{userDataDir}\"";
            }
            if (url != null)
            {
                arguments += $" {url}";
            }
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            try
            {
                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.Start();

                    // Read the output asynchronously
                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync();

                    await process.WaitForExitAsync(); // Wait asynchronously for the process to exit

                    if (process.ExitCode != 0)
                    {
                        throw new InvalidOperationException($"Browser process exited with code {process.ExitCode}.\nError: {error}");
                    }
                    else
                    {
                        Console.WriteLine("Browser launched successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to launch browser: {ex.Message}");
                throw;
            }
        }
        private void CopyDirectory(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string targetFile = Path.Combine(targetDir, Path.GetFileName(file));
                System.IO.File.Copy(file, targetFile, true);
            }

            foreach (string directory in Directory.GetDirectories(sourceDir))
            {
                string targetDirectory = Path.Combine(targetDir, Path.GetFileName(directory));
                CopyDirectory(directory, targetDirectory);
            }
        }

        private void LoadWebsites()
        {
            try
            {
                if (System.IO.File.Exists(WebsiteDataFilePath))
                {
                    string json = System.IO.File.ReadAllText(WebsiteDataFilePath);
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
                    System.IO.File.WriteAllText(WebsiteDataFilePath, json);
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void InitializeComboBox()
        {
            // ��ӹ̶��ġ���ʼҳ��ѡ��
            BrowserComboBox.Items.Add(new Website { Name = "��ʼҳ", Url = null });

            // ��Ӵ�JSON�ļ���ȡ��ѡ��
            foreach (var website in Websites)
            {
                BrowserComboBox.Items.Add(website);
            }

            // ������ʾ����
            BrowserComboBox.DisplayMemberPath = "Name";
            BrowserComboBox.SelectedIndex = 0; // ����Ĭ��ѡ����Ϊ����ʼҳ��
        }

        public void BrowserComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BrowserComboBox.SelectedItem is Website selectedWebsite)
            {
                url = selectedWebsite.Url;
                
                // ������Ը�����Ҫ����ѡ���URL��������WebView�м���
            }
        }

        private void SelectAll_Checked(object sender, RoutedEventArgs e)
        {
            if (ChromeOP == false )
            {
                EdgeSelectAll = true;
                
            }
            else
            {
                ChromeSelectAll = true;
            }
            foreach (Item i in Items)
            {
                i.IsSelected = true;

            }
            UpdateSelectionCount();
        }
        private void SelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            if (EdgeOP == true)
            {
                EdgeSelectAll = false;
               
            }
            else
            {
                ChromeSelectAll = false;
            }
            foreach (Item i in Items)
            {
                i.IsSelected = false;

            }
            UpdateSelectionCount();
        }


        

    }

    

}
    
