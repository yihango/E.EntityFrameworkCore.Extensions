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

## 用法

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
  modelBuilder.SetAllDbSetTableNameAndColumnName<YourContext>();
  modelBuilder.SetAllDbQueryViewNameAndColumnName<YourContext>();

```

**Oracle**
```
  base.OnModelCreating(modelBuilder);
  
  // 设置转大写为 true
  E.EntityFrameworkCoreTableViewExtensions.UseUpperCase = true;
  
  // 设置字段类型为字符串的默认长度(因为oracle限制字符串最大长度为2000),若字段标记 StringLength 则取 StringLength 长度
  E.EntityFrameworkCoreTableViewExtensions.UseDefaultStringMaxLength = true;
  E.EntityFrameworkCoreTableViewExtensions.DefaultStringMaxLength = 256;
  
  // 将名为 YourContext 中的所有 DbSet 和 DbQuery 的表名、视图名、列名转换为大写
  modelBuilder.SetAllDbSetTableNameAndColumnName<YourContext>();
  modelBuilder.SetAllDbQueryViewNameAndColumnName<YourContext>();

```


