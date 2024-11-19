using Google.Protobuf;
using Message;
using Response;
using System;

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Utils;

namespace Net {
    public class NetClient : MonoSingleton<NetClient> {
        private string IP;
        private int Port;


        // 服务器ip,端口信息
        private IPEndPoint address;

        // 发送/接收 消息
        private Socket socket;

        // 接收 缓冲区 64k
        private MemoryStream receiveBuff = new MemoryStream(64 * 1024);

        // 解码缓冲区
        private MemoryStream stream = new MemoryStream(64 * 1024);

        private int readOffset = 0;

        private bool Runing;

        public bool Init(string IP, int port) {
            Debug.Log("开始连接....");
            Runing = true;
            this.IP = IP;
            this.Port = port;
            // string ip, int port
            return Connect();
        }

        private bool Connect() {
            if (!Runing) {
                return false;
            }
            if (socket != null) {
                socket.Close();
            }

            try {
                address = new IPEndPoint(IPAddress.Parse(IP), Port);
                this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.socket.Blocking = true;
                IAsyncResult result = socket.BeginConnect(address, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(1000);
                if (success) {
                    Runing = true;
                    socket.EndConnect(result);
                } else {
                    Debug.LogError("无法连接到服务器");
                    return false;
                }
            } catch (SocketException ex) {
                if (ex.SocketErrorCode == SocketError.ConnectionRefused) {
                    CloseConnection();
                }
                Debug.LogError(ex.ErrorCode + "" + ex.SocketErrorCode + "" + ex.ToString());
            } catch (Exception ex) {
                Debug.LogError("DoConnectin Exception: " + ex.ToString());
            }

            if (socket.Connected) {
                socket.Blocking = false;
                Debug.Log("连接完成....");
                return true;
            }
            return false;
        }

        public bool Connected {
            get { return (socket != default(Socket)) ? socket.Connected : false; }
        }

        private void OnDestroy() {
            CloseConnection();
        }

        public void CloseConnection() {
            Debug.LogWarning("CloseConnection...");
            if (socket != null) {
                socket.Close();
                Runing = false;
            }
        }



        private void Update() {
            ProcessReve();
        }


        private void ProcessReve() {
            if (!Runing) {
                return;
            }

            if (socket == null) {
                return;
            }
            // 测试 时 不要捕获异常.会找不到 问题; 打包时,可以将 下面的代码 放入 try catch
            bool error = socket.Poll(0, SelectMode.SelectError);
            if (error) {
                Debug.LogError("服务器 无法连接");
                CloseConnection();
                Runing = false;
                return;
            }



            try {
                // 测试时 不 捕获 这里的异常
                bool res = socket.Poll(0, SelectMode.SelectRead);
                if (res) {
                    int n = socket.Receive(receiveBuff.GetBuffer(), 0, receiveBuff.Capacity, SocketFlags.None);
                    if (n <= 0) {
                        this.CloseConnection();
                        return;
                    }
                    this.ReceiveData(this.receiveBuff.GetBuffer(), 0, n);
                }

            } catch (Exception ex) {
                Debug.LogError(ex.Message);
                Runing = false;
            }

        }

        private void ReceiveData(byte[] data, int offset, int count) {
            // 消息太多,装不下
            if (stream.Position + count > stream.Capacity) {
                throw new Exception("stream.Position + count > stream.Capacity");
            }
            stream.Write(data, offset, count);
            ParsePackage();

        }

        // short 12345
        private void ParsePackage() {
            // 判断解码区 是否 有足够的 字节
            if (readOffset + 4 < stream.Position) {
                int packageSize = BitConverter.ToInt32(stream.GetBuffer(), readOffset);
                if (packageSize + readOffset + 4 <= stream.Position) {
                    byte[] data = stream.GetBuffer();
                    // 4字节,是长度  + 2字节消息号 + 消息数据
                    MsgData msg = MsgData.Build(data, packageSize, readOffset);

                    RespManager.execute(msg);

                    // 记录解码缓冲区最新的位置
                    readOffset += (packageSize + 4);
                    ParsePackage();
                }
            }
            // 10: 1 + 9
            //未接收完/要结束了
            if (this.readOffset > 0) {
                long size = stream.Position - this.readOffset;
                if (this.readOffset < stream.Position) {
                    Array.Copy(stream.GetBuffer(), this.readOffset,
                        stream.GetBuffer(), 0, stream.Position - this.readOffset);
                }
                this.readOffset = 0;
                stream.Position = size;
                stream.SetLength(size);
            }


        }

        public void Send<T>(ECode code, T msg) where T : IMessage {
            if (!Connected) {
                Connect();
                if (!Connected) {
                    Debug.LogError("Connect(); Error");
                    CloseConnection();
                    throw new Exception("send error");
                }
            }
            var v = Serialize(msg);

            byte[] send = Pack((int)code, v);
            socket.Send(send);

        }

        /// <summary>
        /// 使用protobuf把对象序列化为Byte数组
        /// </summary>
        /// <typeparam name="T">需要反序列化的对象类型</typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public byte[] Serialize<T>(T obj) where T : IMessage {
            using (var memory = new MemoryStream()) {
                MessageExtensions.WriteTo(obj, memory);
                // obj.WriteTo(memory); // 这个也能用
                return memory.ToArray();
            }
        }



        /// <summary>
        /// 打包
        /// </summary>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private byte[] Pack(int code, byte[] msg) {
            // 申请一个 消息长度+6的数组; 多出来的这6个字节,  4长度信息+2命令号 + data
            // 前4个字节写整个请求包的长度,后2个字节写 协议号
            byte[] package = new byte[msg.Length + 4 + 2];
            byte[] lenArr = IntToArr4(msg.Length + 2);
            byte[] codeArr = IntToArr2(code);
            lenArr.CopyTo(package, 0);
            codeArr.CopyTo(package, 4);
            msg.CopyTo(package, 6);

            return package;
        }

        private byte[] IntToArr4(int num) {
            byte[] data = new byte[4];
            data[3] = (byte)(num & 0xff);
            data[2] = (byte)(num >> 8 & 0xff);
            data[1] = (byte)(num >> 16 & 0xff);
            data[0] = (byte)(num >> 24 & 0xff);
            return data;
        }

        private byte[] IntToArr2(int num) {
            byte[] data = new byte[2];
            data[1] = (byte)(num >> 8);
            data[0] = (byte)(num & 255);
            return data;
        }
    }

    /// <summary>
    /// 收到服务器 推送的消息后,将消息 包装
    /// </summary>
    public class MsgData {
        public int code;
        public byte[] data;

        // 5 : 1 1234
        public static MsgData Build(byte[] b, int packLen, int begin) {
            MsgData msg = new MsgData();
            msg.code = BitConverter.ToInt16(b, begin + 4);
            msg.data = new byte[packLen - 2];
            for (int i = 0; i < packLen - 2; i++) {
                msg.data[i] = b[i + begin + 4 + 2];
            }
            return msg;
        }
    }
}