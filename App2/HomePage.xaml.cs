using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Windowing;
using System;
using System.Diagnostics;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using System.Runtime.InteropServices;
using WinRT.Interop;
using Windows.UI.Popups;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using Microsoft.UI.Text;
using Windows.Storage;


namespace BrowserLauncherApp
{
    public sealed partial class HomePage : Page
    {

        //private AppWindow appWindow;
        private const string PortPlaceholderText = "0-65535 �ɲ���д";
        private const string FolderPlaceholderText = "��������Ĭ������";
        private const string URLPlaceholderText = "����������ʼҳ";
        public HomePage()

        {
            this.InitializeComponent();

            PortTextBox.Text = PortPlaceholderText;
            PortTextBox.Foreground = new SolidColorBrush(Colors.Gray);
            FolderTextBox.Text = FolderPlaceholderText;
            FolderTextBox.Foreground = new SolidColorBrush(Colors.Gray);
            URLTextBox.Text = URLPlaceholderText;
            URLTextBox.Foreground = new SolidColorBrush(Colors.Gray);
            LoadData();

            // ��ȡ��ǰ���ڵ� AppWindow ʵ��
            //appWindow = GetAppWindowForCurrentWindow();

            // ���ô��ڴ�С
            //this.InitializeWindow();
            //SetWindowSize(400, 400);


            // ��ѡ����չ���ݵ�������
            // this.ExtendsContentIntoTitleBar = true; 
            // this.SetTitleBar(AppTitleBar);
        }
        private void LoadData()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

            string userName = Environment.UserName;

            // Set default paths
            string defaultChromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
            string defaultEdgePath = @"C:\Program Files (x86)\Microsoft\Edge\Application";
            string defaultChromedPath = $@"C:\Users\{userName}\AppData\Local\Chrome";
            string defaultEdgedPath = $@"C:\Users\{userName}\AppData\Local\Edge";

            // Check and set ChromePath
            if (!localSettings.Values.ContainsKey("ChromePath"))
            {
                localSettings.Values["ChromePath"] = defaultChromePath;
            }

            // Check and set EdgePath
            if (!localSettings.Values.ContainsKey("EdgePath"))
            {
                localSettings.Values["EdgePath"] = defaultEdgePath;
            }

            // Check and set ChromedPath
            if (!localSettings.Values.ContainsKey("ChromedPath"))
            {
                localSettings.Values["ChromedPath"] = defaultChromedPath;
            }

            // Check and set EdgedPath
            if (!localSettings.Values.ContainsKey("EdgedPath"))
            {
                localSettings.Values["EdgedPath"] = defaultEdgedPath;
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

        private void BrowserComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            // ����Ĭ��ѡ�������Ϊ1�����ڶ���"Edge"
            BrowserComboBox.SelectedIndex = 1;
        }
        private AppWindow GetAppWindowForCurrentWindow()
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            return AppWindow.GetFromWindowId(windowId);
        }




        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Name == "PortTextBox" && textBox.Text == PortPlaceholderText)
            {
                textBox.Text = string.Empty;
                textBox.Foreground = new SolidColorBrush(Colors.Black);
            }
            else if (textBox.Name == "FolderTextBox" && textBox.Text == FolderPlaceholderText)
            {

                textBox.Text = string.Empty;
                textBox.Foreground = new SolidColorBrush(Colors.Black);
            }else if(textBox.Name == "URLTextBox" && textBox.Text == URLPlaceholderText)
            {
                textBox.Text = string.Empty;
                textBox.Foreground = new SolidColorBrush(Colors.Black);
            }


        }
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox.Name == "PortTextBox")
                {
                    textBox.Text = PortPlaceholderText;
                }
                else if (textBox.Name == "FolderTextBox")
                {
                    textBox.Text = FolderPlaceholderText;
                }else if(textBox.Name == "URLTextBox")
                {
                    textBox.Text = URLPlaceholderText;
                }

                textBox.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        /*
        private async Task ShowAlertMessage(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "����",
                Content = message,
                CloseButtonText = "ȷ��",
                XamlRoot = this.Content.XamlRoot
            };

            // ��ʾ�Ի��򲢵ȴ��û��ر�
            await dialog.ShowAsync();
        }*/

        private void CopyDirectory(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string targetFile = Path.Combine(targetDir, Path.GetFileName(file));
                File.Copy(file, targetFile, true);
            }

            foreach (string directory in Directory.GetDirectories(sourceDir))
            {
                string targetDirectory = Path.Combine(targetDir, Path.GetFileName(directory));
                CopyDirectory(directory, targetDirectory);
            }
        }


        public async Task LaunchBrowserAsync(int port, string folderName, string browser, bool withPort = true,string url = null)
        {
            string chromeExe = LoadSettings("chromePath") ?? "No username saved.";
            string edgeExe = LoadSettings("edgePath") ?? "No username saved.";
            string exePath = browser == "Chrome" ? chromeExe : edgeExe;

            if (string.IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentException("Folder name cannot be empty or null.", nameof(folderName));
            }
            if (!System.IO.File.Exists(exePath))
            {
                throw new InvalidOperationException($"{browser} executable not found at {exePath}");
            }
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



        private async void LaunchBrowserButton_Click(object sender, RoutedEventArgs e)
        {
            bool withPort = true;
            if (!int.TryParse(PortTextBox.Text, out int port))
            {
                withPort = false;
                //await LaunchBrowserAsync(port, folderName, browser);
                //await ShowAlertMessage("Please enter a valid port number.");
                //return;
            }

            string folderName = FolderTextBox.Text;
            string browser = (BrowserComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string url = URLTextBox.Text;

            if (folderName == FolderPlaceholderText)
            {
                folderName = "def";
                //await LaunchBrowserAsync(port, folderName, browser,withPort);
                // await ShowAlertMessage("Please enter a valid folder name.");
                //return;
            }

            if (string.IsNullOrWhiteSpace(browser))
            {
                //await ShowAlertMessage("Please select a browser.");
                return;
            }
            if (URLTextBox.Text == URLPlaceholderText)
            {
                url = null;
            }
            {
                
            }

            try
            {
                //BrowserLauncher launcher = new();
                await LaunchBrowserAsync(port, folderName, browser, withPort,url);
            }
            catch (Exception ex)
            {
                //await ShowAlertMessage($"Failed to launch browser: {ex.Message}");
            }
        }
    }
}
