using RobotChanger.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Path = System.IO.Path;

namespace RobotChanger.Frames
{
    /// <summary>
    /// Логика взаимодействия для LoadingPage.xaml
    /// </summary>
    public partial class LoadingPage : Page
    {
        private const string AccessFileToReadDisk = "vaga.txt";
        private const string SystemFolder = "System Volume Information";

        private Uri imageLink;
        private string _theme;
        private KeyStates _prevkeyState;
        private string _currDisk;

        //private Dictionary<string, ListBoxItem> _disks = new Dictionary<string, ListBoxItem>();
        public LoadingPage(string theme)
        {
            InitializeComponent();

            // Add actions to devices
            DevicesManager.AddDisk += disk => AddDisk(disk);
            DevicesManager.RemoveDisk += RemoveDisk;

            _theme = theme;

            KeepAlive = true;

            LblInfo.Content = "Доступных дисков нет... Идет поиск...";

            LB.SelectionMode = SelectionMode.Single;
            LB.SelectedIndex = 0;
            LB.FocusVisualStyle = null;
            LB.Focus();
            KeyDown +=AdditionalKeys;

            LoadParams();
            LoadTheme();
            
            DevicesManager.StartListening();
        }
        private void LoadTheme()
        {
            LblInfo.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), Addition.Themes + _theme + "/#" + ConfigManager.Config.FontName);
            imageLink = new Uri(Path.GetFullPath(Addition.Themes + _theme + "/VagaImage.png"));
        }

        private void LoadParams()
        {
            LblInfo.FontSize = ConfigManager.Config.FontSize;
            LblInfo.Opacity = ConfigManager.Config.Opacity;
            LblInfo.Foreground = (Brush)new BrushConverter().ConvertFromString(ConfigManager.Config.TerminalColor);
        }
        private void AddDisk(string disk, bool addToList = true)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                try
                {

                    var allFiles = Directory.GetFiles(disk).Select(Path.GetFileName).ToArray();

                    if (!allFiles.Contains(AccessFileToReadDisk)) return;

                    var fullPath = File.ReadAllText(disk + AccessFileToReadDisk);
                    if (Directory.Exists(fullPath))
                    {
                        LblInfo.Content = "";
                        LblInfo.Visibility = Visibility.Hidden;
                        var diskName = Path.GetFileNameWithoutExtension(fullPath);

                        //var lbi = new ListBoxItem()
                        //{
                        //    DataContext = new BitmapImage(imageLink),
                        //    Content = diskName,
                        //    Tag = fullPath,
                        //    Style = (Style)App.Current.FindResource("ImageText"),
                        //    Foreground = (Brush)new BrushConverter().ConvertFrom(ConfigManager.Config.TerminalColor),
                        //    FontFamily = LblInfo.FontFamily,
                        //    FontSize = LblInfo.FontSize,
                        //
                        //};
                        Thread.Sleep(1000);
                        OpenFolder(fullPath);
                    }
                    Focus();
                }
                catch { }
            }));
        }

        private void RemoveDisk(string diskName)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {


                if (_currDisk == diskName)
                {
                    LB.SelectedIndex = 0;
                    _currDisk = null;
                    LB.Items.Clear();
                }
                if (LB.Items.Count == 0)
                {
                    LblInfo.Content = "Доступных дисков нет...";
                    LblInfo.Visibility = Visibility.Visible;
                }
            }));
        }
        //private void ExecuteFile()
        //{
        //
        //}
        private void OpenFolder(string directory)
        {
           // FindFolders(directory);
            FindFiles(directory);

            LB.SelectedIndex = 0;
            LB.Focus();
        }
        private void FindFiles(string directory)
        {
            var files = Directory.GetFiles(directory).ToList();
            var directories = Directory.GetDirectories(directory);

            //for (var i = 0; i < files.Count; i++)
            //{
            //    if (files[i].Contains(ExtensionConfig) && (files.Contains(files[i].RemoveLast(ExtensionConfig)) || directories.Contains(files[i].RemoveLast(ExtensionConfig))))
            //    {
            //        files.RemoveAt(i);
            //        i--;
            //    }
            //}

            foreach (var file in files)
            {
                var filename = Path.GetFileName(file);
                var name = Path.GetFileNameWithoutExtension(file);
                var extension = Path.GetExtension(file).Remove(0, 1);

                var lbi = new ListBoxItem()
                {
                    Content = name,
                    Tag = $@"{directory}\{filename}",
                    Style = (Style)App.Current.FindResource("ImageText"),
                    Foreground = (Brush)new BrushConverter().ConvertFrom(ConfigManager.Config.TerminalColor),
                    FontFamily = LblInfo.FontFamily,
                    FontSize = LblInfo.FontSize,
                    DataContext = new BitmapImage(imageLink),

                };

                LB.Items.Add(lbi);
            }
        }
        private void lstB_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var lbi = (ListBoxItem)LB.SelectedItem;
            if (lbi == null)
                return;

            var directory = lbi.Tag.ToString();

            Addition.NavigationService.Navigate(new TextPage(directory, _theme));
           
                //ExecuteFile(directory);
            
        }
        private void AdditionalKeys(object sender, KeyEventArgs e)
        {
            if (_prevkeyState == e.KeyStates) return;

            switch (e.Key)
            {
                case Key.Enter:
                    lstB_MouseDoubleClick(null, null);
                    break;
                case Key.Escape:
                    App.Current.MainWindow.Close();
                    break;
            }

            _prevkeyState = e.KeyStates;
        }
    }
}
