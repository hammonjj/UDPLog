using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace UPDLog
{
    public class UdpListener
    {
        private int _listenPort;
        private volatile bool _listen;
        private readonly ConcurrentQueue<string> _messageQueue;

        public UdpListener(int listenPort, ref ConcurrentQueue<string> messageQueue)
        {
            _listenPort = listenPort;
            _messageQueue = messageQueue;
        }

        public void BeginListening()
        {
            _listen = true;

            //Add try/catch if this port is already in use
            var udpClient = new UdpClient(_listenPort);
            var remoteEp = new IPEndPoint(IPAddress.Any, _listenPort);
            while (_listen)
            {
                var data = udpClient.Receive(ref remoteEp);

                if(data.Length <= 0) { continue; }
                var result = System.Text.Encoding.UTF8.GetString(data);
                _messageQueue.Enqueue(result);
            }

            udpClient.Close();
        }

        public void StopListening()
        {
            _listen = false;
        }
    }
}
