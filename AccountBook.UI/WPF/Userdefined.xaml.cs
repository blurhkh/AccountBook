using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AccountBook.Service;
using static AccountBook.Common.AccountBookCommon;

namespace AccountBook.WPF
{
    /// <summary>
    /// Userdefined.xaml 的交互逻辑
    /// </summary>
    public partial class UserDefined : Window
    {
        private AccountBookService service = new AccountBookService();
        // 头像路径
        private string portraitPath;
        // 背景路径
        private string backgroudImagePath;
        // 获取程序运行根目录
        private string appPath = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// 是否为第一次登陆
        /// </summary>
        public bool IsFirstFlg { get; set; } = false;

        public UserDefined()
        {
            InitializeComponent();

            // 初始化文本框内容
            lblContent.Text = "点击设置登录时显示的文本内容";

            this.Title = "登录界面设置";

            // 显示画面设置
            Display();

        }

        /// <summary>
        /// 允许窗口通过其左键中的鼠标向下拖动在窗口的工作区中显示的区域
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            if (args.OriginalSource is Window)
            {
                this.DragMove();
            }
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

        /// <summary>
        /// 显示画面设置效果
        /// </summary>
        private void Display()
        {
            string path1;
            string path2;

            // 判断是否是加载更改后的图片
            if (portraitPath == null)
            {
                path1 = appPath + @"Resource\Portrait\2121.jpg";

            }
            else
            {
                path1 = portraitPath;
            }
            if (backgroudImagePath == null)
            {
                path2 = appPath + @"Resource\BackgroudImage\login.jpg";
            }
            else
            {
                path2 = backgroudImagePath;
            }

            // 头像初始化
            imgPortrait.Source = ReadPicture(path1);

            imgPortrait.Stretch = Stretch.UniformToFill;
            imgPortrait.HorizontalAlignment = HorizontalAlignment.Center;
            imgPortrait.VerticalAlignment = VerticalAlignment.Center;

            // 背景初始化
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.Stretch = Stretch.UniformToFill;
            imageBrush.ImageSource = ReadPicture(path2);
            this.grdLogin.Background = imageBrush;
        }

        /// <summary>
        /// 清除文本框内容
        /// </summary>
        private void lblContent_GotFocus(object sender, RoutedEventArgs e)
        {
            if (lblContent.Text == "点击设置登录时显示的文本内容")
            {
                lblContent.Text = string.Empty;
            }
        }

        /// <summary>
        /// 初始化文本框内容
        /// </summary>
        private void lblContent_LostFocus(object sender, RoutedEventArgs e)
        {
            if (lblContent.Text == string.Empty)
            {
                lblContent.Text = "点击设置登录时显示的文本内容";
            }
        }

        /// <summary>
        /// 更换头像
        /// </summary>
        private void imgPortrait_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog() { Filter = "图片文件 (*.jpg)|*.jpg" };
            fileDialog.Title = "请选择图片作为头像使用";
            fileDialog.ShowDialog();
            portraitPath = fileDialog.FileName == string.Empty ? portraitPath : fileDialog.FileName;
            if (fileDialog.CheckFileExists && !string.IsNullOrEmpty(fileDialog.FileName))
            {
                // 显示效果
                this.Display();
            }
        }

        /// <summary>
        /// 更换背景
        /// </summary>
        private void grdLogin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // 阻止事件冒泡
            if (!(e.OriginalSource is Grid))
            {
                return;
            }
            OpenFileDialog fileDialog = new OpenFileDialog() { Filter = "图片文件 (*.jpg)|*.jpg" };
            fileDialog.Title = "请选择图片作为背景使用";
            fileDialog.ShowDialog();
            backgroudImagePath = fileDialog.FileName == string.Empty ? backgroudImagePath : fileDialog.FileName;
            if (fileDialog.CheckFileExists && !string.IsNullOrEmpty(fileDialog.FileName))
            {
                // 显示效果
                this.Display();
            }
        }

        /// <summary>
        /// 确定设置
        /// </summary>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (this.portraitPath != null)
            {
                // 备份原头像
                File.Copy(appPath + @"Resource\Portrait\2121.jpg",
                    appPath + $"Resource\\Portrait\\2121{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg", true);
                // 设置头像
                File.Copy(this.portraitPath, appPath + @"Resource\Portrait\2121.jpg", true);
            }

            if (this.backgroudImagePath != null)
            {
                // 备份原背景
                File.Copy(appPath + @"Resource\BackgroudImage\login.jpg",
                    appPath + $"Resource\\BackgroudImage\\login{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg", true);
                // 设置背景
                File.Copy(this.backgroudImagePath, appPath + @"Resource\BackgroudImage\login.jpg", true);
            }

            if (lblContent.Text != "点击设置登录时显示的文本内容" && !string.IsNullOrEmpty(lblContent.Text))
            {
                service.SetLabelContent(lblContent.Text);
            }

            if (this.IsFirstFlg)
            {
                // 打开登录界面
                Login win = new Login();
                win.Show();
            }

            this.Close();
        }

        /// <summary>
        /// 取消按钮按下
        /// </summary>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.backgroudImagePath = null;
            this.portraitPath = null;
            // 显示原先的设置
            this.Display();
        }

    }
}
