// See https://aka.ms/new-console-template for more information



using System.Net.NetworkInformation;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using System.Net.Sockets;
using DebugServer;


public class Program
{

    public static void Main(string[] args)
    {
        RunServer();
    }


    public static void RunServer()
    {
        UdpServer.GetInstance().Start();
        while(true)
        {
            Console.ReadLine();
        }
    }
}