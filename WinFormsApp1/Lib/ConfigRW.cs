using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eProject
{
    public class ConfigRW
    {
        /// <summary>
        /// 写入配置
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        public static void appSettings_Write(string key, string val)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[key].Value = val;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static string? appSettings_Read(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// 写入数据库连接字符串
        /// </summary>
        /// <param name="val">数据库连接字符串</param>
        public static void connectionStrings_Write(string val)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.ConnectionStrings.ConnectionStrings["conDB"].ConnectionString = val;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("connectionStrings");
        }
        /// <summary>
        /// 读取数据库连接字符串
        /// </summary>
        /// <returns></returns>
        public static string connectionStrings_Read()
        {
            return ConfigurationManager.ConnectionStrings["conDB"].ConnectionString;
        }

    }
}
