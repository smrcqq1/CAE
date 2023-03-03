using CAE.Demo;
using System;

namespace CAE.DTO
{
    /// <summary>
    /// 第一步：前端先使用这个获取该任务下所有可以下载的文件清单
    /// </summary>
    public class 获取任务结果文件列表Request : LoginedRequest
    {
        /// <summary>
        /// 值为11
        /// </summary>
        public override MessageType type => MessageType.下载任务结果文件清单;
        /// <summary>
        /// 任务ID
        /// </summary>
        public Guid ID { get; set; }
    }
    /// <summary>
    /// 第2步：服务器返回该任务下所有可以下载的文件清单
    /// </summary>
    public class 任务结果文件列表 : DTOBase
    {
        /// <summary>
        /// 值为11
        /// </summary>
        public override MessageType type => MessageType.下载任务结果文件清单;
        /// <summary>
        /// 该任务下所有可以下载的文件清单
        /// </summary>
        public FileInfo[] Files { get; set; }
    }
    /// <summary>
    /// 单个可下载文件的具体信息
    /// </summary>
    public class FileInfo
    {
        /// <summary>
        /// 显示名称，用于列表显示使用
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 完整相对路径，用于下载,不显示给用户
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 文件总长度，前端根据长度来决定是否分片下载
        /// </summary>
        public int Length { get; set; }
    }
    /// <summary>
    /// 第三步：下载单个文件请求：前端使用这个命令请求后端，后端按照请求直接返回2进制数据即可
    /// </summary>
    /// <remarks>
    /// 注意：这个命令将使用新的连接
    /// </remarks>
    public class DownLoadFileRequest : LoginedRequest
    {
        /// <summary>
        /// 值为12
        /// </summary>
        public override MessageType type => MessageType.下载文件;
        /// <summary>
        /// 要下载的文件路径，来自FileInfo
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 当前要下载的二进制文件的起始序号
        /// </summary>
        public int Start { get; set; }
        /// <summary>
        /// 当前要下载的二进制文件的结束序号
        /// </summary>
        public int End { get; set; }
    }
}