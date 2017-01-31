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
using AccountBook.Service;
using static AccountBook.Common.AccountBookCommon;

namespace AccountBook.WPF
{
    /// <summary>
    /// EditSorts.xaml 的交互逻辑
    /// </summary>
    public partial class EditSorts : Window
    {
        private AccountBookService service = new AccountBookService();

        // 收入或支持
        private string kind;

        // 被选中的分类号
        private string sortCd;

        public EditSorts(string sortCd, string kind)
        {
            InitializeComponent();

            this.kind = kind;

            // 背景初始化
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.Stretch = Stretch.UniformToFill;
            imageBrush.ImageSource = ReadPicture(@"Resource\BackgroudImage\main.jpg");
            this.editBrush.Background = imageBrush;
            // 初始化下拉菜单
            cmbSort.ItemsSource = service.GetSorts().Where(x => x.SortCd.StartsWith(kind));
            cmbSort.SelectedValuePath = "SortCd";
            cmbSort.DisplayMemberPath = "SortName";
            cmbSort.SelectedValue = sortCd;

            // 将此前选中的分类名设置到文本框上
            this.cmbSort_SelectionChanged(null, null);
        }

        /// <summary>
        /// 允许窗口通过其左键中的鼠标向下拖动在窗口的工作区中显示的区域
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            this.DragMove();
        }

        /// <summary>
        /// 关闭编辑窗口
        /// </summary>
        private void btnEditClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 下拉框选中内容变化
        /// </summary>
        private void cmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (txtNewSort != null)
            {
                // 获取分类号
                this.sortCd = cmbSort.SelectedValue?.ToString();
                if (!(bool)rdoAdd.IsChecked)
                {
                    // 将此前选中的分类名设置到文本框上
                    txtNewSort.Text = service.GetSorts().Where(x => x.SortCd == sortCd).
                        Select(x => x.SortName).FirstOrDefault()?.ToString();
                }
            }
        }

        /// <summary>
        /// 确认按钮按下
        /// </summary>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)rdoDelete.IsChecked)
            {
                if (service.RemoveSort(cmbSort.SelectedValue?.ToString()))
                {
                    this.Close();
                    Message.ShowMessage("删除分类成功");
                }
                else
                {
                    Message.ShowMessage("请勿删除已用分类");
                }
            }
            else
            {
                if (txtNewSort.Text.Length <= 10 && txtNewSort.Text.Length > 0)
                {
                    if ((bool)rdoAdd.IsChecked)
                    {
                        int count = service.GetSorts()
                            .Where(x => x.SortCd.StartsWith(this.kind) 
                             && x.SortName == txtNewSort.Text).Count();
                        if (count > 0)
                        {
                            Message.ShowMessage("请勿添加重复数据", errorFlg: true);
                        }
                        else
                        {
                            if (service.AddSort(this.kind, txtNewSort.Text))
                            {
                                this.Close();
                                Message.ShowMessage("分类添加成功");
                            }
                        }
                    }
                    else
                    {
                        var record = service.GetSorts().Where(x => x.SortName == txtNewSort.Text).ToList();
                        if (record.Count() > 0 && record[0].SortCd != sortCd)
                        {
                            Message.ShowMessage("请勿修改为已有分类", errorFlg: true);
                        }

                        else if (record.Count() > 0 && record[0].SortCd == sortCd)
                        {
                            this.Close();
                            Message.ShowMessage("并未对所选分类做出任何修改", errorFlg: true);
                        }
                        else
                        {
                            if (service.EditSort(this.sortCd, txtNewSort.Text))
                            {
                                this.Close();
                                Message.ShowMessage("分类修改成功");
                            }
                        }
                    }
                }
                else
                {
                    Message.ShowMessage("请输入1至10位字符作为分类名", errorFlg: true);
                }
            }
        }

        /// <summary>
        /// 选择添加分类
        /// </summary>
        private void rdoAdd_Checked(object sender, RoutedEventArgs e)
        {
            // 清空分类名输入框
            txtNewSort.Text = string.Empty;
        }

        /// <summary>
        /// 选择修改分类
        /// </summary>
        private void rdoEdit_Checked(object sender, RoutedEventArgs e)
        {
            // 将此前选中的分类名设置到文本框上
            this.cmbSort_SelectionChanged(null, null);
        }

        /// <summary>
        /// 选择删除分类
        /// </summary>
        private void rdoDelete_Checked(object sender, RoutedEventArgs e)
        {
            // 将此前选中的分类名设置到文本框上
            this.cmbSort_SelectionChanged(null, null);
        }
    }
}
