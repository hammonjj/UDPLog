using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using UPDLog.DataStructures;

namespace UPDLog
{
    public class UdpListener
    {
        private int _listenPort;
        private volatile bool _listen;
        private readonly ConcurrentQueue<RawMessage> _messageQueue;

        public UdpListener(int listenPort, ref ConcurrentQueue<RawMessage> messageQueue)
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

                var rawMessage = new RawMessage()
                {
                    Port = remoteEp.Port,
                    IpAddress = remoteEp.Address,
                    Message = System.Text.Encoding.UTF8.GetString(data)
                };

                _messageQueue.Enqueue(rawMessage);
            }

            udpClient.Close();
        }

        public void StopListening()
        {
            _listen = false;
        }

        public void UpdateListeningPort(int port)
        {
            //Stop listening if we are
            _listenPort = port;
            //Start listening if we were
        }
    }
}
