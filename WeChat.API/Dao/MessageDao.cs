﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.API.Dao
{
    /***
    * ===========================================================
    * 创建人：yuanj
    * 创建时间：2018/01/17 16:24:30
    * 说明：
    * ==========================================================
    * */
    public class MessageDao
    {

        public const string TABLE_NAME = "T_Message";
        private static SqLiteHelper Helper;
        /// <summary>
        /// 添加聊天记录
        /// </summary>
        /// <param name="Seq"></param>
        /// <param name="Content"></param>
        /// <param name="IsSend"></param>
        /// <param name="FileName"></param>
        /// <param name="MsgType"></param>
        /// <param name="FileSize"></param>
        public void InsertMessage(Message msg,string Seq)
        {
            bool IsSend = msg.IsSend;
            string Content = msg.Content;
            string FileName = msg.fileName;
            int MsgType = msg.MsgType;
            string FileSize = msg.FileSize;
            object[] values = { Seq, Content, IsSend, FileName,DateTime.Now, MsgType, FileSize };
            Helper.InsertValues(TABLE_NAME, values);
        }
        /// <summary>
        /// 查询聊天记录
        /// </summary>
        /// <param name="FormUser"></param>
        /// <param name="ToUser"></param>
        /// <returns></returns>
        public List<Message> GetMessage(string FormUser)
        {
            List<Message> list = new List<Message>();
            string sql = string.Format("select * from {0} where Seq={1} order by CreateTime asc", TABLE_NAME, FormUser);
            DataTable dt = Helper.GetTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    string Seq = item["Seq"].ToString();
                    string Content = item["Content"].ToString();
                    bool IsSend = Convert.ToBoolean(item["IsSend"]);
                    string FileName = item["FileName"].ToString();
                    int MsgType = Convert.ToInt32(item["MsgType"]);
                    string FileSize = item["FileSize"].ToString();
                    Message message = CreateMessage(Seq, Content, IsSend, FileName, MsgType, FileSize);
                    list.Add(message);
                }
            }
            return list;
        }

        private Message CreateMessage(string Seq, string Content, bool IsSend, string FileName, int MsgType, string FileSize)
        {
            Message msg = new Message();
            msg.Content = Content;
            msg.fileName = FileName;
            msg.FileSize = FileSize;
            msg.MsgType = MsgType;
            msg.IsSend = IsSend;
            Contact remote = WechatAPIService.GetContactBySeq(Seq);
            msg.Remote = remote;
            msg.Mime = WechatAPIService.Self;
            return msg;
        }


        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ifNotExists"></param>
        public static void CreateTable(SqLiteHelper db)
        {
            Helper = db;
            string[] colNames = {
                "Seq","Content","IsSend","FileName","CreateTime","MsgType","FileSize"
            };
            string[] colTypes = {
                "TEXT","TEXT","Boolean","TEXT","TEXT","INTEGER","TEXT"
            };
            db.CreateTable(TABLE_NAME, colNames, colTypes);
        }

        public static void UpdateTable(SqLiteHelper db)
        {
            db.ExecuteScalar("DROP TABLE " + TABLE_NAME);
            CreateTable(db);
        }




    }
}
