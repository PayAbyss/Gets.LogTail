#region License

// Copyright (c) 2017-2020 iGets Inc. All rights reserved. 
// See LICENSE in the project root for license information.

#endregion

/*****************************************************************************
 * 
 * Created On: 2017-12-21
 * Purpose:   文件处理工具类,用于文件获取，筛选，读取等操作
 * 
 ***************************************************************************
 */
using Gets.LogTail.Dao;
using Gets.LogTail.Models;
using log4net;
using System;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Gets.LogTail.Utils
{
    public class LogFileUtils
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(LogFileUtils));

        //定义获取数据匹配规则
        private static Regex _regex = new Regex(@"(?<date>\d+-\d+-\d+)(?<time>\s(\S+))(?<s_ip>\s(\S+))(?<cs_method>\s(\w+))(?<cs_uri_stem>\s(\S+))(?<cs_uri_query>\s(\S+))(?<s_port>\s(\d+))(?<cs_username>\s(\S+))(?<c_ip>\s(\S+))(?<cs_version>\s(\S+))(?<cs_user_agent>\s(\S+))(?<cs_referer>\s(\S+))(?<sc_status>\s(\d+))(?<sc_substatus>\s(\d+))(?<sc_win32_substatus>\s(\d+))(?<sc_bytes>\s(\d+))(?<cs_bytes>\s(\d+))(?<time_taken>\s(\d+))(?<original_c_ip>\s(\S+))");
        private static string _groupId = ConfigurationManager.AppSettings[Constants.GROUP_ID_KEY];

        /// <summary>
        ///       读取文件信息入口
        /// </summary>
        /// <param name="path">log文件根目录</param>
        /// <param name="groupId">服务器地址标识</param>
        /// <param name="day">距离当前日期的天数</param>
        public static void ReadContrast(string path, string serverId, int day)
        {
            //定义全局DbContext属性
            var lDbContext = new LogDbContext();

            //获取当前日期
            var lDay = DateTime.Today;

            //计算开始日期
            var lBeginDay = lDay.AddDays(-day);

            //实例当前跟路径下的文件夹
            var lLogFolder = new DirectoryInfo(path);

            //获取当前跟路径下的文件夹
            var lLogFolders = lLogFolder.GetDirectories();

            try
            {
                //遍历文件夹
                foreach (var lLogFolderInfo in lLogFolders)
                {
                    //获取数据库已处理文件集合
                    var lDbFileList = lDbContext.LogFileDbSet.Where(x => x.ServerId == serverId && x.GroupId == _groupId && x.FolderName == lLogFolderInfo.Name && x.LogLastModifyTime > lBeginDay).Select(x => new { x.FolderName, x.LogFileName, x.Status, x.LogLastModifyTime }).ToList();

                    //获取文件夹内文件集合
                    var lLogFileInfos = lLogFolderInfo.GetFiles("*.log", SearchOption.TopDirectoryOnly).Where(f => f.LastWriteTime > lBeginDay && f.LastWriteTime < lDay).ToList();

                    //判断集合是否有值
                    if (lDbFileList.Count > 0)
                    {
                        //文件筛选
                        for (var i = 0; i < lDbFileList.Count; i++)
                        {

                            //取出集合内容
                            var lLogFile = lDbFileList[i];

                            //循环比较
                            for (var j = 0; j < lLogFileInfos.Count; j++)
                            {

                                //取出集合内容
                                var lLogFileInfo = lLogFileInfos[j];

                                //比较筛选
                                if (lLogFile.FolderName == lLogFolderInfo.Name && lLogFile.LogFileName == lLogFileInfo.Name && lLogFile.Status == (int)FileStatus.Finished && lLogFile.LogLastModifyTime == lLogFileInfo.LastWriteTime)
                                {
                                    lLogFileInfos.Remove(lLogFileInfo);
                                    break;
                                }

                            }// end lLogFileInfos for

                        }//end  lDbFileList for 

                    }//end lDbFileList if

                    //判断集合是否存有内容
                    if (lLogFileInfos.Count > 0)
                    {
                        //文件信息存储
                        foreach (var lLogFileInfo in lLogFileInfos)
                        {
                            var lLogFile = new LogFile();
                            lLogFile.ServerId = serverId;

                            lLogFile.FolderName = lLogFolderInfo.Name;
                            lLogFile.LogFileName = lLogFileInfo.Name;
                            lLogFile.LogLastModifyTime = lLogFileInfo.LastWriteTime;
                            lLogFile.Status = (int)FileStatus.Processing;
                            lLogFile.GroupId = _groupId;
                            lLogFile.ModifiedBy = "";
                            lLogFile.Ctime = DateTime.Now;
                            lLogFile.CreatedBy = "by:HF";
                            try
                            {
                                lDbContext.LogFileDbSet.Add(lLogFile);
                                lDbContext.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                //修改文件状态异常
                                lLogFile.Status = (int)FileStatus.Exception;

                                //保存处理行数
                                lLogFile.ReadLine = 0;

                                lDbContext.Entry(lLogFile).State = EntityState.Modified;

                                lDbContext.SaveChanges();

                                //打印Log
                                _logger.Error("数据库存储文件信息异常  \r  文件夹:" + lLogFile.FolderName + "\r  Log文件:" + lLogFile.LogFileName + "\r  异常信息:" + e);
                            }

                            //获取当前存入数据库文件信息
                            var lTheFileInfoList = lDbContext.LogFileDbSet.Where(x => x.ServerId == serverId && x.GroupId == _groupId && x.FolderName == lLogFolderInfo.Name && x.LogFileName == lLogFileInfo.Name).ToList();

                            //判定是否保存成功
                            if (lTheFileInfoList.Count > 0)
                            {
                                //读取数据
                                ReadData(path + "/" + lLogFolderInfo.Name + "/" + lLogFileInfo.Name, lTheFileInfoList[0]);
                            }

                            //检测异常处理

                            //检测是否存在异常文件
                            var lDbFileExceptionList = lDbContext.LogFileDbSet.Where(x => x.ServerId == serverId && x.GroupId == _groupId && x.Status == (int)FileStatus.Exception).ToList();

                            if (lDbFileExceptionList.Count > 0)
                            {
                                //循环处理异常文件
                                foreach (var lLogExceptionFile in lDbFileExceptionList)
                                {
                                    //循环处理异常数据
                                    ReadData(path + "/" + lLogExceptionFile.FolderName + "/" + lLogExceptionFile.LogFileName, lLogFile);
                                }
                            }//end  lDbFileExceptionList if

                            //检测未更改状态
                            var lDbProcessingFileList = lDbContext.LogFileDbSet.Where(x => x.ServerId == serverId && x.GroupId == _groupId && x.Status == (int)FileStatus.Processing).ToList();

                            //检测是否存未正常修改状态的文件
                            if (lDbProcessingFileList.Count > 0)
                            {

                                foreach (var lDbProcessingFile in lDbProcessingFileList)
                                {

                                    //修改文件为异常文件
                                    lDbProcessingFile.Status = (int)FileStatus.Exception;

                                    lDbContext.Entry(lDbProcessingFile).State = EntityState.Modified;

                                }

                                lDbContext.SaveChanges();

                            }//end  lDbProcessingFileList ifk

                        }//end  lLogFileInfo for

                    }//end  lLogFileInfos if

                } //end lLogFolderInfo for
            }
            catch (Exception e)
            {
                //打印异常Log
                _logger.Error("文件处理异常 \r 异常信息:" + e);
                Console.WriteLine(e);
            }

        } // end ReadContrast Method


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Log文件路径</param>
        /// <param name="lLogFile">与该Log文件相关联的LogFile</param>
        private static void ReadData(string path, LogFile lLogFile)
        {

            //定义全局DbContext属性
            var lDbContext = new LogDbContext();

            //文件数据行数
            var lLogDataLine = 0;

            //定义存储字符串
            var lLogOneLineData = string.Empty;

            // 用于正则匹配数据接收
            MatchCollection lMatchCollection;

            //读取文件内容
            var lFileRead = File.OpenText(path);

            //定义数据类
            LogData lLogData;

            //异常文件已经处理行数
            var lLogDataExceptionLine = 0;

            //定义数据索引行数
            var lMatchCollectionIndex = 0;

            try
            {
                //判定文件状态
                if (lLogFile.Status == (int)FileStatus.Exception)
                {

                    //跳过以处理行数
                    for (var i = 0; i <= lLogFile.ReadLine; i++)
                    {

                        lLogDataExceptionLine++;

                        lFileRead.ReadLine();

                    }
                }

                while ((lLogOneLineData = lFileRead.ReadLine()) != null)
                {

                    //行数自增
                    lLogDataLine++;

                    //匹配数据
                    lMatchCollection = StrMatc(lLogOneLineData);

                    //判断是否匹配数据
                    if (lMatchCollection == null) continue;

                    //示例数据类
                    lLogData = new LogData();

                    try
                    {
                        //读取数据
                        lLogData.RequestDate = DateTime.Parse(lMatchCollection[lMatchCollectionIndex].Groups["date"].Value + lMatchCollection[lMatchCollectionIndex].Groups["time"].Value);

                        lLogData.s_ip = lMatchCollection[lMatchCollectionIndex].Groups["s_ip"].Value.Trim();

                        lLogData.cs_method = lMatchCollection[lMatchCollectionIndex].Groups["cs_method"].Value.Trim();

                        lLogData.cs_uri_stem = lMatchCollection[lMatchCollectionIndex].Groups["cs_uri_stem"].Value.Trim();

                        lLogData.cs_uri_query = isEmpty(HttpUtility.UrlDecode(lMatchCollection[lMatchCollectionIndex].Groups["cs_uri_query"].Value.Trim()));

                        lLogData.s_port = int.Parse(lMatchCollection[lMatchCollectionIndex].Groups["s_port"].Value.Trim());

                        lLogData.cs_userName = isEmpty(lMatchCollection[lMatchCollectionIndex].Groups["cs_username"].Value.Trim());

                        lLogData.c_ip = isEmpty(lMatchCollection[lMatchCollectionIndex].Groups["c_ip"].Value.Trim());

                        lLogData.cs_version = lMatchCollection[lMatchCollectionIndex].Groups["cs_version"].Value.Trim();

                        lLogData.cs_user_agent = isEmpty(HttpUtility.UrlDecode(lMatchCollection[lMatchCollectionIndex].Groups["cs_user_agent"].Value.Trim()));

                        lLogData.cs_referer = isEmpty(lMatchCollection[lMatchCollectionIndex].Groups["cs_referer"].Value.Trim());

                        lLogData.sc_status = int.Parse(lMatchCollection[lMatchCollectionIndex].Groups["sc_status"].Value.Trim());

                        lLogData.sc_subStatus = int.Parse(lMatchCollection[lMatchCollectionIndex].Groups["sc_substatus"].Value.Trim());

                        lLogData.sc_win32_status = int.Parse(lMatchCollection[lMatchCollectionIndex].Groups["sc_win32_substatus"].Value.Trim());

                        lLogData.sc_bytes = int.Parse(lMatchCollection[lMatchCollectionIndex].Groups["sc_bytes"].Value.Trim());

                        lLogData.cs_bytes = int.Parse(lMatchCollection[lMatchCollectionIndex].Groups["cs_bytes"].Value.Trim());

                        lLogData.time_taken = int.Parse(lMatchCollection[lMatchCollectionIndex].Groups["time_taken"].Value.Trim());

                        lLogData.original_c_ip = lMatchCollection[lMatchCollectionIndex].Groups["original_c_ip"].Value.Trim();

                        lLogData.FileId = lLogFile.Id;

                        lLogData.ServerId = lLogFile.ServerId;

                        lLogData.GroupId = _groupId;

                        lLogData.ModifiedBy = "";
                    }
                    catch (ArgumentNullException e)
                    {
                        //异常处理
                        _logger.Error("空数据读取异常 \r 文件夹:" + lLogFile.FolderName + "\rLog文件:" + lLogFile.LogFileName + "\r异常信息:" + e);
                    }

                    lLogData.Ctime = DateTime.Now;

                    lLogData.CreatedBy = "by:HF";

                    lLogData.ReadLine = lLogDataLine + lLogDataExceptionLine;

                    //添加至数据集
                    lDbContext.LogDataDbSet.Add(lLogData);

                    if (lLogDataLine % 500 == 0)

                        //更新保存数据库
                        lDbContext.SaveChanges();


                }// end while

                try
                {
                    //更改文件状态
                    lLogFile.Status = (int)FileStatus.Finished;

                    lLogFile.ReadLine = lLogDataLine + lLogDataExceptionLine;

                    //更新数据库
                    lDbContext.Entry(lLogFile).State = EntityState.Modified;

                    lDbContext.SaveChanges();

                }
                catch (Exception e)
                {
                    //修改文件状态异常
                    lLogFile.Status = (int)FileStatus.Exception;

                    //保存处理行数
                    lLogFile.ReadLine = lLogDataLine;

                    //更新数据库
                    lDbContext.Entry(lLogFile).State = EntityState.Modified;

                    lDbContext.SaveChanges();

                    //打印Log
                    _logger.Error("数据库存储异常  \r  文件夹:" + lLogFile.FolderName + "\r  Log文件:" + lLogFile.LogFileName + "\r  异常信息:" + e);
                }
                //关闭数据流
                lFileRead.Close();
            }
            catch (Exception e)
            {
                //打印异常Log
                _logger.Error("数据读取异常  \r  文件夹:" + lLogFile.FolderName + "\r  Log文件:" + lLogFile.LogFileName + "\r  异常信息:" + e);
                Console.WriteLine(e);
            }
        }// end ReadData Method



        /// <summary>
        ///     传入字符串进行数据匹配 如匹配成功则返回数据否则返回null
        /// </summary>
        /// <param name="str">要匹配的数据</param>
        /// <returns>匹配成功的数据</returns>
        private static MatchCollection StrMatc(string str)
        {

            //定义Regex类
            Regex lConfigXmlRegex;

            //获取配置文件匹配字符
            var lRegexStr = ConfigurationManager.AppSettings[Constants.IGNORED_PATH_KEY].Split(' ');

            //循环匹配配置文件匹配规则
            for (var i = 0; i < lRegexStr.Length; i++)
            {
                lConfigXmlRegex = new Regex(@"(" + lRegexStr[i] + ")");
                if (lConfigXmlRegex.IsMatch(str))
                {
                    return null;
                }
            }

            //返回数据
            return _regex.IsMatch(str) ? _regex.Matches(str) : null;
        }

        /// <summary>
        ///      根据传入字符串判断是否为真实数据
        /// </summary>
        /// <param name="str">判断的数据</param>
        /// <returns>真实数据则原样返回,否则清空</returns>
        private static string isEmpty(string str)
        {
            if (str == "-")
            {
                return "";
            }
            return str;
        }
    }
}
