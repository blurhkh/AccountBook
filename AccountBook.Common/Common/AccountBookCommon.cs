﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;

namespace AccountBook.Common
{
    public static class AccountBookCommon
    {
        /// <summary>
        /// 读取图片并释放资源
        /// </summary>
        /// <param name="path">图片路径</param>
        /// <returns></returns>
        public static BitmapImage ReadPicture(string path)
        {
            BitmapImage bitmap = new BitmapImage();

            if (File.Exists(path))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;

                using (Stream ms = new MemoryStream(File.ReadAllBytes(path)))
                {
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                    bitmap.Freeze();
                }
            }

            return bitmap;
        }

        /// <summary>
        /// 重启软件
        /// </summary>
        public static void ReLogin()
        {
            System.Reflection.Assembly.GetEntryAssembly();
            string startpath = System.IO.Directory.GetCurrentDirectory();
            System.Diagnostics.Process.Start(startpath + "/AccountBook.exe");
        }

        /// <summary>
        /// 输出日志文件
        /// </summary>
        public static void Log(string txt)
        {
            if (!Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}Log"))
            {
                Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}Log");
            }
            string path = $"{AppDomain.CurrentDomain.BaseDirectory}Log\\{DateTime.Now.ToString("yyyyMMddHHmmss")}.log";
            File.WriteAllText(path, txt);
        }

        /// <summary>
        /// 返回线程标识符
        /// </summary>
        /// <param name="hwnd">指定句柄</param>
        /// <param name="ID">线程的标识符</param>
        /// <returns></returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
    }
}
