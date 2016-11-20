using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AccountBook.Service;
using static AccountBook.Common.AccountBookCommon;

namespace AccountBook.WPF
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        private AccountBookService service = new AccountBookService();
        /// <summary>
        /// 构造函数
        /// </summary>
        public Login()
        {
            InitializeComponent();
            // 获取标签显示内容
            lblMessage.Content = service.GetLabelContent();

            // 获取程序运行根目录
            string appPath = AppDomain.CurrentDomain.BaseDirectory;

            // 头像初始化
            imgPortrait.Source = ReadPicture(appPath + @"Resource\Portrait\2121.jpg");
            imgPortrait.Stretch = Stretch.UniformToFill;
            imgPortrait.HorizontalAlignment = HorizontalAlignment.Center;
            imgPortrait.VerticalAlignment = VerticalAlignment.Center;

            // 背景初始化
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.Stretch = Stretch.UniformToFill;
            imageBrush.ImageSource = ReadPicture(appPath + @"Resource\BackgroudImage\login.jpg");
            this.grdLogin.Background = imageBrush;
        }

        /// <summary>
        /// 允许窗口通过其左键中的鼠标向下拖动在窗口的工作区中显示的区域
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            this.DragMove();
        }

        /// <summary>
        /// 登录按钮按下
        /// </summary>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            // 验证密码
            string pwd = txtPwd.Password;
            if (service.CheckPwdOK(pwd))
            {
                // 打开主界面
                Main win = new Main();
                win.Show();
                this.Close();
            }
            else if (string.IsNullOrEmpty(pwd))
            {
                lblMessage.Content = "请输入密码";
                lblMessage.Foreground = Brushes.Red;
            }
            else
            {
                lblMessage.Content = "密码错误，请重新输入";
                lblMessage.Foreground = Brushes.Red;
            };

        }

        /// <summary>
        /// 密码框回车键
        /// </summary>
        private void txtPwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.btnLogin_Click(sender, e);
            }
        }

        /// <summary>
        /// 重设标签内容
        /// </summary>
        private void txtPwd_GotFocus(object sender, RoutedEventArgs e)
        {
            lblMessage.Content = service.GetLabelContent();
            lblMessage.Foreground = Brushes.Black;
        }

        /// <summary>
        /// 最小化窗口
        /// </summary>
        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
