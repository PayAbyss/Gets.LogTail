#region License

// Copyright (c) 2017-2020 iGets Inc. All rights reserved. 
// See LICENSE in the project root for license information.

#endregion

/*****************************************************************************
 * 
 * Created On: 2017-12-21
 * Purpose:   服务程序入口
 * 
 ****************************************************************************/

using Gets.LogTail.Service;
using Topshelf;

namespace Gets.LogTail
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<LogService>();
                x.RunAsLocalSystem();
                x.SetDescription("GetsLogService");
                x.SetDisplayName("GetLogTail");
                x.SetServiceName("GetLogTail");
            });
        }
    }
}
