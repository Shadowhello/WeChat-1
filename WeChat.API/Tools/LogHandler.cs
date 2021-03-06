﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.API.Tools
{
    /***
    * ===========================================================
    * 创建人：yuanj
    * 创建时间：2018/01/17 11:39:37
    * 说明：
    * ==========================================================
    * */
    public class LogHandler
    {
        private static string logPath = string.Empty;
        /// <summary>  
        /// 保存日志的文件夹  
        /// </summary>  
        public static string LogPath
        {
            get
            {
                if (logPath == string.Empty)
                {
                    // Windows Forms 应用  
                    logPath = AppDomain.CurrentDomain.BaseDirectory + @"log/";
                    if (Directory.Exists(logPath) == false)
                    {
                        Directory.CreateDirectory(logPath);
                    }
                }
                return logPath;
            }
            set { logPath = value; }
        }

        //private static string logFielPrefix = string.Empty;
        ///// <summary>  
        ///// 日志文件前缀  
        ///// </summary>  
        //public static string LogFielPrefix
        //{
        //    get { return logFielPrefix; }
        //    set { logFielPrefix = value; }
        //}

        /// <summary>  
        /// 写日志  
        /// </summary>  
        public static void WriteLog(string logFile, string msg)
        {
            try
            {
                string filePath = LogPath + logFile + " " + DateTime.Now.ToString("yyyyMMdd") + ".Log";
                System.IO.StreamWriter sw = System.IO.File.AppendText(filePath);
                StringBuilder sb = new StringBuilder();
                sb.Append("------------------------------- 开始--------------------------------------" + System.Environment.NewLine);
                sb.Append("--------------------------" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "-----------------------------" + System.Environment.NewLine);
                sb.Append(msg + System.Environment.NewLine);
                sb.Append("--------------------------------结束--------------------------------------" + System.Environment.NewLine);
                sw.WriteLine(sb.ToString());
                sw.Close();
            }
            catch (Exception IoEx)
            {
                WriteLog(LogFile.Error, IoEx.ToString());
            }
        }

        /// <summary>  
        /// 写日志  
        /// </summary>  
        public static void WriteLog(LogFile logFile, string msg)
        {
            WriteLog(logFile.ToString(), msg);
        }



        public static void i(string message)
        {
            WriteLog(LogFile.Info, message);
        }

        public static void d(string message)
        {
            WriteLog(LogFile.Debug, message);
        }
        public static void e(string message)
        {
            WriteLog(LogFile.Error, message);
        }
        public static void e(Exception e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("时间：" + DateTime.Now.ToString());
            sb.Append("\n异常信息：" + e.Message);
            sb.Append("\n异常对象：" + e.Source);
            sb.Append("\n调用堆栈：" + e.StackTrace);
            sb.Append("\n触发方法：" + e.TargetSite);

            WriteLog(LogFile.Error, sb.ToString());
        }


        public static void i(string ClassName, string Method, params object[] param) {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("ClassName："+ ClassName);
            sb.AppendLine("Method：" + Method);
            if (param != null)
            {
                foreach (var item in param)
                {
                    sb.Append(param+",");
                }
            }
            i(sb.ToString());
        }

        public static void i(string ClassName, string Method, object param)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("ClassName：" + ClassName);
            sb.AppendLine("Method：" + Method);
            if (param != null)
            {
                Type tp= param.GetType();
                FieldInfo[] Fields= tp.GetFields();
                foreach (FieldInfo item in Fields)
                {
                    sb.AppendLine(item.Name+"=" + item.GetValue(param));
                }
            }
            else
            {
                sb.AppendLine("param：null");
            }
            i(sb.ToString());
        }

    }
    /// <summary>  
    /// 日志类型  
    /// </summary>  
    public enum LogFile
    {
        Info,
        Debug,
        Error,
        SQL
    }
}
