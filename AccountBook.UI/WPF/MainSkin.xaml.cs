using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using static AccountBook.Common.AccountBookCommon;

namespace AccountBook.WPF
{
    /// <summary>
    /// MainSkin.xaml 的交互逻辑
    /// </summary>
    public partial class MainSkin : Window
    {
        // 主窗体
        private Main main;
        // 背景路径
        private string backgroudImagePath;
        // 获取程序运行根目录
        private string appPath = AppDomain.CurrentDomain.BaseDirectory;

        public MainSkin(Main Owner)
        {
            InitializeComponent();
            main = Owner;

            btnSkinConfirm.Visibility = Visibility.Hidden;
            btnSkinCancel.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 显示画面设置效果
        /// </summary>
        private void Display()
        {
            string path;

            // 判断是否是加载更改后的图片
            if (backgroudImagePath == null)
            {
                path = appPath + @"Resource\BackgroudImage\main.jpg";
            }
            else
            {
                path = backgroudImagePath;
            }
            // 背景初始化
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.Stretch = Stretch.UniformToFill;
            imageBrush.ImageSource = ReadPicture(path);
            main.grdMain.Background = imageBrush;
            this.skinBrush.Background = imageBrush;
        }

        /// <summary>
        /// 允许窗口通过其左键中的鼠标向下拖动在窗口的工作区中显示的区域
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            this.DragMove();
        }

        /// <summary>
        /// 背景设置窗口自定义背景键按下
        /// </summary>
        private void btnSkinOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog() { Filter = "图片文件 (*.jpg)|*.jpg" };
            fileDialog.Title = "请选择图片作为背景使用";
            fileDialog.ShowDialog();
            backgroudImagePath = fileDialog.FileName == string.Empty ? backgroudImagePath : fileDialog.FileName;
            if (fileDialog.CheckFileExists && !string.IsNullOrEmpty(fileDialog.FileName))
            {
                // 显示效果
                this.Display();

                btnSkinOpen.Visibility = Visibility.Hidden;
                btnSkinConfirm.Visibility = Visibility.Visible;
                btnSkinCancel.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 背景设置窗口确认键按下
        /// </summary>
        private void btnSkinConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (this.backgroudImagePath != null)
            {
                // 备份原背景
                File.Copy(appPath + @"Resource\BackgroudImage\main.jpg",
                    appPath + $"Resource\\BackgroudImage\\main{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg", true);
                // 设置背景
                File.Copy(this.backgroudImagePath, appPath + @"Resource\BackgroudImage\main.jpg", true);
            }
            this.Close();
        }

        /// <summary>
        /// 背景设置窗口取消按钮按下
        /// </summary>
        private void btnSkinCancel_Click(object sender, RoutedEventArgs e)
        {
            this.backgroudImagePath = null;
            // 显示原先的设置
            this.Display();
            btnSkinOpen.Visibility = Visibility.Visible;
            btnSkinConfirm.Visibility = Visibility.Hidden;
            btnSkinCancel.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 背景设置窗口关闭按钮按下
        /// </summary>
        private void btnSkinClose_Click(object sender, RoutedEventArgs e)
        {
            this.backgroudImagePath = null;
            // 显示原先的设置
            this.Display();
            this.Close();
        }
    }
}
