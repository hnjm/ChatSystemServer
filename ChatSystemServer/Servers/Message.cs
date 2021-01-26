﻿
namespace ChatSystemServer.Servers
{
    using System;
    using Common;

    /// <summary>
    /// 用于处理传输中数据的转换等
    /// </summary>
    public class Message
    {
        private byte[] data = new byte[1024];
        private int startIndex = 0; // 我们存取了多少个字节的数据在数组里面

        /// <summary>
        /// 获取data数组
        /// </summary>
        public byte[] Data
        {
            get { return data; }
        }

        /// <summary>
        /// 获取数组里面为空的第一个下标
        /// </summary>
        public int StartIndex
        {
            get { return startIndex; }
        }

        /// <summary>
        /// 剩余的空间数量
        /// </summary>
        public int RemainSize
        {
            get { return data.Length - startIndex; }
        }

        /// <summary>
        /// 读取消息
        /// </summary>
        /// <param name="newDataCount">新接收的消息的数量</param>
        /// <param name="onProcessMessage">传递的委托方法，响应外部事件</param>
        public void ReadMessage(int newDataCount, Action<RequestCode, ActionCode, string> onProcessMessage)
        {
            startIndex += newDataCount; // 初始地址也需要增加
            while (true)
            {
                // int类型4个字节
                if (startIndex - newDataCount <= 4)
                {
                    return;
                }

                int count = BitConverter.ToInt32(data, 0);
                if (startIndex - 4 >= count)
                {
                    RequestCode requestCode = (RequestCode)BitConverter.ToInt32(data, 4);
                    ActionCode actionCode = (ActionCode)BitConverter.ToInt32(data, 8);
                    string s = BitConverter.ToString(data, 12, count - 8);
                    onProcessMessage(requestCode, actionCode, s);

                    // Console.WriteLine("接收到数据" + s);
                    Array.Copy(data, count + 4, data, 0, startIndex - 4 - count);
                    startIndex -= count + 4;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
