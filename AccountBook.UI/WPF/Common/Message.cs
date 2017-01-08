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
        public static void ShowMessage(string message, Window owner = null, bool shutdownFlg = false, bool errorFlg = false)
        {
            MessageBox win = new MessageBox();
            win.Owner = owner;
            win.lblMessage.Content = errorFlg ? message + "！" : message;
            win.ShowDialog();
            if (shutdownFlg)
            {
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// 显示确认框
        /// </summary>
        /// <param name="message"></param>
        public static bool ShowConfirmMessage(string message)
        {
            ConfirmBox win = new ConfirmBox();
            win.lblMessage.Content = message;
            win.ShowDialog();
            return win.ConfirmFlg;
        }
    }
}
