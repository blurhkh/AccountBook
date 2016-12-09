using AccountBook.Service;
using System;
using System.IO;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AccountBook.Model;
using AccountBook.Common;
using static AccountBook.Common.AccountBookCommon;
using System.Text;
using System.Collections.Generic;

namespace AccountBook.WPF
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Window
    {
        #region 字段
        private AccountBookService service = new AccountBookService();
        // 获取程序运行根目录
        private string appPath = AppDomain.CurrentDomain.BaseDirectory;
        #endregion

        #region 构造函数
        public Main()
        {
            InitializeComponent();
            //缩放，最大化修复
            WindowBehaviorHelper wh = new WindowBehaviorHelper(this);
            wh.RepairBehavior();

            // 背景初始化
            this.Display();

            // 欢迎标签显示内容
            lblWelcome.Content = service.GetLabelContent();

            // 初始化分类下拉框
            rdoIn.IsChecked = true;
            this.InitSortCmb();

            // 显示今日合计
            this.GetDayTotal(DateTime.Now);

            // 显示本月合计
            this.GetMonthTotal(DateTime.Now);

            // 初始化日期显示
            DateTime dateFrom = DateTime.Parse($"{DateTime.Now.Year}/{DateTime.Now.Month}/01");
            DateTime dateTo = dateFrom.AddMonths(1).AddDays(-1);
            this.dateFrom.SelectedDate = dateFrom;
            this.dateTo.SelectedDate = dateTo;

            // 获取一览信息
            this.GetListByDay(dateFrom, dateTo, this.txtFilter.Text);
        }
        #endregion

        #region 事件
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
        /// 关闭按钮按下
        /// </summary>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 最大化与还原
        /// </summary>
        private void btnMax_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized)
            {
                this.WindowState = WindowState.Maximized;
                btnMax.ToolTip = "还原";
            }
            else
            {
                this.WindowState = WindowState.Normal;
                btnMax.ToolTip = "最大化";
            }
        }

        /// <summary>
        /// 最小化
        /// </summary>
        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 皮肤设置
        /// </summary>
        private void btnSkin_Click(object sender, RoutedEventArgs e)
        {
            MainSkin win = new MainSkin(this);
            win.skinBrush.Background = this.grdMain.Background;
            win.Show();
        }

        /// <summary>
        /// 设置按钮按下
        /// </summary>
        private void btnSet_Click(object sender, RoutedEventArgs e)
        {
            popSet.IsOpen = true;
        }

        /// <summary>
        /// 重设密码
        /// </summary>
        private void btnResetPwd_Click(object sender, RoutedEventArgs e)
        {
            ResetPwd win = new ResetPwd();
            win.ShowDialog();
        }

        /// <summary>
        /// 重设登录界面
        /// </summary>
        private void btnResetLogin_Click(object sender, RoutedEventArgs e)
        {
            UserDefined win = new UserDefined();
            win.Show();
        }

        /// <summary>
        /// 清空所有数据
        /// </summary>
        private void btnResetData_Click(object sender, RoutedEventArgs e)
        {
            // 备份数据文件
            this.MakeBackup();

        }

        /// <summary>
        /// 搜索图标放大
        /// </summary>
        private void btnSearch_MouseEnter(object sender, MouseEventArgs e)
        {
            btnSearch.FontSize = 18;
        }

        /// <summary>
        /// 搜索图标还原
        /// </summary>
        private void btnSearch_MouseLeave(object sender, MouseEventArgs e)
        {
            btnSearch.FontSize = 17;
        }

        /// <summary>
        /// 切换到收入
        /// </summary>
        private void rdoIn_Checked(object sender, RoutedEventArgs e)
        {
            this.lblMoney.Content = "收入金额：";
            this.InitSortCmb(CommConst.IncomeCd);
        }

        /// <summary>
        /// 切换到支出
        /// </summary>
        private void rdoEx_Checked(object sender, RoutedEventArgs e)
        {
            this.lblMoney.Content = "支出金额：";
            this.InitSortCmb(CommConst.ExpenditureCd);
        }

        /// <summary>
        /// 修改分类信息
        /// </summary>
        private void btnEditSorts_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string sortCd = Convert.ToString(cmbSort.SelectedValue);
            string kind = Convert.ToBoolean(rdoIn.IsChecked) ? CommConst.IncomeCd : CommConst.ExpenditureCd;
            EditSorts win = new EditSorts(sortCd, kind);
            win.ShowDialog();
            // 更新下拉框内容
            this.InitSortCmb(kind);
        }

        /// <summary>
        /// 金额输入框失去焦点
        /// </summary>
        private void txtMoney_LostFocus(object sender, RoutedEventArgs e)
        {
            decimal money;
            if (string.IsNullOrEmpty(this.txtMoney.Text.Trim())) return;

            if (decimal.TryParse(this.txtMoney.Text.Trim(), out money))
            {
                this.txtMoney.Text = money.ToString("#,0.00");
            }
            else
            {
                txtMoney.Text = string.Empty;
                Message.ShowMessage("请勿输入非法字符");
            }
        }

        /// <summary>
        /// 金额输入框得到焦点
        /// </summary>
        private void txtMoney_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtMoney.Text.Trim())) return;

            txtMoney.Text = Convert.ToDecimal(txtMoney.Text.Trim()).ToString("0.00");
        }

        /// <summary>
        /// 点击保存
        /// </summary>
        private void btnSave_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (txtMoney.Text.Trim().Length == 0)
            {
                Message.ShowMessage("金额不能为空");
                return;
            }
            // 得到分类号
            string sortCd = cmbSort.SelectedValue.ToString();
            decimal money = Convert.ToDecimal(txtMoney.Text);
            if (service.AddAccount(sortCd, money, txtComments.Text))
            {
                this.GetDayTotal(DateTime.Now);
                this.GetMonthTotal(DateTime.Now);
                Message.ShowMessage("记录添加成功");
                // 刷新显示
                this.GetListByDay(this.dateFrom.SelectedDate, this.dateTo.SelectedDate, this.txtFilter.Text);
                this.GetDayTotal(DateTime.Now);
                this.GetMonthTotal(DateTime.Now);
            }
        }

        /// <summary>
        /// 一览明细选中事件
        /// </summary>
        private void dataGeneral_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Account account = this.dataGeneral.SelectedItem as Account;
            if (account != null)
            {
                // 得到被选中的明细的日期
                DateTime date = Convert.ToDateTime(account.DateStr);
            }
        }

        /// <summary>
        /// 开始时间发生变化
        /// </summary>
        private void dateFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            this.GetListByDay(this.dateFrom.SelectedDate, this.dateTo.SelectedDate, this.txtFilter.Text);
        }
        /// <summary>
        /// 结束时间发生变化
        /// </summary>
        private void dateTo_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            this.GetListByDay(this.dateFrom.SelectedDate, this.dateTo.SelectedDate, this.txtFilter.Text);
        }

        /// <summary>
        /// 检索条件发生变化
        /// </summary>
        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.GetListByDay(this.dateFrom.SelectedDate, this.dateTo.SelectedDate, this.txtFilter.Text);
        }
        #endregion

        #region 方法
        /// <summary>
        /// 显示画面设置效果
        /// </summary>
        private void Display()
        {
            string path = appPath + @"Resource\BackgroudImage\main.jpg";
            // 背景初始化
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.Stretch = Stretch.UniformToFill;
            imageBrush.ImageSource = ReadPicture(path);
            this.grdMain.Background = imageBrush;
        }

        /// <summary>
        /// 备份数据文件
        /// </summary>
        public void MakeBackup()
        {
            string path = appPath + @"AppData\AccountBook";
            File.Copy($"{path}.db", $"{path}.bk", true);
        }

        /// <summary>
        /// 初始化分类下拉框
        /// </summary>
        public void InitSortCmb(string kind = CommConst.IncomeCd)
        {
            cmbSort.ItemsSource = service.GetSorts().Where(x => x.SortCd.StartsWith(kind)).OrderBy(x => x.SortCd);
            cmbSort.SelectedValuePath = "SortCd";
            cmbSort.DisplayMemberPath = "SortName";
            cmbSort.SelectedIndex = 0;
        }

        /// <summary>
        /// 获取指定日合计
        /// </summary>
        public void GetDayTotal(DateTime date)
        {
            lblDay.Content =
                $"本日总收入{service.GetDayTotalIn(date).ToString("#,0.00")}元，总支出{service.GetDayTotalEx(date).ToString("#,0.00")}元";
        }

        /// <summary>
        /// 获取指定月合计
        /// </summary>
        public void GetMonthTotal(DateTime date)
        {
            lblMonth.Content =
               $"本月总收入{service.GetMonthTotalIn(date).ToString("#,0.00")}元，总支出{service.GetMonthTotalEx(date).ToString("#,0.00")}元";
        }

        /// <summary>
        /// 获取一览数据
        /// </summary>
        public void GetListByDay(DateTime? dateFrom, DateTime? dateTo, string filter)
        {
            filter = filter.Trim();
            this.dataGeneral.DataContext =
                service.GetListByDay(dateFrom, dateTo)
                .Where(x => string.IsNullOrEmpty(filter) ? true : x.Summary.Contains(filter));
        }
        #endregion
    }
}
