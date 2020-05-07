using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cmpp.Client
{
	public class SqliteContext : DbContext
	{
		public SqliteContext(string dbName)
		{
			DbName = dbName + ".db";
			this.Database.EnsureCreated(); //如果没有创建数据库会自动创建
		}

		public string DbName { get; private set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			string connectionString = $"Data source={DbName}";
			optionsBuilder.UseSqlite(connectionString);    //创建文件夹的位置        
		}

		public DbSet<Sms.Common.Sms> Sms { get; set; }
	}
}
