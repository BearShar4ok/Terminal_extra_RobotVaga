using RobotChanger.Classes;
using RobotChanger.Frames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RobotChanger
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _theme;
        public MainWindow()
        {
            InitializeComponent();
            ConfigManager.Load();

            _theme = ConfigManager.Config.Theme;

            if (!Addition.IsDebugMod)
            {
                Topmost = true;
                Cursor = Cursors.None;
            }

            //KeyDown += (obj, e) =>
            //{
            //    if (e.Key == Key.Escape)
            //        Close();
            //};

            LoadTheme(_theme);
            LoadParams();
        }
        private void LoadTheme(string name)
        {
            Background = new ImageBrush() { ImageSource = new BitmapImage(new Uri(Addition.Themes + name + "/Background.png", UriKind.Relative)) };
        }
        private void LoadParams()
        {
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;
            ResizeMode = ResizeMode.NoResize;
            AllowsTransparency = true;
            Closing += (obj, e) => DevicesManager.StopListening();

            Addition.NavigationService?.Navigate(new LoadingPage(_theme));
        }
    }
}
