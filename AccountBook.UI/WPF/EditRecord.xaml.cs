using AccountBook.Common;
using AccountBook.Model;
using AccountBook.Service;
using AccountBook.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static AccountBook.Common.AccountBookCommon;

namespace AccountBook.WPF
{
    /// <summary>
    /// EditRecord.xaml 的交互逻辑
    /// </summary>
    public partial class EditRecord : Window
    {
        // 是否正在处理 
        private bool isDoing = false;
        // 是否是更新数据 
        private bool isEdit = false;
        // 需要更新或追加的账目的日期
        private DateTime? accountDate;
        // 主窗体
        private Main main;
        private AccountBookService service = new AccountBookService();
        public EditRecord(Main main)
        {
            this.main = main;
            main.SuccessFlg = false;
            InitializeComponent();

            // 背景初始化
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.Stretch = Stretch.UniformToFill;
            imageBrush.ImageSource = ReadPicture(@"Resource\BackgroudImage\main.jpg");
            this.border.Background = imageBrush;

            // 初始化分类下拉框
            rdoIn.IsChecked = true;
            this.InitSortCmb();

            // 内容初始化
            this.Init();
        }

        /// <summary>
        /// 允许窗口通过其左键中的鼠标向下拖动在窗口的工作区中显示的区域
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            try
            {
                this.DragMove();
            }
            catch
            {
            }
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
            string kind = rdoIn.IsChecked.Value ? CommConst.IncomeCd : CommConst.ExpenditureCd;
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
                Message.ShowMessage("请勿输入非法字符", errorFlg: true);
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
        /// 关闭按钮按下
        /// </summary>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 点击保存
        /// </summary>
        private void btnSave_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (txtMoney.Text.Trim().Length == 0)
            {
                Message.ShowMessage("金额不能为空", errorFlg: true);
                return;
            }
            // 得到分类号
            string sortCd = cmbSort.SelectedValue.ToString();
            decimal money = Convert.ToDecimal(txtMoney.Text);
            if (!this.isDoing)
            {
                this.isDoing = true;
                if (!this.isEdit)
                {
                    if (this.accountDate.HasValue)
                    {
                        if (service.AddAccount(sortCd, money, txtComments.Text, this.accountDate.Value))
                        {
                            this.main.SuccessFlg = true;
                        }
                    }
                    else
                    {
                        if (service.AddAccount(sortCd, money, txtComments.Text))
                        {
                            this.main.SuccessFlg = true;
                        }
                    }

                }
                else
                {
                    if (service.EditAccount(this.accountDate.Value, sortCd, money, txtComments.Text))
                    {
                        this.main.SuccessFlg = true;
                    }
                }
            }
            this.Close();
        }

        /// <summary>
        /// 初始化编辑部
        /// </summary>
        public void Init(Account account = null)
        {
            if (account != null)
            {
                this.isEdit = true;
                this.accountDate = account.AccountDate;
                this.lblTitle.Content = "修改记录";
                this.lblDate.Content = account.AccountDate.GetDateTimeFormats('D')[3].ToString();
                if (account.SortCd.StartsWith(CommConst.IncomeCd))
                {
                    this.rdoIn.IsChecked = true;
                    this.rdoIn_Checked(null, null);
                }
                else
                {
                    this.rdoEx.IsChecked = true;
                    this.rdoEx_Checked(null, null);
                }
                this.cmbSort.SelectedValue = account.SortCd;
                this.txtMoney.Text = account.MoneyStr.Remove(account.MoneyStr.Length - 1);
                this.txtComments.Text = account.Comments;
            }
            else
            {
                this.lblTitle.Content = "添加记录";
                this.lblDate.Content = main.dateEdit.SelectedDate.Value.GetDateTimeFormats('D')[3].ToString();
                if (main.dateEdit.SelectedDate.Value.ToString("yyyy/MM/dd").CompareTo(DateTime.UtcNow.ToString("yyyy/MM/dd")) < 0)
                {
                    this.accountDate = main.dateEdit.SelectedDate;
                }
                this.rdoIn.IsChecked = true;
                this.rdoIn_Checked(null, null);
                this.txtMoney.Text = string.Empty;
                this.txtComments.Text = string.Empty;
            }

        }

        /// <summary>
        /// 初始化分类下拉框
        /// </summary>
        private void InitSortCmb(string kind = CommConst.IncomeCd)
        {
            cmbSort.ItemsSource = service.GetSorts().Where(x => x.SortCd.StartsWith(kind)).OrderBy(x => x.SortCd);
            cmbSort.SelectedValuePath = "SortCd";
            cmbSort.DisplayMemberPath = "SortName";
            cmbSort.SelectedIndex = 0;
        }
    }
}
