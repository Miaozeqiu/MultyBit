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

    // 加载数据
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
                    // 加载数据到 ObservableCollection
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
            // 如果文件不存在，可以创建一个新的保存数据的文件
            await SaveDataAsync();
        }
        catch (Exception ex)
        {
            // 处理加载数据时的异常
            Console.WriteLine($"Failed to load data: {ex.Message}");
        }
    }

    // 保存数据
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
            // 处理保存数据时的异常
            Console.WriteLine($"Failed to save data: {ex.Message}");
        }
    }

    // 用于保存和加载的辅助类
    private class SavedData
    {
        public List<Item> EdgeItems { get; set; }
        public List<Item> ChromeItems { get; set; }
    }

    public async Task ModifyEdgeItemAsyncName(string itemName, string newName)
    {
        // 查找具有特定 ID 的项
        var itemToModify = EdgeItems.FirstOrDefault(item => item.Name == itemName);
        if (itemToModify != null)
        {
            // 修改项的属性
            itemToModify.Name = newName;
            Console.WriteLine($"Edge item {itemName} 的名称已更新为 {newName}");

            // 保存修改后的数据
            await SaveDataAsync();
        }
        else
        {
            Console.WriteLine($"未找到 ID 为 {itemName} 的 Edge item");
        }
    }

    public async Task ModifyEdgeItemAsyncDis(int itemId, string newDis)
    {
        // 查找具有特定 ID 的项
        var itemToModify = EdgeItems.FirstOrDefault(item => item.Id == itemId);
        if (itemToModify != null)
        {
            // 修改项的属性
            itemToModify.Description = newDis;
            //Console.WriteLine($"Edge item {itemName} 的名称已更新为 {newName}");

            // 保存修改后的数据
            await SaveDataAsync();
        }
        else
        {
            //Console.WriteLine($"未找到 ID 为 {itemName} 的 Edge item");
        }
    }




    public async Task AddEdgeItemAsync(string name, string description)
    {
        try
        {
            // 创建一个新的 Item 对象
            Item newItem = new Item
            {
                Name = name,
                Description = description
            };

            // 添加新项到 EdgeItems 集合
            EdgeItems.Add(newItem);
            Console.WriteLine($"已添加新项到 EdgeItems: Name={newItem.Name}, Description={newItem.Description}");

            // 保存修改后的数据
            await SaveDataAsync();
        }
        catch (Exception ex)
        {
            // 处理添加数据时的异常
            Console.WriteLine($"Failed to add edge item: {ex.Message}");
        }
    }
    public void DeleteItemByName(string name)
    {
        // 在 EdgeItems 中查找并删除
        var edgeItem = EdgeItems.FirstOrDefault(item => item.Name == name);
        if (edgeItem != null)
        {
            EdgeItems.Remove(edgeItem);
        }

        // 在 ChromeItems 中查找并删除
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

        // 加载数据到 UI 控件中，比如 ListView
        EdgeItems = dataManager.EdgeItems;
            Console.WriteLine(EdgeItems);



            ChromeItems = dataManager.ChromeItems;

        Items = EdgeItems;
        // 将数据绑定到ListView
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
                // 在这里执行对选中项的操作
                foreach (var item in selectedItems)
                {
                    // 示例操作：输出选中项的名称
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
            // 安全类型检查和转换
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
                // 处理ItemsSource不是预期类型的情况
                // 这里可以添加日志或错误处理逻辑
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
            Console.WriteLine("已点击全选按钮");
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
            // 将数据绑定到ListView
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
            // 将数据绑定到ListView
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
            // 获取被点击的 TextBlock
            TextBlock textBlock = sender as TextBlock;

                var properties = e.GetCurrentPoint(sender as UIElement).Properties;
                if (properties.IsRightButtonPressed)
                {
                // 右键被点击
                // 在这里添加你的逻辑
            }
            else {
                if (textBlock != null)
                {
                    // 找到对应的 Item
                    Item item = textBlock.DataContext as Item;
                    if (item != null)
                    {
                        // 隐藏 TextBlock，显示 TextBox，并让 TextBox 获得焦点
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
            // 获取失去焦点的 TextBox
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                // 找到对应的 TextBlock
                TextBlock textBlock = FindVisualChild<TextBlock>(textBox.Parent as Grid);
                if (textBlock != null)
                {
                    // 隐藏 TextBox，显示 TextBlock，并更新绑定的 Name 属性
                    textBox.Visibility = Visibility.Collapsed;
                    textBlock.Visibility = Visibility.Visible;
                    BindingExpression binding = textBox.GetBindingExpression(TextBox.TextProperty);
                    binding?.UpdateSource();
                    _ = dataManager.ModifyEdgeItemAsyncName(textBlock.Text, newName: textBox.Text);
                    _ = dataManager.SaveDataAsync();
                    textBlock.Text = textBox.Text; // 确保将编辑后的值更新到绑定的属性中
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
            // 获取被点击的 TextBlock
            TextBlock textBlock = sender as TextBlock;

            var properties = e.GetCurrentPoint(sender as UIElement).Properties;
            if (properties.IsRightButtonPressed)
            {
                // 右键被点击
                // 在这里添加你的逻辑，例如显示上下文菜单等
            }
            else
            {
                if (textBlock != null)
                {
                    // 找到对应的 Item
                    Item item = textBlock.DataContext as Item;
                    if (item != null)
                    {
                        // 隐藏 TextBlock，显示 TextBox，并让 TextBox 获得焦点
                        
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
            // 获取失去焦点的 TextBox
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                // 找到对应的 TextBlock
                TextBlock textBlock = FindVisualChild2<TextBlock>(textBox.Parent as UIElement,"describeBlock");
                if (textBlock != null)
                {
                    // 隐藏 TextBox，显示 TextBlock，并更新绑定的 Name 属性
                    textBox.Visibility = Visibility.Collapsed;
                    textBlock.Visibility = Visibility.Visible;
                    BindingExpression binding = textBox.GetBindingExpression(TextBox.TextProperty);
                    binding?.UpdateSource();
                    _ = dataManager.ModifyEdgeItemAsyncDis(textBlock.Text,  textBox.Text);
                    _ = dataManager.SaveDataAsync();
                    textBlock.Text = textBox.Text; // 确保将编辑后的值更新到绑定的属性中
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
            // 获取 MenuFlyoutItem 的父级
            var menuFlyoutItem = sender as MenuFlyoutItem;
            if (menuFlyoutItem != null)
            {
                // 获取 ContextMenu 的父级 StackPanel
                var stackPanel = GetParentOfType<StackPanel>(menuFlyoutItem);
                if (stackPanel != null)
                {
                    // 获取 StackPanel 的 DataContext
                    var item = stackPanel.DataContext as Item;
                    if (item != null)
                    {
                        // 获取 TextBlock 名字
                        var nameBlock = FindChild<TextBlock>(stackPanel, "nameBlock");


                        //var name = nameBlock.Text;
                            dataManager.DeleteItemByName(item.Name);
                            _ = dataManager.SaveDataAsync();
                            Items = dataManager.EdgeItems;
                            UpdateSelectionCount();

                            // 进行其他操作，例如删除项目
                            DeleteItem(item);
                        
                    }
                }
            }
        }

        private void DeleteItem(Item item)
        {
            // 在这里根据 item 进行删除操作
            // 例如从集合中移除项目
            
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
            // 在这里根据 name 进行删除操作
            // 例如从集合中移除项目
            // yourCollection.Remove(item => item.Name == name);
        }

        /*
        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            
            Console.WriteLine("已删除");
            // 获取点击的MenuFlyoutItem
            var menuFlyoutItem = sender as MenuFlyoutItem;

            // 获取MenuFlyoutItem的父对象，这里是MenuFlyout
            var menuFlyout = menuFlyoutItem?.Parent as MenuFlyout;

            // 获取MenuFlyout的目标元素，这里是ListViewItem
            var listViewItem = menuFlyout?.Target as ListViewItem;

            if (listViewItem != null)
            {
                // 获取ListViewItem对应的数据项
                var item = listViewItem.Content as Item;

                // 获取ListView的ItemsSource，假设是ObservableCollection<Item>
                //var items = myListView.ItemsSource as ObservableCollection<Item>;

                
                    // 从集合中移除选中的项
                    Items.Remove(item);
                    _ = dataManager.ModifyEdgeItemAsync("Item 1", newName: "Item 2");
                    _ = dataManager.SaveDataAsync();
                    Items = dataManager.EdgeItems;
                    UpdateSelectionCount();
                    Console.WriteLine("已删除");
                    
                
            }
        }*/

        private void StackPanel_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (sender is StackPanel stackPanel)
            {
                stackPanel.Background = new SolidColorBrush(Colors.LightGray); // 修改为你想要的颜色
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
                // 右键被点击
                // 在这里添加你的逻辑，例如显示上下文菜单等
            }
            else
            {
                if (textBlock != null)
                {
                    // 找到对应的 Item
                    Item item = textBlock.DataContext as Item;
                    if (item != null)
                    {
                        // 隐藏 TextBlock，显示 TextBox，并让 TextBox 获得焦点

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
            // 获取失去焦点的 TextBox
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                // 找到对应的 TextBlock
                TextBlock textBlock = FindVisualChild2<TextBlock>(textBox.Parent as UIElement, "portBlock");
                if (textBlock != null)
                {
                    // 隐藏 TextBox，显示 TextBlock，并更新绑定的 Name 属性
                    textBox.Visibility = Visibility.Collapsed;
                    textBlock.Visibility = Visibility.Visible;
                    BindingExpression binding = textBox.GetBindingExpression(TextBox.TextProperty);
                    binding?.UpdateSource();
                    _ = dataManager.ModifyEdgeItemAsyncDis(textBlock.Text, textBox.Text);
                    _ = dataManager.SaveDataAsync();
                    textBlock.Text = textBox.Text; // 确保将编辑后的值更新到绑定的属性中
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
        // 复选框被选中时执行的操作
        myCheckBox.Content = "Terms and Conditions Accepted";
    }

    private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        // 复选框取消选中时执行的操作
        myCheckBox.Content = "Accept Terms and Conditions";
    }*/


}
    
