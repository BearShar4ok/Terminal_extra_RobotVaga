﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace RobotChanger.Classes
{
    public static class DevicesManager
    {
        public static event Action<string> AddDisk;
        public static event Action<string> RemoveDisk;

        private const int Delay = 1000;

        private static List<string> _disks = new List<string>();

        private static bool _isActive;
        private static Thread _thread;

        public static void StartListening()
        {
            _isActive = true;
            new Thread(Update).Start();
        }

        public static void StopListening() => _isActive = false;

        public static void ClearDisks() => _disks.Clear();

        private static void Update()
        {
            try
            {
                while (_isActive)
                {
                    var drives = DriveInfo.GetDrives();
                    var tempDisks = new List<string>();

                    foreach (var disk in drives)
                    {
                        tempDisks.Add(disk.Name);

                        if (_disks.Contains(disk.Name)) continue;

                        AddDisk?.Invoke(disk.Name);
                        _disks.Add(disk.Name);
                    }

                    for (var i = 0; i < _disks.Count; i++)
                    {
                        if (tempDisks.Contains(_disks[i])) continue;

                        RemoveDisk?.Invoke(_disks[i]);
                        _disks.RemoveAt(i);
                        i--;
                    }

                    Thread.Sleep(Delay);
                }
            }
            catch (Exception ex)
            {
                //Logger.Error(ex);
            }
        }
    }
}
