using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LilCloudClientConsole.Classes
{
    internal class Server
    {
        public uint CPULimit { get; set; }
        public int TCPport { get; set; }
        public string? ip { get; set; }
        public uint gRPCport { get; set; }
        public string? CPUname  { get; set; }
        public string? OSname { get; set; }

        public Server() { }
        public Server(uint CPULimit, int TCPport, uint gRPCport,
                    string CPUname, string OSname, string ip)
        {
            this.CPULimit = CPULimit;
            this.TCPport = TCPport;
            this.gRPCport = gRPCport;
            this.ip = ip;
            this.CPUname = CPUname;
            this.OSname = OSname;
        }
    }
}
