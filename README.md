## E.EntityFrameworkCore.Extensions
---

此仓库已停止更新,请移步到此处: [链接](https://github.com/rivenfx/EntityFrameworkCore)

---
EntityFrameworkCore 的扩展库

[![NuGet Version](https://img.shields.io/nuget/v/E.EntityFrameworkCore.Extensions.svg?style=flat)](https://www.nuget.org/packages/E.EntityFrameworkCore.Extensions/)

[![NuGet Downloads](https://img.shields.io/nuget/dt/E.EntityFrameworkCore.Extensions.svg?style=flat)](https://www.nuget.org/packages/E.EntityFrameworkCore.Extensions/)

## 功能
---
**目前已经实现的功能**：
* PostgreSql DbSet 表名、视图名、列名自动处理为小写
* Oracle DbSet 表名、视图名、列名自动处理为大写
* 自定义 DbSet 校验处理函数
* 自定义 DbQuery 校验处理函数
* 自定义 Column(Field) 校验处理函数
* 自定义列名长度,自动进行裁剪
* 自动跳过处理 DbContext 程序集中已实现的 IEntityTypeConfiguration
* 增加是否使用 DbSet/DbQuery 名称作为 表名称/视图名称 选项
* 增加字符串列设置默认数据长度 校验函数


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
  
// 禁用 使用DbSet名称作为表名和视图名称,优先读Table标记,其次类名
E.EntityFrameworkCoreTableViewExtensions.UseDbSetNameToTableName = false;

// 设置小写
E.EntityFrameworkCoreTableViewExtensions.SetCaseType(modelBuilder, ColumnNameCaseType.Lower);

// 自定义要处理的字段类型,较多,此处不贴完整
E.EntityFrameworkCoreTableViewExtensions.DbMapTypes.Add("System.String[]");
E.EntityFrameworkCoreTableViewExtensions.DbMapTypes.Add("System.Boolean[]");
E.EntityFrameworkCoreTableViewExtensions.DbMapTypes.Add("System.Int32[]");

// 处理此数据库上下文类型中 DbSet
// 参数为 true,则 自动跳过处理 DbContext 程序集中已实现的 IEntityTypeConfiguration 的 DbSet<Entity>
modelBuilder.CaseAllDbSetNameAndColumnName<BloggingContext>(/* true */);

```

**Oracle**
```
base.OnModelCreating(modelBuilder);


// 禁用 使用DbSet名称作为表名和视图名称,优先读Table标记,其次类名
E.EntityFrameworkCoreTableViewExtensions.UseDbSetNameToTableName = false;

// 设置大写
E.EntityFrameworkCoreTableViewExtensions.SetCaseType(modelBuilder, ColumnNameCaseType.Upper);

// 使用列名长度限制,最大30
E.EntityFrameworkCoreTableViewExtensions.UseColumnNameMaxLength = true;
E.EntityFrameworkCoreTableViewExtensions.ColumnNameMaxLength = 30;

// 使用默认字符串长度
E.EntityFrameworkCoreTableViewExtensions.UseDefaultStringMaxLength = true;
E.EntityFrameworkCoreTableViewExtensions.DefaultStringMaxLength = 256;
// 自定义 字符串列数据长度 是否设置默认长度 校验函数
E.EntityFrameworkCoreTableViewExtensions.CheckUseDefaultStringMaxLength = (type, prop) =>
{

    var checkResult = new CheckUseDefaultStringMaxLenghtResult();

    // 如果实体的命名空间为 TestWebApp.Database.Models 则设置长度为200
    if (type.Namespace.StartsWith("TestWebApp.Database.Models"))
    {
        checkResult.Success = true;
        checkResult.MaxLength = 200;
    }


    return checkResult;
};

// 处理此数据库上下文类型中 DbSet
// 参数为 true,则 自动跳过处理 DbContext 程序集中已实现的 IEntityTypeConfiguration 的 DbSet<Entity>
modelBuilder.CaseAllDbSetNameAndColumnName<BloggingContext>(/* true */);

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
  
  // Column Chcek
  E.EntityFrameworkCoreTableViewExtensions.ColumnCheck = (info) =>
  {
      // 你的校验逻辑
      return true;// or false
  };

```

