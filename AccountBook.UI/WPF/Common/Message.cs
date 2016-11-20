using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AccountBook.WPF
{
    public  class Message
    {
        /// <summary>
        /// 显示消息框
        /// </summary>
        /// <param name="message"></param>
        /// <param name="Owner">当前主窗体</param>
        /// <param name="flg">是否结束当前进程</param>
        public static void ShowMessage(string message, Window Owner = null,bool flg = false)
        {
            MessageBox win = new MessageBox();
            win.Owner = Owner;
            win.lblMessage.Content = message;
            win.ShowDialog();     
            if(flg)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
