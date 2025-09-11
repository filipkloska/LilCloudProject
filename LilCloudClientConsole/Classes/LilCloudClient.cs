using Grpc.Core;
using Grpc.Net.Client;
using LilCloudClientConsole;
using Microsoft.AspNetCore.Http;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace LilCloudClientConsole.Classes
{
    //TODO: Exception handling when calling (when interceptor tells you to fuck off, or call is stopped)
    internal class LilCloudClient
    {
        public List<Server> Servers;
        public string _guid = "";
        public LilCloudClient()
        {
            Servers = new List<Server>();
        }
        public LilCloudClient(string guid)
        {
            _guid = guid;
            Servers = new List<Server>();
        }
        public async Task<string> AccessAccount(string login, string password,
            string ip, uint gRPC_port)
        {
            //TODO: ip and port validation
            string channelAddress = "http://" + ip + ":" + gRPC_port;
            // The port number must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress(channelAddress);
            var client = new LilCloud.LilCloudClient(channel);
            AccessRequest ar = new AccessRequest
            {
                Login = login,
                Password = password
            };
            var reply = await client.AccessAccountAsync(ar);
            Console.WriteLine($"Token:  {reply.AuthToken}");
            Console.WriteLine($"Status: {reply.Status}");
            return reply.AuthToken;
        }
        
        public async Task ConnectToServer(string ip, uint gRPC_port)
        {
            //TODO: ip and port validation
            string channelAddress = "http://" + ip + ":" + gRPC_port;
            // The port number must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress(channelAddress);
            var client = new LilCloud.LilCloudClient(channel);
            var reply1 = client.EstablishConnection(new ConnectionRequest
            {
                AuthToken = _guid,
            });

            if (_guid == "")
            {
                _guid = reply1.AuthToken;
            }


            Console.WriteLine(reply1.OSName);
            Console.WriteLine(reply1.CPUName);
            Console.WriteLine(reply1.TcpPort);
            Console.WriteLine(reply1.CPULimit);
            Console.WriteLine(reply1.AuthToken);
            Servers.Add(new Server
            {
                CPULimit = reply1.CPULimit,
                OSname = reply1.OSName,
                CPUname = reply1.CPUName,
                ip = ip,
                gRPCport = gRPC_port,
                TCPport = reply1.TcpPort
            });

            return;
        }

        public async Task SendFile(FileToSend file, Server server, string token)
        {
            string channelAddress = "http://" + server.ip + ":" + server.gRPCport;
            //// The port number must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress(channelAddress);
            var client = new LilCloud.LilCloudClient(channel);
            var header = new Metadata
            {
                {"filename", $"{file.fileName}" },
                {"authorization", $"{token}"}
            };
            const int bufferSize = 64 * 1024;
            
            byte[] buffer = new byte[bufferSize];
            using var fs = File.OpenRead(Path.Combine(file.fileOriginPath,file.fileName));
            int bytesRead;
            using var call = client.UploadFile(header);

            while ((bytesRead = await fs.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                var chunk = new FileChunk
                {
                    Data = Google.Protobuf.ByteString.CopyFrom(buffer, 0, bytesRead)
                };
                await call.RequestStream.WriteAsync(chunk);
            }
            await call.RequestStream.CompleteAsync();
            
            while (await call.ResponseStream.MoveNext())
            {
                var reply = call.ResponseStream.Current;
                Console.WriteLine($"Completion: {reply.Completion}");
            };
        }

        public async Task CheckServerStatus(Server server)
        {
            string channelAddress = "http://" + server.ip + ":" + server.gRPCport;
            //// The port number must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress(channelAddress);
            var client = new LilCloud.LilCloudClient(channel);
            var reply = await client.CheckAvailabilityAsync(new EmptyRequest());
            Console.WriteLine($"Reply received: {reply.Status}");
        }
    }
}
