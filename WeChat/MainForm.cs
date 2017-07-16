﻿using formSkin;
using formSkin.Controls;
using formSkin.Controls._List;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeChat.Business.APP;
using WeChat.Business.Base;
using WeChat.Business.BLL;
using WeChat.Business.Model;
using WeChat.Business.Utils;
using WeChat.ListAdapter;

namespace WeChat
{
    public partial class MainForm : FormSkin
    {

        private RContactManager RContactManager;
        private TaskFactory AsyncTask;
        /// <summary>
        /// UI线程的同步上下文
        /// </summary>
        private SynchronizationContext m_SyncContext = null;


        private LastRContactAdapter LastRContactAdapter;
        private RContactAdapter RContactAdapter;
        private LoginForm loginForm;
        private API api;

        public MainForm(LoginForm loginForm)
        {
            InitializeComponent();
            //获取UI线程同步上下文
            m_SyncContext = SynchronizationContext.Current;
            AsyncTask = new TaskFactory();
            api = loginForm.api;
            
            RContactManager = api.RContactManager;
            RContactManager.m_SyncContext = m_SyncContext;
            this.loginForm = loginForm;

        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            LastRContactAdapter = new LastRContactAdapter(api);
            this.LastList.Adapter = LastRContactAdapter;

            HideTable();
            LastList.Dock = DockStyle.Fill;
            pictureBoxSkin1.Selected = true;

            //获取用户信息
            AsyncTask.StartNew(() => {
                RContactManager.Webwxinit(UpdateUser);
            });
            
        }

      

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="obj"></param>
        public void UpdateUser(Object obj)
        {
            UserResponse response = obj as UserResponse;
            if (response == null)
            {
                ShowToast("获取用户数据失败！");
                LogHandler.e("UpdateUser ================>response==null");
                return;
            }
            User user = response.User;
            string str = Context.root_uri + user.HeadImgUrl;
            //string str = Context.base_uri + "/webwxgeticon?seq=0&username=" + user.UserName + "&skey=" + Context.skey;
            api.Imageloader.Add(this.pbHead, str);
            List<RContact> List = new List<RContact>(response.ContactList);
            LastRContactAdapter.SetItems(List);

            RContactAdapter = new ListAdapter.RContactAdapter(api);
            this.ContartList.Adapter = RContactAdapter;
            //加载好友信息
            AsyncTask.StartNew(() =>
            {
                RContactManager.GetRContact(UpdateRContact);
            });
        }

        private void UpdateRContact(object state)
        {
            ContactResponse response = state as ContactResponse;
            if (response == null) 
            {
                ShowToast("获取好友数据失败！");
                LogHandler.e("UpdateRContact ================>response==null");
                return;
            }

            List<RContact> List = new List<RContact>(response.MemberList);
            RContactAdapter.SetItems(List);
        }


        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (loginForm != null)
                loginForm.Close();
        }

        private void ShowToast(string message) 
        {
            Toast.MakeText(this,message).Show();
        }

        #region table
        /// <summary>
        /// table切换事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxSkin1_Click(object sender, EventArgs e)
        {
            if (((PictureBoxSkin)sender).Selected)
                return;

            foreach (Control item in panel5.Controls)
            {
                if (item is PictureBoxSkin)
                {
                    ((PictureBoxSkin)item).Selected = false;
                }
            }
            ((PictureBoxSkin)sender).Selected = true;
        }

        private void HideTable()
        {
            foreach (Control item in panel6.Controls)
            {
                if (item is FListView)
                {
                    ((FListView)item).Visible = false;
                }
            }
        }
        /// <summary>
        /// table选中改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxSkin1_SelectedItem(object sender, EventArgs e)
        {
            PictureBoxSkin view = sender as PictureBoxSkin;
            int stap = Convert.ToInt32(view.Tag);
            HideTable();
            switch (stap)
            {
                case 0:
                    this.LastList.Visible = true;
                    this.LastList.Dock = DockStyle.Fill;
                    break;
                case 1:
                    this.ContartList.Visible = true;
                    this.ContartList.Dock = DockStyle.Fill;
                    break;
                case 2:
                    //this.Collection.Visible = true;
                    break;
                default:
                    break;
            }
        } 
        #endregion


    }
}