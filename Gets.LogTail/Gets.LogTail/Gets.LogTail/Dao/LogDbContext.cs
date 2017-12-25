using Gets.LogTail.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gets.LogTail.Dao
{
    public class LogDbContext : DbContext
    {

        /// <summary>
        /// 构造函数并制定数据库
        /// </summary>
        public LogDbContext() : base("name=DefaultConnection")
        {

            //数据库操作形式
            Database.SetInitializer<LogDbContext>(null);

        }

        public DbSet<LogFile> LogFileDbSet { get; set; }
        public DbSet<LogData> LogDataDbSet { get; set; }
    }
}
