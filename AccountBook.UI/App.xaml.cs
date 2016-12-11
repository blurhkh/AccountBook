using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AccountBook.Service;
using System.Threading;
using AccountBook.WPF;
using AccountBook.Common;

namespace AccountBook
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private AccountBookService service = new AccountBookService();

        // 互斥锁
        private Mutex mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            // 确保单例运行
            bool flg;
            mutex = new Mutex(true, "AccountBook", out flg);

            if (!flg)
            {
                Message.ShowMessage("已有一个程序实例运行");
                // 关闭新打开的实例
                this.Shutdown();
                return;
            }

            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);

            if (service.IsFirst())
            {
                //如果是初次使用，则进入欢迎界面
                Application.Current.StartupUri = new Uri("WPF/ResetPwd.xaml", UriKind.Relative);
            }
            else
            {
                Application.Current.StartupUri = new Uri("WPF/Login.xaml", UriKind.Relative);
            }

        }

        /// <summary>
        /// 程序异常发生
        /// </summary>
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            AccountBookCommon.Log($"Message:{e.Exception.Message}\r\nSource:{e.Exception.Source}\r\nStackTrace:{e.Exception.StackTrace.TrimStart()}");
            e.Handled = true;
            Message.ShowMessage("程序发生异常,请联系开发人员", shutdownFlg: true, errorFlg: true);
            MessageBoxResult result = System.Windows.MessageBox.Show("重启软件", "请联系开发人员", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                AccountBookCommon.ReLogin();
            }         
        }
    }
}
