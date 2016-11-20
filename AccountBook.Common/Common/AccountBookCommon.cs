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
        /// 将带逗号的金额转换为数字
        /// </summary>
        /// <param name="StrMoney">带逗号的金额</param>
        /// <returns>不带逗号的金额</returns>
        public static decimal ConvertMoney(string StrMoney)
        {
            StringBuilder sb = new StringBuilder();
            StrMoney.Split(',').ToList()
                .ForEach(x => sb.Append(x));

           return Convert.ToDecimal(sb.ToString()) ;
        }
    }
}
