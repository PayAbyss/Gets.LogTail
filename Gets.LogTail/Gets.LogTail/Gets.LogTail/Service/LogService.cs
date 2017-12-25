#region License

// Copyright (c) 2017-2020 iGets Inc. All rights reserved. 
// See LICENSE in the project root for license information.

#endregion

/*****************************************************************************
 * 
 * Created On: 2017-12-22
 * Purpose:   服务启动类
 * 
 ****************************************************************************/


using Gets.LogTail.Utils;
using log4net;
using System;
using System.Configuration;
using System.Timers;
using Topshelf;

namespace Gets.LogTail.Service
{
    public class LogService : ServiceControl
    {

        //什么实例Log4net
        private static readonly ILog _logger = LogManager.GetLogger(typeof(LogService));

        public bool Start(HostControl hostControl)
        {
            /** Timer timer = new Timer();
             timer.Elapsed += new ElapsedEventHandler(timerTask);
             timer.Interval = 1000*60*60*24;
             timer.Start();
             timer.Enabled = true;
              **/

            string lPath = ConfigurationManager.AppSettings[Constants.LOG_FOLDER_PATH_KEY];
            string lServerId = ConfigurationManager.AppSettings[Constants.SERVER_ID_KEY];
            int lDay = int.Parse(ConfigurationManager.AppSettings[Constants.DAYS_KEY]);
            _logger.Info("**************************************服务已启动**************************************");
            LogFileUtils.ReadContrast(lPath, lServerId, lDay);
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _logger.Info("**************************************服务已结束**************************************");
            throw new NotImplementedException();
        }

        private void timerTask(object source, ElapsedEventArgs e)
        {
            string lPath = ConfigurationManager.AppSettings["logPath"];
            string lGroupId = ConfigurationManager.AppSettings["serverName"];
            int lDay = int.Parse(ConfigurationManager.AppSettings["day"]);
            _logger.Info("**************************************服务已启动**************************************");
            LogFileUtils.ReadContrast(lPath, lGroupId, lDay);
        }
    }
}
