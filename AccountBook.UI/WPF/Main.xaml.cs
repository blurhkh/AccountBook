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
using Microsoft.Win32;

namespace AccountBook.WPF
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Window
    {
        #region 字段
        private AccountBookService service = new AccountBookService();
        // 是否正在处理
        private bool isDoing = false;
        // 获取程序运行根目录
        private string appPath = AppDomain.CurrentDomain.BaseDirectory;
        #endregion

        #region 属性
        // 表示是否添加或者修改数据成功
        public bool SuccessFlg { get; set; }
        #endregion

        #region 构造函数
        public Main()
        {
            InitializeComponent();
            // 缩放，最大化修复
            WindowBehaviorHelper wh = new WindowBehaviorHelper(this);
            wh.RepairBehavior();

            // 背景初始化
            this.Display();

            // 欢迎标签显示内容
            lblWelcome.Content = service.GetLabelContent();

            // 显示今日合计
            this.GetDayTotal(DateTime.Now);

            // 显示本月合计
            this.GetMonthTotal(DateTime.Now);

            // 初始化日期显示
            DateTime dateFrom = DateTime.Parse($"{DateTime.Now.Year}/{DateTime.Now.Month}/01");
            DateTime dateTo = dateFrom.AddMonths(1).AddDays(-1);
            this.dateFrom.SelectedDate = dateFrom;
            this.dateTo.SelectedDate = dateTo;

            this.dateEdit.SelectedDate = DateTime.Now;

            // 获取一览信息
            this.GetListByDay(dateFrom, dateTo, this.txtFilter.Text);
            this.GetListDetail(DateTime.Now);
        }
        #endregion

        #region 事件
        /// <summary>
        /// 允许窗口通过其左键中的鼠标向下拖动在窗口的工作区中显示的区域
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            if (args.OriginalSource is Window ||
                (args.OriginalSource is Grid && (args.OriginalSource as Grid).Name == this.grdMain.Name))
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
            bool result = Message.ShowConfirmMessage($"确认清空所有数据么？");
            if (result)
            {
                // 备份数据文件
                this.MakeBackup();
                // 删除数据
                service.DeleteAll();
                Message.ShowMessage("已成功清空所有数据", shutdownFlg: true);
                // 回到登录界面
                AccountBookCommon.ReLogin();
            }
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
        /// 一览明细选中事件
        /// </summary>
        private void dataGeneral_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Account account = this.dataGeneral.SelectedItem as Account;
            if (account != null)
            {
                // 得到被选中的明细的日期
                DateTime date = Convert.ToDateTime(account.DateStr);
                // 获取详情
                this.GetListDetail(date);
                this.GetMonthTotal(date);
                this.GetDayTotal(date);
            }
        }

        /// <summary>
        /// 详细明细选中事件
        /// </summary>
        private void Item_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            Account account = this.dataDetail.SelectedItem as Account;
            if (account != null)
            {
                EditRecord win = new EditRecord(this);
                win.Init(account);
                win.ShowDialog();

                if (this.SuccessFlg)
                {
                    this.GetDayTotal(DateTime.Now);
                    this.GetMonthTotal(DateTime.Now);
                    Message.ShowMessage("记录修改成功");
                    // 刷新显示
                    this.Refresh(account.AccountDate);
                }
            }
        }

        /// <summary>
        /// 开始时间发生变化
        /// </summary>
        private void dateFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.dateFrom.SelectedDate != null
                && this.dateFrom.SelectedDate > this.dateTo.SelectedDate)
            {
                Message.ShowMessage("开始时间不得大于结束时间", errorFlg: true);
                this.dateFrom.SelectedDate = this.dateTo.SelectedDate;
            }
            this.GetListByDay(this.dateFrom.SelectedDate, this.dateTo.SelectedDate, this.txtFilter.Text);
        }

        /// <summary>
        /// 结束时间发生变化
        /// </summary>
        private void dateTo_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.dateTo.SelectedDate != null
                && this.dateFrom.SelectedDate > this.dateTo.SelectedDate)
            {
                Message.ShowMessage("结束时间不得小于开始时间", errorFlg: true);
                this.dateTo.SelectedDate = this.dateFrom.SelectedDate;
            }
            this.GetListByDay(this.dateFrom.SelectedDate, this.dateTo.SelectedDate, this.txtFilter.Text);
        }

        /// <summary>
        /// 检索条件发生变化
        /// </summary>
        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.GetListByDay(this.dateFrom.SelectedDate, this.dateTo.SelectedDate, this.txtFilter.Text);
        }

        /// <summary>
        /// 新增记录
        /// </summary>
        private void btnAdd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            EditRecord win = new EditRecord(this);
            win.ShowDialog();

            if (this.SuccessFlg)
            {
                this.GetDayTotal(DateTime.Now);
                this.GetMonthTotal(DateTime.Now);
                Message.ShowMessage("记录添加成功");
                // 刷新显示
                this.Refresh(Convert.ToDateTime(this.dateEdit.SelectedDate));
            }
        }

        /// <summary>
        /// 选中所有
        /// </summary>
        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            var data = (sender as CheckBox).DataContext as List<Account>;
            if (data != null && data.Count > 0)
            {
                List<Account> list = this.service.GetListDetail(data[0].AccountDate);
                list.ForEach(account =>
                {
                    account.IsChecked = this.chkAll.IsChecked.Value;
                });
                this.dataDetail.DataContext = list;
            }
        }

        /// <summary>
        /// 删除选中的记录
        /// </summary>
        private void btnDel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.isDoing)
            {
                this.isDoing = true;
                var delList = (this.dataDetail.DataContext as List<Account>).
                        Where(account => account.IsChecked == true).ToList();
                if (delList.Count > 0)
                {
                    bool result = Message.ShowConfirmMessage($"确认删除这{delList.Count}条数据么？");
                    if (result)
                    {
                        this.service.DeleteAccount(delList);
                        // 刷新显示
                        this.Refresh((this.dataDetail.DataContext as List<Account>)[0].AccountDate);
                    }
                }
                this.isDoing = false;
            }
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        private void btnExcel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.isDoing)
            {
                this.isDoing = true;
                SaveFileDialog fileDialog = new SaveFileDialog() { Filter = "Excel文件 (*.xlsx)|*.xlsx" };
                fileDialog.Title = "请选择保存路径";
                fileDialog.ShowDialog();
                string path = fileDialog.FileName;
                if (!string.IsNullOrEmpty(path))
                {
                    bool result = this.service.ExcportExcel(path, this.cmbExcel.SelectedIndex.ToString(),
                         this.dateFrom.SelectedDate, this.dateTo.SelectedDate);
                    if (result)
                    {
                        Message.ShowMessage("导出成功");

                        result = Message.ShowConfirmMessage("是否打开Excel文件？");

                        if (result)
                        {
                            service.OpenExcel(path);
                        }
                    }
                    else
                    {
                        Message.ShowMessage("导出失败，请检查文件是否被占用");
                    }
                }
                this.isDoing = false;
            }
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
        private void MakeBackup()
        {
            string path = appPath + @"AppData\AccountBook";
            File.Copy($"{path}.db", $"{path}{DateTime.Now.ToString("yyyyMMddHHmmss")}.bk", true);
        }

        /// <summary>
        /// 获取指定日合计
        /// </summary>
        private void GetDayTotal(DateTime date)
        {
            lblDay.Content =
                $"{date.Month}月{date.Day}日总收入{service.GetDayTotalIn(date).ToString("#,0.00")}元，总支出{service.GetDayTotalEx(date).ToString("#,0.00")}元";
        }

        /// <summary>
        /// 获取指定月合计
        /// </summary>
        private void GetMonthTotal(DateTime date)
        {
            lblMonth.Content =
               $"{date.Year}年{date.Month}月总收入{service.GetMonthTotalIn(date).ToString("#,0.00")}元，总支出{service.GetMonthTotalEx(date).ToString("#,0.00")}元";
        }

        /// <summary>
        /// 获取一览数据
        /// </summary>
        private void GetListByDay(DateTime? dateFrom, DateTime? dateTo, string filter)
        {
            filter = filter.Trim();
            var list = service.GetListByDay(dateFrom, dateTo)
                .Where(x => string.IsNullOrEmpty(filter) ? true : x.Summary.Contains(filter)).ToList();
            if (list.Count() == 0)
            {
                this.grdExcel.Visibility = Visibility.Hidden;
            }
            else
            {
                this.grdExcel.Visibility = Visibility.Visible;
            }
            this.dataGeneral.DataContext = list;
        }

        /// <summary>
        /// 得到指定日的收入支出详情
        /// </summary>
        private void GetListDetail(DateTime date)
        {
            this.dataDetail.DataContext = service.GetListDetail(date);
        }

        /// <summary>
        /// 刷新显示
        /// </summary>
        /// <param name="date">明细的指定时间</param>
        private void Refresh(DateTime date)
        {
            this.chkAll.IsChecked = false;
            this.GetListByDay(this.dateFrom.SelectedDate, this.dateTo.SelectedDate, this.txtFilter.Text);
            this.GetListDetail(date);
            this.GetDayTotal(DateTime.Now);
            this.GetMonthTotal(DateTime.Now);
        }
        #endregion
    }
}
