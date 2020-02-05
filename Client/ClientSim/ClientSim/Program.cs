using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientSim
{
    class Program
    {
        static void Main(string[] args)
        {
            Program prgm = new Program();
            //Thread listenThread = new Thread(prgm.Listener);
            //listenThread.Start();
            prgm.ClientLoop();
            
        }

        void Listener()
        {

            int dataRecv;
            byte[] data = new byte[1024];

            IPEndPoint thisPoint = new IPEndPoint(IPAddress.Any, 11434);

            Socket thisSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            thisSock.Bind(thisPoint);
            
            IPEndPoint sentAddress = new IPEndPoint(IPAddress.Any, 11434);
            EndPoint remote = (EndPoint)sentAddress;

            Console.Write("Has Been Started!");

          //  thisSock.Connect(remote);

            
            
        }

        public void ClientLoop()
        {

            while (true)
            {
               // Console.Clear();
                Console.Write("Command: ");
                string message = Console.ReadLine();

                if (message == "exit")
                {
                    break;
                }

                Connect(message);
            }
        }

        void Connect(string message = " ")
        {
            if (message.Length < 1)
            {
                Console.WriteLine("Found Too Short Input");
                return;
            }
            
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress targetAddress = IPAddress.Parse("51.89.164.171");
            IPEndPoint endPoint = new IPEndPoint(targetAddress, 800);

            TcpClient client = new TcpClient("51.89.164.171", 800);
            
            NetworkStream stream = client.GetStream();

            byte[] messageA = Encoding.ASCII.GetBytes(message);
            stream.Write(messageA);
            stream.Flush();

            Byte[] data = new byte[256];
           
            String strData = null;

   
                
            int i;
            
            byte[] buffer = new byte[client.ReceiveBufferSize];

            int byteMessageSize = stream.Read(buffer, 0, client.ReceiveBufferSize);

            strData = Encoding.ASCII.GetString(buffer, 0, byteMessageSize);
            Console.WriteLine("Recieved : " + strData);

            

            
        }
    }
}
