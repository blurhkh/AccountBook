using AccountBook.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountBook.Model;
using AccountBook.Common;

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
        public List<Sort> GetSorts()
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
        public bool AddAccount(string sortCd, decimal money, string comments)
        {
            Account account = new Account();
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
    }
}
