using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Buffers;
using System.Net.NetworkInformation;

namespace DebugServer
{
    internal class UdpServer
    {
        private static UdpServer m_instance = new UdpServer();
        static readonly UTF8Encoding encoding = new UTF8Encoding();
        public static UdpServer GetInstance() => m_instance;

        private List<IPAddress> localIpAddress = GetLocalIpAddress();

        private Socket listenSocket;
        public int serverport = 8888;
        public void Start()
        {
            Console.WriteLine("Udp Start");
            serverport = EnvironmentHelper.GetAIServerPort();
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), serverport);
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            // 设置广播过滤器
            listenSocket.SendBufferSize = 1024 * 1000;
            listenSocket.Bind(localEndPoint);
            StartReceive();
        }



        private  void StartReceive()
        {

            // 服务器循环接收和处理客户端请求
            while (true)
            {
                // 接收数据
                byte[] receiveBuffer = new byte[1024*100];
                EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                int receivedBytes = listenSocket.ReceiveFrom(receiveBuffer, ref remoteEndPoint);
                if(receivedBytes > 0)
                {
                    try
                    {
                        OnReceived(receiveBuffer);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"Error:{ex.ToString()}");
                    }
                }
            
            }
        }


        static List<IPAddress> GetLocalIpAddress()
        {
            List<IPAddress> list = new List<IPAddress>();
            string hostName = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            foreach (IPAddress ipAddress in hostEntry.AddressList)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    list.Add(ipAddress);
                }
            }
            return list;
        }

        public void Send(byte[] bytes)
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            IPAddress[] ipAddressArr = Dns.GetHostAddresses(Dns.GetHostName());  // 得到本机所有的IP地址*/
            List<string> ipPrefixList = new List<string>();   // IP地址前三个部分相同则说明处于同一局域网，所以把所有IP的前三部分存起来，放一个list中
            foreach (var item in ipAddressArr)
            {
                if (item.AddressFamily == AddressFamily.InterNetwork)    // 判断是不是IPV4
                {
                    string ipPrefix = item.ToString();
                    int endPointIndex = ipPrefix.LastIndexOf('.');    // 得到最后一个点的位置
                    ipPrefix = ipPrefix.Remove(endPointIndex + 1);    // 移 除IP的第四部分
                    ipPrefixList.Add(ipPrefix);
                }
            }
            foreach (var item in ipPrefixList)
            {
                listenSocket.SendTo(bytes, new IPEndPoint(IPAddress.Parse(item+255), serverport));// 255表示广播地址
            }
        }


        protected virtual void OnReceived(byte[] buffer) 
        {


            Span<byte> span = new Span<byte>(buffer);
            int start = 0;

            var length = span.Slice(start).ReadInt();
            start += 4;
            if(length >= 1024)
            {
                return;
            }


            var logType = span.Slice(start).ReadInt();
            start += 4;

            bool isSever = span.Slice(start).ReadShort() == 1;
            start += 2;
            if (isSever)
            {
                return;
            }

            span.Slice(start-2).Write((short)1);
            var accountLength = span.Slice(start).ReadInt();
            start += 4;
            var bytes = span.Slice(start, accountLength);
            var account = encoding.GetString(bytes);
            start += accountLength;


            var playerLength = span.Slice(start).ReadInt();
            start += 4;
            bytes = span.Slice(start, playerLength);
            var playerIdx =  encoding.GetString(bytes);
            start += playerLength;



            var nameLength = span.Slice(start).ReadInt();
            start += 4;
            bytes = span.Slice(start, nameLength);
            var name = encoding.GetString(bytes);
            start += nameLength;




            var errorLength = span.Slice(start).ReadInt(); 
            start += 4;
            bytes = span.Slice(start, errorLength);
            var errorStr = encoding.GetString(bytes);
            start += errorLength;

            var sendBytes = span.Slice(0, length).ToArray();
            Send(sendBytes);
        }
    }
}
