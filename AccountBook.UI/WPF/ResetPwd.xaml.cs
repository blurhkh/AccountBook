using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using AccountBook.Service;
using System.Text.RegularExpressions;
using System;
using System.Windows.Media.Imaging;
using static AccountBook.Common.AccountBookCommon;

namespace AccountBook.WPF
{
    /// <summary>
    /// ResetPwd.xaml 的交互逻辑
    /// </summary>
    public partial class ResetPwd : Window
    {
        private AccountBookService service = new AccountBookService();

        public ResetPwd()
        {
            InitializeComponent();

            if (service.IsFirst())
            {
                lblMessage.Content = "初次使用，请设置密码";
                // 原密码部分不显示
                lblOldPwd.Visibility = Visibility.Hidden;
                txtOldPwd.Visibility = Visibility.Hidden;
            }
            else
            {
                lblMessage.Content = service.GetLabelContent();
            }

            // 获取程序运行根目录
            string appPath = AppDomain.CurrentDomain.BaseDirectory;

            // 背景初始化
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.Stretch = Stretch.UniformToFill;
            imageBrush.ImageSource = ReadPicture(appPath + @"Resource\BackgroudImage\login.jpg");
            this.grdResetPwd.Background = imageBrush;
        }

        /// <summary>
        /// 允许窗口通过其左键中的鼠标向下拖动在窗口的工作区中显示的区域
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            this.DragMove();
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
        /// 确认按钮按下
        /// </summary>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            string oldPwd = txtOldPwd.Password;

            // 如果是第一次设置密码的情况
            if (btnConfirm.Content.ToString() == "下一步")
            {
                // 加载自定义窗口
                UserDefined win = new UserDefined();
                win.IsFirstFlg = true;
                win.Show();
                // 关闭该窗口
                this.Close();
            }

            // 检查是否是首次使用
            bool isFirst = service.IsFirst();

            if (isFirst || service.CheckPwdOK(oldPwd))
            {
                if (txtNewPwd.Password == txtNewPwdConfirm.Password)
                {
                    string pwd = txtNewPwd.Password;
                    if (Regex.IsMatch(pwd, @"^[A-Za-z0-9]{4,20}$"))
                    {
                        if (service.SetPwd(txtNewPwd.Password))
                        {
                            if (isFirst)
                            {
                                lblMessage.Content = "密码设置成功";
                                btnConfirm.Content = "下一步";
                            }
                            else
                            {
                                this.Close();
                                Message.ShowMessage(message: "密码重置成功，请重新登录", flg: true);
                                // 回到登录界面
                                ReLogin();
                            }
                        }
                    }
                    else
                    {
                        lblMessage.Content = "密码仅限4-20位大小写字母及数字";
                        lblMessage.Foreground = Brushes.Red;
                    }
                }
                else
                {
                    lblMessage.Content = "两次输入不一致，请重新输入";
                    lblMessage.Foreground = Brushes.Red;
                }
            }
            else
            {
                lblMessage.Content = "原密码错误，请重新输入";
                lblMessage.Foreground = Brushes.Red;
            }
        }

        /// <summary>
        /// 原密码框回车
        /// </summary>
        private void txtOldPwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (service.CheckPwdOK(txtOldPwd.Password))
                {
                    txtNewPwd.Focus();
                }
                else
                {
                    lblMessage.Content = "原密码错误，请重新输入";
                    lblMessage.Foreground = Brushes.Red;
                }
            }
        }

        /// <summary>
        /// 新密码框回车
        /// </summary>
        private void txtNewPwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Regex.IsMatch(txtNewPwd.Password, @"^[A-Za-z0-9]{4,20}$"))
                {
                    txtNewPwdConfirm.Focus();
                }
                else
                {
                    lblMessage.Content = "密码仅限4-20位大小写字母及数字";
                    lblMessage.Foreground = Brushes.Red;
                }
            }
        }

        /// <summary>
        /// 新密码确认框回车
        /// </summary>
        private void txtNewPwdConfirm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.btnConfirm_Click(sender, e);
            }
        }

        /// <summary>
        /// 重置标签内容
        /// </summary>
        private void txtNewPwd_GotFocus(object sender, RoutedEventArgs e)
        {
            if (service.IsFirst())
            {
                lblMessage.Content = "初次使用，请设置密码";
                lblMessage.Foreground = lblMessage.Foreground = Brushes.Black;
            }
            else
            {
                lblMessage.Content = service.GetLabelContent();
                lblMessage.Foreground = Brushes.Black;
            }
        }

        /// <summary>
        /// 重置标签内容
        /// </summary>
        private void txtOldPwd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtOldPwd.Password))
            {
                lblMessage.Content = service.GetLabelContent();
                lblMessage.Foreground = Brushes.Black;
            }
        }
    }
}
