using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using AccountBook.Model;
using AccountBook.Common;
using System.Text;

namespace AccountBook.DAL
{
    public class AccountBookDal
    {
        private DbContext dbContext = new AccountBookDbContext();

        /// <summary>
        /// 判断是否为第一次登陆
        /// </summary>
        /// <returns></returns>
        public bool IsFirst()
        {
            string pwd = dbContext.Set<User>().Where(x => x.UserId == "2121").Select(x => x.Password).SingleOrDefault();
            return string.IsNullOrEmpty(pwd);
        }

        /// <summary>
        /// 获取标签显示的内容
        /// </summary>
        /// <returns></returns>
        public string GetLabelContent()
        {
            return dbContext.Set<User>().Where(x => x.UserId == "2121").Select(x => x.UserName).SingleOrDefault();
        }

        /// <summary>
        /// 设置标签显示的内容
        /// </summary>
        /// <param name="content">要设置的内容</param>
        /// <returns></returns>
        public bool SetLabelContent(string content)
        {
            var user = dbContext.Set<User>().Where(x => x.UserId == "2121").FirstOrDefault();
            user.UserName = content;
            dbContext.Entry(user).State = EntityState.Modified;
            return dbContext.SaveChanges() > 0;
        }

        /// <summary>
        /// 检查密码
        /// </summary>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public bool CheckPwdOK(string pwd)
        {
            var count = dbContext.Set<User>().Where(x => x.UserId == "2121" && x.Password == pwd).Count();
            return count > 0;
        }

        /// <summary>
        /// 设置密码
        /// </summary>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public bool SetPwd(string pwd)
        {
            var user = dbContext.Set<User>().Where(x => x.UserId == "2121").FirstOrDefault();
            user.Password = pwd;
            dbContext.Entry(user).State = EntityState.Modified;
            return dbContext.SaveChanges() > 0;
        }

        /// <summary>
        /// 得到分类
        /// </summary>
        /// <returns></returns>
        public List<Sort> GetSorts()
        {
            var sorts = dbContext.Set<Sort>().OrderBy(x => x.SortCd).ToList();
            return sorts;
        }

        /// <summary>
        /// 增加分类
        /// </summary>
        /// <param name="kind">区分收入还是支出</param>
        /// <param name="sortName">分类</param>
        public bool AddSort(string kind, string sortName)
        {
            var lastCd = dbContext.Set<Sort>().OrderByDescending(x => x.SortCd).Select(x => x.SortCd).FirstOrDefault();
            string newCd = (Convert.ToInt32(lastCd.Remove(0, 1)) + 1).ToString().PadLeft(9, '0').Insert(0, kind);
            dbContext.Set<Sort>().Add(new Sort { SortCd = newCd, SortName = sortName });
            return dbContext.SaveChanges() > 0;
        }

        /// <summary>
        /// 修改分类
        /// </summary>
        /// <param name="sortCd">分类索引</param>
        /// <param name="sortName">分类名</param>
        public bool EditSort(string sortCd, string sortName)
        {
            var record = dbContext.Set<Sort>().Where(x => x.SortCd == sortCd).FirstOrDefault();
            record.SortName = sortName;
            dbContext.Entry(record).State = EntityState.Modified;
            return dbContext.SaveChanges() > 0;
        }

        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="sortCd"></param>
        public bool RemoveSort(string sortCd)
        {
            if (dbContext.Set<Account>().Where(x => x.SortCd == sortCd).FirstOrDefault() == null)
            {
                var record = dbContext.Set<Sort>().Where(x => x.SortCd == sortCd).FirstOrDefault();
                dbContext.Set<Sort>().Remove(record);
                return dbContext.SaveChanges() > 0;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 添加账目记录
        /// </summary>
        public bool AddAccount(Account account)
        {
            account.UpdateDate = Convert.ToDateTime(DateTime.Now.ToString("s"));
            account.DeleteFlg = CommConst.NotDeleted;
            dbContext.Set<Account>().Add(account);
            return dbContext.SaveChanges() > 0;
        }

        /// <summary>
        /// 编辑账目记录
        /// </summary>
        public bool EditAccount(Account account)
        {
            account.UpdateDate = Convert.ToDateTime(DateTime.Now.ToString("s"));
            // 因为一开始删除flg没取出来，所以这边加上
            account.DeleteFlg = CommConst.NotDeleted;
            dbContext.Entry(account).State = EntityState.Modified;
            account.UpdateDate = DateTime.Now;
            return dbContext.SaveChanges() > 0;
        }

        /// <summary>
        /// 获取指定日收入或支出合计
        /// </summary>
        /// <param name="date">指定日期</param>
        /// <returns>指定日收入或支出合计</returns>
        public decimal GetDayTotalInOrEx(DateTime date, bool option)
        {
            decimal? totalIncome = dbContext.Set<Account>().
                Where(x => x.DeleteFlg == CommConst.NotDeleted
                && x.AccountDate.Year == date.Year
                && x.AccountDate.Month == date.Month
                && x.AccountDate.Day == date.Day).Sum(x => option ? x.Income : x.Expenditure).GetValueOrDefault();
            return totalIncome == null ? decimal.Zero : (decimal)totalIncome;
        }

        /// <summary>
        /// 获取指定月收入或支出合计
        /// </summary>
        /// <param name="date">指定日期</param>
        /// <returns>指定月收入或支出合计</returns>
        public decimal GetMonthTotalInOrEx(DateTime date, bool option)
        {
            decimal? totalIncome = dbContext.Set<Account>().
                Where(x => x.DeleteFlg == CommConst.NotDeleted
                && x.AccountDate.Year == date.Year
                && x.AccountDate.Month == date.Month).Sum(x => option ? x.Income : x.Expenditure).GetValueOrDefault();
            return totalIncome == null ? decimal.Zero : (decimal)totalIncome;
        }

        /// <summary>
        /// 获取一览账目明细
        /// </summary>
        public List<Account> GetListByDay(DateTime? dateFrom, DateTime? dateTo)
        {
            var query = dbContext.Set<Account>()
                .Where(x => x.DeleteFlg == CommConst.NotDeleted
                && (dateFrom != null ? x.AccountDate >= dateFrom : true)
                && (dateTo != null ? x.AccountDate <= dateTo : true))
                .GroupBy(x => new
                {
                    Year = x.AccountDate.Year,
                    Month = x.AccountDate.Month,
                    Day = x.AccountDate.Day
                })
                .Select(y => new
                {
                    AccountDate = y.Key.Year.ToString() + "/" + y.Key.Month.ToString() + "/" + y.Key.Day.ToString(),
                    Income = y.Sum(i => i.Income == null ? 0 : i.Income),
                    Expenditure = y.Sum(e => e.Expenditure == null ? 0 : e.Expenditure),
                    InCnt = y.Sum(cntIn => cntIn.SortCd.StartsWith(CommConst.IncomeCd) ? 1 : 0),
                    ExCnt = y.Sum(cntEx => cntEx.SortCd.StartsWith(CommConst.ExpenditureCd) ? 1 : 0),
                }).
                OrderBy(z => z.AccountDate).ToList();

            List<Account> list1 = new List<Account>();

            query.ForEach(x =>
            {
                list1.Add(new Account
                {
                    AccountDate = DateTime.Parse(x.AccountDate),
                    InInfo = $"{x.InCnt}笔,共{Convert.ToDecimal(x.Income).ToString("#,0.00")}元",
                    ExInfo = $"{x.ExCnt}笔,共{Convert.ToDecimal(x.Expenditure).ToString("#,0.00")}元"
                });
            });

            var list2 = this.GetMainInfo(list1);

            var result = (from account in list1
                          join accountJoin in list2
                          on account.AccountDate equals accountJoin.AccountDate into temp
                          from accountExtend in temp.DefaultIfEmpty()
                          select new Account
                          {
                              DateStr = account.AccountDate.ToString("yyyy/MM/dd"),
                              Income = account.Income,
                              Expenditure = account.Expenditure,
                              InInfo = account.InInfo,
                              ExInfo = account.ExInfo,
                              Summary = accountExtend.Summary
                          }).OrderBy(x => x.DateStr).ToList();
            return result;
        }

        /// <summary>
        /// 得到每月主要信息
        public List<Account> GetMainInfo(List<Account> accountList)
        {
            List<DateTime> dateList = accountList.Select(x => x.AccountDate).ToList();

            List<Account> accountListNew = new List<Account>();
            StringBuilder sb = new StringBuilder();
            dateList.ForEach(date =>
            {
                sb.Length = 0;
                var main = this.GetListDetial(date);
                var mainIn = main.OrderByDescending(x => x.Income).FirstOrDefault();
                if (mainIn.Income != null)
                {
                    sb.Append(
                        $"{mainIn.SortName}{Convert.ToDecimal(mainIn.Income).ToString("#,0.00")}元");
                }
                var mainEx = main.OrderByDescending(x => x.Expenditure).FirstOrDefault();
                if (mainEx.Expenditure != null)
                {
                    if (sb.Length != 0)

                    {
                        sb.Append(",");
                    }
                    sb.Append(
                        $"{mainEx.SortName}{Convert.ToDecimal(mainEx.Expenditure).ToString("#,0.00")}元");
                }

                accountListNew.Add(new Account
                {
                    AccountDate = date,
                    Summary = sb.Replace("（", "").Replace("）", "").ToString()
                });
            });
            return accountListNew;
        }

        /// <summary>
        /// 得到指定日的收入支出详情
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<Account> GetListDetial(DateTime date)
        {
            var query = (from account in dbContext.Set<Account>()
                         join sortJoin in dbContext.Set<Sort>()
                         on account.SortCd equals sortJoin.SortCd into sortTemp
                         from sort in sortTemp.DefaultIfEmpty()
                         where account.AccountDate.Year == date.Year
                         && account.AccountDate.Month == date.Month
                         && account.AccountDate.Day == date.Day
                         && account.DeleteFlg == CommConst.NotDeleted
                         select new
                         {
                             AccountDate = account.AccountDate,
                             Income = account.Income,
                             Expenditure = account.Expenditure,
                             SortCd = sort.SortCd,
                             SortName = sort.SortName,
                             Comments = account.Comments,
                             Money = account.Income != null ? account.Income : account.Expenditure
                         }).ToList();
            List<Account> result = new List<Account>();
            query.ForEach(x => result.Add(new Account
            {
                AccountDate = x.AccountDate,
                DateStr = x.AccountDate.ToString("HH:mm:ss"),
                Income = x.Income,
                Expenditure = x.Expenditure,
                SortCd = x.SortCd,
                SortName = x.SortName + (x.SortCd.StartsWith(CommConst.IncomeCd) ? "（收入）" : "（支出）"),
                Comments = x.Comments,
                MoneyStr = Convert.ToDecimal(x.Money).ToString("#,0.00") + "元"
            }));
            return result.OrderBy(x => x.AccountDate).ToList();
        }

        /// <summary>
        /// 删除所有账目数据
        /// </summary>
        /// <returns></returns>
        public void DeleteAll()
        {
            // 开启事务
            using (var dbContextTransaction = dbContext.Database.BeginTransaction())
            {
                // 清空密码
                this.SetPwd(null);
                // 清空账户
                StringBuilder sql = new StringBuilder();
                sql.Append("DELETE FROM ACCOUNT");
                dbContext.Database.ExecuteSqlCommand(sql.ToString());
                dbContextTransaction.Commit();
            }
        }

        public int DeleteAccount(List<DateTime> list)
        {
            // 开启事务
            int count;
            using (var dbContextTransaction = dbContext.Database.BeginTransaction())
            {
                // 清空账户
                dbContext.Set<Account>().RemoveRange(dbContext.Set<Account>().
                    Where(account => list.Contains(account.AccountDate)));
                count = dbContext.SaveChanges();
                dbContextTransaction.Commit();
            }
            return count;
        }
    }
}
