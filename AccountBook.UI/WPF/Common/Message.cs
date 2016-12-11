using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AccountBook.WPF
{
    public class Message
    {
        /// <summary>
        /// 显示消息框
        /// </summary>
        /// <param name="message"></param>
        /// <param name="Owner">当前主窗体</param>
        /// <param name="shutdownFlg">是否结束当前进程</param>
        /// <param name="errorFlg">是否是警告信息</param>
        public static void ShowMessage(string message, Window Owner = null, bool shutdownFlg = false, bool errorFlg = false)
        {
            MessageBox win = new MessageBox();
            win.Owner = Owner;
            win.lblMessage.Content = errorFlg ? message + "！" : message;
            win.ShowDialog();
            if (shutdownFlg)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
