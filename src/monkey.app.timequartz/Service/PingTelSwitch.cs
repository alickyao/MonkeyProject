﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monkey.app.timequartz;
using System.Net.NetworkInformation;
using System.Threading;

namespace monkey.app.timequartz.Service
{
    /// <summary>
    /// PING成都公司的电话程控交换机
    /// </summary>
    class PingTelSwitch : IThreading
    {
        /// <summary>
        /// 成都公司电话程控交换机地址
        /// </summary>
        const string HOST = "192.168.0.235";

        /// <summary>
        /// 是否发送错误报告
        /// </summary>
        public static bool IsSendMsg
        {
            get; set;
        }

        /// <summary>
        /// 错误报告发送时间
        /// </summary>
        public static DateTime? SendMsgTime {
            get;
            set;
        }

        /// <summary>
        /// 首次发现错误的上报时间
        /// </summary>
        public static DateTime? FirstSendMsgTime {
            get;
            set;
        }

        /// <summary>
        /// 发送错误报告的时间间隔（如果已发送了错误报告并且未复位那么则间隔一下分钟重复发送）
        /// 单位 分钟
        /// </summary>
        const int SendMsgTimeInterval = 30;


        /// <summary>
        /// 错误计数器
        /// </summary>
        public static int ErrorCount = 0;

        /// <summary>
        /// 连续超过多少次则发送错误报告
        /// </summary>
        const int ErrorCountMax = 1;

        /// <summary>
        /// 执行
        /// </summary>
        public void Run()
        {
            try
            {
                //变更为PING 11次 间隔1秒

                int eC = 0;
                int sC = 0;
                bool isSuccess = false;
                List<string> eCString = new List<string>();
                for (int i = 0; i < 11; i++)
                {
                    Ping p = new Ping();
                    var rR = p.Send(HOST);
                    if (rR.Status == IPStatus.Success)
                    {
                        sC = sC + 1;
                    }
                    else {
                        eC = eC + 1;
                        eCString.Add(rR.Status.ToString());
                    }
                    Thread.Sleep(1000);
                }

                if (eC > sC)
                {
                    //错误比正确多
                }
                else {
                    //正确比错误多
                    isSuccess = true;
                }

                string msg = string.Format("ping {0} result [Success time:{1},Error time:{2},error String List is: {3}]", HOST, sC, eC, eCString.Count > 0 ? string.Join(",", eCString) : "none");
                if (isSuccess)
                {
                    //成功，写入日志。
                    SysLog.CreateTextLog(LogType.runing, msg);
                    if (IsSendMsg == true) {
                        //曾经发送过错误报告现在恢复了，则发送恢复通知
                        SysLog.CreateTextLog(LogType.error, "错误已恢复");
                        UshangService.UploadNotice("成都公司电话交换机故障已恢复", true);
                    }
                    IsSendMsg = false;
                    SendMsgTime = null;
                    FirstSendMsgTime = null;
                    ErrorCount = 0;
                }
                else {
                    ErrorCount = ErrorCount + 1;
                    //其他情况，写入错误日志以及发起通知等操作
                    SysLog.CreateTextLog(LogType.error, msg + string.Format(",error count is {0}", ErrorCount));
                    string postString = "发现成都公司电话交换机链接失败";
                    if (!IsSendMsg)
                    {
                        //没有发送错误报告，并且超过了错误累计的上限，立即发送错误报告
                        if (ErrorCount > ErrorCountMax) {
                            SysLog.CreateTextLog(LogType.error, string.Format("错误已经连续{0}次发生，执行发送错误报告至U上商侣后台以及短信通知管理员", ErrorCount));
                            IsSendMsg = true;
                            SendMsgTime = DateTime.Now;
                            FirstSendMsgTime = DateTime.Now;
                            UshangService.UploadNotice(postString, true);
                        }
                    }
                    else {
                        if (SendMsgTime.Value.AddMinutes(SendMsgTimeInterval) <= DateTime.Now)
                        {
                            //如果当前日期已经超过了发送错误报告加上间隙分钟，则重复发送错误报告，并更新错误报告发送时间
                            postString = postString + string.Format("，错误还未解决，距离上次发送错误报告已过去了{0}分钟，距离该错误发生已经过去了{1}分钟", SendMsgTimeInterval, (DateTime.Now - FirstSendMsgTime.Value).TotalMinutes.ToString("0"));
                            SysLog.CreateTextLog(LogType.error, postString);
                            SendMsgTime = DateTime.Now;
                            UshangService.UploadNotice(postString, true);
                        }
                        else {
                            //如果在间隙时间内，则不发送任何错误报告
                            SysLog.CreateTextLog(LogType.warning, string.Format("{0},错误还在继续，但应间隔时间未到，不上报错误",msg));
                        }
                    }
                }
            }
            catch (Exception e) {
                SysLog.CreateTextLog(LogType.error, e.Message);
                SysLog.CreateTextLog(LogType.error, e.StackTrace);
            }
        }
    }
}
