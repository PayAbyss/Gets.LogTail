#region License

// Copyright (c) 2017-2020 iGets Inc. All rights reserved. 
// See LICENSE in the project root for license information.

#endregion

/*****************************************************************************
 * 
 * Created On: 2017-12-21
 * Purpose:   文件内容数据存储类
 * 
 ****************************************************************************/
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gets.LogTail.Models
{
    [Table("LOG_DATA")]
    public class LogData
    {
        // [Key]
        // [Required, Column("Id",TypeName ="INT")]
        public int Id { get; set; } //主关键字

        // [Column("RequestDate",TypeName ="DATETIME2")]
        public DateTime RequestDate { get; set; }//请求时间

        // [MaxLength(128)]
        // [Column("s_ip",TypeName ="NVARCHAR")]
        public string s_ip { get; set; }// 服务器ip

        //  [MaxLength(50)]
        // [Column("cs_method", TypeName = "NVARCHAR")]
        public string cs_method { get; set; }//请求方法

        // [MaxLength(255)]
        //[Column("cs_uri_stem", TypeName = "NVARCHAR")]
        public string cs_uri_stem { get; set; }// 请求地址

        // [MaxLength(255)]
        //[Column("cs_uri_query", TypeName = "NVARCHAR")]
        public string cs_uri_query { get; set; } //请求参数

        // [Column("s_port", TypeName = "INT")]
        public int s_port { get; set; }//请求端口

        //[MaxLength(128)]
        //[Column("cs_userName", TypeName = "NVARCHAR")]
        public string cs_userName { get; set; }//请求用户名

        //[MaxLength(128)]
        //[Column("c_ip", TypeName = "NVARCHAR")]
        public string c_ip { get; set; }//客户端ip

        // [MaxLength(255)]
        // [Column("cs_user_agent", TypeName = "NVARCHAR")]
        public string cs_user_agent { get; set; }//请求头（url解码后）

        // [MaxLength(255)]
        //[Column("cs_referer", TypeName = "NVARCHAR")]
        public string cs_referer { get; set; }//引用

        //[Column("sc_status", TypeName = "INT")]
        public int sc_status { get; set; }//请求状态码

        //[Column("sc_subStatus", TypeName = "INT")]
        public int sc_subStatus { get; set; }//返回子状态码

        //[Column("sc_win32_status", TypeName = "INT")]
        public int sc_win32_status { get; set; }//服务器状态码

        //[MaxLength(128)]
        //[Column("original_c_ip", TypeName = "NVARCHAR")]
        public string original_c_ip { get; set; }//请求ip

        // [Column("time_taken", TypeName = "INT")]
        public int time_taken { get; set; }//请求用时

        //[MaxLength(128)]
        //[Column("ServerId", TypeName = "NVARCHAR")]
        public string ServerId { get; set; }//服务器标识

        //[MaxLength(128)]
        //[Column("GroupId", TypeName = "NVARCHAR")]
        public string GroupId { get; set; }//服务器地址

        // [Column("FileId", TypeName = "INT")]
        public int FileId { get; set; }//文件id

        //[Column("ReadLine", TypeName = "INT")]
        public int ReadLine { get; set; }//数据所在文件行数

        //[MaxLength(128)]
        //[Column("cs_version", TypeName = "NVARCHAR")]
        public string cs_version { get; set; } //客户端版本

        //[MaxLength(32)]
        //[Column("sc_bytes", TypeName = "NVARCHAR")]
        public int sc_bytes { get; set; }  //

        //[MaxLength(32)]
        //[Column("cs_bytes", TypeName = "NVARCHAR")]
        public int cs_bytes { get; set; } //

        //[Column("cTime", TypeName = "DATETIME")]
        public DateTime Ctime { get; set; }  //创建时间

        //[Column("mTime", TypeName = "DATETIME")]
        public DateTime Mtime { get; set; } //修改时间

        //[MaxLength(128)]
        //[Column("CreateDby", TypeName = "NVARCHAR")]
        public string CreatedBy { get; set; } //创建者Id

        //[MaxLength(128)]
        //[Column("ModifieDby", TypeName = "NVARCHAR")]
        public string ModifiedBy { get; set; } //修改者Id
    }
}
