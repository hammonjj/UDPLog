using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using UPDLog.DataStructures;

namespace UPDLog
{
    public class UdpListener
    {
        private int _listenPort;
        private int _messageFloodLimit;
        private volatile bool _listen;
        private readonly ConcurrentQueue<RawMessage> _messageQueue;

        public UdpListener(int listenPort, int messageFloodLimit, ref ConcurrentQueue<RawMessage> messageQueue)
        {
            _listenPort = listenPort;
            _messageFloodLimit = messageFloodLimit;
            _messageQueue = messageQueue;
        }

        public void Listen()
        {
            using (var udpClient = new UdpClient(_listenPort))
            {
                _listen = true;
                var remoteEp = new IPEndPoint(IPAddress.Any, _listenPort);
                while (_listen)
                {
                    var data = udpClient.Receive(ref remoteEp);
                    if (data.Length <= 0)
                    {
                        continue;
                    }

                    if (_messageQueue.Count > _messageFloodLimit) //Overflow
                    {
                        continue;
                    }

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
        }

        public void StopListening()
        {
            _listen = false;
        }

        public void UpdateListeningPort(int port)
        {
            _listenPort = port;
        }
    }
}
