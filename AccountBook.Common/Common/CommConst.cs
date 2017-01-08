using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBook.Common
{
    public static class CommConst
    {
        /// <summary>
        /// 收入
        /// </summary>
        public const string IncomeCd = "I";

        /// <summary>
        /// 支出
        /// </summary>
        public const string ExpenditureCd = "E";

        /// <summary>
        /// 未删除
        /// </summary>
        public const string NotDeleted = "0";

        /// <summary>
        /// 已删除
        /// </summary>
        public const string Deleted = "1";

        /// <summary>
        /// 按月
        /// </summary>
        public const string ByMonth = "0";

        /// <summary>
        /// 按季度
        /// </summary>
        public const string ByQuarter = "1";

        /// <summary>
        /// 按年
        /// </summary>
        public const string ByYear = "2";
    }
}
