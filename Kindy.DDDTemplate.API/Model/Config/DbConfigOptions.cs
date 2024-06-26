﻿using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Kindy.DDDTemplate.API.Model.Config
{
    /// <summary>
    /// 数据库配置
    /// </summary>
    public class DbConfigOptions
    {
        public List<DbConfig> DbConfigs { get; set; }
    }

    public class DbConfig
    {
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DbName { get; set; }
        /// <summary>
        /// 数据库编号
        /// </summary>
        public string DbNumber { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DbType { get; set; }
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string DbString { get; set; }
    }
}
