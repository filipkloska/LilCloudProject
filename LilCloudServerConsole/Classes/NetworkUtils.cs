using System.Net;
using System.Net.Sockets;

namespace LilCloudServerConsole.Classes
{
    public static class NetworkUtils
    {
        public static IPAddress GetLocalIPv4()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var address = host
            .AddressList
            .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork
            && !IPAddress.IsLoopback(ip));
            if (address == null)
            {
                address = IPAddress.Any;
            }
            return address;
        }
    }
}
