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

    public async Task ModifyEdgeItemAsyncName(string itemName, string newName)
    {
        // ���Ҿ����ض� ID ����
        var itemToModify = EdgeItems.FirstOrDefault(item => item.Name == itemName);
        if (itemToModify != null)
        {
            // �޸��������
            itemToModify.Name = newName;
            Console.WriteLine($"Edge item {itemName} �������Ѹ���Ϊ {newName}");

            // �����޸ĺ������
            await SaveDataAsync();
        }
        else
        {
            Console.WriteLine($"δ�ҵ� ID Ϊ {itemName} �� Edge item");
        }
    }

    public async Task ModifyEdgeItemAsyncDis(int itemId, string newDis)
    {
        // ���Ҿ����ض� ID ����
        var itemToModify = EdgeItems.FirstOrDefault(item => item.Id == itemId);
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




    public async Task AddEdgeItemAsync(string name, string description)
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
            EdgeItems.Add(newItem);
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
    public void DeleteItemByName(string name)
    {
        // �� EdgeItems �в��Ҳ�ɾ��
        var edgeItem = EdgeItems.FirstOrDefault(item => item.Name == name);
        if (edgeItem != null)
        {
            EdgeItems.Remove(edgeItem);
        }

        // �� ChromeItems �в��Ҳ�ɾ��
        /*var chromeItem = ChromeItems.FirstOrDefault(item => item.Name == name);
        if (chromeItem != null)
        {
            ChromeItems.Remove(chromeItem);
        }*/
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
        private string port;
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
        public string Port
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
        private DataManager dataManager = new DataManager();
        public ObservableCollection<Item> Items { get; set; }
        public ObservableCollection<Item> EdgeItems { get; set; }
        public ObservableCollection<Item> ChromeItems { get; set; }

        bool EdgeOP = true;
        bool ChromeOP = false;
        public ProcessesPage()

        {

            this.InitializeComponent();
            LoadData();
        }
        

        private void LoadData()
    {
            _ = dataManager.LoadDataAsync();

        // �������ݵ� UI �ؼ��У����� ListView
        EdgeItems = dataManager.EdgeItems;
            Console.WriteLine(EdgeItems);



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
            if (myListView.ItemsSource is IEnumerable<Item> items)
            {
                int selectedCount = items.Count(item => item.IsSelected);
                selectionCountTextBlock.Text = $" {selectedCount}";
            }
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
            if (textBox != null)
            {
                // �ҵ���Ӧ�� TextBlock
                TextBlock textBlock = FindVisualChild<TextBlock>(textBox.Parent as Grid);
                if (textBlock != null)
                {
                    // ���� TextBox����ʾ TextBlock�������°󶨵� Name ����
                    textBox.Visibility = Visibility.Collapsed;
                    textBlock.Visibility = Visibility.Visible;
                    BindingExpression binding = textBox.GetBindingExpression(TextBox.TextProperty);
                    binding?.UpdateSource();
                    _ = dataManager.ModifyEdgeItemAsyncName(textBlock.Text, newName: textBox.Text);
                    _ = dataManager.SaveDataAsync();
                    textBlock.Text = textBox.Text; // ȷ�����༭���ֵ���µ��󶨵�������
                }

                

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
                    _ = dataManager.ModifyEdgeItemAsyncDis(textBlock.Text,  textBox.Text);
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
            // Handle delete menu item click
            // Example: Prompt for deletion confirmation and remove item from collection
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
                        // ��ȡ TextBlock ����
                        var nameBlock = FindChild<TextBlock>(stackPanel, "nameBlock");


                        //var name = nameBlock.Text;
                            dataManager.DeleteItemByName(item.Name);
                            _ = dataManager.SaveDataAsync();
                            Items = dataManager.EdgeItems;
                            UpdateSelectionCount();

                            // ������������������ɾ����Ŀ
                            DeleteItem(item);
                        
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

        private void DeleteItem(string name)
        {
            // ��������� name ����ɾ������
            // ����Ӽ������Ƴ���Ŀ
            // yourCollection.Remove(item => item.Name == name);
        }

        /*
        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            
            Console.WriteLine("��ɾ��");
            // ��ȡ�����MenuFlyoutItem
            var menuFlyoutItem = sender as MenuFlyoutItem;

            // ��ȡMenuFlyoutItem�ĸ�����������MenuFlyout
            var menuFlyout = menuFlyoutItem?.Parent as MenuFlyout;

            // ��ȡMenuFlyout��Ŀ��Ԫ�أ�������ListViewItem
            var listViewItem = menuFlyout?.Target as ListViewItem;

            if (listViewItem != null)
            {
                // ��ȡListViewItem��Ӧ��������
                var item = listViewItem.Content as Item;

                // ��ȡListView��ItemsSource��������ObservableCollection<Item>
                //var items = myListView.ItemsSource as ObservableCollection<Item>;

                
                    // �Ӽ������Ƴ�ѡ�е���
                    Items.Remove(item);
                    _ = dataManager.ModifyEdgeItemAsync("Item 1", newName: "Item 2");
                    _ = dataManager.SaveDataAsync();
                    Items = dataManager.EdgeItems;
                    UpdateSelectionCount();
                    Console.WriteLine("��ɾ��");
                    
                
            }
        }*/

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
                stackPanel.Background = Application.Current.Resources["ApplicationPageBackgroundThemeBrush"] as SolidColorBrush;
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

        private void portBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // ��ȡʧȥ����� TextBox
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                // �ҵ���Ӧ�� TextBlock
                TextBlock textBlock = FindVisualChild2<TextBlock>(textBox.Parent as UIElement, "portBlock");
                if (textBlock != null)
                {
                    // ���� TextBox����ʾ TextBlock�������°󶨵� Name ����
                    textBox.Visibility = Visibility.Collapsed;
                    textBlock.Visibility = Visibility.Visible;
                    BindingExpression binding = textBox.GetBindingExpression(TextBox.TextProperty);
                    binding?.UpdateSource();
                    _ = dataManager.ModifyEdgeItemAsyncDis(textBlock.Text, textBox.Text);
                    _ = dataManager.SaveDataAsync();
                    textBlock.Text = textBox.Text; // ȷ�����༭���ֵ���µ��󶨵�������
                }



            }
        }

       private void onAddClicked(object sender, RoutedEventArgs e)
        {

        }

    }

    


    /*
    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        // ��ѡ��ѡ��ʱִ�еĲ���
        myCheckBox.Content = "Terms and Conditions Accepted";
    }

    private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        // ��ѡ��ȡ��ѡ��ʱִ�еĲ���
        myCheckBox.Content = "Accept Terms and Conditions";
    }*/


}
    
