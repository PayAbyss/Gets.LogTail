#region License

// Copyright (c) 2017-2020 iGets Inc. All rights reserved. 
// See LICENSE in the project root for license information.

#endregion

/*****************************************************************************
 * 
 * Created On: 2017-12-21
 * Purpose:   枚举用于常量字段数据处理
 * 
 ****************************************************************************/

namespace Gets.LogTail.Utils
{

    public enum FileStatus
    {
        //处理完成
        Finished = 0,

        //正在处理
        Processing = 1,

        //处理异常
        Exception = 2,
    }
}
