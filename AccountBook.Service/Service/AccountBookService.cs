using AccountBook.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using AccountBook.Model;
using AccountBook.Common;
using Microsoft.Office.Interop.Excel;


namespace AccountBook.Service
{
    public class AccountBookService
    {
        private AccountBookDal dal = new AccountBookDal();

        /// <summary>
        /// 判断是否为第一次登陆
        /// </summary>
        /// <returns></returns>
        public bool IsFirst()
        {
            return dal.IsFirst();
        }

        /// <summary>
        /// 检查密码
        /// </summary>
        /// <param name="pwd">密码</param>
        /// <returns>正常：true</returns>
        public bool CheckPwdOK(string pwd)
        {
            return dal.CheckPwdOK(pwd);
        }

        /// <summary>
        /// 设置密码
        /// </summary>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public bool SetPwd(string pwd)
        {
            return dal.SetPwd(pwd);
        }

        /// <summary>
        /// 获取标签显示的内容
        /// </summary>
        /// <returns></returns>
        public string GetLabelContent()
        {
            return dal.GetLabelContent();
        }

        /// <summary>
        /// 设置标签显示的内容
        /// </summary>
        /// <param name="content">要设置的内容</param>
        /// <returns></returns>
        public bool SetLabelContent(string content)
        {
            return dal.SetLabelContent(content);
        }

        /// <summary>
        /// 得到分类
        /// </summary>
        /// <returns></returns>
        public List<Model.Sort> GetSorts()
        {
            return dal.GetSorts();
        }

        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="sortCd"></param>
        public bool RemoveSort(string sortCd)
        {
            return dal.RemoveSort(sortCd);
        }

        /// <summary>
        /// 增加分类
        /// </summary>
        /// <param name="kind">区分收入还是支出</param>
        /// <param name="sortName">分类</param>
        public bool AddSort(string kind, string sortName)
        {
            return dal.AddSort(kind, sortName);
        }

        /// <summary>
        /// 修改分类
        /// </summary>
        /// <param name="sortCd">分类号</param>
        /// <param name="sortName">分类名</param>
        public bool EditSort(string sortCd, string sortName)
        {
            return dal.EditSort(sortCd, sortName);
        }

        /// <summary>
        /// 添加账目记录
        /// </summary>
        public bool AddAccount(string sortCd, decimal money, string comments, DateTime? accountDate = null)
        {
            Account account = new Account();
            // 判断是不是追加的记录
            if (!accountDate.HasValue)
            {
                account.AccountDate = Convert.ToDateTime(DateTime.Now.ToString("s"));
            }
            else
            {
                var date = this.GetListDetail(accountDate.Value).
                     OrderByDescending(x => x.AccountDate).FirstOrDefault()?.AccountDate;
                if (date.HasValue)
                {
                    // 从最后一条的记录加1秒开始
                    account.AccountDate = date.Value.AddSeconds(1);
                }
                else
                {
                    // 从这一天的第0秒开始
                    account.AccountDate = accountDate.Value;
                }
            }
            account.SortCd = sortCd;
            if (sortCd.StartsWith(CommConst.IncomeCd))
            {
                account.Income = money;
            }
            else
            {
                account.Expenditure = money;
            }
            account.Comments = comments;
            return dal.AddAccount(account);
        }

        /// <summary>
        /// 更新账目记录
        /// </summary>
        public bool EditAccount(DateTime accountDate, string sortCd, decimal money, string comments)
        {
            Account account = new Account();
            account.AccountDate = accountDate;
            account.SortCd = sortCd;
            if (sortCd.StartsWith(CommConst.IncomeCd))
            {
                account.Income = money;
            }
            else
            {
                account.Expenditure = money;
            }
            account.Comments = comments;
            return dal.EditAccount(account);
        }

        /// <summary>
        /// 获取指定日收入合计
        /// </summary>
        /// <param name="date">指定日期</param>
        /// <returns>指定日收入合计</returns>
        public decimal GetDayTotalIn(DateTime date)
        {
            return dal.GetDayTotalInOrEx(date, true);
        }

        /// <summary>
        /// 获取指定日支出合计
        /// </summary>
        /// <param name="date">指定日期</param>
        /// <returns>指定日支出合计</returns>
        public decimal GetDayTotalEx(DateTime date)
        {
            return dal.GetDayTotalInOrEx(date, false);
        }

        /// <summary>
        ///  获取指定月收入合计
        /// </summary>
        /// <param name="date">指定日期</param>
        /// <returns>指定月收入合计</returns>
        public decimal GetMonthTotalIn(DateTime date)
        {
            return dal.GetMonthTotalInOrEx(date, true);
        }

        /// <summary>
        ///  获取指定月支出合计
        /// </summary>
        /// <param name="date">指定日期</param>
        /// <returns>指定月支出合计</returns>
        public decimal GetMonthTotalEx(DateTime date)
        {
            return dal.GetMonthTotalInOrEx(date, false);
        }

        /// <summary>
        /// 获取一览数据
        /// </summary>
        /// <returns></returns>
        public List<Account> GetListByDay(DateTime? dateFrom, DateTime? dateTo)
        {
            dateTo = dateTo?.AddDays(1).AddSeconds(-1);
            return dal.GetListByDay(dateFrom, dateTo);
        }

        /// <summary>
        /// 得到指定日的收入支出详情
        /// </summary>
        public List<Account> GetListDetail(DateTime date)
        {
            return dal.GetListDetial(date);
        }

        /// <summary>
        /// 删除所有账目数据
        /// </summary>
        /// <returns></returns>
        public void DeleteAll()
        {
            dal.DeleteAll();
        }

        /// <summary>
        /// 删除指定账目数据
        /// </summary>
        /// <returns></returns>
        public int DeleteAccount(List<Account> list)
        {
            return dal.DeleteAccount(list.Select(account => account.AccountDate).ToList());
        }

        /// <summary>
        /// 打开Excel
        /// </summary>
        public void OpenExcel(string path)
        {
            // 创建Excel主程序
            Application excelApp = new Application();
            excelApp.Visible = true;
            Workbooks workbooks = excelApp.Workbooks;
            workbooks.Open(path);
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        public bool ExcportExcel(string path, string type, DateTime? dateFrom, DateTime? dateTo)
        {
            bool result = true;

            // 工作表
            Worksheet worksheet;
            var list = this.dal.GetListDetial(dateFrom, dateTo);

            // 创建Excel主程序
            Application excelApp = new Application();
            excelApp.Visible = false;
            Workbooks workbooks = excelApp.Workbooks;
            // 创建工作簿
            Workbook workbook = workbooks.Add(true);

            switch (type)
            {
                case CommConst.ByMonth:
                    #region 按月导出
                    var months = list.GroupBy(x => new
                    {
                        Year = x.AccountDate.Year,
                        Month = x.AccountDate.Month
                    }).Select(y => DateTime.Parse(y.Key.Year.ToString() + "/" + y.Key.Month.ToString() + "/01")).ToList();

                    for (int i = 0; i < months.Count(); i++)
                    {
                        if (i > 0)
                        {
                            // 原基础上添加Sheet
                            workbook.Worksheets.Add(After: workbook.Worksheets[i]);
                        }
                        worksheet = workbook.Worksheets[i + 1] as Worksheet;
                        worksheet.Name = $"{months[i].Year}年{months[i].Month}月";
                        this.SetExcelTitle(worksheet);
                        var listByMonth = list.Where(x => x.AccountDate.Year == months[i].Year
                                                     && x.AccountDate.Month == months[i].Month).ToList();
                        this.SetExcelContentByDay(worksheet, listByMonth);

                        // 计算合计 空一行
                        int row = worksheet.UsedRange.Rows.Count + 2;
                        this.SetExcelContentTotal(worksheet, listByMonth, $"{months[i].Year}年{months[i].Month}月合计", row);
                        row++;
                        this.SetSurplus(worksheet, row, CommConst.ByMonth, months[i]);

                        // 格式设置
                        this.FormatExcel(worksheet, excelApp);
                    }
                    #endregion
                    break;
                case CommConst.ByQuarter:
                    #region 按季度导出
                    list.ForEach(x => x.Quarter = this.GetQuarter(x.AccountDate));

                    var quarters = list.GroupBy(x => x.Quarter).Select(y => y.Key).ToList();
                    for (int i = 0; i < quarters.Count(); i++)
                    {
                        if (i > 0)
                        {
                            // 原基础上添加Sheet
                            workbook.Worksheets.Add(After: workbook.Worksheets[i]);
                        }

                        worksheet = workbook.Worksheets[i + 1] as Worksheet;
                        worksheet.Name = quarters[i];
                        this.SetExcelTitle(worksheet);

                        var listByQuarter = list.Where(x => x.Quarter == quarters[i]).ToList();
                        this.SetExcelContentByDay(worksheet, listByQuarter, true);

                        // 计算季度合计 空一行
                        int row = worksheet.UsedRange.Rows.Count + 2;
                        this.SetExcelContentTotal(worksheet, listByQuarter, $"{quarters[i]}合计", row);
                        row++;

                        DateTime lastDate = listByQuarter.OrderBy(x => x.AccountDate).Last().AccountDate;
                        this.SetSurplus(worksheet, row, CommConst.ByQuarter, lastDate);

                        // 格式设置
                        this.FormatExcel(worksheet, excelApp);
                    }
                    #endregion
                    break;
                case CommConst.ByYear:
                    #region 按年导出
                    var years = list.GroupBy(x => x.AccountDate.Year).Select(y => y.Key).ToList();

                    for (int i = 0; i < years.Count(); i++)
                    {
                        if (i > 0)
                        {
                            // 原基础上添加Sheet
                            workbook.Worksheets.Add(After: workbook.Worksheets[i]);
                        }

                        worksheet = workbook.Worksheets[i + 1] as Worksheet;
                        worksheet.Name = $"{years[i]}年";
                        this.SetExcelTitle(worksheet);

                        var listByYear = list.Where(x => x.AccountDate.Year == years[i]).ToList();
                        this.SetExcelContentByDay(worksheet, listByYear, true);

                        // 计算年合计 空一行
                        int row = worksheet.UsedRange.Rows.Count + 2;
                        this.SetExcelContentTotal(worksheet, listByYear, $"{years[i]}年合计", row);
                        row++;
                        this.SetSurplus(worksheet, row, CommConst.ByYear, DateTime.Parse($"{years[i].ToString()}/01/01"));

                        // 格式设置
                        this.FormatExcel(worksheet, excelApp);
                    }
                    #endregion
                    break;
            }

            // 保存文件
            try
            {
                workbook.SaveAs(path);
            }
            catch
            {
                result = false;
            }
            finally
            {
                // 关闭Excel线程
                workbooks.Close();
                excelApp.Quit();
                IntPtr hwnd = new IntPtr(excelApp.Hwnd);
                int processId = 0;
                AccountBookCommon.GetWindowThreadProcessId(hwnd, out processId);
                System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(processId);
                process.Kill();
            }

            return result;
        }

        /// <summary>
        /// 设置Excel标题
        /// </summary>
        public void SetExcelTitle(Worksheet worksheet)
        {
            string[] titles = new string[] { "时间", "分类", "收入（元）", "支出（元）", "备注" };
            for (int i = 0; i < titles.Length; i++)
            {
                ((Range)worksheet.Cells[1, i + 1]).Value = titles[i];
            }

            // 加粗
            Range range = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, 5]];
            range.Font.Bold = true;
        }

        /// <summary>
        /// 设置Excel明细内容
        /// </summary>
        /// <param name="row"></param>
        /// <param name="account"></param>
        public void SetExcelContent(Worksheet worksheet, int row, Account account)
        {
            // 时间
            ((Range)worksheet.Cells[row, 1]).Value = account.AccountDate.ToString("yyyy/MM/dd HH:mm:ss");
            // 分类
            ((Range)worksheet.Cells[row, 2]).Value = account.SortName;
            // 收入
            ((Range)worksheet.Cells[row, 3]).Value = account.Income.HasValue ?
                account.Income.Value.ToString("#,0.00") : string.Empty;
            // 支出
            ((Range)worksheet.Cells[row, 4]).Value = account.Expenditure.HasValue ?
                account.Expenditure.Value.ToString("#,0.00") : string.Empty;
            // 备注
            ((Range)worksheet.Cells[row, 5]).Value = account.Comments;
        }

        /// <summary>
        /// Excel格式设置
        /// </summary>
        public void FormatExcel(Worksheet worksheet, Application excelApp)
        {
            // 靠左处理
            int rowEnd = worksheet.UsedRange.Rows.Count;
            Range range = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[rowEnd, 5]];
            range.HorizontalAlignment = XlHAlign.xlHAlignLeft;

            // 自动列宽
            range = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[rowEnd, 5]];
            range.Columns.AutoFit();

            // 固定标题行
            excelApp.ActiveWindow.SplitRow = 1;
            excelApp.ActiveWindow.FreezePanes = true;
        }

        /// <summary>
        /// 按天设置Excel明细内容
        /// </summary>
        public void SetExcelContentByDay(Worksheet worksheet, List<Account> list, bool printMonthTotal = false)
        {
            int row = worksheet.UsedRange.Rows.Count + 1;
            var days = list.GroupBy(x => x.AccountDate.Date)
                .Select(y => y.Key).ToList();
            for (int i = 0; i < days.Count(); i++)
            {
                var listByday = list.Where(x => x.AccountDate.Date == days[i].Date).ToList();
                foreach (var account in listByday)
                {
                    this.SetExcelContent(worksheet, row, account);
                    row++;
                }

                // 计算合计
                this.SetExcelContentTotal(worksheet, listByday, $"{days[i].Year}年{days[i].Month}月{days[i].Day}日合计", row);
                row += 2;

                // 判断是否打印月合计
                if (printMonthTotal && (i + 1 == days.Count() || days[i].Month != days[i + 1].Month))
                {
                    var listByMonth
                        = list.Where(x => x.AccountDate.Date.Year == days[i].Year
                        && x.AccountDate.Month == days[i].Month).ToList();

                    this.SetExcelContentTotal(worksheet, listByMonth, $"{days[i].Year}年{days[i].Month}月合计", row);
                    row += 2;
                }
            }
        }

        /// <summary>
        /// 设置合计
        /// </summary>
        public void SetExcelContentTotal(Worksheet worksheet, List<Account> list, string title, int row)
        {
            ((Range)worksheet.Cells[row, 2]).Value = title;
            var sumIn = list.Sum(x => x.Income);
            var sumEx = list.Sum(x => x.Expenditure);
            ((Range)worksheet.Cells[row, 3]).Value =
                sumIn.HasValue ? sumIn.Value.ToString("#,0.00") : string.Empty;
            ((Range)worksheet.Cells[row, 4]).Value =
                sumEx.HasValue ? sumEx.Value.ToString("#,0.00") : string.Empty;
        }

        /// <summary>
        /// 设置剩余额
        /// </summary>
        public void SetSurplus(Worksheet worksheet, int row, string type, DateTime date)
        {
            string title = string.Empty;
            switch (type)
            {
                case CommConst.ByMonth:
                    date = date.AddMonths(1).AddSeconds(-1);
                    title = $"截止{date.Year}年{date.Month}月末余额";
                    break;
                case CommConst.ByQuarter:
                    title = $"截止{this.GetQuarter(date)}末余额";
                    break;
                case CommConst.ByYear:
                    date = date.AddYears(1).AddSeconds(-1);
                    title = $"截止{date.Year}年末余额";
                    break;
            }

            decimal surplus = dal.GetSetSurplus(date);

            ((Range)worksheet.Cells[row, 2]).Value = title;
            ((Range)worksheet.Cells[row, 3]).Value = surplus.ToString("#,0.00");
        }

        public string GetQuarter(DateTime date)
        {
            string quarter;
            if (date.Month >= 1 && date.Month <= 3)
            {
                quarter = "一";
            }
            else if (date.Month >= 4 && date.Month <= 6)
            {
                quarter = "二";
            }
            else if (date.Month >= 7 && date.Month <= 9)
            {
                quarter = "三";
            }
            else
            {
                quarter = "四";
            }

            return $"{date.Year}年第{quarter}季度";
        }
    }
}
