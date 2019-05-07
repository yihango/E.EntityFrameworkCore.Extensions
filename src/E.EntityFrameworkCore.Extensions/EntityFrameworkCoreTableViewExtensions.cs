﻿using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace E
{
    /// <summary>
    /// EntityFrameworkCore 表和视图的扩展
    /// [Extension to the EntityFrameworkCore table and view]
    /// </summary>
    public static class EntityFrameworkCoreTableViewExtensions
    {
        #region 可自定义配置 [Customizable configuration]

        /// <summary>
        /// 启用转大写,默认为false,默认转为小写
        /// [Enables uppercase, false by default, and lowercase by default]
        /// </summary>
        public static bool ToUpperEnable { get; set; }
        /// <summary>
        /// 启用设置字符串默认长度,默认为 false
        /// [Sets the default length of the string to false by default]
        /// </summary>
        public static bool UseDefaultStringMaxLength { get; set; }
        /// <summary>
        /// 默认字符串使用的长度 (默认 512)
        /// [The length of the default string used (default 512)]
        /// </summary>
        public static int Default_String_Length { get; set; } = 512;
        /// <summary>
        /// 外部自定义属性映射到字段检查方法
        /// [External custom attributes map to field inspection methods]
        /// </summary>
        public static Func<PropertyInfo, bool> PropertyCheck { get; set; }
        /// <summary>
        /// 所有允许映射到数据库的字段类型(谨慎更改)
        /// [All field types that allow mapping to the database (changed carefully)]
        /// </summary>
        public static List<string> DbMapTypes { get; set; } = new List<string>() {
            "System.Int32",
            "System.Boolean",
            "System.Byte",
            "System.Byte[]",
            "System.SByte",
            "System.Char",
            "System.Decimal",
            "System.Double",
            "System.Single",
            "System.Int32",
            "System.UInt32",
            "System.Int64",
            "System.UInt64",
            "System.Int16",
            "System.UInt16",
            "System.String"
        };
        /// <summary>
        /// DbSet 类型字符串(谨慎修改)
        /// [DbSet type string (carefully modified)]
        /// </summary>
        public static string DbSetTypeStr { get; set; } = "Microsoft.EntityFrameworkCore.DbSet";
        /// <summary>
        /// DbQuery 类型字符串(谨慎修改)
        /// [DbQuery type string (carefully modified)]
        /// </summary>
        public static string DbQueryTypeStr { get; set; } = "Microsoft.EntityFrameworkCore.DbQuery";

        #endregion


        #region 内置配置(不可更改)   [Built-in configuration (unchangeable)]

        /// <summary>
        /// 表特性类型
        /// [Table property type]
        /// </summary>
        static Type TableAttr { get; set; } = typeof(TableAttribute);
        /// <summary>
        /// 列特性类型
        /// [Column attribute type]
        /// </summary>
        static Type ColumnAttr { get; set; } = typeof(ColumnAttribute);
        /// <summary>
        /// 不映射特性类型
        /// [Attribute types are not mapped]
        /// </summary>
        static Type NoMappedAttribute { get; set; } = typeof(NotMappedAttribute);
        /// <summary>
        /// 字符串类型
        /// [String type]
        /// </summary>
        static Type StringType { get; set; } = typeof(string);
        /// <summary>
        /// 字符串长度特性类型
        /// [String length attribute type]
        /// </summary>
        static Type StringLengthAttr { get; set; } = typeof(StringLengthAttribute);

        #endregion


        #region 公开 数据库上下文扩展函数 ModelBuilder(DbContext)

        /// <summary>
        /// 设置DbContext的所有DbSet 对应的 表名和列名
        /// [Set the table and column names for all dbsets of the DbContext]
        /// </summary>
        /// <typeparam name="DbContext"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ModelBuilder SetAllDbSetTableNameAndColumnName<DbContext>(this ModelBuilder builder)
            where DbContext : class
        {
            var properties = typeof(DbContext).GetProperties();

            var allDbSet = properties.Where(o => o.PropertyType.FullName.StartsWith(DbSetTypeStr)).ToList();

            if (allDbSet == null || allDbSet.Count == 0)
            {
                return builder;
            }

            var dbSetName = string.Empty;
            var dbSetTypeStr = string.Empty;

            foreach (var dbSet in allDbSet)
            {
                dbSetName = dbSet.Name;
                dbSetTypeStr = dbSet.GetDbSetTypeStr();

                builder.Entity(dbSetTypeStr, (entityTypeBuilder) =>
                {
                    entityTypeBuilder.SetTableNameAndAllCloumName(dbSetName);
                });
            }

            return builder;
        }

        /// <summary>
        /// 设置DbContext的所有DbQuery  对应的 视图名和列名
        /// [Set the view name and column name for all dbqueries of the DbContext]
        /// </summary>
        /// <typeparam name="DbContext"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ModelBuilder SetAllDbQueryViewNameAndColumnName<DbContext>(this ModelBuilder builder)
            where DbContext : class
        {
            var properties = typeof(DbContext).GetProperties();
            var allDbQuery = properties.Where(o => o.PropertyType.FullName.StartsWith(DbQueryTypeStr)).ToList();

            if (allDbQuery == null || allDbQuery.Count == 0)
            {
                return builder;
            }

            var dbQueryName = string.Empty;
            Type dbQueryType = null;

            foreach (var dbQuery in allDbQuery)
            {
                dbQueryName = dbQuery.Name;
                dbQueryType = dbQuery.GetDbQueryType();

                builder.Query(dbQueryType, (queryTypeBuilder) =>
                {
                    queryTypeBuilder.SetViewNameAndAllCloumName(dbQueryName);
                });
            }

            return builder;
        }

        #endregion


        #region 公开 表扩展函数 EntityTypeBuilder [Public table extension function EntityTypeBuilder]

        /// <summary>
        /// 自动将表名和列名转换大小写,部分特殊列需要手动处理
        /// [Automatically converts table and column names to case, and some special columns need to be handled manually]
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="tableName"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        public static EntityTypeBuilder SetTableNameAndAllCloumName(
          this EntityTypeBuilder builder, string tableName = null, string schema = null)
        {
            builder.SetTableName(tableName, schema);

            return builder.SetAllColumnName();
        }


        /// <summary>
        /// 设置表名和Schema [Set the table name and Schema]
        /// 优先级:
        /// * 传入的 tableName 和 schema
        /// * 实体标记的 TableAttribute
        /// * 实体名称, schema 不设置
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="tableName"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        public static EntityTypeBuilder SetTableName(
            this EntityTypeBuilder builder,
            string tableName = null,
            string schema = null)
        {
            if (!string.IsNullOrWhiteSpace(tableName))
            {
                return RelationalEntityTypeBuilderExtensions.ToTable(builder, C(tableName), C(schema));
            }

            var tableAttr = builder.Metadata.ClrType.GetTableAttribute();
            if (tableAttr != null)
            {
                return RelationalEntityTypeBuilderExtensions.ToTable(builder, C(tableAttr.Name), C(tableAttr.Schema));
            }
            return RelationalEntityTypeBuilderExtensions.ToTable(builder, C(builder.Metadata.ClrType.Name));
        }

        /// <summary>
        /// 设置所有列名,部分特殊列需要手动处理
        /// [Set all column names. Some special columns need to be handled manually]
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static EntityTypeBuilder SetAllColumnName(this EntityTypeBuilder builder)
        {
            var entityProps = builder.Metadata.ClrType.GetProperties();

            // 字段
            var cloumnName = string.Empty;

            foreach (var prop in entityProps)
            {
                if (!prop.CheckPropIsColumn())
                {
                    continue;
                }

                // 设置字段列名
                var propertyBuilder = builder.Property(prop.PropertyType, prop.Name);
                propertyBuilder.SetColumnName();

                if (UseDefaultStringMaxLength && prop.CheckPropIsStringAndNoSetMaxLength())
                {
                    // 设置字符串列默认长度
                    propertyBuilder.HasMaxLength(Default_String_Length);
                }
            }

            return builder;
        }

        #endregion


        #region 公开 视图扩展函数 QueryTypeBuilder [Public view extension function QueryTypeBuilder]

        /// <summary>
        /// 自动将视图名和列名转换大小写,部分特殊列需要手动处理
        /// [Automatically converts view and column names to case, and some special columns need to be handled manually]
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="tableName"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        public static QueryTypeBuilder SetViewNameAndAllCloumName(
          this QueryTypeBuilder builder, string tableName = null, string schema = null)
        {
            builder.SetViewName(tableName, schema);

            return builder.SetAllColumnName();
        }


        /// <summary>
        /// 设置视图名称和Schema [Set the view name and Schema]
        /// 优先级:
        /// * 传入的 tableName 和 schema
        /// * 实体标记的 TableAttribute
        /// * 实体名称, schema 不设置
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="builder">ModelBuiler</param>
        /// <param name="viewName">表名</param>
        /// <param name="schema">表schema</param>
        /// <returns>ModelBuiler</returns>
        public static QueryTypeBuilder SetViewName(
            this QueryTypeBuilder builder,
            string viewName = null,
            string schema = null)
        {
            if (!string.IsNullOrWhiteSpace(viewName))
            {
                return RelationalQueryTypeBuilderExtensions.ToView(builder, C(viewName), C(schema));
            }

            var tableAttr = builder.Metadata.ClrType.GetTableAttribute();
            if (tableAttr != null)
            {
                return RelationalQueryTypeBuilderExtensions.ToView(builder, C(tableAttr.Name), C(tableAttr.Schema));
            }
            return RelationalQueryTypeBuilderExtensions.ToView(builder, C(builder.Metadata.ClrType.Name));
        }

        /// <summary>
        /// 设置所有列名,部分特殊列需要手动处理
        /// [Set all column names. Some special columns need to be handled manually]
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static QueryTypeBuilder SetAllColumnName(this QueryTypeBuilder builder)
        {
            var entityProps = builder.Metadata.ClrType.GetProperties();

            // 字段
            var cloumnName = string.Empty;

            foreach (var prop in entityProps)
            {
                if (!prop.CheckPropIsColumn())
                {
                    continue;
                }

                // 设置字段列名
                var propertyBuilder = builder.Property(prop.PropertyType, prop.Name);
                propertyBuilder.SetColumnName();

                if (UseDefaultStringMaxLength && prop.CheckPropIsStringAndNoSetMaxLength())
                {
                    // 设置字符串列默认长度
                    propertyBuilder.HasMaxLength(Default_String_Length);
                }
            }

            return builder;
        }

        #endregion


        #region 公开 公共 设置列名函数


        /// <summary>
        /// 设置列名 [Set the column name]
        /// 优先级:
        /// * 传入的 columnName
        /// * 字段标记的 ColumnAttribute 的 Name
        /// * fieldName
        /// </summary>
        /// <param name="property"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static PropertyBuilder SetColumnName(
            this PropertyBuilder property,
            string columnName = null
            )
        {
            if (!string.IsNullOrWhiteSpace(columnName))
            {
                return property.HasColumnName(C(columnName));
            }

            var columnAttr = property.Metadata.ClrType.GetColumnAttribute();
            if (columnAttr != null && !string.IsNullOrWhiteSpace(columnAttr.Name))
            {
                return property.HasColumnName(C(columnAttr.Name));
            }

            var fieldName = Regex.Match(property.Metadata.FieldInfo.Name, "<(.*?)>").Groups[1].Value;
            return property.HasColumnName(C(fieldName));
        }

        #endregion


        #region 公开 公共函数

        /// <summary>
        /// 检查属性是否为可映射到数据库
        /// [Check if the property is mappable to the database]
        /// </summary>
        /// <param name="property"></param>
        /// <returns>是返回true，否返回false</returns>
        public static bool CheckPropIsColumn(this PropertyInfo property)
        {
            // 是否不映射
            if (property.GetCustomAttribute(NoMappedAttribute, false) != null)
            {
                return false;
            }

            // 是否静态属性
            if (CheckStatic(property))
            {
                return false;
            }

            // 如果自定义了检查器,并通过了检查,返回true
            if (PropertyCheck != null && PropertyCheck.Invoke(property))
            {
                return true;
            }

            // 默认检查是否支持类型
            return DbMapTypes.Exists(o => o == property.PropertyType.FullName);
        }

        /// <summary>
        /// 检查属性是否是字符串类型,且没有设置长度
        /// [Checks whether the property is of type string and does not set the length]
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool CheckPropIsStringAndNoSetMaxLength(this PropertyInfo property)
        {
            return property.PropertyType == StringType
                    && property.GetCustomAttribute(StringLengthAttr, false) == null;
        }

        /// <summary>
        /// 检查是否是静态成员
        /// [Check if it is a static member]
        /// </summary>
        /// <returns></returns>
        public static bool CheckStatic(this PropertyInfo property)
        {
            var getMethod = property.GetMethod;
            if (getMethod != null)
            {
                return getMethod.IsStatic;
            }
            else
            {
                var setMethod = property.SetMethod;
                return setMethod.IsStatic;
            }
        }

        /// <summary>
        /// 获取 TableAttribute
        /// [Get TableAttribute]
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TableAttribute GetTableAttribute(this Type type)
        {
            var tableAttr = type.GetCustomAttributes(TableAttr, false);
            if (tableAttr != null && tableAttr.Length >= 1)
            {
                var tableAttribute = (TableAttribute)tableAttr[0];
                return tableAttribute;
            }

            return null;
        }

        /// <summary>
        /// 获取 ColumnAttribute
        /// [Get ColumnAttribute]
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ColumnAttribute GetColumnAttribute(this Type type)
        {
            var columnAttr = type.GetCustomAttributes(ColumnAttr, false);
            if (columnAttr != null && columnAttr.Length >= 1)
            {
                var columnAttribute = (ColumnAttribute)columnAttr[0];
                return columnAttribute;
            }

            return null;
        }

        /// <summary>
        /// 获取DbSet类型字符串
        /// [Gets the DbSet type string]
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetDbSetTypeStr(this PropertyInfo property)
        {
            var startIndex = property.PropertyType.FullName.IndexOf("[[") + 2;
            var endIndex = property.PropertyType.FullName.IndexOf(",");
            var entityTypeStr = property.PropertyType.FullName.Substring(startIndex, endIndex - startIndex);

            return entityTypeStr;
        }

        /// <summary>
        /// 获取DbQuery Type
        /// [Get DbQuery Type]
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static Type GetDbQueryType(this PropertyInfo property)
        {
            var startIndex = property.PropertyType.FullName.IndexOf("[[") + 2;
            var endIndex = property.PropertyType.FullName.IndexOf(",");
            var entityTypeStr = property.PropertyType.FullName.Substring(startIndex, endIndex - startIndex);
            return Type.GetType(entityTypeStr);
        }

        #endregion


        #region 私有函数

        /// <summary>
        /// 转换字符串大小写
        /// [Converts string case]
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string C(string str)
        {
            if (ToUpperEnable)
            {
                return str?.ToUpperInvariant();
            }

            return str?.ToLowerInvariant();
        }

        #endregion
    }
}