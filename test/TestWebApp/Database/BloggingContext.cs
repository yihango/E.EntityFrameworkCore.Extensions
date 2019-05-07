using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestWebApp.Database.Models;
using E;

namespace TestWebApp.Database
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        public BloggingContext(DbContextOptions<BloggingContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            #region 自定义校验器

            //// DbSet Check
            //E.EntityFrameworkCoreTableViewExtensions.DbSetCheck = (info) =>
            //{
            //    // 你的校验逻辑
            //    return true;// or false
            //};
            //// DbQuery Check
            //E.EntityFrameworkCoreTableViewExtensions.DbQueryCheck = (info) =>
            //{
            //    // 你的校验逻辑
            //    return true;// or false
            //};
            //// Column Chcek
            //E.EntityFrameworkCoreTableViewExtensions.ColumnCheck = (info) =>
            //{
            //    // 你的校验逻辑
            //    return true;// or false
            //}; 

            #endregion


            // 配置... [configurations...]

            #region 如果使用 postgre_sql  [if use postgre_sql ]

            E.EntityFrameworkCoreTableViewExtensions.UseUpperCase = false;


            // 添加映射字段类型 [Adds a string of mapped field types]

            E.EntityFrameworkCoreTableViewExtensions.DbMapTypes.Add("System.String[]");
            E.EntityFrameworkCoreTableViewExtensions.DbMapTypes.Add("System.Boolean[]");
            E.EntityFrameworkCoreTableViewExtensions.DbMapTypes.Add("System.Int32[]");

            // 添加更多 [add more...]

            #endregion

            #region 如果使用 oracle [if use oracle]

            //E.EntityFrameworkCoreTableViewExtensions.UseUpperCase = true;
            //E.EntityFrameworkCoreTableViewExtensions.UseDefaultStringMaxLength = true;
            //E.EntityFrameworkCoreTableViewExtensions.DefaultStringMaxLength = 256;

            #endregion


            #region 如果使用postgresql 或者 oracle  [if use postgre_sql or oracle]

            modelBuilder.SetAllDbSetTableNameAndColumnName<BloggingContext>();
            modelBuilder.SetAllDbQueryViewNameAndColumnName<BloggingContext>();

            #endregion

        }


    }
}
