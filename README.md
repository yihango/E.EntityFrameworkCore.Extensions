## E.EntityFrameworkCore.Extensions
---
EntityFrameworkCore 的扩展库

[![NuGet Version](https://img.shields.io/nuget/v/E.EntityFrameworkCore.Extensions.svg?style=flat)](https://www.nuget.org/packages/E.EntityFrameworkCore.Extensions/)

[![NuGet Downloads](https://img.shields.io/nuget/dt/E.EntityFrameworkCore.Extensions.svg?style=flat)](https://www.nuget.org/packages/E.EntityFrameworkCore.Extensions/)

## 功能
---
**目前已经实现的功能**：
* PostgreSql DbSet和 DbQeruy 表名、视图名、列名自动处理为小写
* Oracle DbSet 和 DbQuery 表名、视图名、列名自动处理为大写
* 自定义 DbSet 校验处理函数
* 自定义 DbQuery 校验处理函数
* 自定义 Column(Field) 校验处理函数
* 自定义列名长度,自动进行裁剪
* 自动跳过处理 DbContext 程序集中已实现的 IEntityTypeConfiguration 和 IQueryTypeConfiguration


## 基本用法

### 1. 引入nuget包
* dotnet cli

`dotnet add package E.EntityFrameworkCore.Extensions`

* nuget package manager

`Install-Package E.EntityFrameworkCore.Extensions`

### 2. 配置 DbContext

* 重写 DbContext的 OnModelCreating 函数
* 引入命名空间 `using E;`


### 3. 配置 E.EntityFrameworkCore.Extensions
***以下代码均处于 OnModelCreating 函数中***

**PostgreSql**
```
  base.OnModelCreating(modelBuilder);
  
  // 设置转大写为false
  E.EntityFrameworkCoreTableViewExtensions.UseUpperCase = false;
  
  // 增加映射的字段类型字符串, 由于 PostgreSql 支持数据类型过于丰富,此处省略部分...
  E.EntityFrameworkCoreTableViewExtensions.DbMapTypes.Add("System.String[]");
  E.EntityFrameworkCoreTableViewExtensions.DbMapTypes.Add("System.Boolean[]");
  E.EntityFrameworkCoreTableViewExtensions.DbMapTypes.Add("System.Int32[]");
  
  // 将名为 YourContext 中的所有 DbSet 和 DbQuery 的表名、视图名、列名转换为小写
  // 参数位 true,则 自动跳过处理 DbContext 程序集中已实现的 IEntityTypeConfiguration 和 IQueryTypeConfiguration
  modelBuilder.SetAllDbSetTableNameAndColumnName<YourContext>(/* true */);
  modelBuilder.SetAllDbQueryViewNameAndColumnName<YourContext>(/* true */);

```

**Oracle**
```
  base.OnModelCreating(modelBuilder);
  
  // 设置转大写为 true
  E.EntityFrameworkCoreTableViewExtensions.UseUpperCase = true;
  
  // 设置字段类型为字符串的默认长度(因为oracle限制字符串最大长度为2000),若字段标记 StringLength 则取 StringLength 长度
  E.EntityFrameworkCoreTableViewExtensions.UseDefaultStringMaxLength = true;
  E.EntityFrameworkCoreTableViewExtensions.DefaultStringMaxLength = 256;
  
  // 启用限制列名长度,超出长度自动裁剪(因为oracle限制列名最大长度为30)
  E.EntityFrameworkCoreTableViewExtensions.UseColumnNameMaxLength = true;
  E.EntityFrameworkCoreTableViewExtensions.ColumnNameMaxLength = 30;
  
  // 将名为 YourContext 中的所有 DbSet 和 DbQuery 的表名、视图名、列名转换为大写
  // 参数位 true,则 自动跳过处理 DbContext 程序集中已实现的 IEntityTypeConfiguration 和 IQueryTypeConfiguration
  modelBuilder.SetAllDbSetTableNameAndColumnName<YourContext>(/* true */);
  modelBuilder.SetAllDbQueryViewNameAndColumnName<YourContext>(/* true */);

```

## 自定义校验处理函数
***以下代码均处于 OnModelCreating 函数中***
```
  // DbSet Check
  E.EntityFrameworkCoreTableViewExtensions.DbSetCheck = (info) =>
  {
      // 你的校验逻辑
      return true;// or false
  };
  
  // DbQuery Check
  E.EntityFrameworkCoreTableViewExtensions.DbQueryCheck = (info) =>
  {
      // 你的校验逻辑
      return true;// or false
  };
  
  // Column Chcek
  E.EntityFrameworkCoreTableViewExtensions.ColumnCheck = (info) =>
  {
      // 你的校验逻辑
      return true;// or false
  };

```

