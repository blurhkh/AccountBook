using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using AccountBook.Model;
using AccountBook.Common;

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
            var record = dbContext.Set<Sort>().Where(x => x.SortCd == sortCd).FirstOrDefault();
            dbContext.Set<Sort>().Remove(record);
            return dbContext.SaveChanges() > 0;
        }

        /// <summary>
        /// 添加账目记录
        /// </summary>
        public bool AddAccount(Account account)
        {
            account.AccountDate = Convert.ToDateTime(DateTime.Now.ToString("s"));
            account.UpdateDate = Convert.ToDateTime(DateTime.Now.ToString("s"));
            account.DeleteFlg = CommConst.NotDeleted;
            dbContext.Set<Account>().Add(account);
            return dbContext.SaveChanges() > 0;
        }

        public bool EditAccount(Account account)
        {
            dbContext.Database.BeginTransaction();
            dbContext.Entry(account).State = EntityState.Modified;
            account.UpdateDate = DateTime.Now;
            return dbContext.SaveChanges() > 0;
        }

        /// <summary>
        /// 获取今日收入合计
        /// </summary>
        public decimal GetTodayTotalIncome()
        {
            decimal? totalIncome = dbContext.Set<Account>().
                Where(x => x.AccountDate != null
                && x.AccountDate.Year == DateTime.Now.Year
                && x.AccountDate.Day == DateTime.Now.Day).Sum(x => x.Income).GetValueOrDefault();
            return totalIncome == null ? decimal.Zero : (decimal)totalIncome;
        }

        /// <summary>
        /// 获取今日支出合计
        /// </summary>
        public decimal GetTodayTotalExpenditure()
        {
            decimal? totalIncome = dbContext.Set<Account>().
                Where(x => x.AccountDate != null
                && x.AccountDate.Year == DateTime.Now.Year
                && x.AccountDate.Day == DateTime.Now.Day).Sum(x => x.Expenditure).GetValueOrDefault();
            return totalIncome == null ? decimal.Zero : (decimal)totalIncome;
        }
    }
}
