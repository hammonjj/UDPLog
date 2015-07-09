using System.Net;

namespace UPDLog.DataStructures
{
    public struct RawMessage
    {
        public int Port;
        public IPAddress IpAddress;
        public string Message;
    }
}
