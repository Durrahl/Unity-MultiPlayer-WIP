using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;

namespace Base_Server
{
    class MP_MasterServer
    {
        List<GameServer> servers = new List<GameServer>();
        string DisasableList()
        {
            string returnString = "svrList";

            for (int i = 0; i < servers.Count; i++)
            {
                returnString = returnString + ("| " + servers[i].serverName + ", " + servers[i].maxCount + ", " + servers[i].currentCount + ", " + servers[i].owner);
            }
            return returnString;
        }

        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("Starting Master Server...");
            MP_MasterServer clnt = new MP_MasterServer();
            clnt.startServer();



        }


        //////////////////////////////////////////////////////////////////
        //////////////// Internal Server List Methods ////////////////////
        //////////////////////////////////////////////////////////////////
        void UpdateServer(EndPoint serverRef, string passedString)
        {
            int index = FindInServerList(GetServer(serverRef));

            if (index != -1)
            {
                String[] properties = passedString.Split((","));
                servers[index].serverName = properties[1];
                servers[index].maxCount = int.Parse(properties[2]);
                servers[index].currentCount = int.Parse(properties[3]);
            }
        }

        int FindInServerList(GameServer toFind)
        {
            
            for (int i = 0; i < servers.Count; i++)
            {
                if (servers[i] == toFind)
                {
                    return i;
                }
            }

            return -1;
        }

        void RegisterServer(EndPoint serverRef, String passedString)
        {
            if (GetServer(serverRef) == null)
            {
                String[] properties = passedString.Split((","));
                GameServer tmpServer = new GameServer();

                Console.WriteLine("Recieved Request: Name: " + properties[1] + " " + properties[3] + "/" + properties[2] + " Players!");

                tmpServer.serverName = properties[1];
                tmpServer.maxCount = int.Parse(properties[2]);
                tmpServer.currentCount = int.Parse(properties[3]);
                tmpServer.owner = serverRef;
                
                servers.Add(tmpServer);
            }
            else
            {
                Console.Write("[ERROR]: Server is already is list!");
            }
        }

        void DeRegisterServer(EndPoint serverRef)
        {
            GameServer toRemove = GetServer(serverRef);

            if (toRemove != null)
            {
                servers.Remove(toRemove);
            }
        }

        GameServer GetServer(EndPoint serverRef)
        {
            for (int i = 0; i < servers.Count; i++)
            {
                if (servers[i].owner == serverRef)
                {
                    return servers[i];
                }
            }

            return null;
        }

        void ReturnServerList(EndPoint target)
        {
            // To Do Update
            
        }

        void startServer()
        {
            TcpListener server = null;

            Int32 thisPort = 800;
            IPAddress thisIp = IPAddress.Parse("127.0.0.1");

            server = new TcpListener(IPAddress.Any, thisPort);

            server.Start();
            
            Byte[] data = new Byte[256];
            String strData = null;

            // Listen Loop
            while (true)
            {
                TcpClient client;
                try
                {
                    // Setting
                    Console.Clear();
                    Console.WriteLine("Waiting For a connection");

                    // Accept Client
                    client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    // Setting Default Parameters
                    strData = null;
                    NetworkStream stream = client.GetStream();
                    byte[] buffer = new byte[client.ReceiveBufferSize];

                    // Reading Stream
                    int byteMessageSize = stream.Read(buffer, 0, client.ReceiveBufferSize);

                    // Convert To Text
                    strData = Encoding.ASCII.GetString(buffer, 0, byteMessageSize);
                    Console.WriteLine("Recieved : " + strData);

                    // Proccess Request
                    String result;
                    if (strData != null)
                    {
                        IPEndPoint clientAddress = (IPEndPoint)client.Client.RemoteEndPoint;
                        result = ProccessResult(strData, clientAddress);
                    }
                    else
                    {
                        result = "Inputted Data Did Not Pass Required Input Sterlisation Tests (EMPTY or UNREADABLE)";
                    }


                    // Convert Result
                    byte[] returnResults = Encoding.ASCII.GetBytes(result);

                    // Return Result
                    stream.Write(returnResults, 0, result.Length);
                    client.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("EXCEPTION CAUGHT IN LISTEN LOOP: " + e.Message);
                }
            }
        }

        String ProccessResult(String input, IPEndPoint clientEP)
        {
            String result = "Error Selection Option";

            String[] segments = input.Split(",");
            String cat = segments[0];

            Console.WriteLine("Got request with line starting: " + segments[0]);

            // Register Server
            switch (cat)
            {
                // Register Server
                case "regsrv":
                    RegisterServer((EndPoint)clientEP, input);
                    result = "Registered Server";

                    break;

                // De-Register Server
                case "delsrv":
                    DeRegisterServer((EndPoint)clientEP);
                    break;

                // Get Server
                case "findsrv":
                    return DisasableList();
                    break;

                // Update Server
                case "updtsrv":
                    UpdateServer((EndPoint)clientEP, input);
                    break;

                // Enroll In party System
                case "prtyenrl":

                    break;

                // De-Enroll in party System
                case "prtydenrl":

                    break;

                // Matchmake solo
                case "mmsolo":

                    break;

                // Matchmake party
                case "mmparty":

                    break;

                default:
                    return "ERROR: THIS COMMAND DID NOT MATCH ANY ACCEPTED INPUT.";
                    break;
            }



            return result;
        }
    }
}
