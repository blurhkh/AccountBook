using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

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
            //使用FileStream来写入数据
            if (!Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}Log"))
            {
                Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}Log");
            }
            string path = $"{AppDomain.CurrentDomain.BaseDirectory}Log\\{DateTime.Now.ToString("yyyyMMddHHmmss")}.log";
            using (FileStream fsWrite = new FileStream(path, FileMode.Append, FileAccess.Write))
            {
                //将字符 串转换成字节数组
                byte[] buffer = System.Text.Encoding.Default.GetBytes(txt);
                fsWrite.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
