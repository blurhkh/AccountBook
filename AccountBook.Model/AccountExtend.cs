using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBook.Model
{
    /// <summary>
    /// 账目数据扩充
    /// </summary>
    public partial class Account
    {
        /// <summary>
        /// 字符串型日期
        /// </summary>
        public string DateStr { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 收入信息
        /// </summary>
        public string InInfo { get; set; }

        /// <summary>
        /// 支出信息
        /// </summary>
        public string ExInfo { get; set; }

        /// <summary>
        /// 分类名
        /// </summary>
        public string SortName { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public string MoneyStr { get; set; }

        /// <summary>
        /// 是否被选中
        /// </summary>
        public bool IsChecked { get; set; }
    }
}
