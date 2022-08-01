﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.IO;

namespace RobotChanger.Classes
{
    public struct FragmentText
    {
        public string Text;
        public uint Delay;

        public FragmentText(string text, uint delay = 0)
        {
            Text = text;
            Delay = delay;
        }
    }
    public static class Addition
    {
        // Path to directory with themes 
        public const string Themes = "Assets/Themes/";
        // Path to directory with assets 
        public const string Assets = "Assets";

        // Dubug Mode
        public const bool IsDebugMod = false;

        // NavigationService
        public static NavigationService NavigationService { get; } = (Application.Current.MainWindow as MainWindow)?.Frame.NavigationService;

        // Get files types
        public static readonly string[] Audio = { "wav", "m4a", "mp3", "aac", "flac" };
        public static readonly string[] Image = { "jpeg", "png", "jpg", "tiff", "bmp" };
        public static readonly string[] Video = { "mp4", "gif", "wmv", "avi" };
        public static readonly string[] Text = { "txt" };

        public static void ForEach<T>(this IEnumerable<T> lst, Action<T> action)
        {
            if (action == null)
                return;

            foreach (var item in lst)
            {
                action.Invoke(item);
            }
        }

        public static J Pop<T, J>(this Dictionary<T, J> dct, T key)
        {
            if (!dct.ContainsKey(key))
                return default;

            var item = dct[key];
            dct.Remove(key);

            return item;
        }

        public static T FindKey<T, J>(this Dictionary<T, J> dct, J val) =>
            dct.Keys.FirstOrDefault(key => dct[key].Equals(val));

        public static string RemoveLast(this string str, string key) => str.Remove(str.LastIndexOf(key, StringComparison.Ordinal));

        // Method to print list string to textblock with delay
        public static void PrintLines(TextBox element, Dispatcher dispatcher, ref bool working, Mutex mutex = default, params FragmentText[] TextArray)
        {
            foreach (var fragmentText in TextArray)
            {
                foreach (var symbol in fragmentText.Text)
                {
                    if (!working)
                        return;

                    mutex?.WaitOne();

                    dispatcher.BeginInvoke(DispatcherPriority.Background,
                    new Action(() =>
                    {
                        if (element.Text.Length > 0 && element.Text[element.Text.Length - 1].ToString() == ConfigManager.Config.SpecialSymbol)
                            element.Text = element.Text.Insert(element.Text.Length - 1, symbol.ToString());
                        else
                            element.Text += symbol.ToString();
                    }));

                    mutex?.ReleaseMutex();

                    if (fragmentText.Delay > 0)
                        Thread.Sleep((int)fragmentText.Delay);
                }
            }
        }

        // Get page file by filename
        //public static Page GetPageByFilename(string filename, string theme, bool clearPage = false)
        //{
        //    var exct = Path.GetExtension(filename).Remove(0, 1);
        //
        //    if (Audio.Contains(exct))
        //        return new AudioViewPage(filename, theme, clearPage);
        //
        //    if (Video.Contains(exct))
        //        return new VideoViewPage(filename, theme, clearPage);
        //
        //    if (Image.Contains(exct))
        //        return new PictureViewPage(filename, theme, clearPage);
        //
        //    if (Text.Contains(exct))
        //        return new TextViewPage(filename, theme, clearPage);
        //
        //    return default;
        //}
    }
}
