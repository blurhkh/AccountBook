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
    }
}
