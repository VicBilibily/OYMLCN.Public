﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace OYMLCN
{
    /// <summary>
    /// ZipExtension
    /// </summary>
    public static class ZipExtension
    {
        /// <summary>
        /// 将传入字符串以GZip算法压缩后，返回Base64编码字符
        /// </summary>
        /// <param name="rawString">需要压缩的字符串</param>
        /// <returns>压缩后的Base64编码的字符串</returns>
        public static string GZipCompressString(this string rawString)
        {
            if (string.IsNullOrEmpty(rawString) || rawString.Length == 0)
                return "";
            byte[] rawData = Encoding.UTF8.GetBytes(rawString.ToString());
            byte[] zippedData = Compress(rawData);
            return Convert.ToBase64String(zippedData);
        }

        /// <summary>
        /// GZip压缩
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        private static byte[] Compress(byte[] rawData)
        {
            MemoryStream ms = new MemoryStream();
            using (GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true))
                compressedzipStream.Write(rawData, 0, rawData.Length);
            return ms.ToArray();
        }

        /// <summary>
        /// 将传入的二进制字符串资料以GZip算法解压缩
        /// </summary>
        /// <param name="zippedString">经GZip压缩后的二进制字符串</param>
        /// <returns>原始未压缩字符串</returns>
        public static string GZipDecompressString(this string zippedString)
        {
            if (string.IsNullOrEmpty(zippedString) || zippedString.Length == 0)
                return "";
            byte[] zippedData = Convert.FromBase64String(zippedString.ToString());
            return Encoding.UTF8.GetString(Decompress(zippedData));
        }

        /// <summary>
        /// ZIP解压
        /// </summary>
        /// <param name="zippedData"></param>
        /// <returns></returns>
        private static byte[] Decompress(byte[] zippedData)
        {
            using (MemoryStream ms = new MemoryStream(zippedData))
            using (MemoryStream outBuffer = new MemoryStream())
            {
                using (GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Decompress))
                {
                    byte[] block = new byte[1024];
                    while (true)
                    {
                        int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                        if (bytesRead <= 0)
                            break;
                        else
                            outBuffer.Write(block, 0, bytesRead);
                    }
                }
                return outBuffer.ToArray();
            }
        }

        /// <summary>
        /// 使用指定的文件夹创建Zip压缩文件
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="fileName">压缩文件路径</param>
        public static void CreateZipFile(this DirectoryInfo directory, string fileName) =>
            ZipFile.CreateFromDirectory(directory.FullName, fileName);
        /// <summary>
        /// 解压Zip压缩文件到指定文件夹
        /// </summary>
        /// <param name="file"></param>
        /// <param name="targetPath">文件夹路径</param>
        public static void ExtractZipFile(this FileInfo file, string targetPath) =>
            ZipFile.ExtractToDirectory(file.FullName, targetPath);
    }
}
