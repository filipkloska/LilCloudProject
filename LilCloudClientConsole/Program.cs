//C:\Filip\Projects\test
//C:\Filip\Projects\test1
using LilCloudClientConsole.Classes;

FileToSend fileToSend = new FileToSend
{
    fileName = "bruh1.txt",
    fileOriginPath = "C:\\Filip\\Projects\\test",
    fileSavePath = "C:\\Filip\\Projects\\test1"
};

Server server = new Server
{
    ip = "127.0.0.1",
    gRPCport = 50421,
    TCPport = 11000
};



LilCloudClient client = new LilCloudClient();
var token = await client.AccessAccount("admin", "admin", "127.0.0.1", 50421);
await client.SendFile(fileToSend, server, token);