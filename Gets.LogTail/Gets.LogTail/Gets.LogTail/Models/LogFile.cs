#region License

// Copyright (c) 2017-2020 iGets Inc. All rights reserved. 
// See LICENSE in the project root for license information.

#endregion

/*****************************************************************************
 * 
 * Created On: 2017-12-21
 * Purpose:   文件信息存储类
 * 
 ****************************************************************************/

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gets.LogTail.Models
{
    [Table("LOG_FILE")]
    public class LogFile
    {
        //[Key]
        // [Required, Column("Id", TypeName = "INT")]
        public int Id { get; set; }//主关键字

        // [MaxLength(50)]
        // [Column("FolderName", TypeName = "NVARCHAR")]
        public string FolderName { get; set; } //文件夹名称

        // [MaxLength(50)]
        //[Column("LogFileName", TypeName = "NVARCHAR")]
        public string LogFileName { get; set; } //日志文件名称

        // [Column("LogLastModifyTime", TypeName = "DATETIME2")]
        public DateTime LogLastModifyTime { get; set; }//日志最后修改时间

        // [MaxLength(128)]
        // [Column("ServerId", TypeName = "NVARCHAR")]
        public string ServerId { get; set; } //服务端标识

        //[MaxLength(128)]
        //[Column("GroupId", TypeName = "NVARCHAR")]
        public string GroupId { get; set; } //服务器地址

        // [Column("Status", TypeName = "INT")]
        public int Status { get; set; } //处理标识

        // [Column("Ctime", TypeName = "DATETIME2")]
        public DateTime Ctime { get; set; }  //创建时间

        // [Column("Mtime", TypeName = "DATETIME2")]
        public DateTime Mtime { get; set; } //修改时间

        //[MaxLength(128)]
        //[Column("Createdby", TypeName = "NVARCHAR")]
        public string CreatedBy { get; set; } //创建者Id

        // [MaxLength(128)]
        //[Column("Modifiedby", TypeName = "NVARCHAR")]
        public string ModifiedBy { get; set; } //修改者Id

        // [Column("ReadLine", TypeName = "INT")]
        public int ReadLine { get; set; }//数据所在文件行数

    }
}
