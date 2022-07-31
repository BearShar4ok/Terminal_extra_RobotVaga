using RobotChanger.Classes;
using RobotChanger.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace RobotChanger.Frames
{
    /// <summary>
    /// Логика взаимодействия для TextPage.xaml
    /// </summary>
    public partial class TextPage : Page
    {
        private string _filename;
        private string _theme;
        private bool _update;
        private Mutex _mutex = new Mutex();
        public static RoutedCommand SaveCommand = new RoutedCommand();

        public TextPage(string filename, string theme)
        {
            InitializeComponent();

            LoadTheme(theme);
            LoadParams();

            _filename = filename;
            _theme = theme;
            Output.Text = ConfigManager.Config.SpecialSymbol;

            KeyDown += AdditionalKeys;

            LoadText();
            Output.Focus();
            SaveCommand.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
        }

        public void Closing()
        {
            _update = false;
            // KeyDown -= AdditionalKeys;
        }

        //public void Reload()
        //{
        //    ConfigManager.Load();
        //    LoadParams();
        //    LoadTheme(_theme);
        //
        //    _update = false;
        //
        //    _mutex?.WaitOne();
        //    Output.Text = ConfigManager.Config.SpecialSymbol;
        //    _mutex?.ReleaseMutex();
        //
        //    LoadText();
        //}

        private void LoadText()
        {
            if (!File.Exists(_filename))
                return;

            _update = true;

            new Thread(() =>
            {
                using (var stream = File.OpenText(_filename))
                {
                    var text = stream.ReadToEnd();

                    Addition.PrintLines(Output, Dispatcher, ref _update, _mutex,
                        new FragmentText(text,
                            ConfigManager.Config.UsingDelayFastOutput ? ConfigManager.Config.DelayFastOutput : 0));
                    UpdateCarriage();
                }
            }).Start();
        }

        private void LoadTheme(string theme)
        {
            Output.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), Addition.Themes + theme + "/#" + ConfigManager.Config.FontName);
        }

        private void LoadParams()
        {
            Output.FontSize = ConfigManager.Config.FontSize;
            Output.Opacity = ConfigManager.Config.Opacity;
            Output.Foreground = (Brush)new BrushConverter().ConvertFromString(ConfigManager.Config.TerminalColor);
            Output.AcceptsReturn = true;
            LoadingPage.DiskRemoved += CloseFile;
        }

        private void UpdateCarriage()
        {
            new Thread(() =>
            {
                while (_update)
                {
                    _mutex?.WaitOne();

                    Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    new Action(() =>
                    {
                        if (Output.Text.Length > 0 && Output.Text[Output.Text.Length - 1].ToString() == ConfigManager.Config.SpecialSymbol)
                        {
                            Output.Text = Output.Text.Remove(Output.Text.Length - 1);
                            Output.CaretIndex = Output.Text.Length;
                        }
                        else
                        {
                            Output.Text += ConfigManager.Config.SpecialSymbol;
                            Output.CaretIndex = Output.Text.Length - 1;
                        }


                    }));

                    _mutex?.ReleaseMutex();

                    Thread.Sleep((int)ConfigManager.Config.DelayUpdateCarriage);

                }
            }).Start();
        }

        private void AdditionalKeys(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Closing();
                    Addition.NavigationService.GoBack();
                    break;
            }
        }
        private void SaveFile(object sender, ExecutedRoutedEventArgs e)
        {
            if (!File.Exists(_filename))
            {
                var aw = new AlertWindow("Уведомление", "Фыйл не обнаружен...", "Закрыть", _theme);
                if (aw.ShowDialog()==false)
                {
                    Closing();
                    Addition.NavigationService.GoBack();
                    return;
                }
            }

            if (Output.Text.Length > 0 && Output.Text[Output.Text.Length - 1].ToString() == ConfigManager.Config.SpecialSymbol)
                Output.Text = Output.Text.Remove(Output.Text.Length - 1);
            File.WriteAllText(_filename, Output.Text);

            new AlertWindow("Уведомление", "Фыйл сохранен.", "Закрыть", _theme).Show();
        }
        private void CloseFile()
        {
            var aw = new AlertWindow("Уведомление", "Фыйл не обнаружен...", "Закрыть", _theme);
            if (aw.ShowDialog() == false)
            {
                Closing();
                Addition.NavigationService.GoBack();
                return;
            }
        }
    }
}
